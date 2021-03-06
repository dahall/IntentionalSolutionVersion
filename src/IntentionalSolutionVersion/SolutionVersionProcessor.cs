﻿using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace IntentionalSolutionVersion
{
	internal static class SolutionVersionProcessor
	{
		private const string defRegex = @"(\d+(?:\.\d+){1,2})";

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
			const string nupkgRegex = @".*\.(\d+\.\d+\.\d+)(?:\.[^\s\.]+)?\.nupkg";

			return await Task.Run(() =>
			{
				var d = new HashSet<VerData>(EqualityComparer<VerData>.Default);
				if (!string.IsNullOrEmpty(slnPath))
					Directory.SetCurrentDirectory(Path.GetDirectoryName(slnPath));
				foreach (var proj in slnFiles.Keys)
				{
					if (cancellationToken.IsCancellationRequested) break;
					var projName = Path.GetFileName(proj.TrimEnd('\\'));
					progress?.Report((0, $"Retrieving version information for files in {projName}..."));

					var foundVer = false;
					void AddVer(VerData ver) { if (!d.Contains(ver)) { ver.Project = projName; d.Add(ver); foundVer = true; } }

					switch (Path.GetExtension(proj).ToLowerInvariant())
					{
						case ".csproj":
						case ".vbproj":
							ProcessProjFile(proj, AddVer);
							foreach (var fn in GetProjectFiles(slnFiles[proj], ".vsixmanifest"))
							{
								foreach (var ver in GetXmlTagVersions(fn, "/x:PackageManifest/x:Metadata/x:Identity/@Version", vsix, null))
									AddVer(ver);
								foreach (var ver in GetXmlTagVersions(fn, "/x:PackageManifest/x:Assets/x:Asset/@Type", vsix, nupkgRegex))
									AddVer(ver);
								foreach (var ver in GetXmlTagVersions(fn, "/x:PackageManifest/x:Assets/x:Asset/@Path", vsix, nupkgRegex))
									AddVer(ver);
							}
							foreach (var fn in GetProjectFiles(slnFiles[proj], ".vstemplate"))
							{
								foreach (var ver in GetXmlTagVersions(fn, "/x:VSTemplate/x:WizardData/x:packages/x:package/@version", vst, null))
									AddVer(ver);
							}
							goto case "";

						case ".shfbproj":
							foreach (var ver in GetXmlTagVersions(proj, "/x:Project/x:PropertyGroup/x:HelpFileVersion", msbld, null))
								AddVer(ver);
							break;

						case "":
							foreach (var aifn in asmInfoFiles)
							{
								foreach (var fn in GetProjectFiles(slnFiles[proj], aifn))
								{
									if (TryGetAttrVersion(fn, "AssemblyVersion", out var aver))
										AddVer(aver);
									if (TryGetAttrVersion(fn, "AssemblyFileVersion", out aver))
										AddVer(aver);
									if (TryGetAttrVersion(fn, "AssemblyInformationalVersion", out aver))
										AddVer(aver);
								}
							}
							foreach (var fn in Directory.GetFiles(Path.GetDirectoryName(string.IsNullOrEmpty(Path.GetExtension(proj)) ? slnPath : proj), "Directory.Build.*"))
								ProcessProjFile(fn, AddVer);
							foreach (var fn in GetProjectFiles(slnFiles[proj], ".nuspec"))
								ProcessNuspecFile(fn, AddVer);
							if (!foundVer && includeWithoutVer)
							{
								// Set invalid version, permit the initialization of all project without any version
								AddVer(new VerData(proj, new Version(0,0,0), "<Version>0.0.0</Version>", null, null));
							}
							break;

						default:
							break;
					}

					progress?.Report((1, ""));
				}

				return d.ToList();
			});


			IEnumerable<string> GetProjectFiles(IEnumerable<string> projectFiles, string name)
			{
				foreach (var i in projectFiles)
				{
					if (i != null && (name[0] == '.' && Path.GetExtension(i).Equals(name, StringComparison.InvariantCultureIgnoreCase)) ||
						(name[0] != '.' && Path.GetFileName(i).Equals(name, StringComparison.InvariantCultureIgnoreCase)))
						yield return i;
				}
			}

			IEnumerable<VerData> GetXmlTagVersions(string fn, string xmlPath, string ns, string regEx = null)
			{
				var xmlDoc = new XmlDocument();
				xmlDoc.Load(fn);
				var nsp = new XmlNamespaceManager(xmlDoc.NameTable);
				nsp.AddNamespace("x", ns);
				var nodes = xmlDoc.SelectNodes(xmlPath, nsp);
				if (nodes is null || nodes.Count == 0)
				{
					ns = "";
					nsp.AddNamespace("x", ns);
					nodes = xmlDoc.SelectNodes(xmlPath, nsp);
				}
				for (var i = 0; !(nodes is null) && i < nodes.Count; i++)
				{
					if (regEx == null) regEx = defRegex;
					var m = Regex.Match(nodes[i].InnerText, regEx);
					if (m.Success)
						yield return new VerData(fn, new Version(m.Groups[1].Value), nodes[i].OuterXml, nodes.Count == 1 ? xmlPath : $"({xmlPath})[{i + 1}]", regEx, ns);
				}
			}

			void ProcessNuspecFile(string fn, Action<VerData> addVer)
			{
				foreach (var ver in GetXmlTagVersions(fn, "/x:package/x:metadata/x:version", nuspecold, null))
					addVer(ver);
				foreach (var ver in GetXmlTagVersions(fn, "/x:package/x:metadata/x:version", nuspec, null))
					addVer(ver);
				foreach (var ver in GetXmlTagVersions(fn, "/x:package/x:metadata/x:dependencies/x:dependency/@version", nuspec, @"(\d+\.\d+\.\d+)(?:\.[^\s\.]+)?"))
					addVer(ver);
			}

			void ProcessProjFile(string projFile, Action<VerData> addVer)
			{
				foreach (var ver in GetXmlTagVersions(projFile, "/x:Project/x:PropertyGroup/x:Version", msbld, null))
					addVer(ver);
				foreach (var ver in GetXmlTagVersions(projFile, "/x:Project/x:PropertyGroup/x:ApplicationVersion", msbld, null))
					addVer(ver);
				foreach (var ver in GetXmlTagVersions(projFile, "/x:Project/x:ItemGroup/x:PackageReference/x:Version", msbld, null))
					addVer(ver);
				foreach (var ver in GetXmlTagVersions(projFile, "/x:Project/x:PropertyGroup/x:VersionPrefix", msbld, null))
					addVer(ver);
				foreach (var ver in GetXmlTagVersions(projFile, "/x:Project/x:ItemGroup/x:Content/@Include", msbld, nupkgRegex))
					addVer(ver);
			}

			bool TryGetAttrVersion(string fn, string attr, out VerData ver)
			{
				var expr = $@"\[assembly:.*{attr}(?:Attribute)?\s*\(\s*\""(\d+\.\d+\.\d+)(?:\.[^\s\.]+)?\""\s*\)\s*\]";
				var n = 0;
				foreach (var l in File.ReadLines(fn))
				{
					n++;
					var m = Regex.Match(l, expr);
					if (!m.Success) continue;
					ver = new VerData(fn, new Version(m.Groups[1].Value), l, n.ToString(), expr);
					return true;
				}
				ver = null;
				return false;
			}
		}

		public static async Task UpdateAsync(IEnumerable<VerData> vers, Version newVer, CancellationToken cancellationToken = default, IProgress<(int, string)> progress = null)
		{
			await Task.Run(() =>
			{
				foreach (var grp in vers.GroupBy(v => v.FileName))
				{
					var pver = grp.First();
					var saveFn =
#if NOSAVE
						Path.GetTempFileName();
#else
						pver.FileName;
#endif
					if (cancellationToken.IsCancellationRequested) break;
					progress?.Report((0, $"Updating version information in {pver.FileName}..."));
					System.Diagnostics.Debug.WriteLine($"Processing {pver.FileName}...");
					if (int.TryParse(pver.Locator, out _))
					{
						using (var ms = new MemoryStream())
						{
							using (var rdr = new StreamReader(pver.FileName, System.Text.Encoding.UTF8, true))
							{
								rdr.Peek();
								var wr = new StreamWriter(ms, rdr.CurrentEncoding);
								var lineLookup = grp.ToDictionary(v => int.Parse(v.Locator));
								for (var i = 0; !rdr.EndOfStream; i++)
								{
									var l = rdr.ReadLine();
									wr.WriteLine(lineLookup.TryGetValue(i + 1, out var ver)
										? ReplaceGroup(l, ver.RegEx, newVer.ToString())
										: l);
								}
								wr.Flush();
							}
							ms.Seek(0, SeekOrigin.Begin);
							using (var fs = new FileStream(saveFn, FileMode.Truncate, FileAccess.Write))
							{
								ms.CopyTo(fs);
								fs.Flush();
							}
						}
					}
					else
					{
						var xmlDoc = new XmlDocument();
						xmlDoc.Load(pver.FileName);
						var nsp = new XmlNamespaceManager(xmlDoc.NameTable);
						nsp.AddNamespace("x", pver.Namespace ?? string.Empty);
						foreach (var ver in grp)
							if (ver.Locator == null)
							{
								foreach (XmlNode node in xmlDoc.SelectNodes("/x:Project/x:PropertyGroup", nsp))
								{
									bool validForVersion = false;
									foreach (XmlNode child in node.ChildNodes)
										if (child.Name.Equals("RootNamespace", StringComparison.OrdinalIgnoreCase))
											validForVersion = true;
									if (validForVersion)
									{ 
										var nodeVer = xmlDoc.CreateNode(XmlNodeType.Element, "Version", null);
										nodeVer.InnerText = newVer.ToString();
										node.AppendChild(nodeVer);
									}
								}
							}
							else
							{
								foreach (XmlNode node in xmlDoc.SelectNodes(ver.Locator, nsp))
									node.InnerText = ReplaceGroup(node.InnerText, ver.RegEx, newVer.ToString());
							}
						xmlDoc.Save(saveFn);
					}
#if NOSAVE
					System.Diagnostics.Process.Start(saveFn);
#endif
					progress?.Report((1, ""));
				}
			});

			string ReplaceGroup(string input, string pattern, string replacement)
			{
				return Regex.Replace(input, pattern, m =>
				{
					var grp = m.Groups[1];
					return string.Concat(m.Value.Substring(0, grp.Index - m.Index), replacement, m.Value.Substring(grp.Index - m.Index + grp.Length));
				});
			}
		}
	}
}