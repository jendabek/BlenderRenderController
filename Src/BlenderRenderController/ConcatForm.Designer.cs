namespace BlenderRenderController
{
    partial class ConcatForm
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
            this.chunksTxtFileTextBox = new System.Windows.Forms.TextBox();
            this.mainLabel = new System.Windows.Forms.Label();
            this.outputFileTextBox = new System.Windows.Forms.TextBox();
            this.joinButton = new System.Windows.Forms.Button();
            this.chunksFolderLabel = new System.Windows.Forms.Label();
            this.outputLabel = new System.Windows.Forms.Label();
            this.changeOutputPathButton = new System.Windows.Forms.Button();
            this.changeChunksFolderButton = new System.Windows.Forms.Button();
            this.manConcatToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.changeMixdownFileButton = new System.Windows.Forms.Button();
            this.mixdownFileTextBox = new System.Windows.Forms.TextBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // chunksTxtFileTextBox
            // 
            this.chunksTxtFileTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chunksTxtFileTextBox.Location = new System.Drawing.Point(27, 78);
            this.chunksTxtFileTextBox.Name = "chunksTxtFileTextBox";
            this.chunksTxtFileTextBox.Size = new System.Drawing.Size(361, 22);
            this.chunksTxtFileTextBox.TabIndex = 0;
            this.chunksTxtFileTextBox.WordWrap = false;
            this.chunksTxtFileTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.Entries_Validating);
            // 
            // mainLabel
            // 
            this.mainLabel.AutoSize = true;
            this.mainLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Underline);
            this.mainLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.mainLabel.Location = new System.Drawing.Point(28, 19);
            this.mainLabel.Name = "mainLabel";
            this.mainLabel.Size = new System.Drawing.Size(172, 20);
            this.mainLabel.TabIndex = 26;
            this.mainLabel.Text = "Manual concatenation";
            // 
            // outputFileTextBox
            // 
            this.outputFileTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputFileTextBox.Location = new System.Drawing.Point(27, 127);
            this.outputFileTextBox.Name = "outputFileTextBox";
            this.outputFileTextBox.Size = new System.Drawing.Size(361, 22);
            this.outputFileTextBox.TabIndex = 2;
            this.outputFileTextBox.WordWrap = false;
            this.outputFileTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.Entries_Validating);
            // 
            // joinButton
            // 
            this.joinButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.joinButton.Location = new System.Drawing.Point(298, 232);
            this.joinButton.Name = "joinButton";
            this.joinButton.Size = new System.Drawing.Size(90, 30);
            this.joinButton.TabIndex = 6;
            this.joinButton.Text = "Join";
            this.joinButton.UseVisualStyleBackColor = true;
            this.joinButton.Click += new System.EventHandler(this.joinCancelButton_Click);
            // 
            // chunksFolderLabel
            // 
            this.chunksFolderLabel.AutoSize = true;
            this.chunksFolderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chunksFolderLabel.Location = new System.Drawing.Point(24, 60);
            this.chunksFolderLabel.Name = "chunksFolderLabel";
            this.chunksFolderLabel.Size = new System.Drawing.Size(89, 15);
            this.chunksFolderLabel.TabIndex = 29;
            this.chunksFolderLabel.Text = "Concat. text file";
            // 
            // outputLabel
            // 
            this.outputLabel.AutoSize = true;
            this.outputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputLabel.Location = new System.Drawing.Point(24, 109);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(43, 15);
            this.outputLabel.TabIndex = 29;
            this.outputLabel.Text = "Output";
            // 
            // changeOutputPathButton
            // 
            this.changeOutputPathButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.changeOutputPathButton.Image = global::BlenderRenderController.Properties.Resources.folder_icon_smaller;
            this.changeOutputPathButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.changeOutputPathButton.Location = new System.Drawing.Point(414, 122);
            this.changeOutputPathButton.Name = "changeOutputPathButton";
            this.changeOutputPathButton.Padding = new System.Windows.Forms.Padding(7, 0, 0, 1);
            this.changeOutputPathButton.Size = new System.Drawing.Size(105, 31);
            this.changeOutputPathButton.TabIndex = 3;
            this.changeOutputPathButton.Text = "   Change";
            this.changeOutputPathButton.UseVisualStyleBackColor = true;
            this.changeOutputPathButton.Click += new System.EventHandler(this.changeOutputFileButton_Click);
            // 
            // changeChunksFolderButton
            // 
            this.changeChunksFolderButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.changeChunksFolderButton.Image = global::BlenderRenderController.Properties.Resources.folder_icon_smaller;
            this.changeChunksFolderButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.changeChunksFolderButton.Location = new System.Drawing.Point(414, 73);
            this.changeChunksFolderButton.Name = "changeChunksFolderButton";
            this.changeChunksFolderButton.Padding = new System.Windows.Forms.Padding(7, 0, 0, 1);
            this.changeChunksFolderButton.Size = new System.Drawing.Size(105, 31);
            this.changeChunksFolderButton.TabIndex = 1;
            this.changeChunksFolderButton.Text = "   Change";
            this.changeChunksFolderButton.UseVisualStyleBackColor = true;
            this.changeChunksFolderButton.Click += new System.EventHandler(this.changeChunksTextFileButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(24, 159);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 15);
            this.label1.TabIndex = 32;
            this.label1.Text = "Mixdown audio file [optional]";
            // 
            // changeMixdownFileButton
            // 
            this.changeMixdownFileButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.changeMixdownFileButton.Image = global::BlenderRenderController.Properties.Resources.folder_icon_smaller;
            this.changeMixdownFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.changeMixdownFileButton.Location = new System.Drawing.Point(414, 171);
            this.changeMixdownFileButton.Name = "changeMixdownFileButton";
            this.changeMixdownFileButton.Padding = new System.Windows.Forms.Padding(7, 0, 0, 1);
            this.changeMixdownFileButton.Size = new System.Drawing.Size(105, 31);
            this.changeMixdownFileButton.TabIndex = 5;
            this.changeMixdownFileButton.Text = "   Change";
            this.changeMixdownFileButton.UseVisualStyleBackColor = true;
            this.changeMixdownFileButton.Click += new System.EventHandler(this.changeMixdownFileButton_Click);
            // 
            // mixdownFileTextBox
            // 
            this.mixdownFileTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mixdownFileTextBox.Location = new System.Drawing.Point(27, 176);
            this.mixdownFileTextBox.Name = "mixdownFileTextBox";
            this.mixdownFileTextBox.Size = new System.Drawing.Size(361, 22);
            this.mixdownFileTextBox.TabIndex = 4;
            this.mixdownFileTextBox.WordWrap = false;
            this.mixdownFileTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.Entries_Validating);
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(416, 232);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(90, 30);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.joinCancelButton_Click);
            // 
            // ConcatForm
            // 
            this.AcceptButton = this.joinButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(540, 277);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.changeMixdownFileButton);
            this.Controls.Add(this.mixdownFileTextBox);
            this.Controls.Add(this.outputLabel);
            this.Controls.Add(this.chunksFolderLabel);
            this.Controls.Add(this.joinButton);
            this.Controls.Add(this.changeOutputPathButton);
            this.Controls.Add(this.changeChunksFolderButton);
            this.Controls.Add(this.mainLabel);
            this.Controls.Add(this.outputFileTextBox);
            this.Controls.Add(this.chunksTxtFileTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(561, 316);
            this.MinimumSize = new System.Drawing.Size(200, 100);
            this.Name = "ConcatForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Concatenation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConcatForm_FormClosing);
            this.Shown += new System.EventHandler(this.ConcatForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label mainLabel;
        private System.Windows.Forms.Button changeChunksFolderButton;
        private System.Windows.Forms.Button changeOutputPathButton;
        private System.Windows.Forms.Button joinButton;
        private System.Windows.Forms.Label chunksFolderLabel;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.TextBox chunksTxtFileTextBox;
        private System.Windows.Forms.TextBox outputFileTextBox;
        private System.Windows.Forms.ToolTip manConcatToolTip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button changeMixdownFileButton;
        private System.Windows.Forms.TextBox mixdownFileTextBox;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Button cancelButton;
    }
}