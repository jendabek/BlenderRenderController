using BlenderRenderController.Ui;

namespace BlenderRenderController
{
    partial class BrcForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrcForm));
            this.renderProgressBar = new System.Windows.Forms.ProgressBar();
            this.totalStartNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.blendDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.startFrameLabel = new System.Windows.Forms.Label();
            this.outputFolderTextBox = new System.Windows.Forms.TextBox();
            this.projectSettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.statusLabel = new System.Windows.Forms.Label();
            this.totalFrameCountLabel = new System.Windows.Forms.Label();
            this.totalEndNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.processCountNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.processCountLabel = new System.Windows.Forms.Label();
            this.totalTimeLabel = new System.Windows.Forms.Label();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tipsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearRecentProjectsListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemBug = new System.Windows.Forms.ToolStripMenuItem();
            this.authorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jendabekMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.isti115MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meTwentyFiveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redRaptorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.infoPanel = new System.Windows.Forms.Panel();
            this.infoMore = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.infoDurationLabel = new System.Windows.Forms.Label();
            this.infoResolution = new System.Windows.Forms.TextBox();
            this.infoFramerate = new System.Windows.Forms.TextBox();
            this.infoDuration = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.infoFramesTotal = new System.Windows.Forms.TextBox();
            this.infoActiveScene = new System.Windows.Forms.TextBox();
            this.infoFramesTotalLabel = new System.Windows.Forms.Label();
            this.toolTipWarn = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipInfo = new System.Windows.Forms.ToolTip(this.components);
            this.chunkLengthLabel = new System.Windows.Forms.Label();
            this.chunkLengthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.afterRenderJoinMixdownRadio = new System.Windows.Forms.RadioButton();
            this.afterRenderJoinRadio = new System.Windows.Forms.RadioButton();
            this.afterRenderDoNothingRadio = new System.Windows.Forms.RadioButton();
            this.renderInfoLabel = new System.Windows.Forms.Label();
            this.reloadBlenderDataButton = new System.Windows.Forms.Button();
            this.mixDownButton = new System.Windows.Forms.Button();
            this.concatenatePartsButton = new System.Windows.Forms.Button();
            this.renderAllButton = new System.Windows.Forms.Button();
            this.showRecentBlendsBtn = new System.Windows.Forms.Button();
            this.recentBlendsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.blendFileLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.timeElapsedLabel = new System.Windows.Forms.Label();
            this.blendFileNameLabel = new System.Windows.Forms.Label();
            this.rendererRadioButtonCycles = new System.Windows.Forms.RadioButton();
            this.rendererRadioButtonBlender = new System.Windows.Forms.RadioButton();
            this.startEndBlendRadio = new System.Windows.Forms.RadioButton();
            this.startEndCustomRadio = new System.Windows.Forms.RadioButton();
            this.renderOptionsAutoRadio = new System.Windows.Forms.RadioButton();
            this.renderOptionsCustomRadio = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.versionLabel = new System.Windows.Forms.Label();
            this.donateButton = new System.Windows.Forms.Button();
            this.openOutputFolderButton = new System.Windows.Forms.Button();
            this.outputFolderBrowseButton = new System.Windows.Forms.Button();
            this.ETALabel = new System.Windows.Forms.Label();
            this.ETALabelTitle = new System.Windows.Forms.Label();
            this.blendBrowseBtn = new System.Windows.Forms.Button();
            this.openBlendDialog = new System.Windows.Forms.OpenFileDialog();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.processManager = new System.Windows.Forms.Timer(this.components);
            this.blendNameLabel = new System.Windows.Forms.Label();
            this.afterRenderBGWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.totalStartNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blendDataBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectSettingsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalEndNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.processCountNumericUpDown)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.infoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chunkLengthNumericUpDown)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // renderProgressBar
            // 
            this.renderProgressBar.Location = new System.Drawing.Point(32, 552);
            this.renderProgressBar.MarqueeAnimationSpeed = 75;
            this.renderProgressBar.Name = "renderProgressBar";
            this.renderProgressBar.Size = new System.Drawing.Size(612, 14);
            this.renderProgressBar.Step = 1;
            this.renderProgressBar.TabIndex = 2;
            this.toolTipInfo.SetToolTip(this.renderProgressBar, "Progress bar");
            // 
            // totalStartNumericUpDown
            // 
            this.totalStartNumericUpDown.CausesValidation = false;
            this.totalStartNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.blendDataBindingSource, "Start", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.totalStartNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalStartNumericUpDown.Location = new System.Drawing.Point(136, 237);
            this.totalStartNumericUpDown.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.totalStartNumericUpDown.Name = "totalStartNumericUpDown";
            this.totalStartNumericUpDown.Size = new System.Drawing.Size(90, 22);
            this.totalStartNumericUpDown.TabIndex = 5;
            this.totalStartNumericUpDown.Tag = "DIRENDER";
            this.totalStartNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTipInfo.SetToolTip(this.totalStartNumericUpDown, "You know what it is.");
            this.totalStartNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Enter_GotoNext);
            this.totalStartNumericUpDown.Leave += new System.EventHandler(this.StartEndNumeric_Changed);
            // 
            // blendDataBindingSource
            // 
            this.blendDataBindingSource.DataSource = typeof(BRClib.BlendData);
            // 
            // startFrameLabel
            // 
            this.startFrameLabel.AutoSize = true;
            this.startFrameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startFrameLabel.Location = new System.Drawing.Point(133, 220);
            this.startFrameLabel.Name = "startFrameLabel";
            this.startFrameLabel.Size = new System.Drawing.Size(71, 15);
            this.startFrameLabel.TabIndex = 6;
            this.startFrameLabel.Text = "Start Frame";
            // 
            // outputFolderTextBox
            // 
            this.outputFolderTextBox.BackColor = System.Drawing.Color.White;
            this.outputFolderTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.blendDataBindingSource, "OutputPath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.outputFolderTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputFolderTextBox.Location = new System.Drawing.Point(32, 321);
            this.outputFolderTextBox.Name = "outputFolderTextBox";
            this.outputFolderTextBox.Size = new System.Drawing.Size(427, 22);
            this.outputFolderTextBox.TabIndex = 11;
            this.outputFolderTextBox.Tag = "DIFNL;DIRENDER";
            this.toolTipInfo.SetToolTip(this.outputFolderTextBox, "Path to folder where your video will be rendered to.\r\nIt is automatically set to " +
        ".blend output folder when you open / reload it.");
            this.outputFolderTextBox.WordWrap = false;
            this.outputFolderTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Enter_GotoNext);
            // 
            // projectSettingsBindingSource
            // 
            this.projectSettingsBindingSource.DataSource = typeof(BRClib.ProjectSettings);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.statusLabel.Location = new System.Drawing.Point(32, 571);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(145, 16);
            this.statusLabel.TabIndex = 11;
            this.statusLabel.Text = "Some status message.";
            this.toolTipInfo.SetToolTip(this.statusLabel, "Progress");
            this.statusLabel.Visible = false;
            // 
            // totalFrameCountLabel
            // 
            this.totalFrameCountLabel.AutoSize = true;
            this.totalFrameCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalFrameCountLabel.Location = new System.Drawing.Point(233, 220);
            this.totalFrameCountLabel.Name = "totalFrameCountLabel";
            this.totalFrameCountLabel.Size = new System.Drawing.Size(68, 15);
            this.totalFrameCountLabel.TabIndex = 13;
            this.totalFrameCountLabel.Text = "End Frame";
            // 
            // totalEndNumericUpDown
            // 
            this.totalEndNumericUpDown.CausesValidation = false;
            this.totalEndNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.blendDataBindingSource, "End", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.totalEndNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalEndNumericUpDown.Location = new System.Drawing.Point(236, 237);
            this.totalEndNumericUpDown.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.totalEndNumericUpDown.Name = "totalEndNumericUpDown";
            this.totalEndNumericUpDown.Size = new System.Drawing.Size(90, 22);
            this.totalEndNumericUpDown.TabIndex = 6;
            this.totalEndNumericUpDown.Tag = "";
            this.totalEndNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.totalEndNumericUpDown.Value = new decimal(new int[] {
            123456,
            0,
            0,
            0});
            this.totalEndNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Enter_GotoNext);
            this.totalEndNumericUpDown.Leave += new System.EventHandler(this.StartEndNumeric_Changed);
            // 
            // processCountNumericUpDown
            // 
            this.processCountNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.projectSettingsBindingSource, "ProcessesCount", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.processCountNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processCountNumericUpDown.Location = new System.Drawing.Point(542, 237);
            this.processCountNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.processCountNumericUpDown.Name = "processCountNumericUpDown";
            this.processCountNumericUpDown.Size = new System.Drawing.Size(88, 22);
            this.processCountNumericUpDown.TabIndex = 8;
            this.processCountNumericUpDown.Tag = "DIRENDER";
            this.processCountNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTipInfo.SetToolTip(this.processCountNumericUpDown, "Maximum number of Blender processes that will be parallely rendering your video.\r" +
        "\n\"Auto\" = number of your PC\'s logical processors (threads), this should work the" +
        " best.\r\n\r\n");
            this.processCountNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.processCountNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Enter_GotoNext);
            // 
            // processCountLabel
            // 
            this.processCountLabel.AutoSize = true;
            this.processCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processCountLabel.Location = new System.Drawing.Point(539, 219);
            this.processCountLabel.Name = "processCountLabel";
            this.processCountLabel.Size = new System.Drawing.Size(91, 15);
            this.processCountLabel.TabIndex = 15;
            this.processCountLabel.Text = "Processes Max";
            this.toolTipInfo.SetToolTip(this.processCountLabel, "Maximum number of Blender processes that will be parallely rendering your video.\r" +
        "\n\"Auto\" = number of your logical processors (threads), which should work the bes" +
        "t.\r\n\r\n\r\n");
            // 
            // totalTimeLabel
            // 
            this.totalTimeLabel.AutoSize = true;
            this.totalTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.totalTimeLabel.Location = new System.Drawing.Point(588, 571);
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
            this.tipsToolStripMenuItem,
            this.clearRecentProjectsListToolStripMenuItem});
            this.aboutToolStripMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.ToolTipText = "Open BRC\'s settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // tipsToolStripMenuItem
            // 
            this.tipsToolStripMenuItem.Checked = true;
            this.tipsToolStripMenuItem.CheckOnClick = true;
            this.tipsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tipsToolStripMenuItem.Name = "tipsToolStripMenuItem";
            this.tipsToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.tipsToolStripMenuItem.Text = "Tooltips";
            this.tipsToolStripMenuItem.ToolTipText = "Show/Hide Tooltips";
            this.tipsToolStripMenuItem.Click += new System.EventHandler(this.tipsToolStripMenuItem_Click);
            // 
            // clearRecentProjectsListToolStripMenuItem
            // 
            this.clearRecentProjectsListToolStripMenuItem.Image = global::BlenderRenderController.Properties.Resources.Broom_50;
            this.clearRecentProjectsListToolStripMenuItem.Name = "clearRecentProjectsListToolStripMenuItem";
            this.clearRecentProjectsListToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.clearRecentProjectsListToolStripMenuItem.Text = "Clear recent projects";
            this.clearRecentProjectsListToolStripMenuItem.Click += new System.EventHandler(this.clearRecentProjectsListToolStripMenuItem_Click);
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemBug,
            this.authorsToolStripMenuItem});
            this.infoToolStripMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.infoToolStripMenuItem.Text = "Extras";
            // 
            // toolStripMenuItemBug
            // 
            this.toolStripMenuItemBug.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripMenuItemBug.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.toolStripMenuItemBug.Image = global::BlenderRenderController.Properties.Resources.bug_icon;
            this.toolStripMenuItemBug.Name = "toolStripMenuItemBug";
            this.toolStripMenuItemBug.Size = new System.Drawing.Size(171, 22);
            this.toolStripMenuItemBug.Text = "Report a Bug";
            this.toolStripMenuItemBug.Click += new System.EventHandler(this.toolStripMenuItemBug_Click);
            // 
            // authorsToolStripMenuItem
            // 
            this.authorsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jendabekMenuItem,
            this.isti115MenuItem,
            this.meTwentyFiveMenuItem,
            this.redRaptorMenuItem});
            this.authorsToolStripMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.authorsToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.authorsToolStripMenuItem.Image = global::BlenderRenderController.Properties.Resources.github_logo_small;
            this.authorsToolStripMenuItem.Name = "authorsToolStripMenuItem";
            this.authorsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.authorsToolStripMenuItem.Text = "Authors on Github";
            // 
            // jendabekMenuItem
            // 
            this.jendabekMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.jendabekMenuItem.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.jendabekMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("jendabekMenuItem.Image")));
            this.jendabekMenuItem.Name = "jendabekMenuItem";
            this.jendabekMenuItem.Size = new System.Drawing.Size(152, 22);
            this.jendabekMenuItem.Text = "jendabek";
            this.jendabekMenuItem.Click += new System.EventHandler(this.AuthorLink_Clicked);
            // 
            // isti115MenuItem
            // 
            this.isti115MenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.isti115MenuItem.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.isti115MenuItem.Image = ((System.Drawing.Image)(resources.GetObject("isti115MenuItem.Image")));
            this.isti115MenuItem.Name = "isti115MenuItem";
            this.isti115MenuItem.Size = new System.Drawing.Size(152, 22);
            this.isti115MenuItem.Text = "Isti115";
            this.isti115MenuItem.Click += new System.EventHandler(this.AuthorLink_Clicked);
            // 
            // meTwentyFiveMenuItem
            // 
            this.meTwentyFiveMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.meTwentyFiveMenuItem.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.meTwentyFiveMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("meTwentyFiveMenuItem.Image")));
            this.meTwentyFiveMenuItem.Name = "meTwentyFiveMenuItem";
            this.meTwentyFiveMenuItem.Size = new System.Drawing.Size(152, 22);
            this.meTwentyFiveMenuItem.Text = "MeTwentyFive";
            this.meTwentyFiveMenuItem.Click += new System.EventHandler(this.AuthorLink_Clicked);
            // 
            // redRaptorMenuItem
            // 
            this.redRaptorMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.redRaptorMenuItem.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.redRaptorMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("redRaptorMenuItem.Image")));
            this.redRaptorMenuItem.Name = "redRaptorMenuItem";
            this.redRaptorMenuItem.Size = new System.Drawing.Size(152, 22);
            this.redRaptorMenuItem.Text = "RedRaptor93";
            this.redRaptorMenuItem.Click += new System.EventHandler(this.AuthorLink_Clicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 15);
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
            this.infoPanel.Controls.Add(this.infoMore);
            this.infoPanel.Controls.Add(this.label4);
            this.infoPanel.Controls.Add(this.infoDurationLabel);
            this.infoPanel.Controls.Add(this.infoResolution);
            this.infoPanel.Controls.Add(this.infoFramerate);
            this.infoPanel.Controls.Add(this.infoDuration);
            this.infoPanel.Controls.Add(this.label7);
            this.infoPanel.Controls.Add(this.infoFramesTotal);
            this.infoPanel.Controls.Add(this.infoActiveScene);
            this.infoPanel.Controls.Add(this.label1);
            this.infoPanel.Controls.Add(this.infoFramesTotalLabel);
            this.infoPanel.Location = new System.Drawing.Point(23, 79);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(443, 87);
            this.infoPanel.TabIndex = 22;
            // 
            // infoMore
            // 
            this.infoMore.AutoSize = true;
            this.infoMore.Location = new System.Drawing.Point(387, 64);
            this.infoMore.Name = "infoMore";
            this.infoMore.Size = new System.Drawing.Size(40, 13);
            this.infoMore.TabIndex = 33;
            this.infoMore.TabStop = true;
            this.infoMore.Text = "More...";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(295, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 15);
            this.label4.TabIndex = 33;
            this.label4.Text = "Resolution";
            // 
            // infoDurationLabel
            // 
            this.infoDurationLabel.AutoSize = true;
            this.infoDurationLabel.BackColor = System.Drawing.SystemColors.Info;
            this.infoDurationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoDurationLabel.Location = new System.Drawing.Point(182, 15);
            this.infoDurationLabel.Name = "infoDurationLabel";
            this.infoDurationLabel.Size = new System.Drawing.Size(75, 15);
            this.infoDurationLabel.TabIndex = 31;
            this.infoDurationLabel.Text = "Length Total";
            // 
            // infoResolution
            // 
            this.infoResolution.BackColor = System.Drawing.SystemColors.Control;
            this.infoResolution.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.infoResolution.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.blendDataBindingSource, "Resolution", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, "..."));
            this.infoResolution.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.infoResolution.Location = new System.Drawing.Point(298, 34);
            this.infoResolution.Name = "infoResolution";
            this.infoResolution.ReadOnly = true;
            this.infoResolution.Size = new System.Drawing.Size(77, 20);
            this.infoResolution.TabIndex = 32;
            this.infoResolution.TabStop = false;
            this.infoResolution.Text = "...";
            this.infoResolution.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // infoFramerate
            // 
            this.infoFramerate.BackColor = System.Drawing.SystemColors.Control;
            this.infoFramerate.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.infoFramerate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.blendDataBindingSource, "Fps", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, "...", "###.##"));
            this.infoFramerate.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.infoFramerate.Location = new System.Drawing.Point(378, 34);
            this.infoFramerate.Name = "infoFramerate";
            this.infoFramerate.ReadOnly = true;
            this.infoFramerate.Size = new System.Drawing.Size(49, 20);
            this.infoFramerate.TabIndex = 30;
            this.infoFramerate.TabStop = false;
            this.infoFramerate.Text = "...";
            this.infoFramerate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // infoDuration
            // 
            this.infoDuration.BackColor = System.Drawing.SystemColors.Control;
            this.infoDuration.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.infoDuration.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.blendDataBindingSource, "Duration", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, "..."));
            this.infoDuration.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.infoDuration.Location = new System.Drawing.Point(185, 34);
            this.infoDuration.Name = "infoDuration";
            this.infoDuration.ReadOnly = true;
            this.infoDuration.Size = new System.Drawing.Size(110, 20);
            this.infoDuration.TabIndex = 30;
            this.infoDuration.TabStop = false;
            this.infoDuration.Text = "...";
            this.infoDuration.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(375, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 15);
            this.label7.TabIndex = 29;
            this.label7.Text = "FPS";
            // 
            // infoFramesTotal
            // 
            this.infoFramesTotal.BackColor = System.Drawing.SystemColors.Control;
            this.infoFramesTotal.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.infoFramesTotal.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.blendDataBindingSource, "TotalFrames", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, "...", "# \"f\""));
            this.infoFramesTotal.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.infoFramesTotal.Location = new System.Drawing.Point(108, 34);
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
            this.infoActiveScene.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.blendDataBindingSource, "ActiveScene", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, "..."));
            this.infoActiveScene.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.infoActiveScene.Location = new System.Drawing.Point(11, 34);
            this.infoActiveScene.Name = "infoActiveScene";
            this.infoActiveScene.ReadOnly = true;
            this.infoActiveScene.Size = new System.Drawing.Size(94, 20);
            this.infoActiveScene.TabIndex = 25;
            this.infoActiveScene.TabStop = false;
            this.infoActiveScene.Text = "...";
            this.infoActiveScene.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTipWarn.SetToolTip(this.infoActiveScene, "This program will only render the ACTIVE scene, if you \r\nhave more then one scene" +
        " on your project sure you \r\nsave it with the scene you want OPEN.");
            // 
            // infoFramesTotalLabel
            // 
            this.infoFramesTotalLabel.AutoSize = true;
            this.infoFramesTotalLabel.BackColor = System.Drawing.SystemColors.Info;
            this.infoFramesTotalLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoFramesTotalLabel.Location = new System.Drawing.Point(105, 15);
            this.infoFramesTotalLabel.Name = "infoFramesTotalLabel";
            this.infoFramesTotalLabel.Size = new System.Drawing.Size(62, 15);
            this.infoFramesTotalLabel.TabIndex = 29;
            this.infoFramesTotalLabel.Text = "T. Frames";
            // 
            // toolTipWarn
            // 
            this.toolTipWarn.AutomaticDelay = 1000;
            this.toolTipWarn.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            this.toolTipWarn.ToolTipTitle = "Caution";
            // 
            // toolTipInfo
            // 
            this.toolTipInfo.AutoPopDelay = 32767;
            this.toolTipInfo.InitialDelay = 500;
            this.toolTipInfo.ReshowDelay = 100;
            this.toolTipInfo.ShowAlways = true;
            // 
            // chunkLengthLabel
            // 
            this.chunkLengthLabel.AutoSize = true;
            this.chunkLengthLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chunkLengthLabel.Location = new System.Drawing.Point(445, 220);
            this.chunkLengthLabel.Name = "chunkLengthLabel";
            this.chunkLengthLabel.Size = new System.Drawing.Size(69, 15);
            this.chunkLengthLabel.TabIndex = 13;
            this.chunkLengthLabel.Text = "Chunk Size";
            this.toolTipInfo.SetToolTip(this.chunkLengthLabel, resources.GetString("chunkLengthLabel.ToolTip"));
            // 
            // chunkLengthNumericUpDown
            // 
            this.chunkLengthNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.projectSettingsBindingSource, "ChunkLenght", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chunkLengthNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chunkLengthNumericUpDown.Location = new System.Drawing.Point(448, 238);
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
            this.chunkLengthNumericUpDown.Tag = "DIRENDER";
            this.chunkLengthNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTipInfo.SetToolTip(this.chunkLengthNumericUpDown, resources.GetString("chunkLengthNumericUpDown.ToolTip"));
            this.chunkLengthNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.chunkLengthNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Enter_GotoNext);
            // 
            // afterRenderJoinMixdownRadio
            // 
            this.afterRenderJoinMixdownRadio.AutoSize = true;
            this.afterRenderJoinMixdownRadio.Checked = true;
            this.afterRenderJoinMixdownRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.afterRenderJoinMixdownRadio.Location = new System.Drawing.Point(3, 9);
            this.afterRenderJoinMixdownRadio.Name = "afterRenderJoinMixdownRadio";
            this.afterRenderJoinMixdownRadio.Size = new System.Drawing.Size(283, 19);
            this.afterRenderJoinMixdownRadio.TabIndex = 33;
            this.afterRenderJoinMixdownRadio.TabStop = true;
            this.afterRenderJoinMixdownRadio.Tag = "DIFNL;DIRENDER";
            this.afterRenderJoinMixdownRadio.Text = "Automatically join chunks && use mixdown audio";
            this.toolTipInfo.SetToolTip(this.afterRenderJoinMixdownRadio, resources.GetString("afterRenderJoinMixdownRadio.ToolTip"));
            this.afterRenderJoinMixdownRadio.UseVisualStyleBackColor = true;
            this.afterRenderJoinMixdownRadio.CheckedChanged += new System.EventHandler(this.AfterRenderAction_Changed);
            // 
            // afterRenderJoinRadio
            // 
            this.afterRenderJoinRadio.AutoSize = true;
            this.afterRenderJoinRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.afterRenderJoinRadio.Location = new System.Drawing.Point(3, 31);
            this.afterRenderJoinRadio.Name = "afterRenderJoinRadio";
            this.afterRenderJoinRadio.Size = new System.Drawing.Size(162, 19);
            this.afterRenderJoinRadio.TabIndex = 33;
            this.afterRenderJoinRadio.Tag = "DIFNL;DIRENDER";
            this.afterRenderJoinRadio.Text = "Automatically join chunks";
            this.toolTipInfo.SetToolTip(this.afterRenderJoinRadio, resources.GetString("afterRenderJoinRadio.ToolTip"));
            this.afterRenderJoinRadio.UseVisualStyleBackColor = true;
            // 
            // afterRenderDoNothingRadio
            // 
            this.afterRenderDoNothingRadio.AutoSize = true;
            this.afterRenderDoNothingRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.afterRenderDoNothingRadio.Location = new System.Drawing.Point(3, 53);
            this.afterRenderDoNothingRadio.Name = "afterRenderDoNothingRadio";
            this.afterRenderDoNothingRadio.Size = new System.Drawing.Size(130, 19);
            this.afterRenderDoNothingRadio.TabIndex = 33;
            this.afterRenderDoNothingRadio.Tag = "DIFNL;DIRENDER";
            this.afterRenderDoNothingRadio.Text = "Render just chunks";
            this.toolTipInfo.SetToolTip(this.afterRenderDoNothingRadio, "Will render only chunks.\r\nStill, you can render mixdown separately and join it ma" +
        "nually by buttons on the right.\r\n");
            this.afterRenderDoNothingRadio.UseVisualStyleBackColor = true;
            // 
            // renderInfoLabel
            // 
            this.renderInfoLabel.AutoSize = true;
            this.renderInfoLabel.BackColor = System.Drawing.SystemColors.Control;
            this.renderInfoLabel.Image = global::BlenderRenderController.Properties.Resources.info_icon;
            this.renderInfoLabel.Location = new System.Drawing.Point(198, 478);
            this.renderInfoLabel.Name = "renderInfoLabel";
            this.renderInfoLabel.Padding = new System.Windows.Forms.Padding(8, 3, 8, 3);
            this.renderInfoLabel.Size = new System.Drawing.Size(16, 19);
            this.renderInfoLabel.TabIndex = 35;
            this.toolTipInfo.SetToolTip(this.renderInfoLabel, resources.GetString("renderInfoLabel.ToolTip"));
            // 
            // reloadBlenderDataButton
            // 
            this.reloadBlenderDataButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reloadBlenderDataButton.Image = global::BlenderRenderController.Properties.Resources.reload_icon_small;
            this.reloadBlenderDataButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.reloadBlenderDataButton.Location = new System.Drawing.Point(472, 132);
            this.reloadBlenderDataButton.Name = "reloadBlenderDataButton";
            this.reloadBlenderDataButton.Padding = new System.Windows.Forms.Padding(20, 0, 5, 0);
            this.reloadBlenderDataButton.Size = new System.Drawing.Size(172, 34);
            this.reloadBlenderDataButton.TabIndex = 2;
            this.reloadBlenderDataButton.Tag = "DIFNL;DIRENDER";
            this.reloadBlenderDataButton.Text = "Reload Blend";
            this.toolTipInfo.SetToolTip(this.reloadBlenderDataButton, "Re-reads data from the .blend again and updates the form accordingly.");
            this.reloadBlenderDataButton.UseVisualStyleBackColor = true;
            this.reloadBlenderDataButton.Click += new System.EventHandler(this.reloadBlenderDataButton_Click);
            // 
            // mixDownButton
            // 
            this.mixDownButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mixDownButton.Image = global::BlenderRenderController.Properties.Resources.volume_small;
            this.mixDownButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mixDownButton.Location = new System.Drawing.Point(378, 395);
            this.mixDownButton.Name = "mixDownButton";
            this.mixDownButton.Padding = new System.Windows.Forms.Padding(8, 0, 7, 0);
            this.mixDownButton.Size = new System.Drawing.Size(155, 38);
            this.mixDownButton.TabIndex = 14;
            this.mixDownButton.Tag = "DIFNL;DIRENDER";
            this.mixDownButton.Text = "Render Mixdown";
            this.mixDownButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipInfo.SetToolTip(this.mixDownButton, "Renders an audio file for the final video.");
            this.mixDownButton.UseVisualStyleBackColor = true;
            this.mixDownButton.Click += new System.EventHandler(this.mixDownButton_Click);
            // 
            // concatenatePartsButton
            // 
            this.concatenatePartsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.concatenatePartsButton.Image = global::BlenderRenderController.Properties.Resources.connect_icon_small;
            this.concatenatePartsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.concatenatePartsButton.Location = new System.Drawing.Point(378, 438);
            this.concatenatePartsButton.Name = "concatenatePartsButton";
            this.concatenatePartsButton.Padding = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.concatenatePartsButton.Size = new System.Drawing.Size(128, 38);
            this.concatenatePartsButton.TabIndex = 15;
            this.concatenatePartsButton.Tag = "DIRENDER";
            this.concatenatePartsButton.Text = "Join Chunks";
            this.concatenatePartsButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipInfo.SetToolTip(this.concatenatePartsButton, "!Warning - previous video will be overwritten!\r\n\r\nJoins rendered chunks to get th" +
        "e final video.\r\nMixdown audio will be used if it is rendered (and found in the o" +
        "utput folder).");
            this.concatenatePartsButton.UseVisualStyleBackColor = true;
            this.concatenatePartsButton.Click += new System.EventHandler(this.concatenatePartsButton_Click);
            // 
            // renderAllButton
            // 
            this.renderAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.renderAllButton.Image = global::BlenderRenderController.Properties.Resources.render_icon_small;
            this.renderAllButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.renderAllButton.Location = new System.Drawing.Point(32, 478);
            this.renderAllButton.Name = "renderAllButton";
            this.renderAllButton.Padding = new System.Windows.Forms.Padding(10, 0, 12, 0);
            this.renderAllButton.Size = new System.Drawing.Size(160, 47);
            this.renderAllButton.TabIndex = 13;
            this.renderAllButton.Tag = "DIFNL";
            this.renderAllButton.Text = "Start Render";
            this.renderAllButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipInfo.SetToolTip(this.renderAllButton, "Saves a lot of your time.");
            this.renderAllButton.UseVisualStyleBackColor = true;
            this.renderAllButton.Click += new System.EventHandler(this.renderAllButton_Click);
            // 
            // showRecentBlendsBtn
            // 
            this.showRecentBlendsBtn.ContextMenuStrip = this.recentBlendsMenu;
            this.showRecentBlendsBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showRecentBlendsBtn.Location = new System.Drawing.Point(619, 79);
            this.showRecentBlendsBtn.Name = "showRecentBlendsBtn";
            this.showRecentBlendsBtn.Size = new System.Drawing.Size(27, 44);
            this.showRecentBlendsBtn.TabIndex = 38;
            this.showRecentBlendsBtn.Tag = "DIRENDER";
            this.showRecentBlendsBtn.Text = "˅";
            this.toolTipInfo.SetToolTip(this.showRecentBlendsBtn, "Recent .blend files");
            this.showRecentBlendsBtn.UseVisualStyleBackColor = true;
            this.showRecentBlendsBtn.Click += new System.EventHandler(this.showRecentBlendsBtn_Click);
            // 
            // recentBlendsMenu
            // 
            this.recentBlendsMenu.Name = "recentBlendsMenu";
            this.recentBlendsMenu.Size = new System.Drawing.Size(61, 4);
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Underline);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(19, 289);
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
            this.label10.Location = new System.Drawing.Point(19, 367);
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
            this.label12.Location = new System.Drawing.Point(342, 186);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(144, 20);
            this.label12.TabIndex = 25;
            this.label12.Text = "3. Render Options";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Underline);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label5.Location = new System.Drawing.Point(374, 367);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 20);
            this.label5.TabIndex = 25;
            this.label5.Text = "Extras";
            // 
            // timeElapsedLabel
            // 
            this.timeElapsedLabel.AutoSize = true;
            this.timeElapsedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeElapsedLabel.Location = new System.Drawing.Point(492, 571);
            this.timeElapsedLabel.Name = "timeElapsedLabel";
            this.timeElapsedLabel.Size = new System.Drawing.Size(96, 16);
            this.timeElapsedLabel.TabIndex = 19;
            this.timeElapsedLabel.Text = "Time Elapsed:";
            // 
            // blendFileNameLabel
            // 
            this.blendFileNameLabel.AutoSize = true;
            this.blendFileNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blendFileNameLabel.ForeColor = System.Drawing.Color.Black;
            this.blendFileNameLabel.Location = new System.Drawing.Point(20, 43);
            this.blendFileNameLabel.Name = "blendFileNameLabel";
            this.blendFileNameLabel.Size = new System.Drawing.Size(0, 25);
            this.blendFileNameLabel.TabIndex = 25;
            // 
            // rendererRadioButtonCycles
            // 
            this.rendererRadioButtonCycles.AutoSize = true;
            this.rendererRadioButtonCycles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rendererRadioButtonCycles.Location = new System.Drawing.Point(478, 268);
            this.rendererRadioButtonCycles.Name = "rendererRadioButtonCycles";
            this.rendererRadioButtonCycles.Size = new System.Drawing.Size(60, 19);
            this.rendererRadioButtonCycles.TabIndex = 10;
            this.rendererRadioButtonCycles.Tag = "DIRENDER";
            this.rendererRadioButtonCycles.Text = "Cycles";
            this.rendererRadioButtonCycles.UseVisualStyleBackColor = true;
            this.rendererRadioButtonCycles.Visible = false;
            this.rendererRadioButtonCycles.CheckedChanged += new System.EventHandler(this.RendererType_RadioChanged);
            // 
            // rendererRadioButtonBlender
            // 
            this.rendererRadioButtonBlender.AutoSize = true;
            this.rendererRadioButtonBlender.Checked = true;
            this.rendererRadioButtonBlender.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rendererRadioButtonBlender.Location = new System.Drawing.Point(364, 268);
            this.rendererRadioButtonBlender.Name = "rendererRadioButtonBlender";
            this.rendererRadioButtonBlender.Size = new System.Drawing.Size(112, 19);
            this.rendererRadioButtonBlender.TabIndex = 9;
            this.rendererRadioButtonBlender.TabStop = true;
            this.rendererRadioButtonBlender.Tag = "DIRENDER";
            this.rendererRadioButtonBlender.Text = "Blender Render";
            this.rendererRadioButtonBlender.UseVisualStyleBackColor = true;
            this.rendererRadioButtonBlender.Visible = false;
            this.rendererRadioButtonBlender.CheckedChanged += new System.EventHandler(this.RendererType_RadioChanged);
            // 
            // startEndBlendRadio
            // 
            this.startEndBlendRadio.AutoSize = true;
            this.startEndBlendRadio.Checked = true;
            this.startEndBlendRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startEndBlendRadio.Location = new System.Drawing.Point(3, 3);
            this.startEndBlendRadio.Name = "startEndBlendRadio";
            this.startEndBlendRadio.Size = new System.Drawing.Size(89, 19);
            this.startEndBlendRadio.TabIndex = 3;
            this.startEndBlendRadio.TabStop = true;
            this.startEndBlendRadio.Tag = "DIFNL;DIRENDER";
            this.startEndBlendRadio.Text = "From Blend";
            this.startEndBlendRadio.UseVisualStyleBackColor = true;
            this.startEndBlendRadio.CheckedChanged += new System.EventHandler(this.AutoOptionsRadio_CheckedChanged);
            // 
            // startEndCustomRadio
            // 
            this.startEndCustomRadio.AutoSize = true;
            this.startEndCustomRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startEndCustomRadio.Location = new System.Drawing.Point(3, 25);
            this.startEndCustomRadio.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.startEndCustomRadio.Name = "startEndCustomRadio";
            this.startEndCustomRadio.Size = new System.Drawing.Size(67, 19);
            this.startEndCustomRadio.TabIndex = 4;
            this.startEndCustomRadio.Tag = "DIFNL;DIRENDER";
            this.startEndCustomRadio.Text = "Custom";
            this.startEndCustomRadio.UseVisualStyleBackColor = true;
            // 
            // renderOptionsAutoRadio
            // 
            this.renderOptionsAutoRadio.AutoSize = true;
            this.renderOptionsAutoRadio.Checked = true;
            this.renderOptionsAutoRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.renderOptionsAutoRadio.Location = new System.Drawing.Point(3, 0);
            this.renderOptionsAutoRadio.Name = "renderOptionsAutoRadio";
            this.renderOptionsAutoRadio.Size = new System.Drawing.Size(49, 19);
            this.renderOptionsAutoRadio.TabIndex = 9;
            this.renderOptionsAutoRadio.TabStop = true;
            this.renderOptionsAutoRadio.Tag = "DIFNL;DIRENDER";
            this.renderOptionsAutoRadio.Text = "Auto";
            this.renderOptionsAutoRadio.UseVisualStyleBackColor = true;
            this.renderOptionsAutoRadio.CheckedChanged += new System.EventHandler(this.AutoOptionsRadio_CheckedChanged);
            // 
            // renderOptionsCustomRadio
            // 
            this.renderOptionsCustomRadio.AutoSize = true;
            this.renderOptionsCustomRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.renderOptionsCustomRadio.Location = new System.Drawing.Point(3, 21);
            this.renderOptionsCustomRadio.Name = "renderOptionsCustomRadio";
            this.renderOptionsCustomRadio.Size = new System.Drawing.Size(67, 19);
            this.renderOptionsCustomRadio.TabIndex = 10;
            this.renderOptionsCustomRadio.Tag = "DIFNL;DIRENDER";
            this.renderOptionsCustomRadio.Text = "Custom";
            this.renderOptionsCustomRadio.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.renderOptionsAutoRadio);
            this.panel1.Controls.Add(this.renderOptionsCustomRadio);
            this.panel1.Location = new System.Drawing.Point(361, 221);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(79, 47);
            this.panel1.TabIndex = 32;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.afterRenderDoNothingRadio);
            this.panel2.Controls.Add(this.afterRenderJoinMixdownRadio);
            this.panel2.Controls.Add(this.afterRenderJoinRadio);
            this.panel2.Location = new System.Drawing.Point(32, 395);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(328, 83);
            this.panel2.TabIndex = 34;
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.versionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionLabel.Location = new System.Drawing.Point(605, 4);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(39, 15);
            this.versionLabel.TabIndex = 36;
            this.versionLabel.Text = "v0.0.0";
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // donateButton
            // 
            this.donateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.donateButton.Image = global::BlenderRenderController.Properties.Resources.donate_icon;
            this.donateButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.donateButton.Location = new System.Drawing.Point(128, 0);
            this.donateButton.Name = "donateButton";
            this.donateButton.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.donateButton.Size = new System.Drawing.Size(90, 24);
            this.donateButton.TabIndex = 2;
            this.donateButton.Text = "Donate";
            this.donateButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.donateButton.UseVisualStyleBackColor = true;
            // 
            // openOutputFolderButton
            // 
            this.openOutputFolderButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.openOutputFolderButton.Image = global::BlenderRenderController.Properties.Resources.folder_icon_smaller;
            this.openOutputFolderButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.openOutputFolderButton.Location = new System.Drawing.Point(564, 319);
            this.openOutputFolderButton.Name = "openOutputFolderButton";
            this.openOutputFolderButton.Padding = new System.Windows.Forms.Padding(8, 0, 5, 0);
            this.openOutputFolderButton.Size = new System.Drawing.Size(80, 25);
            this.openOutputFolderButton.TabIndex = 16;
            this.openOutputFolderButton.Tag = "DIFNL";
            this.openOutputFolderButton.Text = "Open";
            this.openOutputFolderButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.openOutputFolderButton.UseVisualStyleBackColor = true;
            this.openOutputFolderButton.Click += new System.EventHandler(this.openOutputFolderButton_Click);
            // 
            // outputFolderBrowseButton
            // 
            this.outputFolderBrowseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputFolderBrowseButton.Image = global::BlenderRenderController.Properties.Resources.folder_icon_smaller;
            this.outputFolderBrowseButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.outputFolderBrowseButton.Location = new System.Drawing.Point(465, 319);
            this.outputFolderBrowseButton.Name = "outputFolderBrowseButton";
            this.outputFolderBrowseButton.Padding = new System.Windows.Forms.Padding(8, 0, 7, 1);
            this.outputFolderBrowseButton.Size = new System.Drawing.Size(95, 25);
            this.outputFolderBrowseButton.TabIndex = 12;
            this.outputFolderBrowseButton.Tag = "DIFNL;DIRENDER";
            this.outputFolderBrowseButton.Text = "  Change";
            this.outputFolderBrowseButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.outputFolderBrowseButton.UseVisualStyleBackColor = true;
            this.outputFolderBrowseButton.Click += new System.EventHandler(this.outputFolderBrowseButton_Click);
            // 
            // ETALabel
            // 
            this.ETALabel.AutoSize = true;
            this.ETALabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.ETALabel.Location = new System.Drawing.Point(588, 587);
            this.ETALabel.Name = "ETALabel";
            this.ETALabel.Size = new System.Drawing.Size(56, 16);
            this.ETALabel.TabIndex = 19;
            this.ETALabel.Text = "00:00:00";
            // 
            // ETALabelTitle
            // 
            this.ETALabelTitle.AutoSize = true;
            this.ETALabelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ETALabelTitle.Location = new System.Drawing.Point(550, 587);
            this.ETALabelTitle.Name = "ETALabelTitle";
            this.ETALabelTitle.Size = new System.Drawing.Size(38, 16);
            this.ETALabelTitle.TabIndex = 19;
            this.ETALabelTitle.Text = "ETA:";
            // 
            // blendBrowseBtn
            // 
            this.blendBrowseBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blendBrowseBtn.Image = global::BlenderRenderController.Properties.Resources.blend_icon;
            this.blendBrowseBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.blendBrowseBtn.Location = new System.Drawing.Point(472, 79);
            this.blendBrowseBtn.Name = "blendBrowseBtn";
            this.blendBrowseBtn.Padding = new System.Windows.Forms.Padding(3, 5, 0, 0);
            this.blendBrowseBtn.Size = new System.Drawing.Size(145, 44);
            this.blendBrowseBtn.TabIndex = 37;
            this.blendBrowseBtn.Tag = "DIRENDER";
            this.blendBrowseBtn.Text = " Open Blend";
            this.blendBrowseBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.blendBrowseBtn.UseVisualStyleBackColor = true;
            this.blendBrowseBtn.Click += new System.EventHandler(this.blendBrowseBtn_Click);
            // 
            // openBlendDialog
            // 
            this.openBlendDialog.Filter = "Blend|*.blend";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.startEndBlendRadio);
            this.flowLayoutPanel1.Controls.Add(this.startEndCustomRadio);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(27, 220);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(99, 47);
            this.flowLayoutPanel1.TabIndex = 39;
            // 
            // processManager
            // 
            this.processManager.Interval = 50;
            this.processManager.Tick += new System.EventHandler(this.UpdateProcessManagement);
            // 
            // blendNameLabel
            // 
            this.blendNameLabel.AutoSize = true;
            this.blendNameLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.blendDataBindingSource, "ProjectName", true));
            this.blendNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Bold);
            this.blendNameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.blendNameLabel.Location = new System.Drawing.Point(132, 43);
            this.blendNameLabel.Name = "blendNameLabel";
            this.blendNameLabel.Size = new System.Drawing.Size(105, 20);
            this.blendNameLabel.TabIndex = 40;
            this.blendNameLabel.Text = "blend name";
            this.blendNameLabel.Visible = false;
            // 
            // afterRenderBGWorker
            // 
            this.afterRenderBGWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.AfterRenderBGWorker_DoWork);
            this.afterRenderBGWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.AfterRenderBGWorker_RunWorkerCompleted);
            // 
            // BrcForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(662, 621);
            this.Controls.Add(this.blendNameLabel);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.showRecentBlendsBtn);
            this.Controls.Add(this.blendBrowseBtn);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.renderInfoLabel);
            this.Controls.Add(this.rendererRadioButtonBlender);
            this.Controls.Add(this.rendererRadioButtonCycles);
            this.Controls.Add(this.blendFileNameLabel);
            this.Controls.Add(this.blendFileLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.renderAllButton);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.infoPanel);
            this.Controls.Add(this.ETALabelTitle);
            this.Controls.Add(this.timeElapsedLabel);
            this.Controls.Add(this.ETALabel);
            this.Controls.Add(this.totalTimeLabel);
            this.Controls.Add(this.donateButton);
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
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(678, 660);
            this.MinimumSize = new System.Drawing.Size(678, 660);
            this.Name = "BrcForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Blender Render Controller";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BrcForm_FormClosing);
            this.Load += new System.EventHandler(this.BrcForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.totalStartNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blendDataBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectSettingsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalEndNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.processCountNumericUpDown)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.infoPanel.ResumeLayout(false);
            this.infoPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chunkLengthNumericUpDown)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ProgressBar renderProgressBar;
        private System.Windows.Forms.NumericUpDown totalStartNumericUpDown;
        private System.Windows.Forms.Label startFrameLabel;
        private System.Windows.Forms.Button outputFolderBrowseButton;
        private System.Windows.Forms.TextBox outputFolderTextBox;
        private System.Windows.Forms.Label statusLabel;
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
        private System.Windows.Forms.TextBox infoActiveScene;
        private System.Windows.Forms.ToolTip toolTipWarn;
        private System.Windows.Forms.ToolTip toolTipInfo;
        private System.Windows.Forms.ToolStripMenuItem tipsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.Label blendFileLabel;
        private System.Windows.Forms.NumericUpDown chunkLengthNumericUpDown;
        private System.Windows.Forms.Label chunkLengthLabel;
        private System.Windows.Forms.TextBox infoFramerate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox infoDuration;
        private System.Windows.Forms.TextBox infoFramesTotal;
        private System.Windows.Forms.Label infoFramesTotalLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label infoDurationLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label timeElapsedLabel;
        private System.Windows.Forms.Button openOutputFolderButton;
        private System.Windows.Forms.Label blendFileNameLabel;
        private System.Windows.Forms.RadioButton rendererRadioButtonCycles;
        private System.Windows.Forms.RadioButton rendererRadioButtonBlender;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem authorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem isti115MenuItem;
        private System.Windows.Forms.ToolStripMenuItem jendabekMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meTwentyFiveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redRaptorMenuItem;
        private System.Windows.Forms.RadioButton startEndBlendRadio;
        private System.Windows.Forms.RadioButton startEndCustomRadio;
        private System.Windows.Forms.Panel infoPanel;
        private System.Windows.Forms.RadioButton renderOptionsAutoRadio;
        private System.Windows.Forms.RadioButton renderOptionsCustomRadio;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton afterRenderJoinMixdownRadio;
        private System.Windows.Forms.RadioButton afterRenderJoinRadio;
        private System.Windows.Forms.RadioButton afterRenderDoNothingRadio;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button donateButton;
        private System.Windows.Forms.Label renderInfoLabel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBug;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.TextBox infoResolution;
        private System.Windows.Forms.LinkLabel infoMore;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem clearRecentProjectsListToolStripMenuItem;
        private System.Windows.Forms.Label ETALabel;
        private System.Windows.Forms.Label ETALabelTitle;
        private System.Windows.Forms.Button showRecentBlendsBtn;
        private System.Windows.Forms.Button blendBrowseBtn;
        private System.Windows.Forms.ContextMenuStrip recentBlendsMenu;
        private System.Windows.Forms.OpenFileDialog openBlendDialog;
        private System.Windows.Forms.BindingSource projectSettingsBindingSource;
        private System.Windows.Forms.BindingSource blendDataBindingSource;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Timer processManager;
        private System.Windows.Forms.Label blendNameLabel;
        private System.ComponentModel.BackgroundWorker afterRenderBGWorker;
    }
}

