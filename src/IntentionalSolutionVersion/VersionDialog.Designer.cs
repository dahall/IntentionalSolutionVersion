namespace IntentionalSolutionVersion
{
	partial class VersionDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VersionDialog));
			this.cancelBtn = new System.Windows.Forms.Button();
			this.okBtn = new System.Windows.Forms.Button();
			this.commandPanel = new System.Windows.Forms.TableLayoutPanel();
			this.borderPanel = new System.Windows.Forms.Panel();
			this.bodyPanel = new System.Windows.Forms.TableLayoutPanel();
			this.list = new System.Windows.Forms.ListView();
			this.columnHeader0 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.selAllBtn = new System.Windows.Forms.Button();
			this.selNoneBtn = new System.Windows.Forms.Button();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.label2 = new System.Windows.Forms.Label();
			this.selVerEdit = new IntentionalSolutionVersion.VersionEdit();
			this.selVerBtn = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.newVerEdit = new IntentionalSolutionVersion.VersionEdit();
			this.listCtxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.selectAllWithThisVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.groupByFileNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.commandPanel.SuspendLayout();
			this.bodyPanel.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.listCtxMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// cancelBtn
			// 
			resources.ApplyResources(this.cancelBtn, "cancelBtn");
			this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.UseVisualStyleBackColor = true;
			this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
			// 
			// okBtn
			// 
			resources.ApplyResources(this.okBtn, "okBtn");
			this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okBtn.Name = "okBtn";
			this.okBtn.UseVisualStyleBackColor = true;
			this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
			// 
			// commandPanel
			// 
			resources.ApplyResources(this.commandPanel, "commandPanel");
			this.commandPanel.BackColor = System.Drawing.SystemColors.Control;
			this.commandPanel.Controls.Add(this.okBtn, 0, 1);
			this.commandPanel.Controls.Add(this.cancelBtn, 1, 1);
			this.commandPanel.Controls.Add(this.borderPanel, 0, 0);
			this.commandPanel.Name = "commandPanel";
			// 
			// borderPanel
			// 
			this.borderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
			this.commandPanel.SetColumnSpan(this.borderPanel, 2);
			resources.ApplyResources(this.borderPanel, "borderPanel");
			this.borderPanel.Name = "borderPanel";
			// 
			// bodyPanel
			// 
			resources.ApplyResources(this.bodyPanel, "bodyPanel");
			this.bodyPanel.Controls.Add(this.list, 0, 0);
			this.bodyPanel.Controls.Add(this.flowLayoutPanel1, 0, 1);
			this.bodyPanel.Controls.Add(this.tableLayoutPanel1, 1, 0);
			this.bodyPanel.Name = "bodyPanel";
			// 
			// list
			// 
			this.list.CheckBoxes = true;
			this.list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader0,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
			resources.ApplyResources(this.list, "list");
			this.list.FullRowSelect = true;
			this.list.Name = "list";
			this.list.ShowItemToolTips = true;
			this.list.UseCompatibleStateImageBehavior = false;
			this.list.View = System.Windows.Forms.View.Details;
			this.list.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.list_ColumnClick);
			this.list.MouseClick += new System.Windows.Forms.MouseEventHandler(this.list_MouseClick);
			// 
			// columnHeader0
			// 
			resources.ApplyResources(this.columnHeader0, "columnHeader0");
			// 
			// columnHeader1
			// 
			resources.ApplyResources(this.columnHeader1, "columnHeader1");
			// 
			// columnHeader2
			// 
			resources.ApplyResources(this.columnHeader2, "columnHeader2");
			// 
			// columnHeader3
			// 
			resources.ApplyResources(this.columnHeader3, "columnHeader3");
			// 
			// flowLayoutPanel1
			// 
			resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
			this.flowLayoutPanel1.Controls.Add(this.selAllBtn);
			this.flowLayoutPanel1.Controls.Add(this.selNoneBtn);
			this.flowLayoutPanel1.Controls.Add(this.tableLayoutPanel2);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			// 
			// selAllBtn
			// 
			resources.ApplyResources(this.selAllBtn, "selAllBtn");
			this.selAllBtn.Name = "selAllBtn";
			this.selAllBtn.UseVisualStyleBackColor = true;
			this.selAllBtn.Click += new System.EventHandler(this.selAllBtn_Click);
			// 
			// selNoneBtn
			// 
			resources.ApplyResources(this.selNoneBtn, "selNoneBtn");
			this.selNoneBtn.Name = "selNoneBtn";
			this.selNoneBtn.UseVisualStyleBackColor = true;
			this.selNoneBtn.Click += new System.EventHandler(this.selNoneBtn_Click);
			// 
			// tableLayoutPanel2
			// 
			resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
			this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.selVerEdit, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.selVerBtn, 2, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// selVerEdit
			// 
			resources.ApplyResources(this.selVerEdit, "selVerEdit");
			this.selVerEdit.Name = "selVerEdit";
			this.selVerEdit.ShowRevision = false;
			this.selVerEdit.Value = ((System.Version)(resources.GetObject("selVerEdit.Value")));
			// 
			// selVerBtn
			// 
			resources.ApplyResources(this.selVerBtn, "selVerBtn");
			this.selVerBtn.Name = "selVerBtn";
			this.selVerBtn.UseVisualStyleBackColor = true;
			this.selVerBtn.Click += new System.EventHandler(this.selVerBtn_Click);
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.newVerEdit, 0, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// newVerEdit
			// 
			resources.ApplyResources(this.newVerEdit, "newVerEdit");
			this.newVerEdit.Name = "newVerEdit";
			this.newVerEdit.ShowRevision = false;
			this.newVerEdit.Value = ((System.Version)(resources.GetObject("newVerEdit.Value")));
			// 
			// listCtxMenu
			// 
			this.listCtxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllWithThisVersionToolStripMenuItem,
            this.groupByFileNameToolStripMenuItem,
            this.openFileToolStripMenuItem});
			this.listCtxMenu.Name = "listCtxMenu";
			resources.ApplyResources(this.listCtxMenu, "listCtxMenu");
			// 
			// selectAllWithThisVersionToolStripMenuItem
			// 
			this.selectAllWithThisVersionToolStripMenuItem.Name = "selectAllWithThisVersionToolStripMenuItem";
			resources.ApplyResources(this.selectAllWithThisVersionToolStripMenuItem, "selectAllWithThisVersionToolStripMenuItem");
			this.selectAllWithThisVersionToolStripMenuItem.Click += new System.EventHandler(this.selectAllWithThisVersionToolStripMenuItem_Click);
			// 
			// groupByFileNameToolStripMenuItem
			// 
			this.groupByFileNameToolStripMenuItem.CheckOnClick = true;
			this.groupByFileNameToolStripMenuItem.Name = "groupByFileNameToolStripMenuItem";
			resources.ApplyResources(this.groupByFileNameToolStripMenuItem, "groupByFileNameToolStripMenuItem");
			this.groupByFileNameToolStripMenuItem.Click += new System.EventHandler(this.groupByFileNameToolStripMenuItem_Click);
			// 
			// openFileToolStripMenuItem
			// 
			this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
			resources.ApplyResources(this.openFileToolStripMenuItem, "openFileToolStripMenuItem");
			this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
			// 
			// VersionDialog
			// 
			this.AcceptButton = this.okBtn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.CancelButton = this.cancelBtn;
			this.Controls.Add(this.bodyPanel);
			this.Controls.Add(this.commandPanel);
			this.Name = "VersionDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.commandPanel.ResumeLayout(false);
			this.bodyPanel.ResumeLayout(false);
			this.bodyPanel.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.listCtxMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.Button okBtn;
		private System.Windows.Forms.TableLayoutPanel commandPanel;
		private System.Windows.Forms.Panel borderPanel;
		private System.Windows.Forms.TableLayoutPanel bodyPanel;
		private System.Windows.Forms.ListView list;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button selAllBtn;
		private System.Windows.Forms.Button selNoneBtn;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button selVerBtn;
		private VersionEdit newVerEdit;
		private VersionEdit selVerEdit;
		private System.Windows.Forms.ContextMenuStrip listCtxMenu;
		private System.Windows.Forms.ToolStripMenuItem selectAllWithThisVersionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem groupByFileNameToolStripMenuItem;
		private System.Windows.Forms.ColumnHeader columnHeader0;
	}
}