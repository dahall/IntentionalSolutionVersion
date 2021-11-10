using System;
using System.IO;

namespace IntentionalSolutionVersion
{
	internal class VerData : IEquatable<VerData>
	{
		public VerData(string fn, Version ver, string line, string loc, string regex, string ns = null)
		{
			FileName = fn; Version = ver; LineText = line; Locator = loc; RegEx = regex; Namespace = ns;
		}

		public string FileName { get; set; }
		public string LineText { get; set; }

		// This may be an XMLPath stmt or Line#
		public string Locator { get; set; }

		public string Namespace { get; set; }
		public string Project { get; set; }
		public string RegEx { get; set; }
		public Version Version { get; set; }

		public bool Equals(VerData other) => FileName == other.FileName && LineText == other.LineText && RegEx == other.RegEx;

		public override string ToString() => $"{Path.GetFileName(FileName)}={Version}:{LineText}";
	}
}
