using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace IntentionalSolutionVersion
{
	internal static class SolutionVersionProcessor
	{
		// From https://semver.org/#is-there-a-suggested-regular-expression-regex-to-check-a-semver-string
		// const string defRegex = @"(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:\.(0|[1-9]\d*))?(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?";

		public static async Task<IList<VerData>> GetProjectVersionsAsync(string slnPath,
						IDictionary<string, List<string>> slnFiles,
						IEnumerable<string> asmInfoFiles,
						CancellationToken cancellationToken = default,
						IProgress<(int, string)> progress = null,
						bool includeWithoutVer = true)
		{
			return await Task.Run(() =>
			{
				// Setup the instructions for the version retrieval
				JsonNode changeGuide = JsonNode.Parse(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ChangeGuide.json")));
				Dictionary<string, string> lookupTypes = [];
				foreach (var kv in changeGuide["lookupTypes"]!.AsObject())
					lookupTypes.Add(kv.Key, Regex.Replace((string)kv.Value, @"%(\w+)%", match => lookupTypes[match.Groups[1].Value]));
				Dictionary<string, string> ns = changeGuide["namespaces"]!.AsObject().ToDictionary(kv => kv.Key, kv => (string)kv.Value!);
				Dictionary<string, JsonArray> projFiles = changeGuide["projects"]!.AsArray().
					SelectMany(n => n["projExtensions"]!.AsArray().Select(ex => ((string)ex, n["fileTypes"]!.AsArray()))).
					ToDictionary(p => p.Item1, p => p.Item2);

				HashSet<VerData> d = new(EqualityComparer<VerData>.Default);
				if (!string.IsNullOrEmpty(slnPath))
				{
					Directory.SetCurrentDirectory(Path.GetDirectoryName(slnPath));
				}

				foreach (string proj in slnFiles.Keys)
				{
					if (cancellationToken.IsCancellationRequested)
						break;

					string projName = Path.GetFileName(proj.TrimEnd('\\'));
					progress?.Report((0, $"Retrieving version information for files in {projName}..."));

					bool foundVer = false;
					void AddVer(VerData ver) { if (!d.Contains(ver)) { ver.Project = projName; d.Add(ver); foundVer = true; } }

					if (projFiles.TryGetValue(Path.GetExtension(proj).ToLowerInvariant(), out JsonArray fileTypes))
					{
						foreach ((string filter, JsonArray actions) in fileTypes.SelectMany(n => n["fileFilters"]!.AsArray().Select(ex => ((string)ex, n["lookups"]!.AsArray()))))
						{
							if (filter == "")
							{
								if (File.Exists(proj))
									RunActions(proj, actions, AddVer);
							}
							else
							{
								foreach (string fn in GetProjectFiles(slnFiles[proj], filter))
								{
									if (File.Exists(fn))
										RunActions(fn, actions, AddVer);
								}
							}
						}
					}

					if (!foundVer && includeWithoutVer && File.Exists(proj))
					{
						// Set invalid version, permit the initialization of all project without any version
						AddVer(new VerData(proj, new(0, 0, 0), "<Version>0.0.0</Version>", null, null));
					}

					progress?.Report((1, ""));
				}

				return d.ToList();

				void RunActions(string fn, JsonArray actions, Action<VerData> addVer)
				{
					foreach (var action in actions)
					{
						var pattern = lookupTypes[(string)action["type"]];
						if (action["xpath"] is not null)
						{
							foreach (VerData ver in GetXmlTagVersions(fn, (string)action["xpath"], ns[(string)action["ns"]], pattern))
							{
								addVer(ver);
							}
						}
						else if (action["type"] is not null)
						{
							int n = 0;
							foreach (string l in File.ReadLines(fn))
							{
								n++;
								Match m = Regex.Match(l, pattern);
								if (!m.Success)
									continue;

								var ver = new VerData(fn, MakeVer(m), l, n.ToString(), pattern);
								addVer(ver);
							}
						}
					}
				}
			});

			static IEnumerable<string> GetProjectFiles(IEnumerable<string> projectFiles, string name)
			{
				foreach (string i in projectFiles)
				{
					if (i != null && name[0] == '.' && Path.GetExtension(i).Equals(name, StringComparison.InvariantCultureIgnoreCase) ||
						(name[0] != '.' && Path.GetFileName(i).Equals(name, StringComparison.InvariantCultureIgnoreCase)))
					{
						yield return i;
					}
				}
			}

			static IEnumerable<VerData> GetXmlTagVersions(string fn, string xmlPath, string ns, string regEx = null)
			{
				XmlDocument xmlDoc = new();
				xmlDoc.Load(fn);
				XmlNamespaceManager nsp = new(xmlDoc.NameTable);
				nsp.AddNamespace("x", ns);
				XmlNodeList nodes = xmlDoc.SelectNodes(xmlPath, nsp);
				if (nodes is null || nodes.Count == 0)
				{
					ns = "";
					nsp.AddNamespace("x", ns);
					nodes = xmlDoc.SelectNodes(xmlPath, nsp);
				}
				if (nodes is null || nodes.Count == 0 && xmlPath.Contains("/x:"))
				{
					xmlPath = xmlPath.Replace("/x:", "/");
					nodes = xmlDoc.SelectNodes(xmlPath, nsp);
				}
				for (int i = 0; nodes is not null && i < nodes.Count; i++)
				{
					Match m = Regex.Match(nodes[i].InnerText, regEx);
					if (m.Success)
					{
						yield return new VerData(fn, MakeVer(m), RemoveNS(nodes[i].OuterXml), nodes.Count == 1 ? xmlPath : $"({xmlPath})[{i + 1}]", regEx, ns);
					}
				}

				string RemoveNS(string value) => string.IsNullOrEmpty(ns) ? value : value.Replace($" xmlns=\"{ns}\"", string.Empty);
			}

			static NuGetVersion MakeVer(Match m) => NuGetVersion.Parse(m.Groups[1].Value); /*new(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value),
				m.Groups.Count > 4 && int.TryParse(m.Groups[4].Value, out var i) ? i : 0, m.Groups.Count > 5 ? m.Groups[5].Value : "", m.Groups.Count > 6 ? m.Groups[6].Value : "");*/
		}

		/*static string GetXPath(this XmlNode node)
		{
			if (node.NodeType == XmlNodeType.Attribute)
			{
				// attributes have an OwnerElement, not a ParentNode; also they have
				// to be matched by name, not found by position
				return $"{GetXPath(((XmlAttribute)node).OwnerElement)}/@{node.Name}";
			}
			if (node.ParentNode == null)
			{
				// the only node with no parent is the root node, which has no path
				return "";
			}
			// the path to a node is the path to its parent, plus "/node()[n]", where 
			// n is its position among its siblings.
			return $"{GetXPath(node.ParentNode)}/node()[{GetNodePosition(node)}]";

			static int GetNodePosition(XmlNode child)
			{
				for (int i = 0; i < child.ParentNode.ChildNodes.Count; i++)
				{
					if (child.ParentNode.ChildNodes[i] == child)
					{
						// tricksy XPath, not starting its positions at 0 like a normal language
						return i + 1;
					}
				}
				throw new InvalidOperationException("Child node somehow not found in its parent's ChildNodes property.");
			}
		}*/

		public static async Task UpdateAsync(IEnumerable<VerData> vers, NuGetVersion newVer, CancellationToken cancellationToken = default, IProgress<(int, string)> progress = null, bool dontSave = false)
		{
			await Task.Run(() =>
			{
				foreach (IGrouping<string, VerData> grp in vers.GroupBy(v => v.FileName))
				{
					VerData pver = grp.First();
					string saveFn = dontSave ? Path.GetTempFileName() : pver.FileName;
					if (cancellationToken.IsCancellationRequested)
					{
						break;
					}

					progress?.Report((0, $"Updating version information in {pver.FileName}..."));
					System.Diagnostics.Debug.WriteLine($"Processing {pver.FileName}...");
					if (int.TryParse(pver.Locator, out _))
					{
						using MemoryStream ms = new();
						using (StreamReader rdr = new(pver.FileName, System.Text.Encoding.UTF8, true))
						{
							rdr.Peek();
							StreamWriter wr = new(ms, rdr.CurrentEncoding);
							Dictionary<int, VerData> lineLookup = grp.ToDictionary(v => int.Parse(v.Locator));
							for (int i = 0; !rdr.EndOfStream; i++)
							{
								string l = rdr.ReadLine();
								wr.WriteLine(lineLookup.TryGetValue(i + 1, out VerData ver)
									? ReplaceGroup(l, ver, newVer)
									: l);
							}
							wr.Flush();
						}
						ms.Seek(0, SeekOrigin.Begin);
						using FileStream fs = new(saveFn, FileMode.Truncate, FileAccess.Write);
						ms.CopyTo(fs);
						fs.Flush();
					}
					else
					{
						XmlDocument xmlDoc = new();
						xmlDoc.Load(pver.FileName);
						XmlNamespaceManager nsp = new(xmlDoc.NameTable);
						nsp.AddNamespace("x", pver.Namespace ?? string.Empty);
						foreach (VerData ver in grp)
						{
							if (ver.Locator is null)
							{
								XmlNode node = xmlDoc.SelectSingleNode("/x:Project/x:PropertyGroup[x:OutputType|x:TargetFrameworkVersion|x:TargetFramework|x:TargetFrameworks]", nsp);
								if (node is not null)
								{
									XmlNode nodeVer = xmlDoc.CreateNode(XmlNodeType.Element, "Version", null);
									nodeVer.InnerText = ReplaceGroup(nodeVer.InnerText, ver, newVer);
									node.AppendChild(nodeVer);
								}
							}
							else
							{
								foreach (XmlNode node in xmlDoc.SelectNodes(ver.Locator, nsp))
								{
									node.InnerText = ReplaceGroup(node.InnerText, ver, newVer);
								}
							}
						}

						xmlDoc.Save(saveFn);
					}

					if (dontSave)
						System.Diagnostics.Process.Start(saveFn);
					else
						progress?.Report((1, ""));
				}
			});

			string ReplaceGroup(string input, in VerData ver, NuGetVersion newVer) => Regex.Replace(input, ver.RegEx, m =>
			{
				Group grp = m.Groups[1];
				string replacement = grp.Name switch
				{
					"v2" => newVer.ToString("x.y", VersionFormatter.Instance),
					"v3" => newVer.ToString("x.y.z", VersionFormatter.Instance),
					"v4" => newVer.ToString("x.y.z.r", VersionFormatter.Instance),
					"ng" => newVer.ToNormalizedString(),
					_ => newVer.ToString("V", VersionFormatter.Instance)
				};
				return string.Concat(m.Value.Substring(0, grp.Index - m.Index), replacement, m.Value.Substring(grp.Index - m.Index + grp.Length));
			});
		}
	}
}