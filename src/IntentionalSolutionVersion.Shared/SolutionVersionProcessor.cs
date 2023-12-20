using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace IntentionalSolutionVersion
{
	internal static class SolutionVersionProcessor
	{
		// From https://semver.org/#is-there-a-suggested-regular-expression-regex-to-check-a-semver-string
		//const string defRegex = @"(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?";

		public static async Task<IList<VerData>> GetProjectVersionsAsync(string slnPath,
						IDictionary<string, List<string>> slnFiles,
						IEnumerable<string> asmInfoFiles,
						CancellationToken cancellationToken = default,
						IProgress<(int, string)> progress = null,
						bool includeWithoutVer = true)
		{
			const string msbld = "http://schemas.microsoft.com/developer/msbuild/2003";
			const string nuspec = "http://schemas.microsoft.com/packaging/2011/08/nuspec.xsd";
			const string nuspecold = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd";
			const string vsix = "http://schemas.microsoft.com/developer/vsx-schema/2011";
			const string vst = "http://schemas.microsoft.com/developer/vstemplate/2005";
			const string nupkgRegex = "!!";

			return await Task.Run(() =>
			{
				HashSet<VerData> d = new(EqualityComparer<VerData>.Default);
				if (!string.IsNullOrEmpty(slnPath))
				{
					Directory.SetCurrentDirectory(Path.GetDirectoryName(slnPath));
				}

				foreach (string proj in slnFiles.Keys)
				{
					if (cancellationToken.IsCancellationRequested)
					{
						break;
					}

					string projName = Path.GetFileName(proj.TrimEnd('\\'));
					progress?.Report((0, $"Retrieving version information for files in {projName}..."));

					bool foundVer = false;
					void AddVer(VerData ver) { if (!d.Contains(ver)) { ver.Project = projName; d.Add(ver); foundVer = true; } }

					switch (Path.GetExtension(proj).ToLowerInvariant())
					{
						case ".csproj":
						case ".vbproj":
							ProcessProjFile(proj, AddVer);
							foreach (string fn in GetProjectFiles(slnFiles[proj], ".vsixmanifest"))
							{
								foreach (VerData ver in GetXmlTagVersions(fn, "/x:PackageManifest/x:Metadata/x:Identity/@Version", vsix, null))
								{
									AddVer(ver);
								}

								foreach (VerData ver in GetXmlTagVersions(fn, "/x:PackageManifest/x:Assets/x:Asset/@Type", vsix, nupkgRegex))
								{
									AddVer(ver);
								}

								foreach (VerData ver in GetXmlTagVersions(fn, "/x:PackageManifest/x:Assets/x:Asset/@Path", vsix, nupkgRegex))
								{
									AddVer(ver);
								}
							}
							foreach (string fn in GetProjectFiles(slnFiles[proj], ".vstemplate"))
							{
								foreach (VerData ver in GetXmlTagVersions(fn, "/x:VSTemplate/x:WizardData/x:packages/x:package/@version", vst, null))
								{
									AddVer(ver);
								}
							}
							goto case "";

						case ".shfbproj":
							foreach (VerData ver in GetXmlTagVersions(proj, "/x:Project/x:PropertyGroup/x:HelpFileVersion", msbld, null))
							{
								AddVer(ver);
							}

							break;

						case ".vcxproj":
							// TODO: Add support for packages.config
							break;

						case "":
							foreach (string aifn in asmInfoFiles)
							{
								foreach (string fn in GetProjectFiles(slnFiles[proj], aifn))
								{
									if (TryGetAttrVersion(fn, "AssemblyVersion", out VerData aver))
									{
										AddVer(aver);
									}

									if (TryGetAttrVersion(fn, "AssemblyFileVersion", out aver))
									{
										AddVer(aver);
									}

									if (TryGetAttrVersion(fn, "AssemblyInformationalVersion", out aver))
									{
										AddVer(aver);
									}
								}
							}
							foreach (string fn in Directory.GetFiles(Path.GetDirectoryName(string.IsNullOrEmpty(Path.GetExtension(proj)) ? slnPath : proj), "Directory.Build.*"))
							{
								ProcessProjFile(fn, AddVer);
							}

							foreach (string fn in GetProjectFiles(slnFiles[proj], ".nuspec"))
							{
								ProcessNuspecFile(fn, AddVer);
							}

							if (!foundVer && includeWithoutVer && File.Exists(proj))
							{
								// Set invalid version, permit the initialization of all project without any version
								AddVer(new VerData(proj, new(0, 0, 0), "<Version>0.0.0</Version>", null, null));
							}
							break;

						default:
							break;
					}

					progress?.Report((1, ""));
				}

				return d.ToList();
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
				for (int i = 0; nodes is not null && i < nodes.Count; i++)
				{
					if (regEx is null or nupkgRegex)
					{
						if (NuGetVersion.TryParse(nodes[i].InnerText, out var ver))
						{
							yield return new VerData(fn, ver, RemoveNS(nodes[i].OuterXml), nodes.Count == 1 ? xmlPath : $"({xmlPath})[{i + 1}]", regEx, ns);
						}
					}
					else
					{
						Match m = Regex.Match(nodes[i].InnerText, regEx);
						if (m.Success)
						{
							yield return new VerData(fn, new(m.Groups[1].Value), RemoveNS(nodes[i].OuterXml), nodes.Count == 1 ? xmlPath : $"({xmlPath})[{i + 1}]", regEx, ns);
						}
					}
				}

				string RemoveNS(string value) => string.IsNullOrEmpty(ns) ? value : value.Replace($" xmlns=\"{ns}\"", string.Empty);
			}

			static void ProcessNuspecFile(string fn, Action<VerData> addVer)
			{
				foreach (VerData ver in GetXmlTagVersions(fn, "/x:package/x:metadata/x:version", nuspecold, null))
				{
					addVer(ver);
				}

				foreach (VerData ver in GetXmlTagVersions(fn, "/x:package/x:metadata/x:version", nuspec, null))
				{
					addVer(ver);
				}

				foreach (VerData ver in GetXmlTagVersions(fn, "/x:package/x:metadata/x:dependencies/x:dependency/@version", nuspec, @"(\d+\.\d+\.\d+)(?:\.[^\s\.]+)?"))
				{
					addVer(ver);
				}
			}

			static void ProcessProjFile(string projFile, Action<VerData> addVer)
			{
				foreach (VerData ver in GetXmlTagVersions(projFile, "/x:Project/x:PropertyGroup/x:Version", msbld, null))
				{
					addVer(ver);
				}

				foreach (VerData ver in GetXmlTagVersions(projFile, "/x:Project/x:PropertyGroup/x:ApplicationVersion", msbld, null))
				{
					addVer(ver);
				}

				foreach (VerData ver in GetXmlTagVersions(projFile, "/x:Project/x:ItemGroup/x:PackageReference/x:Version", msbld, null))
				{
					addVer(ver);
				}

				foreach (VerData ver in GetXmlTagVersions(projFile, "/x:Project/x:PropertyGroup/x:VersionPrefix", msbld, null))
				{
					addVer(ver);
				}

				foreach (VerData ver in GetXmlTagVersions(projFile, "/x:Project/x:ItemGroup/x:Content/@Include", msbld, nupkgRegex))
				{
					addVer(ver);
				}
			}

			static bool TryGetAttrVersion(string fn, string attr, out VerData ver)
			{
				string expr = $@"(?<!//.*)\[assembly:.*{attr}(?:Attribute)?\s*\(\s*\""(\d+\.\d+\.\d+)(?:\.[^\s\.]+)?\""\s*\)\s*\]";
				int n = 0;
				foreach (string l in File.ReadLines(fn))
				{
					n++;
					Match m = Regex.Match(l, expr);
					if (!m.Success)
					{
						continue;
					}

					ver = new VerData(fn, new (m.Groups[1].Value), l, n.ToString(), expr);
					return true;
				}
				ver = null;
				return false;
			}
		}

		public static async Task UpdateAsync(IEnumerable<VerData> vers, NuGetVersion newVer, CancellationToken cancellationToken = default, IProgress<(int, string)> progress = null)
		{
			await Task.Run(() =>
			{
				foreach (IGrouping<string, VerData> grp in vers.GroupBy(v => v.FileName))
				{
					VerData pver = grp.First();
					string saveFn =
#if NOSAVE
						Path.GetTempFileName();
#else
						pver.FileName;
#endif
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
									? ReplaceGroup(l, ver.RegEx, newVer.ToString())
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
									nodeVer.InnerText = newVer.ToString();
									node.AppendChild(nodeVer);
								}
							}
							else
							{
								foreach (XmlNode node in xmlDoc.SelectNodes(ver.Locator, nsp))
								{
									node.InnerText = ReplaceGroup(node.InnerText, ver.RegEx, newVer.ToString());
								}
							}
						}

						xmlDoc.Save(saveFn);
					}
#if NOSAVE
					System.Diagnostics.Process.Start(saveFn);
#endif
					progress?.Report((1, ""));
				}
			});

			string ReplaceGroup(string input, string pattern, string replacement) => Regex.Replace(input, pattern, m =>
				{
					Group grp = m.Groups[1];
					return string.Concat(m.Value.Substring(0, grp.Index - m.Index), replacement, m.Value.Substring(grp.Index - m.Index + grp.Length));
				});
		}
	}
}