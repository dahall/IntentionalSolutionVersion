using System;

namespace IntentionalSolutionVersion
{
	/// <summary>Indicate how to compare <see cref="Version"/> values.</summary>
	public enum VersionComparison
	{
		/// <summary>Look at all values.</summary>
		AllValues,

		/// <summary>Ignore any unset values.</summary>
		IgnoreUnset
	}

	/// <summary>Extensions for <see cref="Version"/>.</summary>
	public static class VersionExt
	{
		/// <summary>Compares a <see cref="Version"/> instance to another and returns an indication of their relative values.</summary>
		/// <param name="version">The version to compare.</param>
		/// <param name="value">The other version to compare.</param>
		/// <param name="comparisonType">Type of the comparison.</param>
		/// <returns>
		/// A signed number indicating the relative values of this instance and value.
		/// <list type="table">
		/// <listheader>
		/// <term>Return Value</term>
		/// <term>Description</term>
		/// </listheader>
		/// <item>
		/// <term>Less than zero</term>
		/// <description>This instance is less than value.</description>
		/// </item>
		/// <item>
		/// <term>Zero</term>
		/// <description>This instance is equal to value.</description>
		/// </item>
		/// <item>
		/// <term>Greater than zero</term>
		/// <description>This instance is greater than value.</description>
		/// </item>
		/// </list>
		/// </returns>
		public static int CompareTo(this Version version, Version value, VersionComparison comparisonType)
		{
			if (value == null)
			{
				return 1;
			}

			if (version.Major != value.Major)
			{
				return version.Major > value.Major ? 1 : -1;
			}

			if (version.Minor != value.Minor)
			{
				return version.Minor > value.Minor ? 1 : -1;
			}

			if (version.Build != value.Build && comparisonType == VersionComparison.IgnoreUnset &&
				!(version.Build == -1 || value.Build == -1))
			{
				return version.Build > value.Build ? 1 : -1;
			}

			if (version.Revision != value.Revision && comparisonType == VersionComparison.IgnoreUnset &&
				!(version.Revision == -1 || value.Revision == -1))
			{
				return version.Revision > value.Revision ? 1 : -1;
			}

			return 0;
		}

		/// <summary>Determines if two <see cref="Version"/> values are equal.</summary>
		/// <param name="version">The version to compare.</param>
		/// <param name="value">The other version to compare.</param>
		/// <param name="comparisonType">Type of the comparison.</param>
		/// <returns><see langword="true"/> if the values are equal; <see langword="false"/> otherwise.</returns>
		public static bool Equals(this Version version, Version value, VersionComparison comparisonType)
		{
			if (value == null)
			{
				return false;
			}

			return version.Major == value.Major && version.Minor == value.Minor &&
				   (version.Build == value.Build || comparisonType != VersionComparison.IgnoreUnset ||
					version.Build == -1 || value.Build == -1) &&
				   (version.Revision == value.Revision || comparisonType != VersionComparison.IgnoreUnset ||
					version.Revision == -1 || value.Revision == -1);
		}

		/// <summary>
		/// Increments the specified <see cref="Version"/>. If a sub-value is undefined (-1), then the next most prominent value is incremented.
		/// </summary>
		/// <param name="value">The <see cref="Version"/> value.</param>
		/// <returns>New new, incremented <see cref="Version"/> value.</returns>
		public static Version Increment(this Version value)
		{
			if (value == null)
			{
				return null;
			}

			int bld = value.Build;
			int rev = value.Revision;
			return bld == -1 ? new Version(value.Major, value.Minor + 1) :
				rev == -1 ? new Version(value.Major, value.Minor, bld + 1) :
				new Version(value.Major, value.Minor, bld, rev + 1);
		}
	}
}