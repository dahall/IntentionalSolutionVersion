using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IntentionalSolutionVersion
{
	internal static class EnvDTEExt
	{
		public static IEnumerable<ProjectItem> EnumProjectItems(this Project project, string kind = null)
		{
			foreach (var ti in project?.ProjectItems?.Cast<ProjectItem>() ?? new ProjectItem[0])
				foreach (var pi in TreeTraversal(ti, i => i.ProjectItems?.Cast<ProjectItem>().Where(i2 => kind == null || kind == i2.Kind) ?? new ProjectItem[0]))
					yield return pi;
		}

		public static IEnumerable<Project> EnumProjects(this Solution sln)
		{
			foreach (var proj in (sln?.Projects?.Cast<Project>() ?? new Project[0]))
			{
				yield return proj;
				foreach (var cp in EnumProjectItem(proj).Select(PI2P).Where(p => p != null))
					foreach (var ccp in TreeTraversal(cp, pi => EnumProjectItem(pi).Select(PI2P).Where(p => p != null)))
						yield return ccp;
			}

			Project PI2P(ProjectItem projItem) => (projItem.SubProject ?? projItem.Object) as Project;
		}

		public static string GetFileName(this ProjectItem item)
		{
			if (item.FileCount == 0) return null;
			try { return item.FileNames[0]; }
			catch { return item.FileNames[1]; }
		}

		public static IDictionary<string, List<string>> GetFiles(this Solution sln)
		{
			var d = new Dictionary<string, List<string>>();
			var bad = new List<string>();
			foreach (var p in sln.EnumProjects()) //.Distinct(new ProjEqComp()))
			{
				var fn = p.FileName.Contains('\\') ? p.FileName : p.FullName;
				if (string.IsNullOrEmpty(fn)) fn = p.Name;
				var l = new List<string>();
				foreach (var i in p.EnumProjectItems())
				{
					var kn = Name(i.Kind);
					System.Diagnostics.Debug.WriteLine(kn);
					if ((i.Kind == Constants.vsProjectItemKindPhysicalFile || i.Kind == Constants.vsProjectItemKindSolutionItems) && i.FileCount > 0)
					{
						var lfn = i.GetFileName();
						if (!string.IsNullOrEmpty(lfn)) l.Add(lfn);
					}
				}
				try { d.Add(fn, l); } catch { bad.Add(fn); }
			}
			if (bad.Count > 0)
				System.Windows.Forms.MessageBox.Show("Duplicate projects. Please report as issue and include this text. (Press Ctrl-C to capture)\n\r" + string.Join("\n\r", bad) + "\n\r\n\rFound projects:" + string.Join("\n\r", d.Keys));
			return d;
		}

		private static IEnumerable<ProjectItem> EnumProjectItem(Project pr) => pr?.ProjectItems?.Cast<ProjectItem>() ?? new ProjectItem[0];

		private static IEnumerable<ProjectItem> EnumProjectItem(ProjectItem pi) => pi?.ProjectItems?.Cast<ProjectItem>() ?? new ProjectItem[0];

		private static string Name(string vsConst)
		{
			return typeof(Constants).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).
				Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string) && fi.Name.StartsWith("vs") && !fi.Name.StartsWith("vsext") && vsConst.Equals(fi.GetRawConstantValue().ToString(), StringComparison.InvariantCultureIgnoreCase)).
				FirstOrDefault()?.Name ?? vsConst;
		}

		private static IEnumerable<T> TreeTraversal<T>(T root, Func<T, IEnumerable<T>> children)
		{
			var stack = new Stack<T>();
			stack.Push(root);
			while (stack.Count != 0)
			{
				var current = stack.Pop();
				yield return current;
				foreach (var child in children(current).Reverse())
					stack.Push(child);
			}
		}
	}
}