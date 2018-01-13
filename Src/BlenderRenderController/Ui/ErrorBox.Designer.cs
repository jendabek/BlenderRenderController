namespace BlenderRenderController.Ui
{
    partial class ErrorBox
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
            this.BtnLeft = new System.Windows.Forms.Button();
            this.BtnRight = new System.Windows.Forms.Button();
            this.ErrorContentBox = new System.Windows.Forms.TextBox();
            this.ErrorBoxLabel = new System.Windows.Forms.Label();
            this.BtnMiddle = new System.Windows.Forms.Button();
            this.ErrorIcon = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorIcon)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnLeft
            // 
            this.BtnLeft.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.BtnLeft.Location = new System.Drawing.Point(258, 19);
            this.BtnLeft.Name = "BtnLeft";
            this.BtnLeft.Size = new System.Drawing.Size(75, 30);
            this.BtnLeft.TabIndex = 0;
            this.BtnLeft.Text = "btn right";
            this.BtnLeft.UseVisualStyleBackColor = true;
            this.BtnLeft.Click += new System.EventHandler(this.Bnt_Click);
            // 
            // BtnRight
            // 
            this.BtnRight.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.BtnRight.Location = new System.Drawing.Point(96, 19);
            this.BtnRight.Name = "BtnRight";
            this.BtnRight.Size = new System.Drawing.Size(75, 30);
            this.BtnRight.TabIndex = 1;
            this.BtnRight.Text = "btn left";
            this.BtnRight.UseVisualStyleBackColor = true;
            this.BtnRight.Click += new System.EventHandler(this.Bnt_Click);
            // 
            // ErrorContentBox
            // 
            this.ErrorContentBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorContentBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ErrorContentBox.Location = new System.Drawing.Point(12, 87);
            this.ErrorContentBox.Multiline = true;
            this.ErrorContentBox.Name = "ErrorContentBox";
            this.ErrorContentBox.ReadOnly = true;
            this.ErrorContentBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ErrorContentBox.Size = new System.Drawing.Size(413, 173);
            this.ErrorContentBox.TabIndex = 2;
            this.ErrorContentBox.TabStop = false;
            this.ErrorContentBox.Text = "error contents here";
            // 
            // ErrorBoxLabel
            // 
            this.ErrorBoxLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorBoxLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ErrorBoxLabel.Location = new System.Drawing.Point(68, 12);
            this.ErrorBoxLabel.MaximumSize = new System.Drawing.Size(400, 200);
            this.ErrorBoxLabel.MinimumSize = new System.Drawing.Size(320, 40);
            this.ErrorBoxLabel.Name = "ErrorBoxLabel";
            this.ErrorBoxLabel.Size = new System.Drawing.Size(349, 46);
            this.ErrorBoxLabel.TabIndex = 3;
            this.ErrorBoxLabel.Text = "Error Label";
            // 
            // BtnMiddle
            // 
            this.BtnMiddle.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.BtnMiddle.Location = new System.Drawing.Point(177, 19);
            this.BtnMiddle.Name = "BtnMiddle";
            this.BtnMiddle.Size = new System.Drawing.Size(75, 30);
            this.BtnMiddle.TabIndex = 4;
            this.BtnMiddle.Text = "btn middle";
            this.BtnMiddle.UseVisualStyleBackColor = true;
            this.BtnMiddle.Click += new System.EventHandler(this.Bnt_Click);
            // 
            // ErrorIcon
            // 
            this.ErrorIcon.BackColor = System.Drawing.Color.White;
            this.ErrorIcon.Location = new System.Drawing.Point(12, 12);
            this.ErrorIcon.MaximumSize = new System.Drawing.Size(50, 50);
            this.ErrorIcon.Name = "ErrorIcon";
            this.ErrorIcon.Size = new System.Drawing.Size(50, 46);
            this.ErrorIcon.TabIndex = 5;
            this.ErrorIcon.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Details:";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.BtnRight);
            this.panel1.Controls.Add(this.BtnLeft);
            this.panel1.Controls.Add(this.BtnMiddle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 279);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(437, 69);
            this.panel1.TabIndex = 7;
            // 
            // ErrorBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(437, 348);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ErrorIcon);
            this.Controls.Add(this.ErrorBoxLabel);
            this.Controls.Add(this.ErrorContentBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ErrorBox";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.ErrorBox_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.ErrorIcon)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button BtnLeft;
        public System.Windows.Forms.Button BtnRight;
        public System.Windows.Forms.TextBox ErrorContentBox;
        public System.Windows.Forms.Label ErrorBoxLabel;
        public System.Windows.Forms.Button BtnMiddle;
        private System.Windows.Forms.PictureBox ErrorIcon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
    }
}