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
            this.blenderExeLabel = new System.Windows.Forms.Label();
            this.ffmpegPathTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.blenderLabel = new System.Windows.Forms.Label();
            this.ffmpegLabel = new System.Windows.Forms.Label();
            this.ffmpegDownloadLabel = new System.Windows.Forms.Label();
            this.ffmpegChangePathButton = new System.Windows.Forms.Button();
            this.blenderChangePathButton = new System.Windows.Forms.Button();
            this.chkBoxVerboseLog = new System.Windows.Forms.CheckBox();
            this.settingsToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.appSettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.appSettingsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // blenderPathTextBox
            // 
            this.blenderPathTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.appSettingsBindingSource, "BlenderProgram", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.blenderPathTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blenderPathTextBox.Location = new System.Drawing.Point(24, 81);
            this.blenderPathTextBox.Name = "blenderPathTextBox";
            this.blenderPathTextBox.Size = new System.Drawing.Size(482, 22);
            this.blenderPathTextBox.TabIndex = 0;
            this.blenderPathTextBox.WordWrap = false;
            // 
            // blenderExeLabel
            // 
            this.blenderExeLabel.AutoSize = true;
            this.blenderExeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Underline);
            this.blenderExeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.blenderExeLabel.Location = new System.Drawing.Point(12, 21);
            this.blenderExeLabel.Name = "blenderExeLabel";
            this.blenderExeLabel.Size = new System.Drawing.Size(213, 20);
            this.blenderExeLabel.TabIndex = 26;
            this.blenderExeLabel.Text = "Paths to required programs";
            // 
            // ffmpegPathTextBox
            // 
            this.ffmpegPathTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.appSettingsBindingSource, "FFmpegProgram", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ffmpegPathTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ffmpegPathTextBox.Location = new System.Drawing.Point(24, 146);
            this.ffmpegPathTextBox.Name = "ffmpegPathTextBox";
            this.ffmpegPathTextBox.Size = new System.Drawing.Size(482, 22);
            this.ffmpegPathTextBox.TabIndex = 2;
            this.ffmpegPathTextBox.WordWrap = false;
            // 
            // okButton
            // 
            this.okButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(371, 235);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(135, 38);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // blenderLabel
            // 
            this.blenderLabel.AutoSize = true;
            this.blenderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blenderLabel.Location = new System.Drawing.Point(21, 63);
            this.blenderLabel.Name = "blenderLabel";
            this.blenderLabel.Size = new System.Drawing.Size(50, 15);
            this.blenderLabel.TabIndex = 29;
            this.blenderLabel.Text = "Blender";
            // 
            // ffmpegLabel
            // 
            this.ffmpegLabel.AutoSize = true;
            this.ffmpegLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ffmpegLabel.Location = new System.Drawing.Point(21, 128);
            this.ffmpegLabel.Name = "ffmpegLabel";
            this.ffmpegLabel.Size = new System.Drawing.Size(53, 15);
            this.ffmpegLabel.TabIndex = 29;
            this.ffmpegLabel.Text = "FFmpeg";
            // 
            // ffmpegDownloadLabel
            // 
            this.ffmpegDownloadLabel.AutoSize = true;
            this.ffmpegDownloadLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ffmpegDownloadLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ffmpegDownloadLabel.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.ffmpegDownloadLabel.Location = new System.Drawing.Point(317, 179);
            this.ffmpegDownloadLabel.Name = "ffmpegDownloadLabel";
            this.ffmpegDownloadLabel.Size = new System.Drawing.Size(63, 15);
            this.ffmpegDownloadLabel.TabIndex = 29;
            this.ffmpegDownloadLabel.Text = "Download";
            this.ffmpegDownloadLabel.Click += new System.EventHandler(this.ffmpegDownloadLabel_Click);
            // 
            // ffmpegChangePathButton
            // 
            this.ffmpegChangePathButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ffmpegChangePathButton.Image = global::BlenderRenderController.Properties.Resources.folder_icon_smaller;
            this.ffmpegChangePathButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ffmpegChangePathButton.Location = new System.Drawing.Point(411, 174);
            this.ffmpegChangePathButton.Name = "ffmpegChangePathButton";
            this.ffmpegChangePathButton.Size = new System.Drawing.Size(95, 25);
            this.ffmpegChangePathButton.TabIndex = 3;
            this.ffmpegChangePathButton.Text = "   Change";
            this.ffmpegChangePathButton.UseVisualStyleBackColor = true;
            this.ffmpegChangePathButton.Click += new System.EventHandler(this.ffmpegChangePathButton_Click);
            // 
            // blenderChangePathButton
            // 
            this.blenderChangePathButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blenderChangePathButton.Image = global::BlenderRenderController.Properties.Resources.folder_icon_smaller;
            this.blenderChangePathButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.blenderChangePathButton.Location = new System.Drawing.Point(411, 109);
            this.blenderChangePathButton.Name = "blenderChangePathButton";
            this.blenderChangePathButton.Size = new System.Drawing.Size(95, 25);
            this.blenderChangePathButton.TabIndex = 1;
            this.blenderChangePathButton.Text = "   Change";
            this.blenderChangePathButton.UseVisualStyleBackColor = true;
            this.blenderChangePathButton.Click += new System.EventHandler(this.blenderChangePathButton_Click);
            // 
            // chkBoxVerboseLog
            // 
            this.chkBoxVerboseLog.AutoSize = true;
            this.chkBoxVerboseLog.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.appSettingsBindingSource, "Verbose", true));
            this.chkBoxVerboseLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkBoxVerboseLog.Location = new System.Drawing.Point(24, 246);
            this.chkBoxVerboseLog.Name = "chkBoxVerboseLog";
            this.chkBoxVerboseLog.Size = new System.Drawing.Size(102, 19);
            this.chkBoxVerboseLog.TabIndex = 30;
            this.chkBoxVerboseLog.Text = "Detailed Logs";
            this.settingsToolTip.SetToolTip(this.chkBoxVerboseLog, "Logs program operation, could be useful for troubleshooting.\r\n\r\nObs: Error messag" +
        "es will always be logged.");
            this.chkBoxVerboseLog.UseVisualStyleBackColor = true;
            // 
            // appSettingsBindingSource
            // 
            this.appSettingsBindingSource.DataSource = typeof(BlenderRenderController.AppSettings);
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 286);
            this.Controls.Add(this.chkBoxVerboseLog);
            this.Controls.Add(this.ffmpegDownloadLabel);
            this.Controls.Add(this.ffmpegLabel);
            this.Controls.Add(this.blenderLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.ffmpegChangePathButton);
            this.Controls.Add(this.blenderChangePathButton);
            this.Controls.Add(this.blenderExeLabel);
            this.Controls.Add(this.ffmpegPathTextBox);
            this.Controls.Add(this.blenderPathTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(560, 340);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(540, 310);
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SettingsForm_FormClosed);
            this.Load += new System.EventHandler(this.onFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.appSettingsBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label blenderExeLabel;
        private System.Windows.Forms.Button blenderChangePathButton;
        private System.Windows.Forms.Button ffmpegChangePathButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label blenderLabel;
        private System.Windows.Forms.Label ffmpegLabel;
        private System.Windows.Forms.TextBox blenderPathTextBox;
        private System.Windows.Forms.TextBox ffmpegPathTextBox;
        private System.Windows.Forms.Label ffmpegDownloadLabel;
        private System.Windows.Forms.CheckBox chkBoxVerboseLog;
        private System.Windows.Forms.ToolTip settingsToolTip;
        private System.Windows.Forms.BindingSource appSettingsBindingSource;
    }
}