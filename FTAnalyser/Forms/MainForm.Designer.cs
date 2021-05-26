using System;

namespace FTAnalyzer
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
            try
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
                handwritingFont.Dispose();
                fonts.Dispose();
                rfhDuplicates.Dispose();
                if (cts!=null)
                    cts.Dispose();
                storedCursor.Dispose();
            }
            catch (Exception) { }
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.openGedcom = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReload = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPrint = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuRecent = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRecent1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRecent2 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRecent3 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRecent4 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRecent5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.clearRecentFileListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCloseGEDCOM = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.displayOptionsOnLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToDefaultFormSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewOnlineManualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlineGuidesToUsingFTAnalyzerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportAnIssueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.privacyPolicyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.whatsNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuSetRoot = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setAsRootToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewNotesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tsCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsHintsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tspbTabProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.tsStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.dgDataErrors = new FTAnalyzer.Forms.Controls.VirtualDGVDataErrors();
            this.tbDuplicateScore = new System.Windows.Forms.TrackBar();
            this.dgCheckAncestors = new System.Windows.Forms.DataGridView();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.printPreviewDialog = new System.Windows.Forms.PrintPreviewDialog();
            this.printDialog = new System.Windows.Forms.PrintDialog();
            this.printDocument = new System.Drawing.Printing.PrintDocument();
            this.ctxViewNotes = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuViewNotes = new System.Windows.Forms.ToolStripMenuItem();
            this.ckbTTIncludeOnlyOneParent = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtTreetopsSurname = new System.Windows.Forms.TextBox();
            this.treetopsRelation = new FTAnalyzer.Forms.Controls.RelationTypes();
            this.Referrals = new System.Windows.Forms.GroupBox();
            this.ckbReferralInCommon = new System.Windows.Forms.CheckBox();
            this.btnReferrals = new System.Windows.Forms.Button();
            this.cmbReferrals = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.btnViewInvalidRefs = new System.Windows.Forms.Button();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.rtbCheckAncestors = new FTAnalyzer.Utilities.ScrollingRichTextBox();
            this.btnCheckMyAncestors = new System.Windows.Forms.Button();
            this.lblCheckAncestors = new System.Windows.Forms.Label();
            this.txtAliveDates = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.btnRandomSurnameEntered = new System.Windows.Forms.Button();
            this.btnRandomSurnameMissing = new System.Windows.Forms.Button();
            this.chkExcludeUnknownBirths = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCensusSurname = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.udAgeFilter = new System.Windows.Forms.NumericUpDown();
            this.relTypesCensus = new FTAnalyzer.Forms.Controls.RelationTypes();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnMismatchedChildrenStatus = new System.Windows.Forms.Button();
            this.btnNoChildrenStatus = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnReportUnrecognised = new System.Windows.Forms.Button();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.tabDisplayProgress = new System.Windows.Forms.TabPage();
            this.splitGedcom = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.LbProgramName = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.pbRelationships = new System.Windows.Forms.ProgressBar();
            this.label6 = new System.Windows.Forms.Label();
            this.pbFamilies = new System.Windows.Forms.ProgressBar();
            this.label5 = new System.Windows.Forms.Label();
            this.pbIndividuals = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.pbSources = new System.Windows.Forms.ProgressBar();
            this.rtbOutput = new FTAnalyzer.Utilities.ScrollingRichTextBox();
            this.tabSelector = new System.Windows.Forms.TabControl();
            this.tabMainLists = new System.Windows.Forms.TabPage();
            this.tabMainListsSelector = new System.Windows.Forms.TabControl();
            this.tabIndividuals = new System.Windows.Forms.TabPage();
            this.dgIndividuals = new FTAnalyzer.Forms.Controls.VirtualDGVIndividuals();
            this.tabFamilies = new System.Windows.Forms.TabPage();
            this.dgFamilies = new FTAnalyzer.Forms.Controls.VirtualDGVFamily();
            this.tabSources = new System.Windows.Forms.TabPage();
            this.dgSources = new FTAnalyzer.Forms.Controls.VirtualDGVSources();
            this.tabErrorsFixes = new System.Windows.Forms.TabPage();
            this.tabErrorFixSelector = new System.Windows.Forms.TabControl();
            this.tabDataErrors = new System.Windows.Forms.TabPage();
            this.gbDataErrorTypes = new System.Windows.Forms.GroupBox();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.ckbDataErrors = new System.Windows.Forms.CheckedListBox();
            this.tabDuplicates = new System.Windows.Forms.TabPage();
            this.labDuplicateSlider = new System.Windows.Forms.Label();
            this.labCompletion = new System.Windows.Forms.Label();
            this.ckbHideIgnoredDuplicates = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.labCalcDuplicates = new System.Windows.Forms.Label();
            this.pbDuplicates = new System.Windows.Forms.ProgressBar();
            this.dgDuplicates = new FTAnalyzer.Forms.Controls.VirtualDGVDuplicates();
            this.btnCancelDuplicates = new System.Windows.Forms.Button();
            this.tabLooseBirths = new System.Windows.Forms.TabPage();
            this.dgLooseBirths = new System.Windows.Forms.DataGridView();
            this.tabLooseDeaths = new System.Windows.Forms.TabPage();
            this.dgLooseDeaths = new System.Windows.Forms.DataGridView();
            this.tabLooseInfo = new System.Windows.Forms.TabPage();
            this.dgLooseInfo = new System.Windows.Forms.DataGridView();
            this.NonDuplicate = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Score = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DuplicateIndividualID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DuplicateName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DuplicateForenames = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DuplicateSurname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DuplicateBirthDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DuplicateBirthLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MatchIndividualID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MatchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MatchBirthDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MatchBirthLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.saveDatabase = new System.Windows.Forms.SaveFileDialog();
            this.restoreDatabase = new System.Windows.Forms.OpenFileDialog();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.mnuSetRoot.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDataErrors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDuplicateScore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgCheckAncestors)).BeginInit();
            this.ctxViewNotes.SuspendLayout();
            this.Referrals.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udAgeFilter)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabDisplayProgress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitGedcom)).BeginInit();
            this.splitGedcom.Panel1.SuspendLayout();
            this.splitGedcom.Panel2.SuspendLayout();
            this.splitGedcom.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabSelector.SuspendLayout();
            this.tabMainLists.SuspendLayout();
            this.tabMainListsSelector.SuspendLayout();
            this.tabIndividuals.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgIndividuals)).BeginInit();
            this.tabFamilies.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgFamilies)).BeginInit();
            this.tabSources.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgSources)).BeginInit();
            this.tabErrorsFixes.SuspendLayout();
            this.tabErrorFixSelector.SuspendLayout();
            this.tabDataErrors.SuspendLayout();
            this.gbDataErrorTypes.SuspendLayout();
            this.tabDuplicates.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDuplicates)).BeginInit();
            this.tabLooseBirths.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgLooseBirths)).BeginInit();
            this.tabLooseDeaths.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgLooseDeaths)).BeginInit();
            this.tabLooseInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgLooseInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // openGedcom
            // 
            this.openGedcom.FileName = "openFileDialog1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(3, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(1104, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripMenuItem1,
            this.mnuReload,
            this.mnuPrint,
            this.toolStripSeparator6,
            this.mnuRecent,
            this.toolStripSeparator3,
            this.toolStripSeparator5,
            this.mnuCloseGEDCOM,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.openToolStripMenuItem.Text = "Open GEDCOM file...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // mnuReload
            // 
            this.mnuReload.Enabled = false;
            this.mnuReload.Name = "mnuReload";
            this.mnuReload.Size = new System.Drawing.Size(184, 22);
            this.mnuReload.Text = "Reload";
            this.mnuReload.Click += new System.EventHandler(this.ReloadToolStripMenuItem_Click);
            // 
            // mnuPrint
            // 
            this.mnuPrint.Enabled = false;
            this.mnuPrint.Name = "mnuPrint";
            this.mnuPrint.Size = new System.Drawing.Size(184, 22);
            this.mnuPrint.Text = "Print";
            this.mnuPrint.Click += new System.EventHandler(this.MnuPrint_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(181, 6);
            // 
            // mnuRecent
            // 
            this.mnuRecent.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRecent1,
            this.mnuRecent2,
            this.mnuRecent3,
            this.mnuRecent4,
            this.mnuRecent5,
            this.toolStripSeparator7,
            this.clearRecentFileListToolStripMenuItem});
            this.mnuRecent.Name = "mnuRecent";
            this.mnuRecent.Size = new System.Drawing.Size(184, 22);
            this.mnuRecent.Text = "Recent Files";
            this.mnuRecent.DropDownOpening += new System.EventHandler(this.MnuRecent_DropDownOpening);
            // 
            // mnuRecent1
            // 
            this.mnuRecent1.Name = "mnuRecent1";
            this.mnuRecent1.Size = new System.Drawing.Size(182, 22);
            this.mnuRecent1.Text = "1.";
            this.mnuRecent1.Click += new System.EventHandler(this.OpenRecentFile_Click);
            // 
            // mnuRecent2
            // 
            this.mnuRecent2.Name = "mnuRecent2";
            this.mnuRecent2.Size = new System.Drawing.Size(182, 22);
            this.mnuRecent2.Text = "2.";
            this.mnuRecent2.Click += new System.EventHandler(this.OpenRecentFile_Click);
            // 
            // mnuRecent3
            // 
            this.mnuRecent3.Name = "mnuRecent3";
            this.mnuRecent3.Size = new System.Drawing.Size(182, 22);
            this.mnuRecent3.Text = "3.";
            this.mnuRecent3.Click += new System.EventHandler(this.OpenRecentFile_Click);
            // 
            // mnuRecent4
            // 
            this.mnuRecent4.Name = "mnuRecent4";
            this.mnuRecent4.Size = new System.Drawing.Size(182, 22);
            this.mnuRecent4.Text = "4.";
            this.mnuRecent4.Click += new System.EventHandler(this.OpenRecentFile_Click);
            // 
            // mnuRecent5
            // 
            this.mnuRecent5.Name = "mnuRecent5";
            this.mnuRecent5.Size = new System.Drawing.Size(182, 22);
            this.mnuRecent5.Text = "5.";
            this.mnuRecent5.Click += new System.EventHandler(this.OpenRecentFile_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(179, 6);
            // 
            // clearRecentFileListToolStripMenuItem
            // 
            this.clearRecentFileListToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("clearRecentFileListToolStripMenuItem.Image")));
            this.clearRecentFileListToolStripMenuItem.Name = "clearRecentFileListToolStripMenuItem";
            this.clearRecentFileListToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.clearRecentFileListToolStripMenuItem.Text = "Clear Recent File List";
            this.clearRecentFileListToolStripMenuItem.Click += new System.EventHandler(this.ClearRecentFileListToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(181, 6);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(234, 6);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(181, 6);
            // 
            // mnuCloseGEDCOM
            // 
            this.mnuCloseGEDCOM.Name = "mnuCloseGEDCOM";
            this.mnuCloseGEDCOM.Size = new System.Drawing.Size(184, 22);
            this.mnuCloseGEDCOM.Text = "Close GEDCOM file";
            this.mnuCloseGEDCOM.Click += new System.EventHandler(this.MnuCloseGEDCOM_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.toolStripSeparator2,
            this.displayOptionsOnLoadToolStripMenuItem,
            this.resetToDefaultFormSizeToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 22);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.OptionsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(205, 6);
            // 
            // displayOptionsOnLoadToolStripMenuItem
            // 
            this.displayOptionsOnLoadToolStripMenuItem.CheckOnClick = true;
            this.displayOptionsOnLoadToolStripMenuItem.Name = "displayOptionsOnLoadToolStripMenuItem";
            this.displayOptionsOnLoadToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.displayOptionsOnLoadToolStripMenuItem.Text = "Display Options on Load";
            this.displayOptionsOnLoadToolStripMenuItem.Click += new System.EventHandler(this.DisplayOptionsOnLoadToolStripMenuItem_Click);
            // 
            // resetToDefaultFormSizeToolStripMenuItem
            // 
            this.resetToDefaultFormSizeToolStripMenuItem.Name = "resetToDefaultFormSizeToolStripMenuItem";
            this.resetToDefaultFormSizeToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.resetToDefaultFormSizeToolStripMenuItem.Text = "Reset to Default form size";
            this.resetToDefaultFormSizeToolStripMenuItem.Click += new System.EventHandler(this.ResetToDefaultFormSizeToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewOnlineManualToolStripMenuItem,
            this.onlineGuidesToUsingFTAnalyzerToolStripMenuItem,
            this.reportAnIssueToolStripMenuItem,
            this.toolStripSeparator1,
            this.privacyPolicyToolStripMenuItem,
            this.whatsNewToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // viewOnlineManualToolStripMenuItem
            // 
            this.viewOnlineManualToolStripMenuItem.Name = "viewOnlineManualToolStripMenuItem";
            this.viewOnlineManualToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.viewOnlineManualToolStripMenuItem.Text = "View Online Manual";
            this.viewOnlineManualToolStripMenuItem.Click += new System.EventHandler(this.ViewOnlineManualToolStripMenuItem_Click);
            // 
            // onlineGuidesToUsingFTAnalyzerToolStripMenuItem
            // 
            this.onlineGuidesToUsingFTAnalyzerToolStripMenuItem.Name = "onlineGuidesToUsingFTAnalyzerToolStripMenuItem";
            this.onlineGuidesToUsingFTAnalyzerToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.onlineGuidesToUsingFTAnalyzerToolStripMenuItem.Text = "Online Guides to Using FTAnalyzer";
            this.onlineGuidesToUsingFTAnalyzerToolStripMenuItem.Click += new System.EventHandler(this.OnlineGuidesToUsingFTAnalyzerToolStripMenuItem_Click);
            // 
            // reportAnIssueToolStripMenuItem
            // 
            this.reportAnIssueToolStripMenuItem.Name = "reportAnIssueToolStripMenuItem";
            this.reportAnIssueToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.reportAnIssueToolStripMenuItem.Text = "Report an Issue";
            this.reportAnIssueToolStripMenuItem.Click += new System.EventHandler(this.ReportAnIssueToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(251, 6);
            // 
            // privacyPolicyToolStripMenuItem
            // 
            this.privacyPolicyToolStripMenuItem.Name = "privacyPolicyToolStripMenuItem";
            this.privacyPolicyToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.privacyPolicyToolStripMenuItem.Text = "Privacy Policy";
            this.privacyPolicyToolStripMenuItem.Click += new System.EventHandler(this.PrivacyPolicyToolStripMenuItem_Click);
            // 
            // whatsNewToolStripMenuItem
            // 
            this.whatsNewToolStripMenuItem.Name = "whatsNewToolStripMenuItem";
            this.whatsNewToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.whatsNewToolStripMenuItem.Text = "What\'s New";
            this.whatsNewToolStripMenuItem.Click += new System.EventHandler(this.WhatsNewToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(267, 6);
            // 
            // mnuSetRoot
            // 
            this.mnuSetRoot.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.mnuSetRoot.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setAsRootToolStripMenuItem,
            this.viewNotesToolStripMenuItem});
            this.mnuSetRoot.Name = "mnuSetRoot";
            this.mnuSetRoot.Size = new System.Drawing.Size(174, 48);
            this.mnuSetRoot.Opened += new System.EventHandler(this.MnuSetRoot_Opened);
            // 
            // setAsRootToolStripMenuItem
            // 
            this.setAsRootToolStripMenuItem.Name = "setAsRootToolStripMenuItem";
            this.setAsRootToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.setAsRootToolStripMenuItem.Text = "Set As Root Person";
            this.setAsRootToolStripMenuItem.Click += new System.EventHandler(this.SetAsRootToolStripMenuItem_Click);
            // 
            // viewNotesToolStripMenuItem
            // 
            this.viewNotesToolStripMenuItem.Name = "viewNotesToolStripMenuItem";
            this.viewNotesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.viewNotesToolStripMenuItem.Text = "View Notes";
            this.viewNotesToolStripMenuItem.Click += new System.EventHandler(this.ViewNotesToolStripMenuItem_Click);
            // 
            // tsCount
            // 
            this.tsCount.Name = "tsCount";
            this.tsCount.Size = new System.Drawing.Size(52, 17);
            this.tsCount.Text = "Count: 0";
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsCountLabel,
            this.tsHintsLabel,
            this.tspbTabProgress,
            this.tsStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 499);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1104, 22);
            this.statusStrip.TabIndex = 10;
            this.statusStrip.Text = "statusStrip1";
            // 
            // tsCountLabel
            // 
            this.tsCountLabel.Name = "tsCountLabel";
            this.tsCountLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // tsHintsLabel
            // 
            this.tsHintsLabel.Name = "tsHintsLabel";
            this.tsHintsLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // tspbTabProgress
            // 
            this.tspbTabProgress.Name = "tspbTabProgress";
            this.tspbTabProgress.Size = new System.Drawing.Size(200, 16);
            this.tspbTabProgress.Visible = false;
            // 
            // tsStatusLabel
            // 
            this.tsStatusLabel.Name = "tsStatusLabel";
            this.tsStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // dgDataErrors
            // 
            this.dgDataErrors.AllowUserToAddRows = false;
            this.dgDataErrors.AllowUserToDeleteRows = false;
            this.dgDataErrors.AllowUserToOrderColumns = true;
            this.dgDataErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgDataErrors.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgDataErrors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgDataErrors.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgDataErrors.Location = new System.Drawing.Point(6, 166);
            this.dgDataErrors.MultiSelect = false;
            this.dgDataErrors.Name = "dgDataErrors";
            this.dgDataErrors.ReadOnly = true;
            this.dgDataErrors.RowHeadersWidth = 82;
            this.dgDataErrors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgDataErrors.ShowCellToolTips = false;
            this.dgDataErrors.ShowEditingIcon = false;
            this.dgDataErrors.Size = new System.Drawing.Size(1070, 274);
            this.dgDataErrors.TabIndex = 6;
            this.toolTips.SetToolTip(this.dgDataErrors, "Double click to see list of facts for that individual");
            this.dgDataErrors.VirtualMode = true;
            this.dgDataErrors.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgDataErrors_CellDoubleClick);
            // 
            // tbDuplicateScore
            // 
            this.tbDuplicateScore.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDuplicateScore.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbDuplicateScore.Location = new System.Drawing.Point(447, 3);
            this.tbDuplicateScore.Minimum = 1;
            this.tbDuplicateScore.Name = "tbDuplicateScore";
            this.tbDuplicateScore.Size = new System.Drawing.Size(368, 45);
            this.tbDuplicateScore.TabIndex = 22;
            this.tbDuplicateScore.TickFrequency = 5;
            this.toolTips.SetToolTip(this.tbDuplicateScore, "Adjust Slider to right to limit results to more likely matches");
            this.tbDuplicateScore.Value = 1;
            this.tbDuplicateScore.Scroll += new System.EventHandler(this.TbDuplicateScore_Scroll);
            // 
            // dgCheckAncestors
            // 
            this.dgCheckAncestors.AllowUserToAddRows = false;
            this.dgCheckAncestors.AllowUserToDeleteRows = false;
            this.dgCheckAncestors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgCheckAncestors.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgCheckAncestors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgCheckAncestors.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgCheckAncestors.Location = new System.Drawing.Point(-1, 84);
            this.dgCheckAncestors.Name = "dgCheckAncestors";
            this.dgCheckAncestors.ReadOnly = true;
            this.dgCheckAncestors.RowHeadersWidth = 82;
            this.dgCheckAncestors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgCheckAncestors.ShowCellToolTips = false;
            this.dgCheckAncestors.ShowEditingIcon = false;
            this.dgCheckAncestors.Size = new System.Drawing.Size(1062, 302);
            this.dgCheckAncestors.TabIndex = 7;
            this.toolTips.SetToolTip(this.dgCheckAncestors, "Double click to see list of facts for that individual");
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(712, 24);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(92, 13);
            this.label13.TabIndex = 24;
            this.label13.Text = "Aggressive Match";
            this.toolTips.SetToolTip(this.label13, "Will produce duplicates in list when the two individuals are a very close match t" +
        "o each other - only those with highest duplicate match score");
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(444, 24);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(69, 13);
            this.label12.TabIndex = 23;
            this.label12.Text = "Loose Match";
            this.toolTips.SetToolTip(this.label12, "Will produce duplicates in list when the two individuals decent but vague match t" +
        "o each other - Lowest duplicate match score");
            // 
            // printPreviewDialog
            // 
            this.printPreviewDialog.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog.Enabled = true;
            this.printPreviewDialog.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog.Icon")));
            this.printPreviewDialog.Name = "printPreviewDialog";
            this.printPreviewDialog.Visible = false;
            // 
            // printDialog
            // 
            this.printDialog.AllowSelection = true;
            this.printDialog.AllowSomePages = true;
            this.printDialog.UseEXDialog = true;
            // 
            // ctxViewNotes
            // 
            this.ctxViewNotes.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.ctxViewNotes.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewNotes});
            this.ctxViewNotes.Name = "contextMenuStrip1";
            this.ctxViewNotes.Size = new System.Drawing.Size(134, 26);
            this.ctxViewNotes.Opening += new System.ComponentModel.CancelEventHandler(this.CtxViewNotes_Opening);
            // 
            // mnuViewNotes
            // 
            this.mnuViewNotes.Name = "mnuViewNotes";
            this.mnuViewNotes.Size = new System.Drawing.Size(133, 22);
            this.mnuViewNotes.Text = "View Notes";
            this.mnuViewNotes.Click += new System.EventHandler(this.MnuViewNotes_Click);
            // 
            // ckbTTIncludeOnlyOneParent
            // 
            this.ckbTTIncludeOnlyOneParent.AutoSize = true;
            this.ckbTTIncludeOnlyOneParent.Checked = true;
            this.ckbTTIncludeOnlyOneParent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbTTIncludeOnlyOneParent.Location = new System.Drawing.Point(279, 105);
            this.ckbTTIncludeOnlyOneParent.Name = "ckbTTIncludeOnlyOneParent";
            this.ckbTTIncludeOnlyOneParent.Size = new System.Drawing.Size(273, 17);
            this.ckbTTIncludeOnlyOneParent.TabIndex = 29;
            this.ckbTTIncludeOnlyOneParent.Text = "Include Individuals that have only one parent known";
            this.ckbTTIncludeOnlyOneParent.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(595, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "Surname";
            // 
            // txtTreetopsSurname
            // 
            this.txtTreetopsSurname.Location = new System.Drawing.Point(650, 22);
            this.txtTreetopsSurname.Name = "txtTreetopsSurname";
            this.txtTreetopsSurname.Size = new System.Drawing.Size(201, 20);
            this.txtTreetopsSurname.TabIndex = 23;
            // 
            // treetopsRelation
            // 
            this.treetopsRelation.Location = new System.Drawing.Point(270, 3);
            this.treetopsRelation.Margin = new System.Windows.Forms.Padding(6);
            this.treetopsRelation.MarriedToDB = true;
            this.treetopsRelation.Name = "treetopsRelation";
            this.treetopsRelation.Size = new System.Drawing.Size(322, 96);
            this.treetopsRelation.TabIndex = 12;
            // 
            // Referrals
            // 
            this.Referrals.Controls.Add(this.ckbReferralInCommon);
            this.Referrals.Controls.Add(this.btnReferrals);
            this.Referrals.Controls.Add(this.cmbReferrals);
            this.Referrals.Controls.Add(this.label11);
            this.Referrals.Location = new System.Drawing.Point(6, 319);
            this.Referrals.Name = "Referrals";
            this.Referrals.Size = new System.Drawing.Size(498, 83);
            this.Referrals.TabIndex = 40;
            this.Referrals.TabStop = false;
            this.Referrals.Text = "Referrals";
            // 
            // ckbReferralInCommon
            // 
            this.ckbReferralInCommon.AutoSize = true;
            this.ckbReferralInCommon.Location = new System.Drawing.Point(11, 49);
            this.ckbReferralInCommon.Name = "ckbReferralInCommon";
            this.ckbReferralInCommon.Size = new System.Drawing.Size(150, 17);
            this.ckbReferralInCommon.TabIndex = 3;
            this.ckbReferralInCommon.Text = "Limit to Common Relatives";
            this.ckbReferralInCommon.UseVisualStyleBackColor = true;
            // 
            // btnReferrals
            // 
            this.btnReferrals.Location = new System.Drawing.Point(272, 45);
            this.btnReferrals.Name = "btnReferrals";
            this.btnReferrals.Size = new System.Drawing.Size(220, 23);
            this.btnReferrals.TabIndex = 2;
            this.btnReferrals.Text = "Generate Referral Report for this Individual";
            this.btnReferrals.UseVisualStyleBackColor = true;
            this.btnReferrals.Click += new System.EventHandler(this.BtnReferrals_Click);
            // 
            // cmbReferrals
            // 
            this.cmbReferrals.FormattingEnabled = true;
            this.cmbReferrals.Location = new System.Drawing.Point(97, 18);
            this.cmbReferrals.Name = "cmbReferrals";
            this.cmbReferrals.Size = new System.Drawing.Size(395, 21);
            this.cmbReferrals.TabIndex = 1;
            this.cmbReferrals.Click += new System.EventHandler(this.CmbReferrals_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 21);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(85, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Select Individual";
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel2.Location = new System.Drawing.Point(726, 371);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(174, 16);
            this.linkLabel2.TabIndex = 33;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "Visit the Lost Cousins Forum";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel2_LinkClicked);
            // 
            // btnViewInvalidRefs
            // 
            this.btnViewInvalidRefs.Location = new System.Drawing.Point(0, 0);
            this.btnViewInvalidRefs.Name = "btnViewInvalidRefs";
            this.btnViewInvalidRefs.Size = new System.Drawing.Size(75, 23);
            this.btnViewInvalidRefs.TabIndex = 0;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(468, 12);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(383, 16);
            this.label21.TabIndex = 37;
            this.label21.Text = "Census Records with valid Reference to upload to Lost Cousins";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(34, 72);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(68, 16);
            this.label20.TabIndex = 3;
            this.label20.Text = "Password";
            this.label20.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(6, 31);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(96, 16);
            this.label19.TabIndex = 2;
            this.label19.Text = "Email Address";
            this.label19.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // rtbCheckAncestors
            // 
            this.rtbCheckAncestors.BackColor = System.Drawing.SystemColors.Window;
            this.rtbCheckAncestors.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbCheckAncestors.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbCheckAncestors.ForeColor = System.Drawing.Color.Red;
            this.rtbCheckAncestors.Location = new System.Drawing.Point(334, 12);
            this.rtbCheckAncestors.Name = "rtbCheckAncestors";
            this.rtbCheckAncestors.ReadOnly = true;
            this.rtbCheckAncestors.Size = new System.Drawing.Size(733, 66);
            this.rtbCheckAncestors.TabIndex = 37;
            this.rtbCheckAncestors.TabStop = false;
            this.rtbCheckAncestors.Text = "Please Login to see data to update";
            this.rtbCheckAncestors.TextChanged += new System.EventHandler(this.RtbCheckAncestors_TextChanged);
            // 
            // btnCheckMyAncestors
            // 
            this.btnCheckMyAncestors.Location = new System.Drawing.Point(0, 0);
            this.btnCheckMyAncestors.Name = "btnCheckMyAncestors";
            this.btnCheckMyAncestors.Size = new System.Drawing.Size(75, 23);
            this.btnCheckMyAncestors.TabIndex = 38;
            // 
            // lblCheckAncestors
            // 
            this.lblCheckAncestors.AutoSize = true;
            this.lblCheckAncestors.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCheckAncestors.Location = new System.Drawing.Point(12, 12);
            this.lblCheckAncestors.Name = "lblCheckAncestors";
            this.lblCheckAncestors.Size = new System.Drawing.Size(316, 16);
            this.lblCheckAncestors.TabIndex = 0;
            this.lblCheckAncestors.Text = "Not Currently Logged in Use Updates Page to Login";
            // 
            // txtAliveDates
            // 
            this.txtAliveDates.Location = new System.Drawing.Point(711, 84);
            this.txtAliveDates.Name = "txtAliveDates";
            this.txtAliveDates.Size = new System.Drawing.Size(232, 20);
            this.txtAliveDates.TabIndex = 40;
            this.txtAliveDates.Text = "Enter valid GEDCOM date/date range";
            this.txtAliveDates.Enter += new System.EventHandler(this.TxtAliveDates_Enter);
            this.txtAliveDates.Validating += new System.ComponentModel.CancelEventHandler(this.TxtAliveDates_Validating);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(640, 87);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(67, 13);
            this.label22.TabIndex = 39;
            this.label22.Text = "Alive Dates: ";
            // 
            // btnRandomSurnameEntered
            // 
            this.btnRandomSurnameEntered.Location = new System.Drawing.Point(0, 0);
            this.btnRandomSurnameEntered.Name = "btnRandomSurnameEntered";
            this.btnRandomSurnameEntered.Size = new System.Drawing.Size(75, 23);
            this.btnRandomSurnameEntered.TabIndex = 2;
            // 
            // btnRandomSurnameMissing
            // 
            this.btnRandomSurnameMissing.Location = new System.Drawing.Point(0, 0);
            this.btnRandomSurnameMissing.Name = "btnRandomSurnameMissing";
            this.btnRandomSurnameMissing.Size = new System.Drawing.Size(75, 23);
            this.btnRandomSurnameMissing.TabIndex = 3;
            // 
            // chkExcludeUnknownBirths
            // 
            this.chkExcludeUnknownBirths.AutoSize = true;
            this.chkExcludeUnknownBirths.Location = new System.Drawing.Point(340, 63);
            this.chkExcludeUnknownBirths.Name = "chkExcludeUnknownBirths";
            this.chkExcludeUnknownBirths.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkExcludeUnknownBirths.Size = new System.Drawing.Size(238, 17);
            this.chkExcludeUnknownBirths.TabIndex = 31;
            this.chkExcludeUnknownBirths.Text = "Exclude Individuals with unknown birth dates";
            this.chkExcludeUnknownBirths.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(640, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Surname";
            // 
            // txtCensusSurname
            // 
            this.txtCensusSurname.Location = new System.Drawing.Point(711, 36);
            this.txtCensusSurname.Name = "txtCensusSurname";
            this.txtCensusSurname.Size = new System.Drawing.Size(232, 20);
            this.txtCensusSurname.TabIndex = 29;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(340, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(175, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Exclude individuals over the age of ";
            // 
            // udAgeFilter
            // 
            this.udAgeFilter.Location = new System.Drawing.Point(521, 36);
            this.udAgeFilter.Maximum = new decimal(new int[] {
            110,
            0,
            0,
            0});
            this.udAgeFilter.Minimum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.udAgeFilter.Name = "udAgeFilter";
            this.udAgeFilter.Size = new System.Drawing.Size(43, 20);
            this.udAgeFilter.TabIndex = 25;
            this.udAgeFilter.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            // 
            // relTypesCensus
            // 
            this.relTypesCensus.Location = new System.Drawing.Point(9, 19);
            this.relTypesCensus.Margin = new System.Windows.Forms.Padding(6);
            this.relTypesCensus.MarriedToDB = true;
            this.relTypesCensus.Name = "relTypesCensus";
            this.relTypesCensus.Size = new System.Drawing.Size(325, 99);
            this.relTypesCensus.TabIndex = 27;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnMismatchedChildrenStatus);
            this.groupBox5.Controls.Add(this.btnNoChildrenStatus);
            this.groupBox5.Location = new System.Drawing.Point(6, 19);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(327, 59);
            this.groupBox5.TabIndex = 32;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "1911 UK Census";
            // 
            // btnMismatchedChildrenStatus
            // 
            this.btnMismatchedChildrenStatus.Location = new System.Drawing.Point(0, 0);
            this.btnMismatchedChildrenStatus.Name = "btnMismatchedChildrenStatus";
            this.btnMismatchedChildrenStatus.Size = new System.Drawing.Size(75, 23);
            this.btnMismatchedChildrenStatus.TabIndex = 0;
            // 
            // btnNoChildrenStatus
            // 
            this.btnNoChildrenStatus.Location = new System.Drawing.Point(0, 0);
            this.btnNoChildrenStatus.Name = "btnNoChildrenStatus";
            this.btnNoChildrenStatus.Size = new System.Drawing.Size(75, 23);
            this.btnNoChildrenStatus.TabIndex = 1;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnReportUnrecognised);
            this.groupBox6.Location = new System.Drawing.Point(363, 19);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(244, 59);
            this.groupBox6.TabIndex = 31;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Export Missing/Unrecognised data to File";
            // 
            // btnReportUnrecognised
            // 
            this.btnReportUnrecognised.Location = new System.Drawing.Point(0, 0);
            this.btnReportUnrecognised.Name = "btnReportUnrecognised";
            this.btnReportUnrecognised.Size = new System.Drawing.Size(75, 23);
            this.btnReportUnrecognised.TabIndex = 0;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "QuestionMark.png");
            this.imageList.Images.SetKeyName(1, "GoogleMatch.png");
            this.imageList.Images.SetKeyName(2, "GooglePartial.png");
            this.imageList.Images.SetKeyName(3, "Complete_OK.png");
            this.imageList.Images.SetKeyName(4, "CriticalError.png");
            this.imageList.Images.SetKeyName(5, "Flagged.png");
            this.imageList.Images.SetKeyName(6, "OutOfBounds.png");
            this.imageList.Images.SetKeyName(7, "Warning.png");
            // 
            // tabDisplayProgress
            // 
            this.tabDisplayProgress.Controls.Add(this.splitGedcom);
            this.tabDisplayProgress.Location = new System.Drawing.Point(4, 22);
            this.tabDisplayProgress.Name = "tabDisplayProgress";
            this.tabDisplayProgress.Padding = new System.Windows.Forms.Padding(3);
            this.tabDisplayProgress.Size = new System.Drawing.Size(1088, 460);
            this.tabDisplayProgress.TabIndex = 1;
            this.tabDisplayProgress.Text = "Gedcom Stats";
            this.tabDisplayProgress.UseVisualStyleBackColor = true;
            // 
            // splitGedcom
            // 
            this.splitGedcom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitGedcom.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitGedcom.Location = new System.Drawing.Point(3, 3);
            this.splitGedcom.Name = "splitGedcom";
            this.splitGedcom.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitGedcom.Panel1
            // 
            this.splitGedcom.Panel1.Controls.Add(this.panel2);
            this.splitGedcom.Panel1MinSize = 110;
            // 
            // splitGedcom.Panel2
            // 
            this.splitGedcom.Panel2.Controls.Add(this.rtbOutput);
            this.splitGedcom.Size = new System.Drawing.Size(1082, 454);
            this.splitGedcom.SplitterDistance = 110;
            this.splitGedcom.TabIndex = 14;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Controls.Add(this.LbProgramName);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.pbRelationships);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.pbFamilies);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.pbIndividuals);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.pbSources);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1082, 110);
            this.panel2.TabIndex = 13;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(943, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 100);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            // 
            // LbProgramName
            // 
            this.LbProgramName.AutoSize = true;
            this.LbProgramName.Font = new System.Drawing.Font("Microsoft Sans Serif", 52F, System.Drawing.FontStyle.Bold);
            this.LbProgramName.Location = new System.Drawing.Point(415, 13);
            this.LbProgramName.Name = "LbProgramName";
            this.LbProgramName.Size = new System.Drawing.Size(716, 79);
            this.LbProgramName.TabIndex = 17;
            this.LbProgramName.Text = "Family Tree Analyzer";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 79);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(128, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Relationships && Locations";
            // 
            // pbRelationships
            // 
            this.pbRelationships.Location = new System.Drawing.Point(134, 76);
            this.pbRelationships.Name = "pbRelationships";
            this.pbRelationships.Size = new System.Drawing.Size(275, 16);
            this.pbRelationships.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Loading Families";
            // 
            // pbFamilies
            // 
            this.pbFamilies.Location = new System.Drawing.Point(134, 54);
            this.pbFamilies.Name = "pbFamilies";
            this.pbFamilies.Size = new System.Drawing.Size(275, 16);
            this.pbFamilies.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Loading Individuals";
            // 
            // pbIndividuals
            // 
            this.pbIndividuals.Location = new System.Drawing.Point(134, 32);
            this.pbIndividuals.Name = "pbIndividuals";
            this.pbIndividuals.Size = new System.Drawing.Size(275, 16);
            this.pbIndividuals.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Loading Sources";
            // 
            // pbSources
            // 
            this.pbSources.Location = new System.Drawing.Point(134, 10);
            this.pbSources.Name = "pbSources";
            this.pbSources.Size = new System.Drawing.Size(275, 16);
            this.pbSources.TabIndex = 9;
            // 
            // rtbOutput
            // 
            this.rtbOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbOutput.Location = new System.Drawing.Point(0, 0);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.ReadOnly = true;
            this.rtbOutput.Size = new System.Drawing.Size(1082, 340);
            this.rtbOutput.TabIndex = 14;
            this.rtbOutput.Text = "";
            // 
            // tabSelector
            // 
            this.tabSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabSelector.Controls.Add(this.tabDisplayProgress);
            this.tabSelector.Controls.Add(this.tabMainLists);
            this.tabSelector.Controls.Add(this.tabErrorsFixes);
            this.tabSelector.Location = new System.Drawing.Point(0, 27);
            this.tabSelector.Name = "tabSelector";
            this.tabSelector.SelectedIndex = 0;
            this.tabSelector.Size = new System.Drawing.Size(1096, 486);
            this.tabSelector.TabIndex = 9;
            this.tabSelector.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            // 
            // tabMainLists
            // 
            this.tabMainLists.Controls.Add(this.tabMainListsSelector);
            this.tabMainLists.Location = new System.Drawing.Point(4, 22);
            this.tabMainLists.Name = "tabMainLists";
            this.tabMainLists.Padding = new System.Windows.Forms.Padding(3);
            this.tabMainLists.Size = new System.Drawing.Size(1088, 460);
            this.tabMainLists.TabIndex = 18;
            this.tabMainLists.Text = "Main Lists";
            this.tabMainLists.UseVisualStyleBackColor = true;
            // 
            // tabMainListsSelector
            // 
            this.tabMainListsSelector.Controls.Add(this.tabIndividuals);
            this.tabMainListsSelector.Controls.Add(this.tabFamilies);
            this.tabMainListsSelector.Controls.Add(this.tabSources);
            this.tabMainListsSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMainListsSelector.Location = new System.Drawing.Point(3, 3);
            this.tabMainListsSelector.Name = "tabMainListsSelector";
            this.tabMainListsSelector.SelectedIndex = 0;
            this.tabMainListsSelector.Size = new System.Drawing.Size(1082, 454);
            this.tabMainListsSelector.TabIndex = 0;
            this.tabMainListsSelector.SelectedIndexChanged += new System.EventHandler(this.TabMainListSelector_SelectedIndexChanged);
            // 
            // tabIndividuals
            // 
            this.tabIndividuals.Controls.Add(this.dgIndividuals);
            this.tabIndividuals.Location = new System.Drawing.Point(4, 22);
            this.tabIndividuals.Name = "tabIndividuals";
            this.tabIndividuals.Padding = new System.Windows.Forms.Padding(3);
            this.tabIndividuals.Size = new System.Drawing.Size(1074, 428);
            this.tabIndividuals.TabIndex = 0;
            this.tabIndividuals.Text = "Individuals";
            this.tabIndividuals.UseVisualStyleBackColor = true;
            // 
            // dgIndividuals
            // 
            this.dgIndividuals.AllowUserToAddRows = false;
            this.dgIndividuals.AllowUserToDeleteRows = false;
            this.dgIndividuals.AllowUserToOrderColumns = true;
            this.dgIndividuals.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgIndividuals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgIndividuals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgIndividuals.Location = new System.Drawing.Point(3, 3);
            this.dgIndividuals.MultiSelect = false;
            this.dgIndividuals.Name = "dgIndividuals";
            this.dgIndividuals.ReadOnly = true;
            this.dgIndividuals.RowHeadersWidth = 50;
            this.dgIndividuals.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgIndividuals.Size = new System.Drawing.Size(1068, 422);
            this.dgIndividuals.TabIndex = 1;
            this.dgIndividuals.VirtualMode = true;
            this.dgIndividuals.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgIndividuals_CellDoubleClick);
            this.dgIndividuals.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DgIndividuals_MouseDown);
            // 
            // tabFamilies
            // 
            this.tabFamilies.Controls.Add(this.dgFamilies);
            this.tabFamilies.Location = new System.Drawing.Point(4, 22);
            this.tabFamilies.Name = "tabFamilies";
            this.tabFamilies.Padding = new System.Windows.Forms.Padding(3);
            this.tabFamilies.Size = new System.Drawing.Size(1074, 428);
            this.tabFamilies.TabIndex = 1;
            this.tabFamilies.Text = "Families";
            this.tabFamilies.UseVisualStyleBackColor = true;
            // 
            // dgFamilies
            // 
            this.dgFamilies.AllowUserToAddRows = false;
            this.dgFamilies.AllowUserToDeleteRows = false;
            this.dgFamilies.AllowUserToOrderColumns = true;
            this.dgFamilies.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgFamilies.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgFamilies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgFamilies.Location = new System.Drawing.Point(3, 3);
            this.dgFamilies.MultiSelect = false;
            this.dgFamilies.Name = "dgFamilies";
            this.dgFamilies.ReadOnly = true;
            this.dgFamilies.RowHeadersWidth = 50;
            this.dgFamilies.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgFamilies.Size = new System.Drawing.Size(1068, 422);
            this.dgFamilies.TabIndex = 2;
            this.dgFamilies.VirtualMode = true;
            this.dgFamilies.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgFamilies_CellDoubleClick);
            // 
            // tabSources
            // 
            this.tabSources.Controls.Add(this.dgSources);
            this.tabSources.Location = new System.Drawing.Point(4, 22);
            this.tabSources.Name = "tabSources";
            this.tabSources.Size = new System.Drawing.Size(1074, 428);
            this.tabSources.TabIndex = 2;
            this.tabSources.Text = "Sources";
            this.tabSources.UseVisualStyleBackColor = true;
            // 
            // dgSources
            // 
            this.dgSources.AllowUserToAddRows = false;
            this.dgSources.AllowUserToDeleteRows = false;
            this.dgSources.AllowUserToOrderColumns = true;
            this.dgSources.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgSources.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgSources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgSources.Location = new System.Drawing.Point(0, 0);
            this.dgSources.MultiSelect = false;
            this.dgSources.Name = "dgSources";
            this.dgSources.ReadOnly = true;
            this.dgSources.RowHeadersWidth = 50;
            this.dgSources.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgSources.Size = new System.Drawing.Size(1074, 428);
            this.dgSources.TabIndex = 2;
            this.dgSources.VirtualMode = true;
            this.dgSources.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgSources_CellDoubleClick);
            // 
            // tabErrorsFixes
            // 
            this.tabErrorsFixes.Controls.Add(this.tabErrorFixSelector);
            this.tabErrorsFixes.Location = new System.Drawing.Point(4, 22);
            this.tabErrorsFixes.Name = "tabErrorsFixes";
            this.tabErrorsFixes.Size = new System.Drawing.Size(1088, 460);
            this.tabErrorsFixes.TabIndex = 19;
            this.tabErrorsFixes.Text = "Errors/Fixes";
            this.tabErrorsFixes.UseVisualStyleBackColor = true;
            // 
            // tabErrorFixSelector
            // 
            this.tabErrorFixSelector.Controls.Add(this.tabDataErrors);
            this.tabErrorFixSelector.Controls.Add(this.tabDuplicates);
            this.tabErrorFixSelector.Controls.Add(this.tabLooseBirths);
            this.tabErrorFixSelector.Controls.Add(this.tabLooseDeaths);
            this.tabErrorFixSelector.Controls.Add(this.tabLooseInfo);
            this.tabErrorFixSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabErrorFixSelector.Location = new System.Drawing.Point(0, 0);
            this.tabErrorFixSelector.Name = "tabErrorFixSelector";
            this.tabErrorFixSelector.SelectedIndex = 0;
            this.tabErrorFixSelector.ShowToolTips = true;
            this.tabErrorFixSelector.Size = new System.Drawing.Size(1088, 460);
            this.tabErrorFixSelector.TabIndex = 0;
            this.tabErrorFixSelector.SelectedIndexChanged += new System.EventHandler(this.TabErrorFixSelector_SelectedIndexChanged);
            // 
            // tabDataErrors
            // 
            this.tabDataErrors.Controls.Add(this.dgDataErrors);
            this.tabDataErrors.Controls.Add(this.gbDataErrorTypes);
            this.tabDataErrors.Location = new System.Drawing.Point(4, 22);
            this.tabDataErrors.Name = "tabDataErrors";
            this.tabDataErrors.Padding = new System.Windows.Forms.Padding(3);
            this.tabDataErrors.Size = new System.Drawing.Size(1080, 434);
            this.tabDataErrors.TabIndex = 0;
            this.tabDataErrors.Text = "Data Errors";
            this.tabDataErrors.UseVisualStyleBackColor = true;
            // 
            // gbDataErrorTypes
            // 
            this.gbDataErrorTypes.Controls.Add(this.btnSelectAll);
            this.gbDataErrorTypes.Controls.Add(this.btnClearAll);
            this.gbDataErrorTypes.Controls.Add(this.ckbDataErrors);
            this.gbDataErrorTypes.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbDataErrorTypes.Location = new System.Drawing.Point(3, 3);
            this.gbDataErrorTypes.Name = "gbDataErrorTypes";
            this.gbDataErrorTypes.Size = new System.Drawing.Size(1074, 166);
            this.gbDataErrorTypes.TabIndex = 1;
            this.gbDataErrorTypes.TabStop = false;
            this.gbDataErrorTypes.Text = "Types of Data Error to display";
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(8, 134);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 7;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.BtnSelectAll_Click);
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point(89, 134);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(75, 23);
            this.btnClearAll.TabIndex = 6;
            this.btnClearAll.Text = "Clear All";
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.BtnClearAll_Click);
            // 
            // ckbDataErrors
            // 
            this.ckbDataErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ckbDataErrors.CheckOnClick = true;
            this.ckbDataErrors.ColumnWidth = 300;
            this.ckbDataErrors.FormattingEnabled = true;
            this.ckbDataErrors.Location = new System.Drawing.Point(8, 19);
            this.ckbDataErrors.MultiColumn = true;
            this.ckbDataErrors.Name = "ckbDataErrors";
            this.ckbDataErrors.ScrollAlwaysVisible = true;
            this.ckbDataErrors.Size = new System.Drawing.Size(1054, 94);
            this.ckbDataErrors.TabIndex = 0;
            this.ckbDataErrors.SelectedIndexChanged += new System.EventHandler(this.CkbDataErrors_SelectedIndexChanged);
            // 
            // tabDuplicates
            // 
            this.tabDuplicates.Controls.Add(this.labDuplicateSlider);
            this.tabDuplicates.Controls.Add(this.labCompletion);
            this.tabDuplicates.Controls.Add(this.ckbHideIgnoredDuplicates);
            this.tabDuplicates.Controls.Add(this.label16);
            this.tabDuplicates.Controls.Add(this.label13);
            this.tabDuplicates.Controls.Add(this.label12);
            this.tabDuplicates.Controls.Add(this.tbDuplicateScore);
            this.tabDuplicates.Controls.Add(this.labCalcDuplicates);
            this.tabDuplicates.Controls.Add(this.pbDuplicates);
            this.tabDuplicates.Controls.Add(this.dgDuplicates);
            this.tabDuplicates.Controls.Add(this.btnCancelDuplicates);
            this.tabDuplicates.Location = new System.Drawing.Point(4, 22);
            this.tabDuplicates.Name = "tabDuplicates";
            this.tabDuplicates.Padding = new System.Windows.Forms.Padding(3);
            this.tabDuplicates.Size = new System.Drawing.Size(1080, 434);
            this.tabDuplicates.TabIndex = 1;
            this.tabDuplicates.Text = "Duplicates?";
            this.tabDuplicates.UseVisualStyleBackColor = true;
            // 
            // labDuplicateSlider
            // 
            this.labDuplicateSlider.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labDuplicateSlider.AutoSize = true;
            this.labDuplicateSlider.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labDuplicateSlider.Location = new System.Drawing.Point(558, 35);
            this.labDuplicateSlider.Name = "labDuplicateSlider";
            this.labDuplicateSlider.Size = new System.Drawing.Size(104, 13);
            this.labDuplicateSlider.TabIndex = 31;
            this.labDuplicateSlider.Text = "Match Quality : 1";
            // 
            // labCompletion
            // 
            this.labCompletion.AutoSize = true;
            this.labCompletion.Location = new System.Drawing.Point(122, 35);
            this.labCompletion.Name = "labCompletion";
            this.labCompletion.Size = new System.Drawing.Size(0, 13);
            this.labCompletion.TabIndex = 30;
            // 
            // ckbHideIgnoredDuplicates
            // 
            this.ckbHideIgnoredDuplicates.AutoSize = true;
            this.ckbHideIgnoredDuplicates.Checked = true;
            this.ckbHideIgnoredDuplicates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbHideIgnoredDuplicates.Location = new System.Drawing.Point(12, 64);
            this.ckbHideIgnoredDuplicates.Name = "ckbHideIgnoredDuplicates";
            this.ckbHideIgnoredDuplicates.Size = new System.Drawing.Size(228, 17);
            this.ckbHideIgnoredDuplicates.TabIndex = 28;
            this.ckbHideIgnoredDuplicates.Text = "Hide Possible Duplicates marked as Ignore";
            this.ckbHideIgnoredDuplicates.UseVisualStyleBackColor = true;
            this.ckbHideIgnoredDuplicates.CheckedChanged += new System.EventHandler(this.CkbHideIgnoredDuplicates_CheckedChanged);
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(411, 63);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(199, 18);
            this.label16.TabIndex = 26;
            this.label16.Text = "Candidate Duplicates List";
            // 
            // labCalcDuplicates
            // 
            this.labCalcDuplicates.AutoSize = true;
            this.labCalcDuplicates.Location = new System.Drawing.Point(7, 10);
            this.labCalcDuplicates.Name = "labCalcDuplicates";
            this.labCalcDuplicates.Size = new System.Drawing.Size(112, 13);
            this.labCalcDuplicates.TabIndex = 21;
            this.labCalcDuplicates.Text = "Calculating Duplicates";
            // 
            // pbDuplicates
            // 
            this.pbDuplicates.Location = new System.Drawing.Point(125, 6);
            this.pbDuplicates.Name = "pbDuplicates";
            this.pbDuplicates.Size = new System.Drawing.Size(283, 23);
            this.pbDuplicates.TabIndex = 20;
            // 
            // dgDuplicates
            // 
            this.dgDuplicates.AllowUserToAddRows = false;
            this.dgDuplicates.AllowUserToDeleteRows = false;
            this.dgDuplicates.AllowUserToOrderColumns = true;
            this.dgDuplicates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgDuplicates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgDuplicates.Location = new System.Drawing.Point(-1, 87);
            this.dgDuplicates.MultiSelect = false;
            this.dgDuplicates.Name = "dgDuplicates";
            this.dgDuplicates.ReadOnly = true;
            this.dgDuplicates.RowHeadersWidth = 15;
            this.dgDuplicates.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgDuplicates.Size = new System.Drawing.Size(1072, 333);
            this.dgDuplicates.TabIndex = 19;
            this.dgDuplicates.VirtualMode = true;
            this.dgDuplicates.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgDuplicates_CellContentClick);
            this.dgDuplicates.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgDuplicates_CellDoubleClick);
            // 
            // btnCancelDuplicates
            // 
            this.btnCancelDuplicates.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelDuplicates.Image")));
            this.btnCancelDuplicates.Location = new System.Drawing.Point(414, 6);
            this.btnCancelDuplicates.Name = "btnCancelDuplicates";
            this.btnCancelDuplicates.Size = new System.Drawing.Size(23, 23);
            this.btnCancelDuplicates.TabIndex = 27;
            this.btnCancelDuplicates.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancelDuplicates.UseVisualStyleBackColor = true;
            this.btnCancelDuplicates.Visible = false;
            this.btnCancelDuplicates.Click += new System.EventHandler(this.BtnCancelDuplicates_Click);
            // 
            // tabLooseBirths
            // 
            this.tabLooseBirths.Controls.Add(this.dgLooseBirths);
            this.tabLooseBirths.Location = new System.Drawing.Point(4, 22);
            this.tabLooseBirths.Name = "tabLooseBirths";
            this.tabLooseBirths.Size = new System.Drawing.Size(1080, 434);
            this.tabLooseBirths.TabIndex = 2;
            this.tabLooseBirths.Text = "Loose Births";
            this.tabLooseBirths.UseVisualStyleBackColor = true;
            // 
            // dgLooseBirths
            // 
            this.dgLooseBirths.AllowUserToAddRows = false;
            this.dgLooseBirths.AllowUserToDeleteRows = false;
            this.dgLooseBirths.AllowUserToOrderColumns = true;
            this.dgLooseBirths.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgLooseBirths.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgLooseBirths.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgLooseBirths.Location = new System.Drawing.Point(0, 0);
            this.dgLooseBirths.MultiSelect = false;
            this.dgLooseBirths.Name = "dgLooseBirths";
            this.dgLooseBirths.RowHeadersWidth = 82;
            this.dgLooseBirths.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgLooseBirths.Size = new System.Drawing.Size(1080, 434);
            this.dgLooseBirths.TabIndex = 3;
            this.dgLooseBirths.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgLooseBirths_CellDoubleClick);
            // 
            // tabLooseDeaths
            // 
            this.tabLooseDeaths.Controls.Add(this.dgLooseDeaths);
            this.tabLooseDeaths.Location = new System.Drawing.Point(4, 22);
            this.tabLooseDeaths.Name = "tabLooseDeaths";
            this.tabLooseDeaths.Size = new System.Drawing.Size(1080, 434);
            this.tabLooseDeaths.TabIndex = 3;
            this.tabLooseDeaths.Text = "Loose Deaths";
            this.tabLooseDeaths.UseVisualStyleBackColor = true;
            // 
            // dgLooseDeaths
            // 
            this.dgLooseDeaths.AllowUserToAddRows = false;
            this.dgLooseDeaths.AllowUserToDeleteRows = false;
            this.dgLooseDeaths.AllowUserToOrderColumns = true;
            this.dgLooseDeaths.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgLooseDeaths.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgLooseDeaths.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgLooseDeaths.Location = new System.Drawing.Point(0, 0);
            this.dgLooseDeaths.MultiSelect = false;
            this.dgLooseDeaths.Name = "dgLooseDeaths";
            this.dgLooseDeaths.RowHeadersWidth = 82;
            this.dgLooseDeaths.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgLooseDeaths.Size = new System.Drawing.Size(1080, 434);
            this.dgLooseDeaths.TabIndex = 2;
            this.dgLooseDeaths.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgLooseDeaths_CellDoubleClick);
            // 
            // tabLooseInfo
            // 
            this.tabLooseInfo.Controls.Add(this.dgLooseInfo);
            this.tabLooseInfo.Location = new System.Drawing.Point(4, 22);
            this.tabLooseInfo.Name = "tabLooseInfo";
            this.tabLooseInfo.Size = new System.Drawing.Size(1080, 434);
            this.tabLooseInfo.TabIndex = 4;
            this.tabLooseInfo.Text = "All Loose Info";
            this.tabLooseInfo.UseVisualStyleBackColor = true;
            // 
            // dgLooseInfo
            // 
            this.dgLooseInfo.AllowUserToAddRows = false;
            this.dgLooseInfo.AllowUserToDeleteRows = false;
            this.dgLooseInfo.AllowUserToOrderColumns = true;
            this.dgLooseInfo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgLooseInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgLooseInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgLooseInfo.Location = new System.Drawing.Point(0, 0);
            this.dgLooseInfo.MultiSelect = false;
            this.dgLooseInfo.Name = "dgLooseInfo";
            this.dgLooseInfo.RowHeadersWidth = 82;
            this.dgLooseInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgLooseInfo.Size = new System.Drawing.Size(1080, 434);
            this.dgLooseInfo.TabIndex = 4;
            this.dgLooseInfo.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgLooseInfo_CellDoubleClick);
            // 
            // NonDuplicate
            // 
            this.NonDuplicate.DataPropertyName = "IgnoreNonDuplicate";
            this.NonDuplicate.FalseValue = "False";
            this.NonDuplicate.HeaderText = "Ignore";
            this.NonDuplicate.MinimumWidth = 40;
            this.NonDuplicate.Name = "NonDuplicate";
            this.NonDuplicate.ReadOnly = true;
            this.NonDuplicate.TrueValue = "True";
            this.NonDuplicate.Width = 40;
            // 
            // Score
            // 
            this.Score.DataPropertyName = "Score";
            this.Score.HeaderText = "Score";
            this.Score.MinimumWidth = 10;
            this.Score.Name = "Score";
            this.Score.ReadOnly = true;
            this.Score.Width = 200;
            // 
            // DuplicateIndividualID
            // 
            this.DuplicateIndividualID.DataPropertyName = "IndividualID";
            this.DuplicateIndividualID.HeaderText = "ID";
            this.DuplicateIndividualID.MinimumWidth = 10;
            this.DuplicateIndividualID.Name = "DuplicateIndividualID";
            this.DuplicateIndividualID.ReadOnly = true;
            this.DuplicateIndividualID.Width = 200;
            // 
            // DuplicateName
            // 
            this.DuplicateName.DataPropertyName = "Name";
            this.DuplicateName.HeaderText = "Name";
            this.DuplicateName.MinimumWidth = 50;
            this.DuplicateName.Name = "DuplicateName";
            this.DuplicateName.ReadOnly = true;
            this.DuplicateName.Width = 150;
            // 
            // DuplicateForenames
            // 
            this.DuplicateForenames.DataPropertyName = "Forenames";
            this.DuplicateForenames.HeaderText = "Forenames";
            this.DuplicateForenames.MinimumWidth = 10;
            this.DuplicateForenames.Name = "DuplicateForenames";
            this.DuplicateForenames.ReadOnly = true;
            this.DuplicateForenames.Visible = false;
            this.DuplicateForenames.Width = 200;
            // 
            // DuplicateSurname
            // 
            this.DuplicateSurname.DataPropertyName = "Surname";
            this.DuplicateSurname.HeaderText = "Surname";
            this.DuplicateSurname.MinimumWidth = 10;
            this.DuplicateSurname.Name = "DuplicateSurname";
            this.DuplicateSurname.ReadOnly = true;
            this.DuplicateSurname.Visible = false;
            this.DuplicateSurname.Width = 200;
            // 
            // DuplicateBirthDate
            // 
            this.DuplicateBirthDate.DataPropertyName = "BirthDate";
            this.DuplicateBirthDate.HeaderText = "Birthdate";
            this.DuplicateBirthDate.MinimumWidth = 50;
            this.DuplicateBirthDate.Name = "DuplicateBirthDate";
            this.DuplicateBirthDate.ReadOnly = true;
            this.DuplicateBirthDate.Width = 150;
            // 
            // DuplicateBirthLocation
            // 
            this.DuplicateBirthLocation.DataPropertyName = "BirthLocation";
            this.DuplicateBirthLocation.HeaderText = "Birth Location";
            this.DuplicateBirthLocation.MinimumWidth = 100;
            this.DuplicateBirthLocation.Name = "DuplicateBirthLocation";
            this.DuplicateBirthLocation.ReadOnly = true;
            this.DuplicateBirthLocation.Width = 175;
            // 
            // MatchIndividualID
            // 
            this.MatchIndividualID.DataPropertyName = "MatchIndividualID";
            this.MatchIndividualID.HeaderText = "Match ID";
            this.MatchIndividualID.MinimumWidth = 10;
            this.MatchIndividualID.Name = "MatchIndividualID";
            this.MatchIndividualID.ReadOnly = true;
            this.MatchIndividualID.Width = 50;
            // 
            // MatchName
            // 
            this.MatchName.DataPropertyName = "MatchName";
            this.MatchName.HeaderText = "Match Name";
            this.MatchName.MinimumWidth = 50;
            this.MatchName.Name = "MatchName";
            this.MatchName.ReadOnly = true;
            this.MatchName.Width = 150;
            // 
            // MatchBirthDate
            // 
            this.MatchBirthDate.DataPropertyName = "MatchBirthDate";
            this.MatchBirthDate.HeaderText = "Match Birthdate";
            this.MatchBirthDate.MinimumWidth = 50;
            this.MatchBirthDate.Name = "MatchBirthDate";
            this.MatchBirthDate.ReadOnly = true;
            this.MatchBirthDate.Width = 150;
            // 
            // MatchBirthLocation
            // 
            this.MatchBirthLocation.DataPropertyName = "MatchBirthLocation";
            this.MatchBirthLocation.HeaderText = "Match Birth Location";
            this.MatchBirthLocation.MinimumWidth = 100;
            this.MatchBirthLocation.Name = "MatchBirthLocation";
            this.MatchBirthLocation.ReadOnly = true;
            this.MatchBirthLocation.Width = 175;
            // 
            // saveDatabase
            // 
            this.saveDatabase.DefaultExt = "zip";
            this.saveDatabase.Filter = "Zip Files | *.zip";
            // 
            // restoreDatabase
            // 
            this.restoreDatabase.FileName = "*.zip";
            this.restoreDatabase.Filter = "Gecode Databases | *.s3db | Zip Files | *.zip";
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(184, 22);
            this.toolStripMenuItem1.Text = "Open Gramps file ...";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuItem1_Click);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1104, 521);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tabSelector);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(580, 499);
            this.Name = "MainForm";
            this.Text = "Family Tree Analyzer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.Move += new System.EventHandler(this.MainForm_Move);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.mnuSetRoot.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDataErrors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDuplicateScore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgCheckAncestors)).EndInit();
            this.ctxViewNotes.ResumeLayout(false);
            this.Referrals.ResumeLayout(false);
            this.Referrals.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udAgeFilter)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.tabDisplayProgress.ResumeLayout(false);
            this.splitGedcom.Panel1.ResumeLayout(false);
            this.splitGedcom.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitGedcom)).EndInit();
            this.splitGedcom.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabSelector.ResumeLayout(false);
            this.tabMainLists.ResumeLayout(false);
            this.tabMainListsSelector.ResumeLayout(false);
            this.tabIndividuals.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgIndividuals)).EndInit();
            this.tabFamilies.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgFamilies)).EndInit();
            this.tabSources.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgSources)).EndInit();
            this.tabErrorsFixes.ResumeLayout(false);
            this.tabErrorFixSelector.ResumeLayout(false);
            this.tabDataErrors.ResumeLayout(false);
            this.gbDataErrorTypes.ResumeLayout(false);
            this.tabDuplicates.ResumeLayout(false);
            this.tabDuplicates.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDuplicates)).EndInit();
            this.tabLooseBirths.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgLooseBirths)).EndInit();
            this.tabLooseDeaths.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgLooseDeaths)).EndInit();
            this.tabLooseInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgLooseInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openGedcom;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel tsCount;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tsCountLabel;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTips;
        //private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.ToolStripMenuItem mnuPrint;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog;
        private System.Windows.Forms.PrintDialog printDialog;
        private System.Drawing.Printing.PrintDocument printDocument;
        private System.Windows.Forms.ContextMenuStrip mnuSetRoot;
        private System.Windows.Forms.ToolStripMenuItem setAsRootToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewOnlineManualToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem displayOptionsOnLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportAnIssueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuReload;
        private System.Windows.Forms.ToolStripStatusLabel tsHintsLabel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtTreetopsSurname;
        private FTAnalyzer.Forms.Controls.RelationTypes treetopsRelation;
        private System.Windows.Forms.TabPage tabDisplayProgress;
        private System.Windows.Forms.TabControl tabSelector;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripMenuItem whatsNewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.SaveFileDialog saveDatabase;
        private System.Windows.Forms.OpenFileDialog restoreDatabase;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem mnuRecent;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem clearRecentFileListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuRecent1;
        private System.Windows.Forms.ToolStripMenuItem mnuRecent2;
        private System.Windows.Forms.ToolStripMenuItem mnuRecent3;
        private System.Windows.Forms.ToolStripMenuItem mnuRecent4;
        private System.Windows.Forms.ToolStripMenuItem mnuRecent5;
        private System.Windows.Forms.ToolStripMenuItem resetToDefaultFormSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewNotesToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip ctxViewNotes;
        private System.Windows.Forms.ToolStripMenuItem mnuViewNotes;
        private System.Windows.Forms.CheckBox chkExcludeUnknownBirths;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCensusSurname;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown udAgeFilter;
        private FTAnalyzer.Forms.Controls.RelationTypes relTypesCensus;
        private System.Windows.Forms.ToolStripMenuItem onlineGuidesToUsingFTAnalyzerToolStripMenuItem;
        private System.Windows.Forms.ToolStripProgressBar tspbTabProgress;
        private System.Windows.Forms.ToolStripMenuItem mnuCloseGEDCOM;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripStatusLabel tsStatusLabel;
        private System.Windows.Forms.TabPage tabMainLists;
        private System.Windows.Forms.TabControl tabMainListsSelector;
        private System.Windows.Forms.TabPage tabIndividuals;
        private FTAnalyzer.Forms.Controls.VirtualDGVIndividuals dgIndividuals;
        private System.Windows.Forms.TabPage tabFamilies;
        private FTAnalyzer.Forms.Controls.VirtualDGVFamily dgFamilies;
        private System.Windows.Forms.TabPage tabSources;
        private FTAnalyzer.Forms.Controls.VirtualDGVSources dgSources;
        private System.Windows.Forms.TabPage tabErrorsFixes;
        private System.Windows.Forms.TabControl tabErrorFixSelector;
        private System.Windows.Forms.TabPage tabDataErrors;
        private FTAnalyzer.Forms.Controls.VirtualDGVDataErrors dgDataErrors;
        private System.Windows.Forms.GroupBox gbDataErrorTypes;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.CheckedListBox ckbDataErrors;
        private System.Windows.Forms.TabPage tabDuplicates;
        private System.Windows.Forms.CheckBox ckbHideIgnoredDuplicates;
        private System.Windows.Forms.Button btnCancelDuplicates;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TrackBar tbDuplicateScore;
        private System.Windows.Forms.Label labCalcDuplicates;
        private System.Windows.Forms.ProgressBar pbDuplicates;
        private FTAnalyzer.Forms.Controls.VirtualDGVDuplicates dgDuplicates;
        private System.Windows.Forms.TabPage tabLooseBirths;
        private System.Windows.Forms.DataGridView dgLooseBirths;
        private System.Windows.Forms.TabPage tabLooseDeaths;
        private System.Windows.Forms.DataGridView dgLooseDeaths;
        private System.Windows.Forms.CheckBox ckbTTIncludeOnlyOneParent;
        private System.Windows.Forms.ToolStripMenuItem privacyPolicyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.GroupBox Referrals;
        private System.Windows.Forms.CheckBox ckbReferralInCommon;
        private System.Windows.Forms.Button btnReferrals;
        private System.Windows.Forms.ComboBox cmbReferrals;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.SplitContainer splitGedcom;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label LbProgramName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ProgressBar pbRelationships;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ProgressBar pbFamilies;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ProgressBar pbIndividuals;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar pbSources;
        private Utilities.ScrollingRichTextBox rtbOutput;
        private System.Windows.Forms.Button btnViewInvalidRefs;
        private System.Windows.Forms.TabPage tabLooseInfo;
        private System.Windows.Forms.DataGridView dgLooseInfo;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnMismatchedChildrenStatus;
        private System.Windows.Forms.Button btnNoChildrenStatus;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btnReportUnrecognised;
        private System.Windows.Forms.Button btnRandomSurnameEntered;
        private System.Windows.Forms.Button btnRandomSurnameMissing;
        private System.Windows.Forms.DataGridView dgCheckAncestors;
        private System.Windows.Forms.Button btnCheckMyAncestors;
        private System.Windows.Forms.Label lblCheckAncestors;
        private Utilities.ScrollingRichTextBox rtbCheckAncestors;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TextBox txtAliveDates;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label labCompletion;
        private System.Windows.Forms.Label labDuplicateSlider;
        private System.Windows.Forms.DataGridViewCheckBoxColumn NonDuplicate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Score;
        private System.Windows.Forms.DataGridViewTextBoxColumn DuplicateIndividualID;
        private System.Windows.Forms.DataGridViewTextBoxColumn DuplicateName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DuplicateForenames;
        private System.Windows.Forms.DataGridViewTextBoxColumn DuplicateSurname;
        private System.Windows.Forms.DataGridViewTextBoxColumn DuplicateBirthDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn DuplicateBirthLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn MatchIndividualID;
        private System.Windows.Forms.DataGridViewTextBoxColumn MatchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn MatchBirthDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn MatchBirthLocation;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
    }
}

