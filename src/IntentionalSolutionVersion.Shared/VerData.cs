using System;
using System.IO;

namespace IntentionalSolutionVersion
{
	internal class VerData(string fn, Version ver, string line, string loc, string regex, string ns = null) : IEquatable<VerData>
	{
		public string FileName { get; set; } = fn; public string LineText { get; set; } = line;
		// This may be an XMLPath stmt or Line#
		public string Locator { get; set; } = loc;
		public string Namespace { get; set; } = ns;
		public string Project { get; set; }
		public string RegEx { get; set; } = regex; public Version Version { get; set; } = ver;
		public bool Equals(VerData other) => FileName == other.FileName && LineText == other.LineText && RegEx == other.RegEx;

		public override string ToString() => $"{Path.GetFileName(FileName)}={Version}:{LineText}";
	}
}
