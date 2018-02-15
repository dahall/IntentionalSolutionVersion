using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace IntentionalSolutionVersion
{
	internal static class SolutionVersionProcessor
	{
		private const string defRegex = @"(\d+(?:\.\d+){2,3})";

		public static async Task<IList<VerData>> GetProjectVersions(IDictionary<string, List<string>> slnFiles)
		{
			const string msbld = "http://schemas.microsoft.com/developer/msbuild/2003";
			const string nuspec = "http://schemas.microsoft.com/packaging/2011/08/nuspec.xsd";
			const string vsix = "http://schemas.microsoft.com/developer/vsx-schema/2011";
			const string vst = "http://schemas.microsoft.com/developer/vstemplate/2005";

			return await Task.Run(() =>
			{
				var d = new List<VerData>();

				foreach (string proj in slnFiles.Keys)
				{
					var projName = Path.GetFileName(proj);

					void AddVer(VerData ver) { ver.Project = projName; d.Add(ver); }

					switch (Path.GetExtension(proj).ToLowerInvariant())
					{
						case ".csproj":
							foreach (var ver in GetXmlTagVersions(proj, "/x:Project/x:PropertyGroup/x:Version", msbld, null))
								AddVer(ver);
							foreach (var ver in GetXmlTagVersions(proj, "/x:Project/x:PropertyGroup/x:ApplicationVersion", msbld, null))
								AddVer(ver);
							foreach (var ver in GetXmlTagVersions(proj, "/x:Project/x:ItemGroup/x:PackageReference/x:Version", msbld, null))
								AddVer(ver);
							foreach (var ver in GetXmlTagVersions(proj, "/x:Project/x:PropertyGroup/x:VersionPrefix", msbld, null))
								AddVer(ver);
							foreach (var ver in GetXmlTagVersions(proj, "/x:Project/x:ItemGroup/x:Content/@Include", msbld, @".*\.(\d+\.\d+\.\d+)\.nupkg"))
								AddVer(ver);
							foreach (var fn in GetProjectFiles(slnFiles[proj], "AssemblyInfo.cs"))
							{
								if (TryGetAttrVersion(fn, "AssemblyVersion", out VerData aver))
									AddVer(aver);
								if (TryGetAttrVersion(fn, "AssemblyFileVersion", out aver))
									AddVer(aver);
								if (TryGetAttrVersion(fn, "AssemblyInformationalVersion", out aver))
									AddVer(aver);
							}
							foreach (var fn in GetProjectFiles(slnFiles[proj], ".nuspec"))
							{
								foreach (var ver in GetXmlTagVersions(fn, "/x:package/x:metadata/x:version", nuspec, null))
									AddVer(ver);
								foreach (var ver in GetXmlTagVersions(fn, "/x:package/x:metadata/x:dependencies/x:dependency/@version", nuspec, @"\[(\d+\.\d+\.\d+)\]"))
									AddVer(ver);
							}
							foreach (var fn in GetProjectFiles(slnFiles[proj], ".vsixmanifest"))
							{
								foreach (var ver in GetXmlTagVersions(fn, "/x:PackageManifest/x:Metadata/x:Identity/@Version", vsix, null))
									AddVer(ver);
								foreach (var ver in GetXmlTagVersions(fn, "/x:PackageManifest/x:Assets/x:Asset/@Type", vsix, @".*\.(\d+\.\d+\.\d+)\.nupkg"))
									AddVer(ver);
								foreach (var ver in GetXmlTagVersions(fn, "/x:PackageManifest/x:Assets/x:Asset/@Path", vsix, @".*\.(\d+\.\d+\.\d+)\.nupkg"))
									AddVer(ver);
							}
							foreach (var fn in GetProjectFiles(slnFiles[proj], ".vstemplate"))
							{
								foreach (var ver in GetXmlTagVersions(fn, "/x:VSTemplate/x:WizardData/x:packages/x:package/@version", vst, null))
									AddVer(ver);
							}
							break;

						case ".shfbproj":
							foreach (var ver in GetXmlTagVersions(proj, "/x:Project/x:PropertyGroup/x:HelpFileVersion", msbld, null))
								AddVer(ver);
							break;

						default:
							break;
					}
				}

				return d;
			});
		}

		public static async Task Update(IEnumerable<VerData> vers, Version newVer)
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
					System.Diagnostics.Debug.WriteLine($"Processing {pver.FileName}...");
					if (int.TryParse(pver.Locator, out var _))
					{
						using (var ms = new MemoryStream())
						{
							using (var rdr = new StreamReader(pver.FileName, System.Text.Encoding.UTF8, true))
							{
								rdr.Peek();
								var wr = new StreamWriter(ms, rdr.CurrentEncoding);
								var lineLookup = grp.ToDictionary(v => int.Parse(v.Locator));
								for (int i = 0; !rdr.EndOfStream; i++)
								{
									string l = rdr.ReadLine();
									if (lineLookup.TryGetValue(i + 1, out var ver))
										wr.WriteLine(ReplaceGroup(l, ver.RegEx, newVer.ToString()));
									else
										wr.WriteLine(l);
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
						nsp.AddNamespace("x", pver.Namespace);
						foreach (var ver in grp)
							foreach (XmlNode node in xmlDoc.SelectNodes(ver.Locator, nsp))
								node.InnerText = ReplaceGroup(node.InnerText, ver.RegEx, newVer.ToString());
						xmlDoc.Save(saveFn);
					}
#if NOSAVE
					System.Diagnostics.Process.Start(saveFn);
#endif
				}
			});
		}

		private static IEnumerable<string> GetProjectFiles(IEnumerable<string> projectFiles, string name)
		{
			foreach (var i in projectFiles)
			{
				if ((name[0] == '.' && Path.GetExtension(i).Equals(name, StringComparison.InvariantCultureIgnoreCase)) ||
					(name[0] != '.' && Path.GetFileName(i).Equals(name, StringComparison.InvariantCultureIgnoreCase)))
					yield return i;
			}
		}

		private static IEnumerable<VerData> GetXmlTagVersions(string fn, string xmlPath, string ns, string regEx = null)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(fn);
			var nsp = new XmlNamespaceManager(xmlDoc.NameTable);
			nsp.AddNamespace("x", ns);
			var nodes = xmlDoc.SelectNodes(xmlPath, nsp);
			if (nodes.Count == 0)
			{
				ns = "";
				nsp.AddNamespace("x", ns);
				nodes = xmlDoc.SelectNodes(xmlPath, nsp);
			}
			for (int i = 0; i < nodes.Count; i++)
			{
				if (regEx == null) regEx = defRegex;
				var m = Regex.Match(nodes[i].InnerText, regEx);
				if (m.Success)
					yield return new VerData(fn, new Version(m.Groups[1].Value), nodes[i].OuterXml, nodes.Count == 1 ? xmlPath : $"({xmlPath})[{i + 1}]", regEx, ns);
			}
		}

		private static string ReplaceGroup(string input, string pattern, string replacement)
		{
			return Regex.Replace(input, pattern, m =>
			{
				var grp = m.Groups[1];
				return String.Concat(m.Value.Substring(0, grp.Index - m.Index), replacement, m.Value.Substring(grp.Index - m.Index + grp.Length));
			});
		}

		private static bool TryGetAttrVersion(string fn, string attr, out VerData ver)
		{
			var expr = string.Format(@"\[assembly:.*{0}(?:Attribute)?\s*\(\s*\""(\d+(?:\.\d+){{2,3}})\""\s*\)\s*\]", attr);
			int n = 0;
			foreach (var l in File.ReadLines(fn))
			{
				n++;
				var m = Regex.Match(l, expr);
				if (m.Success)
				{
					ver = new VerData(fn, new Version(m.Groups[1].Value), l, n.ToString(), expr);
					return true;
				}
			}
			ver = null;
			return false;
		}
	}
}