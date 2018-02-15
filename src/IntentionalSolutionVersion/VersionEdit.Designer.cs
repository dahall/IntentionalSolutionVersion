namespace IntentionalSolutionVersion
{
	partial class VersionEdit
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.verMajor = new System.Windows.Forms.TextBox();
			this.verMinor = new System.Windows.Forms.TextBox();
			this.verBuild = new System.Windows.Forms.TextBox();
			this.verRev = new System.Windows.Forms.TextBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 7;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.label2, 3, 0);
			this.tableLayoutPanel1.Controls.Add(this.label3, 5, 0);
			this.tableLayoutPanel1.Controls.Add(this.verMajor, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.verMinor, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.verBuild, 4, 0);
			this.tableLayoutPanel1.Controls.Add(this.verRev, 6, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(178, 20);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(37, 5);
			this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(10, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = ".";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(84, 5);
			this.label2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(10, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = ".";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(131, 5);
			this.label3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(10, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = ".";
			// 
			// verMajor
			// 
			this.verMajor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.verMajor.Location = new System.Drawing.Point(0, 0);
			this.verMajor.Margin = new System.Windows.Forms.Padding(0);
			this.verMajor.Name = "verMajor";
			this.verMajor.Size = new System.Drawing.Size(37, 20);
			this.verMajor.TabIndex = 1;
			this.verMajor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.verMajor_KeyDown);
			// 
			// verMinor
			// 
			this.verMinor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.verMinor.Location = new System.Drawing.Point(47, 0);
			this.verMinor.Margin = new System.Windows.Forms.Padding(0);
			this.verMinor.Name = "verMinor";
			this.verMinor.Size = new System.Drawing.Size(37, 20);
			this.verMinor.TabIndex = 1;
			this.verMinor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.verMajor_KeyDown);
			// 
			// verBuild
			// 
			this.verBuild.Dock = System.Windows.Forms.DockStyle.Fill;
			this.verBuild.Location = new System.Drawing.Point(94, 0);
			this.verBuild.Margin = new System.Windows.Forms.Padding(0);
			this.verBuild.Name = "verBuild";
			this.verBuild.Size = new System.Drawing.Size(37, 20);
			this.verBuild.TabIndex = 1;
			this.verBuild.KeyDown += new System.Windows.Forms.KeyEventHandler(this.verMajor_KeyDown);
			// 
			// verRev
			// 
			this.verRev.Dock = System.Windows.Forms.DockStyle.Fill;
			this.verRev.Location = new System.Drawing.Point(141, 0);
			this.verRev.Margin = new System.Windows.Forms.Padding(0);
			this.verRev.Name = "verRev";
			this.verRev.Size = new System.Drawing.Size(37, 20);
			this.verRev.TabIndex = 1;
			this.verRev.KeyDown += new System.Windows.Forms.KeyEventHandler(this.verMajor_KeyDown);
			// 
			// VersionEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "VersionEdit";
			this.Size = new System.Drawing.Size(178, 20);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox verMajor;
		private System.Windows.Forms.TextBox verMinor;
		private System.Windows.Forms.TextBox verBuild;
		private System.Windows.Forms.TextBox verRev;
	}
}
