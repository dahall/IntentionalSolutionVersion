using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static IntentionalSolutionVersion.SolutionVersionProcessor;

namespace IntentionalSolutionVersion.TestProject
{
	public class Tests
	{
		string sln = "";
		readonly Dictionary<string, List<string>> projFiles = [];

		[OneTimeSetUp]
		public void Setup()
		{
			// Load files
			string[] tmp = File.ReadAllLines(@"C:\Temp\prjfileinfo.txt");
			sln = tmp[0];
			string slnDir = Path.GetDirectoryName(sln);
			List<string> files = null;
			for (int i = 1; i < tmp.Length; i++)
			{
				if (tmp[i][0] != ' ')
					projFiles.Add($"{slnDir}\\{tmp[i]}", files = []);
				else
					files.Add($"{slnDir}\\{tmp[i].TrimStart()}");
			}
		}

		[Test]
		public async Task GetDataTestAsync()
		{
			// Get data
			var data = await GetProjectVersionsAsync(sln, projFiles, Properties.Settings.Default.AssemblyInfoFileNames.Split(';'));

			// Write data
			foreach (var d in data)
				TestContext.WriteLine($"{Path.GetFileName(d.FileName)}\t{d.Project}\t{d.Version.ToFullString()}\t{d.LineText}");
		}

		[Test]
		public async Task UpdateTestAsync()
		{
			// Get data
			var data = await GetProjectVersionsAsync(sln, projFiles, Properties.Settings.Default.AssemblyInfoFileNames.Split(';'));

			// Write data
			await UpdateAsync(data, new(2,0,2,0, "beta", null), dontSave: true);
		}
	}
}