namespace GJsonCorrect
{
	partial class GJsonCorrectForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.InputBox = new System.Windows.Forms.TextBox();
			this.StatusControl = new System.Windows.Forms.StatusStrip();
			this.StatusText = new System.Windows.Forms.ToolStripStatusLabel();
			this.StatusControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// InputBox
			// 
			this.InputBox.Dock = System.Windows.Forms.DockStyle.Top;
			this.InputBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.InputBox.Location = new System.Drawing.Point(0, 0);
			this.InputBox.Multiline = true;
			this.InputBox.Name = "InputBox";
			this.InputBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.InputBox.Size = new System.Drawing.Size(428, 231);
			this.InputBox.TabIndex = 0;
			this.InputBox.TextChanged += new System.EventHandler(this.InputBox_TextChanged);
			// 
			// StatusControl
			// 
			this.StatusControl.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusText});
			this.StatusControl.Location = new System.Drawing.Point(0, 234);
			this.StatusControl.Name = "StatusControl";
			this.StatusControl.Size = new System.Drawing.Size(428, 22);
			this.StatusControl.SizingGrip = false;
			this.StatusControl.TabIndex = 1;
			this.StatusControl.Text = "statusStrip1";
			// 
			// StatusText
			// 
			this.StatusText.Name = "StatusText";
			this.StatusText.Size = new System.Drawing.Size(49, 17);
			this.StatusText.Text = "STATUS";
			// 
			// GJsonCorrectForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(428, 256);
			this.Controls.Add(this.StatusControl);
			this.Controls.Add(this.InputBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "GJsonCorrectForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "GJsonCorrect";
			this.StatusControl.ResumeLayout(false);
			this.StatusControl.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip StatusControl;
		private System.Windows.Forms.ToolStripStatusLabel StatusText;
		public System.Windows.Forms.TextBox InputBox;
	}
}

