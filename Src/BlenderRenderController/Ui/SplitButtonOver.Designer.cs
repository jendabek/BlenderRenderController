namespace BlenderRenderController.Ui
{
    partial class SplitButtonOver
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
            this.BrowseBnt = new System.Windows.Forms.Button();
            this.RecentBnt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BrowseBnt
            // 
            this.BrowseBnt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseBnt.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BrowseBnt.BackColor = System.Drawing.SystemColors.Control;
            this.BrowseBnt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BrowseBnt.Image = global::BlenderRenderController.Properties.Resources.blend_icon;
            this.BrowseBnt.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BrowseBnt.Location = new System.Drawing.Point(0, 0);
            this.BrowseBnt.Name = "BrowseBnt";
            this.BrowseBnt.Size = new System.Drawing.Size(133, 52);
            this.BrowseBnt.TabIndex = 0;
            this.BrowseBnt.Text = "button1";
            this.BrowseBnt.UseVisualStyleBackColor = false;
            this.BrowseBnt.Click += new System.EventHandler(this.BrowseBnt_Click);
            // 
            // RecentBnt
            // 
            this.RecentBnt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RecentBnt.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.RecentBnt.BackColor = System.Drawing.SystemColors.Control;
            this.RecentBnt.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RecentBnt.Location = new System.Drawing.Point(133, 0);
            this.RecentBnt.Name = "RecentBnt";
            this.RecentBnt.Size = new System.Drawing.Size(28, 52);
            this.RecentBnt.TabIndex = 1;
            this.RecentBnt.Text = "...";
            this.RecentBnt.UseVisualStyleBackColor = false;
            this.RecentBnt.Click += new System.EventHandler(this.RecentBnt_Click);
            // 
            // SplitButtonOver
            // 
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.RecentBnt);
            this.Controls.Add(this.BrowseBnt);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SplitButtonOver";
            this.Size = new System.Drawing.Size(161, 52);
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.Button BrowseBnt;
        protected System.Windows.Forms.Button RecentBnt;
    }
}
