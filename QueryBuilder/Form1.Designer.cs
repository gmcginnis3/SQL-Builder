namespace TestQueryBuilder
{
	partial class Form1
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
			this.cboQueries = new System.Windows.Forms.ComboBox();
			this.btnGo = new System.Windows.Forms.Button();
			this.txtQuery = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// cboQueries
			// 
			this.cboQueries.DisplayMember = "Title";
			this.cboQueries.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboQueries.FormattingEnabled = true;
			this.cboQueries.Location = new System.Drawing.Point(12, 12);
			this.cboQueries.Name = "cboQueries";
			this.cboQueries.Size = new System.Drawing.Size(359, 21);
			this.cboQueries.TabIndex = 0;
			this.cboQueries.ValueMember = "Demo";
			// 
			// btnGo
			// 
			this.btnGo.Location = new System.Drawing.Point(377, 10);
			this.btnGo.Name = "btnGo";
			this.btnGo.Size = new System.Drawing.Size(31, 23);
			this.btnGo.TabIndex = 1;
			this.btnGo.Text = "&Go";
			this.btnGo.UseVisualStyleBackColor = true;
			this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
			// 
			// txtQuery
			// 
			this.txtQuery.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtQuery.Location = new System.Drawing.Point(12, 39);
			this.txtQuery.Multiline = true;
			this.txtQuery.Name = "txtQuery";
			this.txtQuery.ReadOnly = true;
			this.txtQuery.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtQuery.Size = new System.Drawing.Size(396, 289);
			this.txtQuery.TabIndex = 2;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(420, 340);
			this.Controls.Add(this.txtQuery);
			this.Controls.Add(this.btnGo);
			this.Controls.Add(this.cboQueries);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Test SQL QueryBuilder";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cboQueries;
		private System.Windows.Forms.Button btnGo;
		private System.Windows.Forms.TextBox txtQuery;
	}
}

