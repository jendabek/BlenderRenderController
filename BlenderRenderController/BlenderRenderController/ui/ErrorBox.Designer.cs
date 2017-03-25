namespace BlenderRenderController.ui
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
            this.BntLeft = new System.Windows.Forms.Button();
            this.BntRight = new System.Windows.Forms.Button();
            this.ErrorContentBox = new System.Windows.Forms.TextBox();
            this.ErrorBoxLabel = new System.Windows.Forms.Label();
            this.BntMiddle = new System.Windows.Forms.Button();
            this.ErrorIcon = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // BntLeft
            // 
            this.BntLeft.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.BntLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BntLeft.Location = new System.Drawing.Point(266, 293);
            this.BntLeft.Name = "BntLeft";
            this.BntLeft.Size = new System.Drawing.Size(75, 30);
            this.BntLeft.TabIndex = 0;
            this.BntLeft.Text = "Continue";
            this.BntLeft.UseVisualStyleBackColor = true;
            this.BntLeft.Click += new System.EventHandler(this.Bnt_Click);
            // 
            // BntRight
            // 
            this.BntRight.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.BntRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BntRight.Location = new System.Drawing.Point(81, 293);
            this.BntRight.Name = "BntRight";
            this.BntRight.Size = new System.Drawing.Size(75, 30);
            this.BntRight.TabIndex = 1;
            this.BntRight.Text = "Cancel";
            this.BntRight.UseVisualStyleBackColor = true;
            this.BntRight.Click += new System.EventHandler(this.Bnt_Click);
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
            this.ErrorContentBox.Size = new System.Drawing.Size(405, 191);
            this.ErrorContentBox.TabIndex = 2;
            this.ErrorContentBox.TabStop = false;
            this.ErrorContentBox.Text = "error contents here";
            // 
            // ErrorBoxLabel
            // 
            this.ErrorBoxLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ErrorBoxLabel.AutoSize = true;
            this.ErrorBoxLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ErrorBoxLabel.Location = new System.Drawing.Point(68, 28);
            this.ErrorBoxLabel.MaximumSize = new System.Drawing.Size(349, 0);
            this.ErrorBoxLabel.Name = "ErrorBoxLabel";
            this.ErrorBoxLabel.Size = new System.Drawing.Size(74, 16);
            this.ErrorBoxLabel.TabIndex = 3;
            this.ErrorBoxLabel.Text = "Error Label";
            this.ErrorBoxLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BntMiddle
            // 
            this.BntMiddle.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.BntMiddle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BntMiddle.Location = new System.Drawing.Point(172, 293);
            this.BntMiddle.Name = "BntMiddle";
            this.BntMiddle.Size = new System.Drawing.Size(75, 30);
            this.BntMiddle.TabIndex = 4;
            this.BntMiddle.Text = "asd";
            this.BntMiddle.UseVisualStyleBackColor = true;
            this.BntMiddle.Visible = false;
            this.BntMiddle.Click += new System.EventHandler(this.Bnt_Click);
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
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Details:";
            // 
            // ErrorBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(429, 344);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ErrorIcon);
            this.Controls.Add(this.ErrorBoxLabel);
            this.Controls.Add(this.BntMiddle);
            this.Controls.Add(this.ErrorContentBox);
            this.Controls.Add(this.BntRight);
            this.Controls.Add(this.BntLeft);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ErrorBox";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.ErrorBox_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.ErrorIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button BntLeft;
        public System.Windows.Forms.Button BntRight;
        public System.Windows.Forms.TextBox ErrorContentBox;
        public System.Windows.Forms.Label ErrorBoxLabel;
        public System.Windows.Forms.Button BntMiddle;
        private System.Windows.Forms.PictureBox ErrorIcon;
        private System.Windows.Forms.Label label1;
    }
}