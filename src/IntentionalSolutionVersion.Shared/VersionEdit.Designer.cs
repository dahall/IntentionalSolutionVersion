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
			this.label4 = new System.Windows.Forms.Label();
			this.verMajor = new System.Windows.Forms.TextBox();
			this.verMinor = new System.Windows.Forms.TextBox();
			this.verBuild = new System.Windows.Forms.TextBox();
			this.verRev = new System.Windows.Forms.TextBox();
			this.verSuffix = new System.Windows.Forms.TextBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 9;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66833F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66833F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66833F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66833F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.32667F));
			this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.label2, 3, 0);
			this.tableLayoutPanel1.Controls.Add(this.label3, 5, 0);
			this.tableLayoutPanel1.Controls.Add(this.label4, 7, 0);
			this.tableLayoutPanel1.Controls.Add(this.verMajor, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.verMinor, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.verBuild, 4, 0);
			this.tableLayoutPanel1.Controls.Add(this.verRev, 6, 0);
			this.tableLayoutPanel1.Controls.Add(this.verSuffix, 8, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(6);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(276, 38);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(33, 9);
			this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(18, 25);
			this.label1.TabIndex = 0;
			this.label1.Text = ".";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(84, 9);
			this.label2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(18, 25);
			this.label2.TabIndex = 0;
			this.label2.Text = ".";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(135, 9);
			this.label3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(18, 25);
			this.label3.TabIndex = 0;
			this.label3.Text = ".";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(186, 0);
			this.label4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(19, 25);
			this.label4.TabIndex = 0;
			this.label4.Text = "-";
			// 
			// verMajor
			// 
			this.verMajor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.verMajor.Location = new System.Drawing.Point(0, 0);
			this.verMajor.Margin = new System.Windows.Forms.Padding(0);
			this.verMajor.Name = "verMajor";
			this.verMajor.Size = new System.Drawing.Size(33, 31);
			this.verMajor.TabIndex = 1;
			this.verMajor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.verNum_KeyDown);
			// 
			// verMinor
			// 
			this.verMinor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.verMinor.Location = new System.Drawing.Point(51, 0);
			this.verMinor.Margin = new System.Windows.Forms.Padding(0);
			this.verMinor.Name = "verMinor";
			this.verMinor.Size = new System.Drawing.Size(33, 31);
			this.verMinor.TabIndex = 1;
			this.verMinor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.verNum_KeyDown);
			// 
			// verBuild
			// 
			this.verBuild.Dock = System.Windows.Forms.DockStyle.Fill;
			this.verBuild.Location = new System.Drawing.Point(102, 0);
			this.verBuild.Margin = new System.Windows.Forms.Padding(0);
			this.verBuild.Name = "verBuild";
			this.verBuild.Size = new System.Drawing.Size(33, 31);
			this.verBuild.TabIndex = 1;
			this.verBuild.KeyDown += new System.Windows.Forms.KeyEventHandler(this.verNum_KeyDown);
			// 
			// verRev
			// 
			this.verRev.Dock = System.Windows.Forms.DockStyle.Fill;
			this.verRev.Location = new System.Drawing.Point(153, 0);
			this.verRev.Margin = new System.Windows.Forms.Padding(0);
			this.verRev.Name = "verRev";
			this.verRev.Size = new System.Drawing.Size(33, 31);
			this.verRev.TabIndex = 1;
			this.verRev.KeyDown += new System.Windows.Forms.KeyEventHandler(this.verNum_KeyDown);
			// 
			// verSuffix
			// 
			this.verSuffix.Dock = System.Windows.Forms.DockStyle.Fill;
			this.verSuffix.Location = new System.Drawing.Point(205, 0);
			this.verSuffix.Margin = new System.Windows.Forms.Padding(0);
			this.verSuffix.Name = "verSuffix";
			this.verSuffix.Size = new System.Drawing.Size(71, 31);
			this.verSuffix.TabIndex = 1;
			// 
			// VersionEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(6);
			this.Name = "VersionEdit";
			this.Size = new System.Drawing.Size(276, 38);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox verMajor;
		private System.Windows.Forms.TextBox verMinor;
		private System.Windows.Forms.TextBox verBuild;
		private System.Windows.Forms.TextBox verRev;
		private System.Windows.Forms.TextBox verSuffix;
	}
}
