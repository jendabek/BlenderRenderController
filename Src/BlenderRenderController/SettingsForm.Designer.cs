namespace BlenderRenderController
{
    partial class SettingsForm
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
            this.blenderPathTextBox = new System.Windows.Forms.TextBox();
            this.ffmpegPathTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.blenderLabel = new System.Windows.Forms.Label();
            this.ffmpegLabel = new System.Windows.Forms.Label();
            this.ffmpegChangePathButton = new System.Windows.Forms.Button();
            this.blenderChangePathButton = new System.Windows.Forms.Button();
            this.settingsToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.chkBoxDelChunks = new System.Windows.Forms.CheckBox();
            this.chkBoxShowTooltips = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbLoggingLvl = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.findBlenderDialog = new System.Windows.Forms.OpenFileDialog();
            this.findFFmpegDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // blenderPathTextBox
            // 
            this.blenderPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blenderPathTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blenderPathTextBox.Location = new System.Drawing.Point(10, 47);
            this.blenderPathTextBox.Name = "blenderPathTextBox";
            this.blenderPathTextBox.Size = new System.Drawing.Size(519, 22);
            this.blenderPathTextBox.TabIndex = 0;
            this.blenderPathTextBox.WordWrap = false;
            // 
            // ffmpegPathTextBox
            // 
            this.ffmpegPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ffmpegPathTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ffmpegPathTextBox.Location = new System.Drawing.Point(10, 122);
            this.ffmpegPathTextBox.Name = "ffmpegPathTextBox";
            this.ffmpegPathTextBox.Size = new System.Drawing.Size(519, 22);
            this.ffmpegPathTextBox.TabIndex = 2;
            this.ffmpegPathTextBox.WordWrap = false;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(434, 270);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(117, 36);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // blenderLabel
            // 
            this.blenderLabel.AutoSize = true;
            this.blenderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blenderLabel.Location = new System.Drawing.Point(6, 26);
            this.blenderLabel.Name = "blenderLabel";
            this.blenderLabel.Size = new System.Drawing.Size(50, 15);
            this.blenderLabel.TabIndex = 29;
            this.blenderLabel.Text = "Blender";
            // 
            // ffmpegLabel
            // 
            this.ffmpegLabel.AutoSize = true;
            this.ffmpegLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ffmpegLabel.Location = new System.Drawing.Point(6, 101);
            this.ffmpegLabel.Name = "ffmpegLabel";
            this.ffmpegLabel.Size = new System.Drawing.Size(53, 15);
            this.ffmpegLabel.TabIndex = 29;
            this.ffmpegLabel.Text = "FFmpeg";
            // 
            // ffmpegChangePathButton
            // 
            this.ffmpegChangePathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ffmpegChangePathButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ffmpegChangePathButton.Image = global::BlenderRenderController.Properties.Resources.FolderOpen_16x;
            this.ffmpegChangePathButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ffmpegChangePathButton.Location = new System.Drawing.Point(418, 150);
            this.ffmpegChangePathButton.Name = "ffmpegChangePathButton";
            this.ffmpegChangePathButton.Size = new System.Drawing.Size(111, 29);
            this.ffmpegChangePathButton.TabIndex = 3;
            this.ffmpegChangePathButton.Text = "   Change";
            this.ffmpegChangePathButton.UseVisualStyleBackColor = true;
            this.ffmpegChangePathButton.Click += new System.EventHandler(this.ffmpegChangePathButton_Click);
            // 
            // blenderChangePathButton
            // 
            this.blenderChangePathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.blenderChangePathButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blenderChangePathButton.Image = global::BlenderRenderController.Properties.Resources.FolderOpen_16x;
            this.blenderChangePathButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.blenderChangePathButton.Location = new System.Drawing.Point(418, 75);
            this.blenderChangePathButton.Name = "blenderChangePathButton";
            this.blenderChangePathButton.Size = new System.Drawing.Size(111, 29);
            this.blenderChangePathButton.TabIndex = 1;
            this.blenderChangePathButton.Text = "   Change";
            this.blenderChangePathButton.UseVisualStyleBackColor = true;
            this.blenderChangePathButton.Click += new System.EventHandler(this.blenderChangePathButton_Click);
            // 
            // chkBoxDelChunks
            // 
            this.chkBoxDelChunks.AutoSize = true;
            this.chkBoxDelChunks.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkBoxDelChunks.Location = new System.Drawing.Point(7, 54);
            this.chkBoxDelChunks.Name = "chkBoxDelChunks";
            this.chkBoxDelChunks.Size = new System.Drawing.Size(168, 19);
            this.chkBoxDelChunks.TabIndex = 31;
            this.chkBoxDelChunks.Text = "Delete chunks when done";
            this.settingsToolTip.SetToolTip(this.chkBoxDelChunks, "Individual Chunks will be deleted after the joining process is completed.\r\n\r\nObs:" +
        " This setting is ignored if no joining action is chosen.");
            this.chkBoxDelChunks.UseVisualStyleBackColor = true;
            // 
            // chkBoxShowTooltips
            // 
            this.chkBoxShowTooltips.AutoSize = true;
            this.chkBoxShowTooltips.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkBoxShowTooltips.Location = new System.Drawing.Point(6, 29);
            this.chkBoxShowTooltips.Name = "chkBoxShowTooltips";
            this.chkBoxShowTooltips.Size = new System.Drawing.Size(103, 19);
            this.chkBoxShowTooltips.TabIndex = 32;
            this.chkBoxShowTooltips.Text = "Show Tooltips";
            this.chkBoxShowTooltips.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.ffmpegChangePathButton);
            this.groupBox1.Controls.Add(this.blenderPathTextBox);
            this.groupBox1.Controls.Add(this.ffmpegPathTextBox);
            this.groupBox1.Controls.Add(this.blenderChangePathButton);
            this.groupBox1.Controls.Add(this.blenderLabel);
            this.groupBox1.Controls.Add(this.ffmpegLabel);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(539, 193);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Paths";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkBoxShowTooltips);
            this.groupBox2.Controls.Add(this.chkBoxDelChunks);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 211);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(190, 83);
            this.groupBox2.TabIndex = 34;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Other";
            // 
            // cbLoggingLvl
            // 
            this.cbLoggingLvl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLoggingLvl.FormattingEnabled = true;
            this.cbLoggingLvl.Items.AddRange(new object[] {
            "Warnigs and Errors (default)",
            "Detailed",
            "Developer"});
            this.cbLoggingLvl.Location = new System.Drawing.Point(5, 27);
            this.cbLoggingLvl.Name = "cbLoggingLvl";
            this.cbLoggingLvl.Size = new System.Drawing.Size(146, 23);
            this.cbLoggingLvl.TabIndex = 33;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.cbLoggingLvl);
            this.groupBox3.Location = new System.Drawing.Point(217, 240);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(157, 56);
            this.groupBox3.TabIndex = 35;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Logging level";
            // 
            // findBlenderDialog
            // 
            this.findBlenderDialog.Title = "Find ";
            // 
            // findFFmpegDialog
            // 
            this.findFFmpegDialog.Title = "Find ";
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 317);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.okButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 400);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SettingsForm_FormClosed);
            this.Load += new System.EventHandler(this.onFormLoad);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button blenderChangePathButton;
        private System.Windows.Forms.Button ffmpegChangePathButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label blenderLabel;
        private System.Windows.Forms.Label ffmpegLabel;
        private System.Windows.Forms.TextBox blenderPathTextBox;
        private System.Windows.Forms.TextBox ffmpegPathTextBox;
        private System.Windows.Forms.ToolTip settingsToolTip;
        private System.Windows.Forms.CheckBox chkBoxDelChunks;
        private System.Windows.Forms.CheckBox chkBoxShowTooltips;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cbLoggingLvl;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.OpenFileDialog findBlenderDialog;
        private System.Windows.Forms.OpenFileDialog findFFmpegDialog;
    }
}