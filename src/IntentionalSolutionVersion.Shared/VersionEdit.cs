using NuGet.Versioning;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace IntentionalSolutionVersion
{
	public partial class VersionEdit : UserControl
	{
		private static readonly NuGetVersion zero = new(0, 0, 0);

		public VersionEdit() => InitializeComponent();

		[DefaultValue(true), Category("Appearance")]
		public bool ShowRevision
		{
			get => verRev.Visible;
			set
			{
				verRev.Visible = label3.Visible = value;
				tableLayoutPanel1.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, value ? 20F : 25F);
				tableLayoutPanel1.ColumnStyles[2] = new ColumnStyle(SizeType.Percent, value ? 20F : 25F);
				tableLayoutPanel1.ColumnStyles[4] = new ColumnStyle(SizeType.Percent, value ? 20F : 25F);
				tableLayoutPanel1.ColumnStyles[6] = new ColumnStyle(SizeType.Percent, value ? 20F : 0);
				tableLayoutPanel1.ColumnStyles[8] = new ColumnStyle(SizeType.Percent, value ? 20F : 25F);
				tableLayoutPanel1.PerformLayout();
			}
		}

		[Browsable(false), Category("Appearance")]
		public NuGetVersion Value
		{
			get
			{
				int maj = GetValue(verMajor, 0);
				int min = GetValue(verMinor, 0);
				int bld = GetValue(verBuild, 0);
				int rev = ShowRevision ? GetValue(verRev, 0) : -1;
				string suf = verSuffix.TextLength == 0 ? null : verSuffix.Text;
				return rev == -1 ? new NuGetVersion(maj, min, bld, suf) : new NuGetVersion(maj, min, bld, rev, [ suf ], null);
			}

			set
			{
				value ??= zero;

				verMajor.Text = value.Major.ToString();
				verMinor.Text = value.Minor.ToString();
				verBuild.Text = value.Patch.ToString();
				if (ShowRevision)
				{
					verRev.Text = value.Revision == -1 ? "" : value.Revision.ToString();
				}
				verSuffix.Text = value.IsPrerelease ? value.Release : "";
			}
		}

		private bool ShouldSerializeValue => Value != zero;

		private static int GetValue(TextBox tb, int def = -1) => tb.TextLength == 0 || !int.TryParse(tb.Text, out int val) ? def : val;

		private void ResetValue() => Value = zero;

		private void verMajor_KeyDown(object sender, KeyEventArgs e) => e.SuppressKeyPress = !(e.KeyCode == Keys.Back || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9 && e.Modifiers != Keys.Shift) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9));
	}
}