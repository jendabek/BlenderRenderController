using BlenderRenderController.ui;

namespace BlenderRenderController
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.renderChunkButton = new System.Windows.Forms.Button();
            this.renderProgressBar = new System.Windows.Forms.ProgressBar();
            this.totalStartNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.startFrameLabel = new System.Windows.Forms.Label();
            this.chunkEndLabel = new System.Windows.Forms.Label();
            this.outputFolderTextBox = new System.Windows.Forms.TextBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.nextChunkButton = new System.Windows.Forms.Button();
            this.prevChunkButton = new System.Windows.Forms.Button();
            this.totalFrameCountLabel = new System.Windows.Forms.Label();
            this.totalEndNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.processCountNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.processCountLabel = new System.Windows.Forms.Label();
            this.totalTimeLabel = new System.Windows.Forms.Label();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.speToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.tipsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.isti115MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jendabekMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meTwentyFiveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redRaptorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.infoPanel = new System.Windows.Forms.Panel();
            this.infoDurationLabel = new System.Windows.Forms.Label();
            this.infoFramerate = new System.Windows.Forms.TextBox();
            this.infoDuration = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.infoNoScenes = new System.Windows.Forms.TextBox();
            this.infoFramesTotal = new System.Windows.Forms.TextBox();
            this.infoActiveScene = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.infoFramesTotalLabel = new System.Windows.Forms.Label();
            this.toolTipWarn = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipInfo = new System.Windows.Forms.ToolTip(this.components);
            this.reloadBlenderDataButton = new System.Windows.Forms.Button();
            this.concatenatePartsButton = new System.Windows.Forms.Button();
            this.renderAllButton = new System.Windows.Forms.Button();
            this.blendFileLabel = new System.Windows.Forms.Label();
            this.chunkLengthLabel = new System.Windows.Forms.Label();
            this.chunkStartLabel = new System.Windows.Forms.Label();
            this.chunkLengthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.chunkStartNumericUpDown = new System.Windows.Forms.TextBox();
            this.chunkEndNumericUpDown = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.timeElapsedLabel = new System.Windows.Forms.Label();
            this.currentChunkInfoPanel = new System.Windows.Forms.Panel();
            this.blendFileNameLabel = new System.Windows.Forms.Label();
            this.rendererRadioButtonCycles = new System.Windows.Forms.RadioButton();
            this.rendererRadioButtonBlender = new System.Windows.Forms.RadioButton();
            this.mixDownButton = new System.Windows.Forms.Button();
            this.openOutputFolderButton = new System.Windows.Forms.Button();
            this.outputFolderBrowseButton = new System.Windows.Forms.Button();
            this.startEndBlendRadio = new System.Windows.Forms.RadioButton();
            this.startEndCustomRadio = new System.Windows.Forms.RadioButton();
            this.blendFileBrowseButton = new BlenderRenderController.ui.SplitButton();
            ((System.ComponentModel.ISupportInitialize)(this.totalStartNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalEndNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.processCountNumericUpDown)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.infoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chunkLengthNumericUpDown)).BeginInit();
            this.currentChunkInfoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // renderChunkButton
            // 
            this.renderChunkButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.renderChunkButton.Location = new System.Drawing.Point(21, 75);
            this.renderChunkButton.Name = "renderChunkButton";
            this.renderChunkButton.Size = new System.Drawing.Size(101, 34);
            this.renderChunkButton.TabIndex = 0;
            this.renderChunkButton.TabStop = false;
            this.renderChunkButton.Text = "Render Chunk";
            this.toolTipInfo.SetToolTip(this.renderChunkButton, "Render current segment");
            this.renderChunkButton.UseVisualStyleBackColor = true;
            this.renderChunkButton.Click += new System.EventHandler(this.renderChunkButton_Click);
            // 
            // renderProgressBar
            // 
            this.renderProgressBar.Location = new System.Drawing.Point(32, 475);
            this.renderProgressBar.Name = "renderProgressBar";
            this.renderProgressBar.Size = new System.Drawing.Size(612, 14);
            this.renderProgressBar.Step = 1;
            this.renderProgressBar.TabIndex = 2;
            this.toolTipInfo.SetToolTip(this.renderProgressBar, "Progress bar");
            // 
            // totalStartNumericUpDown
            // 
            this.totalStartNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalStartNumericUpDown.Location = new System.Drawing.Point(139, 239);
            this.totalStartNumericUpDown.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.totalStartNumericUpDown.Name = "totalStartNumericUpDown";
            this.totalStartNumericUpDown.Size = new System.Drawing.Size(90, 22);
            this.totalStartNumericUpDown.TabIndex = 5;
            this.totalStartNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTipInfo.SetToolTip(this.totalStartNumericUpDown, "Segment\'s starting frame");
            this.totalStartNumericUpDown.ValueChanged += new System.EventHandler(this.totalStartNumericUpDown_ValueChanged);
            this.totalStartNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            // 
            // startFrameLabel
            // 
            this.startFrameLabel.AutoSize = true;
            this.startFrameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startFrameLabel.Location = new System.Drawing.Point(136, 222);
            this.startFrameLabel.Name = "startFrameLabel";
            this.startFrameLabel.Size = new System.Drawing.Size(71, 15);
            this.startFrameLabel.TabIndex = 6;
            this.startFrameLabel.Text = "Start Frame";
            this.toolTipInfo.SetToolTip(this.startFrameLabel, "Segment\'s starting frame");
            // 
            // chunkEndLabel
            // 
            this.chunkEndLabel.AutoSize = true;
            this.chunkEndLabel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.chunkEndLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chunkEndLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.chunkEndLabel.Location = new System.Drawing.Point(130, 70);
            this.chunkEndLabel.Name = "chunkEndLabel";
            this.chunkEndLabel.Size = new System.Drawing.Size(68, 15);
            this.chunkEndLabel.TabIndex = 7;
            this.chunkEndLabel.Text = "End Frame";
            this.toolTipInfo.SetToolTip(this.chunkEndLabel, "Segment\'s end frame");
            // 
            // outputFolderTextBox
            // 
            this.outputFolderTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputFolderTextBox.Location = new System.Drawing.Point(32, 330);
            this.outputFolderTextBox.Name = "outputFolderTextBox";
            this.outputFolderTextBox.Size = new System.Drawing.Size(497, 22);
            this.outputFolderTextBox.TabIndex = 11;
            this.outputFolderTextBox.WordWrap = false;
            this.outputFolderTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            this.outputFolderTextBox.Leave += new System.EventHandler(this.outputFolderPathTextBox_TextChanged);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.statusLabel.Location = new System.Drawing.Point(32, 503);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(145, 16);
            this.statusLabel.TabIndex = 11;
            this.statusLabel.Text = "Some status message.";
            this.toolTipInfo.SetToolTip(this.statusLabel, "Progress");
            this.statusLabel.Visible = false;
            // 
            // nextChunkButton
            // 
            this.nextChunkButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nextChunkButton.Location = new System.Drawing.Point(74, 39);
            this.nextChunkButton.Name = "nextChunkButton";
            this.nextChunkButton.Size = new System.Drawing.Size(47, 27);
            this.nextChunkButton.TabIndex = 12;
            this.nextChunkButton.TabStop = false;
            this.nextChunkButton.Text = ">";
            this.toolTipInfo.SetToolTip(this.nextChunkButton, "Segment select");
            this.nextChunkButton.UseVisualStyleBackColor = true;
            this.nextChunkButton.Click += new System.EventHandler(this.nextChunkButton_Click);
            // 
            // prevChunkButton
            // 
            this.prevChunkButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prevChunkButton.Location = new System.Drawing.Point(21, 39);
            this.prevChunkButton.Name = "prevChunkButton";
            this.prevChunkButton.Size = new System.Drawing.Size(47, 27);
            this.prevChunkButton.TabIndex = 12;
            this.prevChunkButton.TabStop = false;
            this.prevChunkButton.Text = "<";
            this.toolTipInfo.SetToolTip(this.prevChunkButton, "Segment select");
            this.prevChunkButton.UseVisualStyleBackColor = true;
            this.prevChunkButton.Click += new System.EventHandler(this.prevChunkButton_Click);
            // 
            // totalFrameCountLabel
            // 
            this.totalFrameCountLabel.AutoSize = true;
            this.totalFrameCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalFrameCountLabel.Location = new System.Drawing.Point(236, 222);
            this.totalFrameCountLabel.Name = "totalFrameCountLabel";
            this.totalFrameCountLabel.Size = new System.Drawing.Size(68, 15);
            this.totalFrameCountLabel.TabIndex = 13;
            this.totalFrameCountLabel.Text = "End Frame";
            this.toolTipInfo.SetToolTip(this.totalFrameCountLabel, "Project\'s end frame");
            // 
            // totalEndNumericUpDown
            // 
            this.totalEndNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalEndNumericUpDown.Location = new System.Drawing.Point(239, 239);
            this.totalEndNumericUpDown.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.totalEndNumericUpDown.Name = "totalEndNumericUpDown";
            this.totalEndNumericUpDown.Size = new System.Drawing.Size(90, 22);
            this.totalEndNumericUpDown.TabIndex = 6;
            this.totalEndNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTipInfo.SetToolTip(this.totalEndNumericUpDown, "Project\'s end frame");
            this.totalEndNumericUpDown.Value = new decimal(new int[] {
            123456,
            0,
            0,
            0});
            this.totalEndNumericUpDown.ValueChanged += new System.EventHandler(this.totalEndNumericUpDown_ValueChanged);
            this.totalEndNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            // 
            // processCountNumericUpDown
            // 
            this.processCountNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processCountNumericUpDown.Location = new System.Drawing.Point(498, 240);
            this.processCountNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.processCountNumericUpDown.Name = "processCountNumericUpDown";
            this.processCountNumericUpDown.Size = new System.Drawing.Size(69, 22);
            this.processCountNumericUpDown.TabIndex = 8;
            this.processCountNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTipInfo.SetToolTip(this.processCountNumericUpDown, "N# of processes. For best results set acording to \r\nhow many logical cores you ha" +
        "ve.");
            this.processCountNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.processCountNumericUpDown.ValueChanged += new System.EventHandler(this.processCountNumericUpDown_ValueChanged);
            this.processCountNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            // 
            // processCountLabel
            // 
            this.processCountLabel.AutoSize = true;
            this.processCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processCountLabel.Location = new System.Drawing.Point(495, 222);
            this.processCountLabel.Name = "processCountLabel";
            this.processCountLabel.Size = new System.Drawing.Size(86, 15);
            this.processCountLabel.TabIndex = 15;
            this.processCountLabel.Text = "Process Count";
            this.toolTipInfo.SetToolTip(this.processCountLabel, "N# of processes. For best results set acording to ");
            // 
            // totalTimeLabel
            // 
            this.totalTimeLabel.AutoSize = true;
            this.totalTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.totalTimeLabel.Location = new System.Drawing.Point(588, 503);
            this.totalTimeLabel.Name = "totalTimeLabel";
            this.totalTimeLabel.Size = new System.Drawing.Size(56, 16);
            this.totalTimeLabel.TabIndex = 19;
            this.totalTimeLabel.Text = "00:00:00";
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.ControlLight;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.infoToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(662, 24);
            this.menuStrip.TabIndex = 20;
            this.menuStrip.Text = "menuStrip1";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.speToolStripMenuItem,
            this.tipsToolStripMenuItem});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.aboutToolStripMenuItem.Text = "Options";
            this.aboutToolStripMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.aboutToolStripMenuItem.ToolTipText = "Extra options";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("settingsToolStripMenuItem.Image")));
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // speToolStripMenuItem
            // 
            this.speToolStripMenuItem.Name = "speToolStripMenuItem";
            this.speToolStripMenuItem.Size = new System.Drawing.Size(113, 6);
            // 
            // tipsToolStripMenuItem
            // 
            this.tipsToolStripMenuItem.Checked = true;
            this.tipsToolStripMenuItem.CheckOnClick = true;
            this.tipsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tipsToolStripMenuItem.Name = "tipsToolStripMenuItem";
            this.tipsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.tipsToolStripMenuItem.Text = "Tooltips";
            this.tipsToolStripMenuItem.Click += new System.EventHandler(this.tipsToolStripMenuItem_Click);
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.infoToolStripMenuItem.Text = "Info";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.isti115MenuItem,
            this.jendabekMenuItem,
            this.meTwentyFiveMenuItem,
            this.redRaptorMenuItem});
            this.toolStripMenuItem1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripMenuItem1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripMenuItem1.Image = global::BlenderRenderController.Properties.Resources.github_logo_small;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(172, 22);
            this.toolStripMenuItem1.Text = "Authors on Github";
            // 
            // isti115MenuItem
            // 
            this.isti115MenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.isti115MenuItem.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.isti115MenuItem.Image = ((System.Drawing.Image)(resources.GetObject("isti115MenuItem.Image")));
            this.isti115MenuItem.Name = "isti115MenuItem";
            this.isti115MenuItem.Size = new System.Drawing.Size(150, 22);
            this.isti115MenuItem.Text = "Isti115";
            this.isti115MenuItem.Click += new System.EventHandler(this.isti115ToolStripMenuItem_Click);
            // 
            // jendabekMenuItem
            // 
            this.jendabekMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.jendabekMenuItem.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.jendabekMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("jendabekMenuItem.Image")));
            this.jendabekMenuItem.Name = "jendabekMenuItem";
            this.jendabekMenuItem.Size = new System.Drawing.Size(150, 22);
            this.jendabekMenuItem.Text = "jendabek";
            this.jendabekMenuItem.Click += new System.EventHandler(this.jendabekToolStripMenuItem_Click);
            // 
            // meTwentyFiveMenuItem
            // 
            this.meTwentyFiveMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.meTwentyFiveMenuItem.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.meTwentyFiveMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("meTwentyFiveMenuItem.Image")));
            this.meTwentyFiveMenuItem.Name = "meTwentyFiveMenuItem";
            this.meTwentyFiveMenuItem.Size = new System.Drawing.Size(150, 22);
            this.meTwentyFiveMenuItem.Text = "MeTwentyFive";
            this.meTwentyFiveMenuItem.Click += new System.EventHandler(this.meTwentyFiveToolStripMenuItem_Click);
            // 
            // redRaptorMenuItem
            // 
            this.redRaptorMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.redRaptorMenuItem.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.redRaptorMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("redRaptorMenuItem.Image")));
            this.redRaptorMenuItem.Name = "redRaptorMenuItem";
            this.redRaptorMenuItem.Size = new System.Drawing.Size(150, 22);
            this.redRaptorMenuItem.Text = "RedRaptor93";
            this.redRaptorMenuItem.Click += new System.EventHandler(this.redRaptor93ToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 15);
            this.label1.TabIndex = 21;
            this.label1.Text = "Active Scene";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTipWarn.SetToolTip(this.label1, "This program will only render the ACTIVE scene, if you \r\nhave more then one scene" +
        " on your project sure you \r\nsave it with the scene you want OPEN.");
            // 
            // infoPanel
            // 
            this.infoPanel.BackColor = System.Drawing.SystemColors.Info;
            this.infoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.infoPanel.Controls.Add(this.infoDurationLabel);
            this.infoPanel.Controls.Add(this.infoFramerate);
            this.infoPanel.Controls.Add(this.infoDuration);
            this.infoPanel.Controls.Add(this.label7);
            this.infoPanel.Controls.Add(this.infoNoScenes);
            this.infoPanel.Controls.Add(this.infoFramesTotal);
            this.infoPanel.Controls.Add(this.infoActiveScene);
            this.infoPanel.Controls.Add(this.label3);
            this.infoPanel.Controls.Add(this.label1);
            this.infoPanel.Controls.Add(this.infoFramesTotalLabel);
            this.infoPanel.Location = new System.Drawing.Point(32, 74);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(434, 87);
            this.infoPanel.TabIndex = 22;
            // 
            // infoDurationLabel
            // 
            this.infoDurationLabel.AutoSize = true;
            this.infoDurationLabel.BackColor = System.Drawing.SystemColors.Info;
            this.infoDurationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoDurationLabel.Location = new System.Drawing.Point(299, 20);
            this.infoDurationLabel.Name = "infoDurationLabel";
            this.infoDurationLabel.Size = new System.Drawing.Size(75, 15);
            this.infoDurationLabel.TabIndex = 31;
            this.infoDurationLabel.Text = "Length Total";
            // 
            // infoFramerate
            // 
            this.infoFramerate.BackColor = System.Drawing.SystemColors.Control;
            this.infoFramerate.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.infoFramerate.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.infoFramerate.Location = new System.Drawing.Point(180, 39);
            this.infoFramerate.Name = "infoFramerate";
            this.infoFramerate.ReadOnly = true;
            this.infoFramerate.Size = new System.Drawing.Size(36, 20);
            this.infoFramerate.TabIndex = 30;
            this.infoFramerate.TabStop = false;
            this.infoFramerate.Text = "...";
            this.infoFramerate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // infoDuration
            // 
            this.infoDuration.BackColor = System.Drawing.SystemColors.Control;
            this.infoDuration.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.infoDuration.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.infoDuration.Location = new System.Drawing.Point(302, 39);
            this.infoDuration.Name = "infoDuration";
            this.infoDuration.ReadOnly = true;
            this.infoDuration.Size = new System.Drawing.Size(121, 20);
            this.infoDuration.TabIndex = 30;
            this.infoDuration.TabStop = false;
            this.infoDuration.Text = "...";
            this.infoDuration.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(177, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 15);
            this.label7.TabIndex = 29;
            this.label7.Text = "FPS";
            // 
            // infoNoScenes
            // 
            this.infoNoScenes.BackColor = System.Drawing.SystemColors.Control;
            this.infoNoScenes.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.infoNoScenes.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.infoNoScenes.Location = new System.Drawing.Point(131, 39);
            this.infoNoScenes.Name = "infoNoScenes";
            this.infoNoScenes.ReadOnly = true;
            this.infoNoScenes.Size = new System.Drawing.Size(43, 20);
            this.infoNoScenes.TabIndex = 26;
            this.infoNoScenes.TabStop = false;
            this.infoNoScenes.Text = "...";
            this.infoNoScenes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTipInfo.SetToolTip(this.infoNoScenes, "Number of scenes in project.");
            // 
            // infoFramesTotal
            // 
            this.infoFramesTotal.BackColor = System.Drawing.SystemColors.Control;
            this.infoFramesTotal.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.infoFramesTotal.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.infoFramesTotal.Location = new System.Drawing.Point(222, 39);
            this.infoFramesTotal.Name = "infoFramesTotal";
            this.infoFramesTotal.ReadOnly = true;
            this.infoFramesTotal.Size = new System.Drawing.Size(74, 20);
            this.infoFramesTotal.TabIndex = 30;
            this.infoFramesTotal.TabStop = false;
            this.infoFramesTotal.Text = "...";
            this.infoFramesTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // infoActiveScene
            // 
            this.infoActiveScene.BackColor = System.Drawing.SystemColors.Control;
            this.infoActiveScene.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.infoActiveScene.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.infoActiveScene.Location = new System.Drawing.Point(11, 39);
            this.infoActiveScene.Name = "infoActiveScene";
            this.infoActiveScene.ReadOnly = true;
            this.infoActiveScene.Size = new System.Drawing.Size(114, 20);
            this.infoActiveScene.TabIndex = 25;
            this.infoActiveScene.TabStop = false;
            this.infoActiveScene.Text = "...";
            this.infoActiveScene.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTipWarn.SetToolTip(this.infoActiveScene, "This program will only render the ACTIVE scene, if you \r\nhave more then one scene" +
        " on your project sure you \r\nsave it with the scene you want OPEN.");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(128, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 15);
            this.label3.TabIndex = 23;
            this.label3.Text = "Scenes";
            this.toolTipWarn.SetToolTip(this.label3, "Number of scenes in project.");
            // 
            // infoFramesTotalLabel
            // 
            this.infoFramesTotalLabel.AutoSize = true;
            this.infoFramesTotalLabel.BackColor = System.Drawing.SystemColors.Info;
            this.infoFramesTotalLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoFramesTotalLabel.Location = new System.Drawing.Point(219, 20);
            this.infoFramesTotalLabel.Name = "infoFramesTotalLabel";
            this.infoFramesTotalLabel.Size = new System.Drawing.Size(79, 15);
            this.infoFramesTotalLabel.TabIndex = 29;
            this.infoFramesTotalLabel.Text = "Frames Total";
            // 
            // toolTipWarn
            // 
            this.toolTipWarn.AutomaticDelay = 1000;
            this.toolTipWarn.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            this.toolTipWarn.ToolTipTitle = "Caution";
            // 
            // toolTipInfo
            // 
            this.toolTipInfo.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipInfo.ToolTipTitle = "Hint";
            // 
            // reloadBlenderDataButton
            // 
            this.reloadBlenderDataButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reloadBlenderDataButton.Image = global::BlenderRenderController.Properties.Resources.reload_icon_small;
            this.reloadBlenderDataButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.reloadBlenderDataButton.Location = new System.Drawing.Point(472, 127);
            this.reloadBlenderDataButton.Name = "reloadBlenderDataButton";
            this.reloadBlenderDataButton.Padding = new System.Windows.Forms.Padding(20, 0, 5, 0);
            this.reloadBlenderDataButton.Size = new System.Drawing.Size(172, 34);
            this.reloadBlenderDataButton.TabIndex = 2;
            this.reloadBlenderDataButton.Text = "Reload Blend";
            this.toolTipInfo.SetToolTip(this.reloadBlenderDataButton, "Re-read info from .blend");
            this.reloadBlenderDataButton.UseVisualStyleBackColor = true;
            this.reloadBlenderDataButton.Click += new System.EventHandler(this.reloadBlenderDataButton_Click);
            // 
            // concatenatePartsButton
            // 
            this.concatenatePartsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.concatenatePartsButton.Image = global::BlenderRenderController.Properties.Resources.connect_icon_small;
            this.concatenatePartsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.concatenatePartsButton.Location = new System.Drawing.Point(372, 416);
            this.concatenatePartsButton.Name = "concatenatePartsButton";
            this.concatenatePartsButton.Padding = new System.Windows.Forms.Padding(12, 0, 7, 0);
            this.concatenatePartsButton.Size = new System.Drawing.Size(143, 38);
            this.concatenatePartsButton.TabIndex = 15;
            this.concatenatePartsButton.Text = "Join Chunks";
            this.concatenatePartsButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipInfo.SetToolTip(this.concatenatePartsButton, "Combine segments in FFmpeg");
            this.concatenatePartsButton.UseVisualStyleBackColor = true;
            this.concatenatePartsButton.Click += new System.EventHandler(this.concatenatePartsButton_Click);
            // 
            // renderAllButton
            // 
            this.renderAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.renderAllButton.Image = global::BlenderRenderController.Properties.Resources.render_icon_small;
            this.renderAllButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.renderAllButton.Location = new System.Drawing.Point(32, 416);
            this.renderAllButton.Name = "renderAllButton";
            this.renderAllButton.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.renderAllButton.Size = new System.Drawing.Size(168, 38);
            this.renderAllButton.TabIndex = 13;
            this.renderAllButton.Text = "Render";
            this.toolTipInfo.SetToolTip(this.renderAllButton, "Starts rendering the timeline.");
            this.renderAllButton.UseVisualStyleBackColor = true;
            this.renderAllButton.Click += new System.EventHandler(this.renderAllButton_Click);
            // 
            // blendFileLabel
            // 
            this.blendFileLabel.AutoSize = true;
            this.blendFileLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Underline);
            this.blendFileLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.blendFileLabel.Location = new System.Drawing.Point(19, 43);
            this.blendFileLabel.Name = "blendFileLabel";
            this.blendFileLabel.Size = new System.Drawing.Size(102, 20);
            this.blendFileLabel.TabIndex = 25;
            this.blendFileLabel.Text = "1. Blend File";
            // 
            // chunkLengthLabel
            // 
            this.chunkLengthLabel.AutoSize = true;
            this.chunkLengthLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chunkLengthLabel.Location = new System.Drawing.Point(394, 222);
            this.chunkLengthLabel.Name = "chunkLengthLabel";
            this.chunkLengthLabel.Size = new System.Drawing.Size(69, 15);
            this.chunkLengthLabel.TabIndex = 13;
            this.chunkLengthLabel.Text = "Chunk Size";
            // 
            // chunkStartLabel
            // 
            this.chunkStartLabel.AutoSize = true;
            this.chunkStartLabel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.chunkStartLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chunkStartLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.chunkStartLabel.Location = new System.Drawing.Point(130, 26);
            this.chunkStartLabel.Name = "chunkStartLabel";
            this.chunkStartLabel.Size = new System.Drawing.Size(71, 15);
            this.chunkStartLabel.TabIndex = 30;
            this.chunkStartLabel.Text = "Start Frame";
            // 
            // chunkLengthNumericUpDown
            // 
            this.chunkLengthNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chunkLengthNumericUpDown.Location = new System.Drawing.Point(397, 240);
            this.chunkLengthNumericUpDown.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.chunkLengthNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.chunkLengthNumericUpDown.Name = "chunkLengthNumericUpDown";
            this.chunkLengthNumericUpDown.Size = new System.Drawing.Size(86, 22);
            this.chunkLengthNumericUpDown.TabIndex = 7;
            this.chunkLengthNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.chunkLengthNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.chunkLengthNumericUpDown.ValueChanged += new System.EventHandler(this.chunkLengthNumericUpDown_ValueChanged);
            this.chunkLengthNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Underline);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(19, 298);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 20);
            this.label2.TabIndex = 25;
            this.label2.Text = "4. Output Folder";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Underline);
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Location = new System.Drawing.Point(19, 186);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(107, 20);
            this.label8.TabIndex = 25;
            this.label8.Text = "2. Start / End";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Underline);
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label10.Location = new System.Drawing.Point(19, 382);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(81, 20);
            this.label10.TabIndex = 25;
            this.label10.Text = "5. Render";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Underline);
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label12.Location = new System.Drawing.Point(380, 186);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(144, 20);
            this.label12.TabIndex = 25;
            this.label12.Text = "3. Render Options";
            // 
            // chunkStartNumericUpDown
            // 
            this.chunkStartNumericUpDown.Cursor = System.Windows.Forms.Cursors.No;
            this.chunkStartNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chunkStartNumericUpDown.Location = new System.Drawing.Point(133, 43);
            this.chunkStartNumericUpDown.Name = "chunkStartNumericUpDown";
            this.chunkStartNumericUpDown.ReadOnly = true;
            this.chunkStartNumericUpDown.Size = new System.Drawing.Size(117, 21);
            this.chunkStartNumericUpDown.TabIndex = 30;
            this.chunkStartNumericUpDown.TabStop = false;
            this.chunkStartNumericUpDown.Text = "...";
            this.chunkStartNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // chunkEndNumericUpDown
            // 
            this.chunkEndNumericUpDown.Cursor = System.Windows.Forms.Cursors.No;
            this.chunkEndNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chunkEndNumericUpDown.Location = new System.Drawing.Point(133, 87);
            this.chunkEndNumericUpDown.Name = "chunkEndNumericUpDown";
            this.chunkEndNumericUpDown.ReadOnly = true;
            this.chunkEndNumericUpDown.Size = new System.Drawing.Size(117, 21);
            this.chunkEndNumericUpDown.TabIndex = 30;
            this.chunkEndNumericUpDown.TabStop = false;
            this.chunkEndNumericUpDown.Text = "...";
            this.chunkEndNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Underline);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label5.Location = new System.Drawing.Point(368, 382);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 20);
            this.label5.TabIndex = 25;
            this.label5.Text = "6. Join";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label15.Location = new System.Drawing.Point(19, 12);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(90, 16);
            this.label15.TabIndex = 25;
            this.label15.Text = "Current Chunk";
            // 
            // timeElapsedLabel
            // 
            this.timeElapsedLabel.AutoSize = true;
            this.timeElapsedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeElapsedLabel.Location = new System.Drawing.Point(492, 503);
            this.timeElapsedLabel.Name = "timeElapsedLabel";
            this.timeElapsedLabel.Size = new System.Drawing.Size(96, 16);
            this.timeElapsedLabel.TabIndex = 19;
            this.timeElapsedLabel.Text = "Time Elapsed:";
            // 
            // currentChunkInfoPanel
            // 
            this.currentChunkInfoPanel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.currentChunkInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.currentChunkInfoPanel.Controls.Add(this.renderChunkButton);
            this.currentChunkInfoPanel.Controls.Add(this.chunkEndNumericUpDown);
            this.currentChunkInfoPanel.Controls.Add(this.prevChunkButton);
            this.currentChunkInfoPanel.Controls.Add(this.chunkStartNumericUpDown);
            this.currentChunkInfoPanel.Controls.Add(this.nextChunkButton);
            this.currentChunkInfoPanel.Controls.Add(this.chunkStartLabel);
            this.currentChunkInfoPanel.Controls.Add(this.label15);
            this.currentChunkInfoPanel.Controls.Add(this.chunkEndLabel);
            this.currentChunkInfoPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.currentChunkInfoPanel.Location = new System.Drawing.Point(158, 52);
            this.currentChunkInfoPanel.Name = "currentChunkInfoPanel";
            this.currentChunkInfoPanel.Size = new System.Drawing.Size(268, 122);
            this.currentChunkInfoPanel.TabIndex = 31;
            this.currentChunkInfoPanel.Visible = false;
            // 
            // blendFileNameLabel
            // 
            this.blendFileNameLabel.AutoSize = true;
            this.blendFileNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Bold);
            this.blendFileNameLabel.ForeColor = System.Drawing.Color.Black;
            this.blendFileNameLabel.Location = new System.Drawing.Point(20, 43);
            this.blendFileNameLabel.Name = "blendFileNameLabel";
            this.blendFileNameLabel.Size = new System.Drawing.Size(0, 20);
            this.blendFileNameLabel.TabIndex = 25;
            // 
            // rendererRadioButtonCycles
            // 
            this.rendererRadioButtonCycles.AutoSize = true;
            this.rendererRadioButtonCycles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rendererRadioButtonCycles.Location = new System.Drawing.Point(515, 271);
            this.rendererRadioButtonCycles.Name = "rendererRadioButtonCycles";
            this.rendererRadioButtonCycles.Size = new System.Drawing.Size(104, 19);
            this.rendererRadioButtonCycles.TabIndex = 10;
            this.rendererRadioButtonCycles.Text = "Cycles Render";
            this.rendererRadioButtonCycles.UseVisualStyleBackColor = true;
            this.rendererRadioButtonCycles.Visible = false;
            this.rendererRadioButtonCycles.CheckedChanged += new System.EventHandler(this.rendererComboBox_CheckedChanged);
            // 
            // rendererRadioButtonBlender
            // 
            this.rendererRadioButtonBlender.AutoSize = true;
            this.rendererRadioButtonBlender.Checked = true;
            this.rendererRadioButtonBlender.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rendererRadioButtonBlender.Location = new System.Drawing.Point(397, 271);
            this.rendererRadioButtonBlender.Name = "rendererRadioButtonBlender";
            this.rendererRadioButtonBlender.Size = new System.Drawing.Size(112, 19);
            this.rendererRadioButtonBlender.TabIndex = 9;
            this.rendererRadioButtonBlender.TabStop = true;
            this.rendererRadioButtonBlender.Text = "Blender Render";
            this.rendererRadioButtonBlender.UseVisualStyleBackColor = true;
            this.rendererRadioButtonBlender.Visible = false;
            // 
            // mixDownButton
            // 
            this.mixDownButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mixDownButton.Image = global::BlenderRenderController.Properties.Resources.volume_small;
            this.mixDownButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mixDownButton.Location = new System.Drawing.Point(206, 416);
            this.mixDownButton.Name = "mixDownButton";
            this.mixDownButton.Padding = new System.Windows.Forms.Padding(8, 0, 10, 0);
            this.mixDownButton.Size = new System.Drawing.Size(110, 38);
            this.mixDownButton.TabIndex = 14;
            this.mixDownButton.Text = "Mixdown";
            this.mixDownButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mixDownButton.UseVisualStyleBackColor = true;
            this.mixDownButton.Click += new System.EventHandler(this.MixdownAudio_Click);
            // 
            // openOutputFolderButton
            // 
            this.openOutputFolderButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.openOutputFolderButton.Image = global::BlenderRenderController.Properties.Resources.folder_icon_smaller;
            this.openOutputFolderButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.openOutputFolderButton.Location = new System.Drawing.Point(521, 416);
            this.openOutputFolderButton.Name = "openOutputFolderButton";
            this.openOutputFolderButton.Padding = new System.Windows.Forms.Padding(10, 0, 5, 0);
            this.openOutputFolderButton.Size = new System.Drawing.Size(123, 38);
            this.openOutputFolderButton.TabIndex = 16;
            this.openOutputFolderButton.Text = "Open Folder";
            this.openOutputFolderButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.openOutputFolderButton.UseVisualStyleBackColor = true;
            this.openOutputFolderButton.Click += new System.EventHandler(this.outputFolderOpenButton_Click);
            // 
            // outputFolderBrowseButton
            // 
            this.outputFolderBrowseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputFolderBrowseButton.Image = global::BlenderRenderController.Properties.Resources.folder_icon_smaller;
            this.outputFolderBrowseButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.outputFolderBrowseButton.Location = new System.Drawing.Point(539, 325);
            this.outputFolderBrowseButton.Name = "outputFolderBrowseButton";
            this.outputFolderBrowseButton.Padding = new System.Windows.Forms.Padding(8, 0, 0, 1);
            this.outputFolderBrowseButton.Size = new System.Drawing.Size(105, 31);
            this.outputFolderBrowseButton.TabIndex = 12;
            this.outputFolderBrowseButton.Text = "  Change";
            this.outputFolderBrowseButton.UseVisualStyleBackColor = true;
            this.outputFolderBrowseButton.Click += new System.EventHandler(this.outputFolderBrowseButton_Click);
            // 
            // startEndBlendRadio
            // 
            this.startEndBlendRadio.AutoSize = true;
            this.startEndBlendRadio.Checked = true;
            this.startEndBlendRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startEndBlendRadio.Location = new System.Drawing.Point(32, 221);
            this.startEndBlendRadio.Name = "startEndBlendRadio";
            this.startEndBlendRadio.Size = new System.Drawing.Size(89, 19);
            this.startEndBlendRadio.TabIndex = 3;
            this.startEndBlendRadio.TabStop = true;
            this.startEndBlendRadio.Text = "From Blend";
            this.startEndBlendRadio.UseVisualStyleBackColor = true;
            // 
            // startEndCustomRadio
            // 
            this.startEndCustomRadio.AutoSize = true;
            this.startEndCustomRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startEndCustomRadio.Location = new System.Drawing.Point(32, 243);
            this.startEndCustomRadio.Name = "startEndCustomRadio";
            this.startEndCustomRadio.Size = new System.Drawing.Size(67, 19);
            this.startEndCustomRadio.TabIndex = 4;
            this.startEndCustomRadio.Text = "Custom";
            this.startEndCustomRadio.UseVisualStyleBackColor = true;
            this.startEndCustomRadio.CheckedChanged += new System.EventHandler(this.startEndCustomRadio_CheckedChanged);
            // 
            // blendFileBrowseButton
            // 
            this.blendFileBrowseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blendFileBrowseButton.Image = ((System.Drawing.Image)(resources.GetObject("blendFileBrowseButton.Image")));
            this.blendFileBrowseButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.blendFileBrowseButton.Location = new System.Drawing.Point(472, 74);
            this.blendFileBrowseButton.Name = "blendFileBrowseButton";
            this.blendFileBrowseButton.Padding = new System.Windows.Forms.Padding(10, 2, 0, 0);
            this.blendFileBrowseButton.Size = new System.Drawing.Size(172, 47);
            this.blendFileBrowseButton.SplitWidth = 30;
            this.blendFileBrowseButton.TabIndex = 1;
            this.blendFileBrowseButton.Text = " Open Blend";
            this.blendFileBrowseButton.UseVisualStyleBackColor = true;
            this.blendFileBrowseButton.Click += new System.EventHandler(this.blendFileBrowseButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(662, 550);
            this.Controls.Add(this.startEndBlendRadio);
            this.Controls.Add(this.startEndCustomRadio);
            this.Controls.Add(this.currentChunkInfoPanel);
            this.Controls.Add(this.rendererRadioButtonBlender);
            this.Controls.Add(this.rendererRadioButtonCycles);
            this.Controls.Add(this.blendFileNameLabel);
            this.Controls.Add(this.blendFileLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.infoPanel);
            this.Controls.Add(this.timeElapsedLabel);
            this.Controls.Add(this.totalTimeLabel);
            this.Controls.Add(this.reloadBlenderDataButton);
            this.Controls.Add(this.mixDownButton);
            this.Controls.Add(this.openOutputFolderButton);
            this.Controls.Add(this.concatenatePartsButton);
            this.Controls.Add(this.processCountLabel);
            this.Controls.Add(this.processCountNumericUpDown);
            this.Controls.Add(this.chunkLengthLabel);
            this.Controls.Add(this.totalFrameCountLabel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.chunkLengthNumericUpDown);
            this.Controls.Add(this.startFrameLabel);
            this.Controls.Add(this.totalEndNumericUpDown);
            this.Controls.Add(this.outputFolderTextBox);
            this.Controls.Add(this.totalStartNumericUpDown);
            this.Controls.Add(this.outputFolderBrowseButton);
            this.Controls.Add(this.renderProgressBar);
            this.Controls.Add(this.blendFileBrowseButton);
            this.Controls.Add(this.renderAllButton);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(678, 589);
            this.MinimumSize = new System.Drawing.Size(678, 39);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Blender Render Controller";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_Close);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.totalStartNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalEndNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.processCountNumericUpDown)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.infoPanel.ResumeLayout(false);
            this.infoPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chunkLengthNumericUpDown)).EndInit();
            this.currentChunkInfoPanel.ResumeLayout(false);
            this.currentChunkInfoPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button renderChunkButton;
        private SplitButton blendFileBrowseButton;
        private System.Windows.Forms.ProgressBar renderProgressBar;
        private System.Windows.Forms.NumericUpDown totalStartNumericUpDown;
        private System.Windows.Forms.Label startFrameLabel;
        private System.Windows.Forms.Label chunkEndLabel;
        private System.Windows.Forms.Button outputFolderBrowseButton;
        private System.Windows.Forms.TextBox outputFolderTextBox;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button nextChunkButton;
        private System.Windows.Forms.Button prevChunkButton;
        private System.Windows.Forms.Label totalFrameCountLabel;
        private System.Windows.Forms.NumericUpDown totalEndNumericUpDown;
        private System.Windows.Forms.NumericUpDown processCountNumericUpDown;
        private System.Windows.Forms.Label processCountLabel;
        private System.Windows.Forms.Button renderAllButton;
        private System.Windows.Forms.Button concatenatePartsButton;
		private System.Windows.Forms.Button reloadBlenderDataButton;
		private System.Windows.Forms.Button mixDownButton;
		private System.Windows.Forms.Label totalTimeLabel;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox infoNoScenes;
        private System.Windows.Forms.TextBox infoActiveScene;
        private System.Windows.Forms.ToolTip toolTipWarn;
        private System.Windows.Forms.ToolTip toolTipInfo;
        private System.Windows.Forms.ToolStripMenuItem tipsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator speToolStripMenuItem;
        private System.Windows.Forms.Label blendFileLabel;
        private System.Windows.Forms.NumericUpDown chunkLengthNumericUpDown;
        private System.Windows.Forms.Label chunkLengthLabel;
        private System.Windows.Forms.TextBox infoFramerate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label chunkStartLabel;
        private System.Windows.Forms.TextBox infoDuration;
        private System.Windows.Forms.TextBox infoFramesTotal;
        private System.Windows.Forms.Label infoFramesTotalLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label infoDurationLabel;
        private System.Windows.Forms.TextBox chunkStartNumericUpDown;
        private System.Windows.Forms.TextBox chunkEndNumericUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label timeElapsedLabel;
        private System.Windows.Forms.Panel currentChunkInfoPanel;
        private System.Windows.Forms.Button openOutputFolderButton;
        private System.Windows.Forms.Label blendFileNameLabel;
        private System.Windows.Forms.RadioButton rendererRadioButtonCycles;
        private System.Windows.Forms.RadioButton rendererRadioButtonBlender;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem isti115MenuItem;
        private System.Windows.Forms.ToolStripMenuItem jendabekMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meTwentyFiveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redRaptorMenuItem;
        private System.Windows.Forms.RadioButton startEndBlendRadio;
        private System.Windows.Forms.RadioButton startEndCustomRadio;
        private System.Windows.Forms.Panel infoPanel;
    }
}

