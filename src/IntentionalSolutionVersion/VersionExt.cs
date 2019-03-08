using System;

namespace IntentionalSolutionVersion
{
	public enum VersionComparison
	{
		AllValues,
		IgnoreUnset
	}

	public static class VersionExt
	{
		public static int CompareTo(this Version version, Version value, VersionComparison comparisonType)
		{
			if (value == null)
				return 1;

			if (version.Major != value.Major)
				return version.Major > value.Major ? 1 : -1;

			if (version.Minor != value.Minor)
				return version.Minor > value.Minor ? 1 : -1;

			if (version.Build != value.Build && comparisonType == VersionComparison.IgnoreUnset &&
			    !(version.Build == -1 || value.Build == -1))
				return version.Build > value.Build ? 1 : -1;

			if (version.Revision != value.Revision && comparisonType == VersionComparison.IgnoreUnset &&
			    !(version.Revision == -1 || value.Revision == -1))
				return version.Revision > value.Revision ? 1 : -1;

			return 0;
		}

		public static bool Equals(this Version version, Version value, VersionComparison comparisonType)
		{
			if (value == null)
				return false;

			return version.Major == value.Major && version.Minor == value.Minor &&
			       (version.Build == value.Build || comparisonType != VersionComparison.IgnoreUnset ||
			        version.Build == -1 || value.Build == -1) &&
			       (version.Revision == value.Revision || comparisonType != VersionComparison.IgnoreUnset ||
			        version.Revision == -1 || value.Revision == -1);
		}

		public static Version Increment(this Version value)
		{
			if (value == null) return null;
			var bld = value.Build;
			var rev = value.Revision;
			return bld == -1 ? new Version(value.Major, value.Minor + 1) :
				rev == -1 ? new Version(value.Major, value.Minor, bld + 1) :
				new Version(value.Major, value.Minor, bld, rev + 1);
		}
	}
}