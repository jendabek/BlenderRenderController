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
            this.outputFileTextBox = new System.Windows.Forms.TextBox();
            this.joinButton = new System.Windows.Forms.Button();
            this.changeOutputPathButton = new System.Windows.Forms.Button();
            this.changeChunksFolderButton = new System.Windows.Forms.Button();
            this.manConcatToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.changeMixdownFileButton = new System.Windows.Forms.Button();
            this.mixdownFileTextBox = new System.Windows.Forms.TextBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.cancelButton = new System.Windows.Forms.Button();
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gBoxConcat = new System.Windows.Forms.GroupBox();
            this.gBoxOutput = new System.Windows.Forms.GroupBox();
            this.gBoxMixdown = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.tableLayout.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gBoxConcat.SuspendLayout();
            this.gBoxOutput.SuspendLayout();
            this.gBoxMixdown.SuspendLayout();
            this.SuspendLayout();
            // 
            // chunksTxtFileTextBox
            // 
            this.chunksTxtFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chunksTxtFileTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chunksTxtFileTextBox.Location = new System.Drawing.Point(6, 21);
            this.chunksTxtFileTextBox.Name = "chunksTxtFileTextBox";
            this.chunksTxtFileTextBox.Size = new System.Drawing.Size(681, 20);
            this.chunksTxtFileTextBox.TabIndex = 0;
            this.chunksTxtFileTextBox.WordWrap = false;
            this.chunksTxtFileTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.Entries_Validating);
            // 
            // outputFileTextBox
            // 
            this.outputFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputFileTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputFileTextBox.Location = new System.Drawing.Point(6, 21);
            this.outputFileTextBox.Name = "outputFileTextBox";
            this.outputFileTextBox.Size = new System.Drawing.Size(681, 20);
            this.outputFileTextBox.TabIndex = 2;
            this.outputFileTextBox.WordWrap = false;
            this.outputFileTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.Entries_Validating);
            // 
            // joinButton
            // 
            this.joinButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.joinButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.joinButton.Location = new System.Drawing.Point(493, 17);
            this.joinButton.Name = "joinButton";
            this.joinButton.Size = new System.Drawing.Size(90, 30);
            this.joinButton.TabIndex = 6;
            this.joinButton.Text = "Join";
            this.joinButton.UseVisualStyleBackColor = true;
            this.joinButton.Click += new System.EventHandler(this.joinCancelButton_Click);
            // 
            // changeOutputPathButton
            // 
            this.changeOutputPathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.changeOutputPathButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.changeOutputPathButton.Image = global::BlenderRenderController.Properties.Resources.folder_icon_smaller;
            this.changeOutputPathButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.changeOutputPathButton.Location = new System.Drawing.Point(592, 46);
            this.changeOutputPathButton.Name = "changeOutputPathButton";
            this.changeOutputPathButton.Size = new System.Drawing.Size(95, 25);
            this.changeOutputPathButton.TabIndex = 3;
            this.changeOutputPathButton.Text = "   Change";
            this.changeOutputPathButton.UseVisualStyleBackColor = true;
            this.changeOutputPathButton.Click += new System.EventHandler(this.changeOutputFileButton_Click);
            // 
            // changeChunksFolderButton
            // 
            this.changeChunksFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.changeChunksFolderButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.changeChunksFolderButton.Image = global::BlenderRenderController.Properties.Resources.folder_icon_smaller;
            this.changeChunksFolderButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.changeChunksFolderButton.Location = new System.Drawing.Point(592, 46);
            this.changeChunksFolderButton.Name = "changeChunksFolderButton";
            this.changeChunksFolderButton.Size = new System.Drawing.Size(95, 25);
            this.changeChunksFolderButton.TabIndex = 1;
            this.changeChunksFolderButton.Text = "   Change";
            this.changeChunksFolderButton.UseVisualStyleBackColor = true;
            this.changeChunksFolderButton.Click += new System.EventHandler(this.changeChunksTextFileButton_Click);
            // 
            // changeMixdownFileButton
            // 
            this.changeMixdownFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.changeMixdownFileButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.changeMixdownFileButton.Image = global::BlenderRenderController.Properties.Resources.folder_icon_smaller;
            this.changeMixdownFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.changeMixdownFileButton.Location = new System.Drawing.Point(592, 46);
            this.changeMixdownFileButton.Name = "changeMixdownFileButton";
            this.changeMixdownFileButton.Size = new System.Drawing.Size(95, 25);
            this.changeMixdownFileButton.TabIndex = 5;
            this.changeMixdownFileButton.Text = "   Change";
            this.changeMixdownFileButton.UseVisualStyleBackColor = true;
            this.changeMixdownFileButton.Click += new System.EventHandler(this.changeMixdownFileButton_Click);
            // 
            // mixdownFileTextBox
            // 
            this.mixdownFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mixdownFileTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mixdownFileTextBox.Location = new System.Drawing.Point(6, 21);
            this.mixdownFileTextBox.Name = "mixdownFileTextBox";
            this.mixdownFileTextBox.Size = new System.Drawing.Size(681, 20);
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
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(594, 17);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(90, 30);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.joinCancelButton_Click);
            // 
            // tableLayout
            // 
            this.tableLayout.ColumnCount = 1;
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout.Controls.Add(this.panel1, 0, 3);
            this.tableLayout.Controls.Add(this.gBoxConcat, 0, 0);
            this.tableLayout.Controls.Add(this.gBoxOutput, 0, 1);
            this.tableLayout.Controls.Add(this.gBoxMixdown, 0, 2);
            this.tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout.Location = new System.Drawing.Point(10, 15);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.RowCount = 4;
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayout.Size = new System.Drawing.Size(714, 286);
            this.tableLayout.TabIndex = 33;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.joinButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 228);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(708, 55);
            this.panel1.TabIndex = 34;
            // 
            // gBoxConcat
            // 
            this.gBoxConcat.Controls.Add(this.chunksTxtFileTextBox);
            this.gBoxConcat.Controls.Add(this.changeChunksFolderButton);
            this.gBoxConcat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gBoxConcat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gBoxConcat.Location = new System.Drawing.Point(0, 0);
            this.gBoxConcat.Margin = new System.Windows.Forms.Padding(0);
            this.gBoxConcat.Name = "gBoxConcat";
            this.gBoxConcat.Size = new System.Drawing.Size(714, 75);
            this.gBoxConcat.TabIndex = 35;
            this.gBoxConcat.TabStop = false;
            this.gBoxConcat.Text = "Concat. text file";
            // 
            // gBoxOutput
            // 
            this.gBoxOutput.Controls.Add(this.outputFileTextBox);
            this.gBoxOutput.Controls.Add(this.changeOutputPathButton);
            this.gBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gBoxOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gBoxOutput.Location = new System.Drawing.Point(0, 75);
            this.gBoxOutput.Margin = new System.Windows.Forms.Padding(0);
            this.gBoxOutput.Name = "gBoxOutput";
            this.gBoxOutput.Size = new System.Drawing.Size(714, 75);
            this.gBoxOutput.TabIndex = 36;
            this.gBoxOutput.TabStop = false;
            this.gBoxOutput.Text = "Output";
            // 
            // gBoxMixdown
            // 
            this.gBoxMixdown.Controls.Add(this.mixdownFileTextBox);
            this.gBoxMixdown.Controls.Add(this.changeMixdownFileButton);
            this.gBoxMixdown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gBoxMixdown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gBoxMixdown.Location = new System.Drawing.Point(0, 150);
            this.gBoxMixdown.Margin = new System.Windows.Forms.Padding(0);
            this.gBoxMixdown.Name = "gBoxMixdown";
            this.gBoxMixdown.Size = new System.Drawing.Size(714, 75);
            this.gBoxMixdown.TabIndex = 37;
            this.gBoxMixdown.TabStop = false;
            this.gBoxMixdown.Text = "Mixdown audio file [optional]";
            // 
            // ConcatForm
            // 
            this.AcceptButton = this.joinButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(734, 316);
            this.Controls.Add(this.tableLayout);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1280, 400);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(260, 345);
            this.Name = "ConcatForm";
            this.Padding = new System.Windows.Forms.Padding(10, 15, 10, 15);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manual Concatenation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConcatForm_FormClosing);
            this.Shown += new System.EventHandler(this.ConcatForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.tableLayout.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.gBoxConcat.ResumeLayout(false);
            this.gBoxConcat.PerformLayout();
            this.gBoxOutput.ResumeLayout(false);
            this.gBoxOutput.PerformLayout();
            this.gBoxMixdown.ResumeLayout(false);
            this.gBoxMixdown.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button changeChunksFolderButton;
        private System.Windows.Forms.Button changeOutputPathButton;
        private System.Windows.Forms.Button joinButton;
        private System.Windows.Forms.TextBox chunksTxtFileTextBox;
        private System.Windows.Forms.TextBox outputFileTextBox;
        private System.Windows.Forms.ToolTip manConcatToolTip;
        private System.Windows.Forms.Button changeMixdownFileButton;
        private System.Windows.Forms.TextBox mixdownFileTextBox;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TableLayoutPanel tableLayout;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox gBoxConcat;
        private System.Windows.Forms.GroupBox gBoxOutput;
        private System.Windows.Forms.GroupBox gBoxMixdown;
    }
}