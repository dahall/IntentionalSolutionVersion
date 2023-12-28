using Microsoft.VisualStudio.Shell;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IntentionalSolutionVersion
{
	internal partial class VersionDialog : Form
	{
		private readonly IDictionary<string, List<string>> sln;
		private readonly string slnFileName;
		private IList<VerData> data;

		public VersionDialog(string slnPath, IDictionary<string, List<string>> dictionary)
		{
			InitializeComponent();
			sln = dictionary;
			slnFileName = slnPath;
			list.ItemChecked += List_ItemChecked;
			list.ListViewItemSorter = new ListViewItemComparer();
		}

		public NuGetVersion NewVersion
		{
			get => newVerEdit.Value;
			private set => newVerEdit.Value = value;
		}

		public NuGetVersion SelVersion
		{
			get => selVerEdit.Value;
			private set => selVerEdit.Value = value;
		}

		public bool ShowRevisionVersionSegment
		{
			get => showRevCheckBox.Checked;
			set => showRevCheckBox.Checked = value;
		}

		public bool ShowSuffixVersionSegment
		{
			get => showSufCheckBox.Checked;
			set => showSufCheckBox.Checked = value;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			Properties.Settings.Default.Save();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Text = $"{Text} (v{new NuGetVersion(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version)})";
			showRevCheckBox_CheckedChanged(this, EventArgs.Empty);
			showSufCheckBox_CheckedChanged(this, EventArgs.Empty);
			RefreshProjectsVer();
		}

		protected void RefreshProjectsVer()
		{
			data = ThreadHelper.JoinableTaskFactory.Run(() => SolutionVersionProcessor.GetProjectVersionsAsync(
				slnFileName, sln,
				Properties.Settings.Default.AssemblyInfoFileNames.Split(';'),
				includeWithoutVer: includeWithoutVer.Checked)
			);
			NuGetVersion cmVer = data.GroupBy(v => v.Version).OrderByDescending(gp => gp.Count()).Take(1).Select(g => g.Key).FirstOrDefault();
			SelVersion = cmVer;
			NewVersion = cmVer.Increment();
			groupByFileNameToolStripMenuItem_Click(this, EventArgs.Empty);
		}

		private void cancelBtn_Click(object sender, EventArgs e) => Close();

		private void CheckAllItems(bool isChecked)
		{
			list.BeginUpdate();
			foreach (ListViewItem i in list.Items)
			{
				i.Checked = isChecked;
			}

			list.EndUpdate();
		}

		private void CheckAllMatchingItems(NuGetVersion ver)
		{
			list.BeginUpdate();
			foreach (ListViewItem i in list.Items)
			{
				i.Checked = ((VerData)i.Tag).Version.CompareTo(ver, VersionComparison.Version) == 0;
			}

			list.EndUpdate();
			NewVersion = ver.Increment();
		}

		private void groupByFileNameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			list.BeginUpdate();
			list.Items.Clear();
			list.Groups.Clear();
			if (groupByFileNameToolStripMenuItem.Checked)
			{
				list.ShowGroups = true;
				foreach (IGrouping<string, VerData> grp in data.GroupBy(v => v.FileName))
				{
					list.Groups.Add(grp.Key, $"{Path.GetFileName(grp.Key)} ({grp.First().Project})");
				}
			}
			else
			{
				list.ShowGroups = false;
			}
			foreach (VerData ver in data)
			{
				list.Items.Add(MakeLVItem(ver));
			}

			list.Sort();
			list.EndUpdate();
		}

		private void includeWithoutVer_CheckedChanged(object sender, EventArgs e) => RefreshProjectsVer();

		private void list_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			((ListViewItemComparer)list.ListViewItemSorter).Column = e.Column;
			list.Sort();
		}

		private void List_ItemChecked(object sender, ItemCheckedEventArgs e) => okBtn.Enabled = list.CheckedIndices.Count > 0;

		private void list_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && (list.FocusedItem?.Bounds.Contains(e.Location) ?? false))
			{
				listCtxMenu.Show(Cursor.Position);
			}
		}

		private ListViewItem MakeLVItem(VerData ver) => new(new[] { ver.Project, Path.GetFileName(ver.FileName), ver.Version.ToString(), ver.LineText }) { Tag = ver, ToolTipText = ver.FileName, Group = groupByFileNameToolStripMenuItem.Checked ? list.Groups[ver.FileName] : null };

#pragma warning disable VSTHRD100 // Avoid async void methods
		private async void okBtn_Click(object sender, EventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
		{
			List<VerData> items = list.Items.Cast<ListViewItem>().Where(i => i.Checked).Select(i => i.Tag as VerData).ToList();
			await SolutionVersionProcessor.UpdateAsync(items, NewVersion);
			Close();
		}

		private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string fn = ((VerData)list.FocusedItem?.Tag)?.FileName;
			if (fn is null)
			{
				return;
			}

			System.Diagnostics.Process.Start(fn);
		}

		private void selAllBtn_Click(object sender, EventArgs e) => CheckAllItems(true);

		private void selectAllWithThisVersionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NuGetVersion v = ((VerData)list.FocusedItem?.Tag)?.Version;
			if (v is null)
			{
				return;
			}

			SelVersion = v;
			CheckAllMatchingItems(v);
		}

		private void selNoneBtn_Click(object sender, EventArgs e) => CheckAllItems(false);

		private void selVerBtn_Click(object sender, EventArgs e) => CheckAllMatchingItems(SelVersion);

		private void showRevCheckBox_CheckedChanged(object sender, EventArgs e) => newVerEdit.ShowRevision = selVerEdit.ShowRevision = showRevCheckBox.Checked;

		private void showSufCheckBox_CheckedChanged(object sender, EventArgs e) => newVerEdit.ShowSuffix = selVerEdit.ShowSuffix = showSufCheckBox.Checked;

		private class ListViewItemComparer(int column = 0) : System.Collections.IComparer
		{
			private bool asc = true;
			private int col = column;

			public int Column
			{
				get => col;
				set { asc = value != col || !asc; col = value; }
			}

			public int Compare(object x, object y)
			{
				int result;
				ListViewItem lvix = x as ListViewItem ?? throw new ArgumentException(nameof(x));
				ListViewItem lviy = y as ListViewItem ?? throw new ArgumentException(nameof(y));
				switch (col)
				{
					case 0:
						int pc = string.CompareOrdinal(lvix.SubItems[col].Text, lviy.SubItems[col].Text);
						result = pc != 0 ? pc : string.CompareOrdinal(lvix.SubItems[1].Text, lviy.SubItems[1].Text);
						break;

					case 2:
						result = ((VerData)lvix.Tag).Version.CompareTo(((VerData)lviy.Tag).Version);
						break;

					default:
						result = string.CompareOrdinal(lvix.SubItems[col].Text, lviy.SubItems[col].Text);
						break;
				}
				return asc ? result : -result;
			}
		}
	}
}