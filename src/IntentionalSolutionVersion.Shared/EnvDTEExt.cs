using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Constants = EnvDTE.Constants;

namespace IntentionalSolutionVersion
{
	internal static class EnvDTEExt
	{
		public static IEnumerable<ProjectItem> EnumProjectItems(this Project project, string kind = null)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
			return (project?.ProjectItems?.Cast<ProjectItem>() ?? Enumerable.Empty<ProjectItem>()).
				SelectMany(ti => TreeTraversal(ti, i => i.ProjectItems?.Cast<ProjectItem>().
				Where(i2 => kind == null || kind == i2.Kind) ?? Enumerable.Empty<ProjectItem>()));
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
		}

		public static IEnumerable<Project> EnumProjects(this Solution sln)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			foreach (var proj in sln.Projects.Cast<Project>())
			{
				yield return proj;
				foreach (var cp in EnumProjectItem(proj).Select(PI2P).Where(p => p != null))
					foreach (var ccp in TreeTraversal(cp, pi => EnumProjectItem(pi).Select(PI2P).Where(p => p != null)))
						yield return ccp;
			}

			static Project PI2P(ProjectItem projItem) => (projItem.SubProject ?? projItem.Object) as Project;
		}

		public static string GetFileName(this ProjectItem item)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (item.FileCount == 0)
			{
				return null;
			}

			try { return item.FileNames[0]; }
			catch { return item.FileNames[1]; }
		}

		public static IDictionary<string, List<string>> GetFiles(this Solution solution)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			Dictionary<string, List<string>> d = new();
			List<string> bad = new();
			HashSet<string> files = new();
			foreach (Project p in solution.EnumProjects())
			{
				List<string> l = new();
				foreach (ProjectItem i in p.EnumProjectItems())
				{
					string kn = Name(i.Kind);
					System.Diagnostics.Debug.WriteLine(kn);
					if (i.Kind != Constants.vsProjectItemKindPhysicalFile && i.Kind != Constants.vsProjectItemKindSolutionItems || i.FileCount <= 0)
					{
						continue;
					}

					string lfn = i.GetFileName();
					if (!string.IsNullOrEmpty(lfn))
					{
						if (!files.Contains(lfn))
						{
							l.Add(lfn);
							files.Add(lfn);
						}
					}
				}
				if (l.Count > 0)
				{
					string fn = null;
					try { fn = p.FileName.Contains('\\') ? p.FileName : p.FullName; } catch { }
					if (string.IsNullOrEmpty(fn))
					{
						fn = p.Name;
					}

					try { d.Add(fn, l); } catch { bad.Add(fn); }
				}
			}
			if (bad.Count > 0)
			{
				ShowMessageBox("Duplicate projects. Please report as issue and include this text. (Press Ctrl-C to capture)\n\r" + string.Join("\n\r", bad) + "\n\r\n\rFound projects:" + string.Join("\n\r", d.Keys));
			}

			return d;
		}

		private static IEnumerable<ProjectItem> EnumProjectItem(Project pr) => pr?.ProjectItems?.Cast<ProjectItem>() ?? Enumerable.Empty<ProjectItem>();

		private static IEnumerable<ProjectItem> EnumProjectItem(ProjectItem pi) => pi?.ProjectItems?.Cast<ProjectItem>() ?? Enumerable.Empty<ProjectItem>();

		private static string Name(string vsConst)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			return typeof(Constants).
				GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).
				FirstOrDefault(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string) && fi.Name.StartsWith("vs") && !fi.Name.StartsWith("vsext") && vsConst.Equals(fi.GetRawConstantValue().ToString(), StringComparison.InvariantCultureIgnoreCase))?.Name ?? vsConst;
		}

		private static IEnumerable<T> TreeTraversal<T>(T root, Func<T, IEnumerable<T>> children)
		{
			Stack<T> stack = new();
			stack.Push(root);
			while (stack.Count != 0)
			{
				T current = stack.Pop();
				yield return current;
				foreach (T child in children(current).Reverse())
				{
					stack.Push(child);
				}
			}
		}

		public static void ShowMessageBox(string text) => VsShellUtilities.ShowMessageBox((IServiceProvider)SetVerCmd.Instance.ServiceProvider.GetServiceAsync(typeof(IServiceProvider)), text, null, OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
	}
}