using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace IntentionalSolutionVersion
{
	public partial class VersionEdit : UserControl
	{
		public VersionEdit()
		{
			InitializeComponent();
		}

		[DefaultValue(true), Category("Appearance")]
		public bool ShowRevision
		{
			get => verRev.Visible;
			set
			{
				verRev.Visible = label3.Visible = value;
				tableLayoutPanel1.ColumnStyles[6] = new ColumnStyle(SizeType.Percent, value ? 25F : 0);
				tableLayoutPanel1.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, value ? 25F : 33.3F);
				tableLayoutPanel1.ColumnStyles[2] = new ColumnStyle(SizeType.Percent, value ? 25F : 33.3F);
				tableLayoutPanel1.ColumnStyles[4] = new ColumnStyle(SizeType.Percent, value ? 25F : 33.3F);
				tableLayoutPanel1.PerformLayout();
			}
		}

		[Browsable(false), Category("Appearance")]
		public Version Value
		{
			get
			{
				var maj = GetValue(verMajor, 0);
				var min = GetValue(verMinor, 0);
				var bld = GetValue(verBuild);
				var rev = ShowRevision ? GetValue(verRev) : -1;
				return bld == -1 ? new Version(maj, min) : (rev == -1 ? new Version(maj, min, bld) : new Version(maj, min, bld, rev));
			}

			set
			{
				if (value == null) value = new Version();
				verMajor.Text = value.Major.ToString();
				verMinor.Text = value.Minor.ToString();
				verBuild.Text = value.Build == -1 ? "" : value.Build.ToString();
				if (ShowRevision)
					verRev.Text = value.Revision == -1 ? "" : value.Revision.ToString();
			}
		}

		private int GetValue(TextBox tb, int def = -1) => tb.TextLength == 0 || !int.TryParse(tb.Text, out var val) ? def : val;

		private void ResetValue() => Value = new Version();

		private bool ShouldSerializeValue => Value != new Version();

		private void verMajor_KeyDown(object sender, KeyEventArgs e)
		{
			e.SuppressKeyPress = !(e.KeyCode == Keys.Back || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9 && e.Modifiers != Keys.Shift) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9));
		}
	}

	public enum VersionComparison
	{
		AllValues,
		IgnoreUnset
	}

	public static class VerExt
	{
		public static int CompareTo(this Version version, Version value, VersionComparison comparisonType)
		{
			if (value == null)
				return 1;

			if (version.Major != value.Major)
				return version.Major > value.Major ? 1 : -1;

			if (version.Minor != value.Minor)
				return version.Minor > value.Minor ? 1 : -1;

			if (version.Build != value.Build && (comparisonType == VersionComparison.IgnoreUnset && !(version.Build == -1 || value.Build == -1)))
				return version.Build > value.Build ? 1 : -1;

			if (version.Revision != value.Revision && (comparisonType == VersionComparison.IgnoreUnset && !(version.Revision == -1 || value.Revision == -1)))
				return version.Revision > value.Revision ? 1 : -1;

			return 0;
		}

		public static bool Equals(this Version version, Version value, VersionComparison comparisonType)
		{
			if (value == null)
				return false;

			if ((version.Major != value.Major) ||
				(version.Minor != value.Minor) ||
				(version.Build != value.Build && (comparisonType == VersionComparison.IgnoreUnset && !(version.Build == -1 || value.Build == -1))) ||
				(version.Revision != value.Revision && (comparisonType == VersionComparison.IgnoreUnset && !(version.Revision == -1 || value.Revision == -1))))
				return false;

			return true;
		}

		public static Version Increment(this Version value)
		{
			if (value == null) return null;
			var bld = value.Build;
			var rev = value.Revision;
			return bld == -1 ? new Version(value.Major, value.Minor + 1) : (rev == -1 ? new Version(value.Major, value.Minor, bld + 1) : new Version(value.Major, value.Minor, bld, rev + 1));
		}
	}
}