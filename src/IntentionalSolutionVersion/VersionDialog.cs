using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using Microsoft.VisualStudio.Shell;

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

		public Version NewVersion
		{
			get => newVerEdit.Value;
			private set => newVerEdit.Value = value;
		}

		public Version SelVersion
		{
			get => selVerEdit.Value;
			private set => selVerEdit.Value = value;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			data = ThreadHelper.JoinableTaskFactory.Run(() => SolutionVersionProcessor.GetProjectVersionsAsync(slnFileName, sln, Properties.Settings.Default.AssemblyInfoFileNames.Split(';')));
			var cmVer = data.GroupBy(v => v.Version).OrderByDescending(gp => gp.Count()).Take(1).Select(g => g.Key).FirstOrDefault();
			SelVersion = cmVer;
			NewVersion = cmVer.Increment();
			groupByFileNameToolStripMenuItem_Click(this, EventArgs.Empty);
		}

		private void cancelBtn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void CheckAllItems(bool isChecked)
		{
			list.BeginUpdate();
			foreach (ListViewItem i in list.Items)
				i.Checked = isChecked;
			list.EndUpdate();
		}

		private void CheckAllMatchingItems(Version ver)
		{
			list.BeginUpdate();
			foreach (ListViewItem i in list.Items)
				i.Checked = ((VerData) i.Tag).Version.CompareTo(ver, VersionComparison.IgnoreUnset) == 0;
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
				foreach (var grp in data.GroupBy(v => v.FileName))
					list.Groups.Add(grp.Key, $"{Path.GetFileName(grp.Key)} ({grp.First().Project})");
			}
			else
			{
				list.ShowGroups = false;
			}
			foreach (var ver in data)
				list.Items.Add(MakeLVItem(ver));
			list.Sort();
			list.EndUpdate();
		}

		private void list_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			((ListViewItemComparer)list.ListViewItemSorter).Column = e.Column;
			list.Sort();
		}

		private void List_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			okBtn.Enabled = list.CheckedIndices.Count > 0;
		}

		private void list_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && (list.FocusedItem?.Bounds.Contains(e.Location) ?? false))
				listCtxMenu.Show(Cursor.Position);
		}

		private ListViewItem MakeLVItem(VerData ver)
		{
			return new ListViewItem(new[] { ver.Project, Path.GetFileName(ver.FileName), ver.Version.ToString(), ver.LineText }) { Tag = ver, ToolTipText = ver.FileName, Group = groupByFileNameToolStripMenuItem.Checked ? list.Groups[ver.FileName] : null };
		}

		private async void okBtn_Click(object sender, EventArgs e)
		{
			var items = list.Items.Cast<ListViewItem>().Where(i => i.Checked).Select(i => i.Tag as VerData).ToList();
			await SolutionVersionProcessor.UpdateAsync(items, NewVersion);
			Close();
		}

		private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var fn = ((VerData)list.FocusedItem?.Tag)?.FileName;
			if (fn == null) return;
			System.Diagnostics.Process.Start(fn);
		}

		private void selAllBtn_Click(object sender, EventArgs e)
		{
			CheckAllItems(true);
		}

		private void selectAllWithThisVersionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var v = ((VerData)list.FocusedItem?.Tag)?.Version;
			if (v == null) return;
			SelVersion = v;
			CheckAllMatchingItems(v);
		}

		private void selNoneBtn_Click(object sender, EventArgs e)
		{
			CheckAllItems(false);
		}

		private void selVerBtn_Click(object sender, EventArgs e)
		{
			CheckAllMatchingItems(SelVersion);
		}

		class ListViewItemComparer : System.Collections.IComparer
		{
			private int col;
			private bool asc = true;
			public ListViewItemComparer(int column = 0) => col = column;
			public int Column { get => col; set { asc = value != col || !asc; col = value; } }
			public int Compare(object x, object y)
			{
				int result;
				var lvix = x as ListViewItem ?? throw new ArgumentException(nameof(x));
				var lviy = y as ListViewItem ?? throw new ArgumentException(nameof(y));
				switch (col)
				{
					case 0:
						var pc = string.CompareOrdinal(lvix.SubItems[col].Text, lviy.SubItems[col].Text);
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
