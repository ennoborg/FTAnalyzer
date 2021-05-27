using FTAnalyzer.Forms;
using FTAnalyzer.Properties;
using FTAnalyzer.UserControls;
using FTAnalyzer.Utilities;
using GeneGenie.Gedcom;
using GeneGenie.Gedcom.Parser;
using HtmlAgilityPack;
using Printing.DataGridViewPrint.Tools;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FTAnalyzer
{
    public partial class MainForm : Form
    {
        public static string VERSION = "8.4.0.0";

        static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        readonly Cursor storedCursor = Cursors.Default;
        readonly FamilyTree ft = FamilyTree.Instance;
        bool stopProcessing;
        string filename;
        readonly PrivateFontCollection fonts = new PrivateFontCollection();
        Font handwritingFont;
        bool loading;
        ReportFormHelper rfhDuplicates;

        public MainForm()
        {
            try
            {
                InitializeComponent();
                loading = true;
                FamilyTree.Instance.Version = $"v{VERSION}";
                _ = NativeMethods.GetTaskBarPos(); // Sets taskbar offset
                displayOptionsOnLoadToolStripMenuItem.Checked = GeneralSettings.Default.ReportOptions;
                treetopsRelation.MarriedToDB = false;
                ShowMenus(false);
                log.Info($"Started FTAnalyzer version {VERSION}");
                int pos = VERSION.IndexOf('-');
                string ver = pos > 0 ? VERSION.Substring(0, VERSION.IndexOf('-')) : VERSION;
                CheckSystemVersion();
                if (!Application.ExecutablePath.Contains("WindowsApps"))
                    CheckWebVersion(); // check for web version if not windows store app
                SetSavePath();
                BuildRecentList();
            }
            catch (Exception e)
            {
                UIHelpers.ShowMessage($"Problem starting up error was : {e.Message}");
            }
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            SetupFonts();
            RegisterEventHandlers();
            Text = $"Family Tree Analyzer v{VERSION}";
            SetHeightWidth();
            rfhDuplicates = new ReportFormHelper(this, "Duplicates", dgDuplicates, ResetDuplicatesTable, "Duplicates", false);
            ft.LoadStandardisedNames(Application.StartupPath);
            tsCountLabel.Text = string.Empty;
            tsHintsLabel.Text = "Welcome to Family Tree Analyzer, if you have any questions please raise them on the User group - see help menu for details";
            loading = false;
        }

        void CheckSystemVersion()
        {
            OperatingSystem os = Environment.OSVersion;
            if (os.Version.Major == 6 && os.Version.Minor < 2)
                MessageBox.Show("Please note Microsoft has ended Windows 7 support as such it is no longer advisable to be connected to the internet using it. Any security flaws that are unpatched may be being actively exploited by hackers. You should upgrade as soon as possible.\n\nPlease be aware that FTAnalyzer may be unstable on an outdated unsupported Operating System.");
        }
        async void CheckWebVersion()
        {
            try
            {
                Settings.Default.StartTime = DateTime.Now;
                Settings.Default.Save();
                HtmlAgilityPack.HtmlDocument doc;
                using (WebClient wc = new WebClient())
                {
                    doc = new HtmlAgilityPack.HtmlDocument();
                    string webData = wc.DownloadString("https://github.com/ShammyLevva/FTAnalyzer/releases");
                    doc.LoadHtml(webData);
                }
                HtmlNode versionNode = doc.DocumentNode.SelectSingleNode("//div/div/div/span/../../ul/li/a");
                string webVersion = versionNode.InnerText.ToUpper().Replace('V', ' ').Trim();
                string thisVersion = VERSION;
                if (VERSION.Contains("-beta"))
                    thisVersion = VERSION.Substring(0, VERSION.IndexOf("-"));
                Version web = new Version(webVersion);
                Version local = new Version(thisVersion);
                if (web > local)
                {
                    string text = $"Version installed: {VERSION}, Web version available: {webVersion}\nDo you want to go to website to download the latest version?\nSelect Cancel to visit release website for older machines.";
                    DialogResult download = MessageBox.Show(text, "FTAnalyzer", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (download == DialogResult.Yes)
                        SpecialMethods.VisitWebsite("https://www.microsoft.com/en-gb/p/ftanalyzer/9pmjl9hvpl7x?cid=clickonceappupgrade");
                    if (download == DialogResult.Cancel)
                        SpecialMethods.VisitWebsite("https://github.com/ShammyLevva/FTAnalyzer/releases");
                }

                await Analytics.CheckProgramUsageAsync().ConfigureAwait(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        void SetupFonts()
        {
            try
            {
                SpecialMethods.SetFonts(this);
                byte[] fontData = Resources.KUNSTLER;
                IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
                System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
                uint dummy = 0;
                fonts.AddMemoryFont(fontPtr, Resources.KUNSTLER.Length);
                NativeMethods.AddFontMemResourceEx(fontPtr, (uint)Resources.KUNSTLER.Length, IntPtr.Zero, ref dummy);
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
                switch (FontSettings.Default.FontNumber)
                {
                    case 1:
                        handwritingFont = new Font(fonts.Families[0], 46.0F, FontStyle.Bold);
                        break;
                    case 2:
                        handwritingFont = new Font(fonts.Families[0], 68.0F, FontStyle.Bold);
                        break;
                    case 3:
                        handwritingFont = new Font(fonts.Families[0], 72.0F, FontStyle.Bold);
                        break;
                    case 4:
                        handwritingFont = new Font(fonts.Families[0], 90.0F, FontStyle.Bold);
                        break;
                }
                LbProgramName.Font = handwritingFont;
                pictureBox1.Left = LbProgramName.Right;
                splitGedcom.SplitterDistance = Math.Max(pbRelationships.Bottom + 18, 110);
                UpdateDataErrorsDisplay();
            }
            catch (Exception) { } // for font sizing exception
        }

        void RegisterEventHandlers()
        {
            Options.ReloadRequired += new EventHandler(Options_ReloadData);
            GeneralSettingsUI.MinParentalAgeChanged += new EventHandler(Options_MinimumParentalAgeChanged);
            GeneralSettingsUI.AliasInNameChanged += new EventHandler(Options_AliasInNameChanged);
            FontSettingsUI.GlobalFontChanged += new EventHandler(Options_GlobalFontChanged);
        }

        void SetHeightWidth()
        {
            MainForm mainForm = this;
            // load height & width from registry - note need to use temp variables as setting them causes form
            // to resize thus setting the values for both
            int Width = (int)Application.UserAppDataRegistry.GetValue("Mainform size - width", mainForm.Width);
            int Height = (int)Application.UserAppDataRegistry.GetValue("Mainform size - height", mainForm.Height);
            int Top = (int)Application.UserAppDataRegistry.GetValue("Mainform position - top", mainForm.Top);
            int Left = (int)Application.UserAppDataRegistry.GetValue("Mainform position - left", mainForm.Left);
            string maxState = (WindowState == FormWindowState.Maximized).ToString();
            string maximised = (string)Application.UserAppDataRegistry.GetValue("Mainform maximised", maxState);
            Point leftTop = ReportFormHelper.CheckIsOnScreen(Top, Left);
            mainForm.Width = Width;
            mainForm.Height = Height;
            mainForm.Top = leftTop.Y;
            mainForm.Left = leftTop.X;
            if (maximised == "True")
                WindowState = FormWindowState.Maximized;
        }

        #region Load File
        async Task LoadFileAsync(string filename)
        {
            try
            {
                HourGlass(true);
                this.filename = filename;
                CloseGEDCOM(false);
                if (!stopProcessing)
                {
                    if (await LoadTreeAsync(filename).ConfigureAwait(true))
                    {
                        SetDataErrorsCheckedDefaults(ckbDataErrors);
                        AddFileToRecentList(filename);
                        Text = $"Family Tree Analyzer v{VERSION}. Analysing: {filename}";
                        Application.UseWaitCursor = false;
                        mnuCloseGEDCOM.Enabled = true;
                        EnableLoadMenus();
                        ShowMenus(true);
                        MessageBox.Show($"Gedcom File {filename} Loaded", "FTAnalyzer");
                    }
                    else
                        CleanUp(true);
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Error: Could not read file from disk. Original error: {ex.Message}", "FTAnalyzer");
            }
            catch (Exception ex2)
            {
                string message = ex2.Message + "\n" + (ex2.InnerException != null ? ex2.InnerException.Message : string.Empty);
                MessageBox.Show("Error: Problem processing your file. Please try again.\n" +
                    "If this problem persists please report this at http://www.ftanalyzer.com/issues. Error was: " + message + "\n" + ex2.InnerException, "FTAnalyzer");
                CleanUp(true);
            }
            finally
            {
                HourGlass(false);
            }
        }

        async Task<bool> LoadTreeAsync(string filename)
        {
            var outputText = new Progress<string>(value => { rtbOutput.AppendText(value); });
            GedcomDatabase db;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            db = await Task.Run(() => LoadGedcomFromFile(filename)).ConfigureAwait(true);
            timer.Stop();
            WriteTime("File Loaded", outputText, timer);
            timer.Start();
            ft.DocumentLoaded = true;
            var sourceProgress = new Progress<int>(value => { pbSources.Value = value; });
            var individualProgress = new Progress<int>(value => { pbIndividuals.Value = value; });
            var familyProgress = new Progress<int>(value => { pbFamilies.Value = value; });
            var RelationshipProgress = new Progress<int>(value => { pbRelationships.Value = value; });
            await Task.Run(() => ft.LoadTreeSources(db, sourceProgress, outputText)).ConfigureAwait(true);
            await Task.Run(() => ft.LoadTreeIndividuals(db, individualProgress, outputText)).ConfigureAwait(true);
            await Task.Run(() => ft.LoadTreeFamilies(db, familyProgress, outputText)).ConfigureAwait(true);
            await Task.Run(() => ft.LoadTreeRelationships(db, RelationshipProgress, outputText)).ConfigureAwait(true);
            ft.DocumentLoaded = false;
            timer.Stop();
            WriteTime("\nFile Loaded and Analysed", outputText, timer);
            WriteMemory(outputText);
            return true;
        }

        void WriteTime(string prefixText, IProgress<string> outputText, Stopwatch timer)
        {
            TimeSpan ts = timer.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = string.Format("{0:00}h {1:00}m {2:00}.{3:00}s", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            outputText.Report($"{prefixText} in {elapsedTime}\n\n");
        }

        void WriteMemory(IProgress<string> outputText)
        {
            long memoryBefore = GC.GetTotalMemory(false);
            long memoryAfter = GC.GetTotalMemory(true);
            string sizeBefore = SpecialMethods.SizeSuffix(memoryBefore, 2);
            string sizeAfter = SpecialMethods.SizeSuffix(memoryAfter, 2);
            outputText.Report($"File used {sizeBefore} during loading, reduced to {sizeAfter} after processing.");
        }

        void EnableLoadMenus()
        {
            openToolStripMenuItem.Enabled = true;
        }

        void CloseGEDCOM(bool keepOutput)
        {
            DisposeIndividualForms();
            ShowMenus(false);
            tabSelector.SelectTab(tabDisplayProgress);
            if (!keepOutput)
                rtbOutput.Text = string.Empty;
            tsCountLabel.Text = string.Empty;
            tsHintsLabel.Text = string.Empty;
            tsStatusLabel.Text = string.Empty;
            rtbCheckAncestors.Text = string.Empty;
            pbSources.Value = 0;
            pbIndividuals.Value = 0;
            pbFamilies.Value = 0;
            pbRelationships.Value = 0;
            SetupGridControls();
            cmbReferrals.Items.Clear();
            cmbReferrals.Text = string.Empty;
            Statistics.Instance.Clear();
            btnReferrals.Enabled = false;
            openToolStripMenuItem.Enabled = false;
            mnuRecent.Enabled = false;
            tabMainListsSelector.SelectedTab = tabIndividuals; // force back to first tab
            tabErrorFixSelector.SelectedTab = tabDataErrors; //force tab back to data errors tab
            Text = "Family Tree Analyzer v" + VERSION;
        }

        void SetupGridControls()
        {
            dgIndividuals.DataSource = null;
            dgFamilies.DataSource = null;
            dgLooseBirths.DataSource = null;
            dgLooseDeaths.DataSource = null;
            dgLooseInfo.DataSource = null;
            dgDataErrors.DataSource = null;
            dgDuplicates.DataSource = null;
            dgSources.DataSource = null;
            ExtensionMethods.DoubleBuffered(dgIndividuals, true);
            ExtensionMethods.DoubleBuffered(dgFamilies, true);
            ExtensionMethods.DoubleBuffered(dgLooseBirths, true);
            ExtensionMethods.DoubleBuffered(dgLooseDeaths, true);
            ExtensionMethods.DoubleBuffered(dgLooseInfo, true);
            ExtensionMethods.DoubleBuffered(dgDataErrors, true);
            ExtensionMethods.DoubleBuffered(dgDuplicates, true);
            ExtensionMethods.DoubleBuffered(dgSources, true);
        }

        void SetSavePath()
        {
            try
            {
                GeneralSettings.Default.SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Family Tree Analyzer");
                if (!Directory.Exists(GeneralSettings.Default.SavePath))
                    Directory.CreateDirectory(GeneralSettings.Default.SavePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Found a problem starting up.\nPlease report this at http://www.ftanalyzer.com/issues\nThe error was :" + ex.Message, "FTAnalyzer");
            }
        }

        async void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Settings.Default.LoadLocation))
                openGedcom.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            else
                openGedcom.InitialDirectory = Settings.Default.LoadLocation;
            openGedcom.FileName = "*.ged";
            openGedcom.Filter = "GED files (*.ged)|*.ged|All files (*.*)|*.*";
            openGedcom.FilterIndex = 1;
            openGedcom.RestoreDirectory = true;

            if (openGedcom.ShowDialog() == DialogResult.OK)
            {
                await LoadFileAsync(openGedcom.FileName).ConfigureAwait(true);
                Settings.Default.LoadLocation = Path.GetFullPath(openGedcom.FileName);
                Settings.Default.Save();
                await Analytics.TrackAction(Analytics.MainFormAction, Analytics.LoadGEDCOMEvent).ConfigureAwait(true);
            }
        }

        void MnuCloseGEDCOM_Click(object sender, EventArgs e)
        {
            if (!loading)
                CleanUp(false);
        }

        void CleanUp(bool retainText)
        {
            CloseGEDCOM(retainText);
            ft.ResetData();
            EnableLoadMenus();
            mnuCloseGEDCOM.Enabled = false;
            BuildRecentList();
        }

        private static GedcomDatabase LoadGedcomFromFile(string filename)
        {
            var gedcomReader = GedcomRecordReader.CreateReader(filename);
            if (gedcomReader.Parser.ErrorState != GeneGenie.Gedcom.Enums.GedcomErrorState.NoError)
            {
                Console.WriteLine($"Could not read file, encountered error {gedcomReader.Parser.ErrorState} press a key to continue.");
                Console.ReadKey();
                return null;
            }

            return gedcomReader.Database;
        }
        #endregion

        void ShowMenus(bool enabled)
        {
            mnuPrint.Enabled = enabled;
            mnuReload.Enabled = enabled;
            mnuCloseGEDCOM.Enabled = enabled;
        }

        void HourGlass(bool on)
        {
            Cursor = on ? Cursors.WaitCursor : Cursors.Default;
            Application.DoEvents();
        }

        void RtbOutput_TextChanged(object sender, EventArgs e) => rtbOutput.ScrollToBottom();
        void RtbCheckAncestors_TextChanged(object sender, EventArgs e) => rtbCheckAncestors.ScrollToBottom();

        bool shutdown;

        async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!shutdown)
            {
                shutdown = true;
                e.Cancel = true;
                await Analytics.EndProgramAsync().ConfigureAwait(true);
                Close();
            }
            DatabaseHelper.Instance.Dispose();
            stopProcessing = true;
        }

        void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => SpecialMethods.VisitWebsite("http://forums.lc");

        void SetAsRootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            var ind = (Individual)dgIndividuals.CurrentRowDataBoundItem;
            if (ind != null)
            {
                var outputText = new Progress<string>(value => { rtbOutput.AppendText(value); });
                ft.UpdateRootIndividual(ind.IndividualID, null, outputText);
                dgIndividuals.Refresh();
                MessageBox.Show($"Root person set as {ind.Name}\n\n{ft.PrintRelationCount()}", "FTAnalyzer");
            }
            HourGlass(false);
        }

        void MnuSetRoot_Opened(object sender, EventArgs e)
        {
            var ind = (Individual)dgIndividuals.CurrentRowDataBoundItem;
            if (ind != null)
                viewNotesToolStripMenuItem.Enabled = ind.HasNotes;
        }

        void ViewNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            Individual ind = (Individual)dgIndividuals.CurrentRowDataBoundItem;
            if (ind != null)
            {
                Notes notes = new Notes(ind);
                notes.Show();
            }
            HourGlass(false);
        }

        #region DataErrors
        void CkbDataErrors_SelectedIndexChanged(object sender, EventArgs e) => UpdateDataErrorsDisplay();

        void UpdateDataErrorsDisplay()
        {
            HourGlass(true);
            SortableBindingList<IDisplayDataError> errors = DataErrors(ckbDataErrors);
            dgDataErrors.DataSource = errors;
            tsCountLabel.Text = Messages.Count + errors.Count;
            tsHintsLabel.Text = Messages.Hints_Individual;
            int index = 0;
            int maxwidth = 0;
            try
            {
                foreach (DataErrorGroup dataError in ckbDataErrors.Items)
                {
                    if (dataError.ToString().Length > maxwidth)
                        maxwidth = dataError.ToString().Length;
                    bool itemChecked = ckbDataErrors.GetItemChecked(index++);
                    Application.UserAppDataRegistry.SetValue(dataError.ToString(), itemChecked);
                }
            }
            catch (IOException)
            {
                UIHelpers.ShowMessage("Unable to save DataError preferences. Please check App has rights to save user preferences to registry.");
            }
            ckbDataErrors.ColumnWidth = (int)(maxwidth * FontSettings.Default.FontWidth);
            HourGlass(false);
        }

        public void SetDataErrorsCheckedDefaults(CheckedListBox list)
        {
            list.Items.Clear();
            foreach (DataErrorGroup dataError in ft.DataErrorTypes)
            {
                int index = list.Items.Add(dataError);
                bool itemChecked = Application.UserAppDataRegistry.GetValue(dataError.ToString(), "True").Equals("True");
                list.SetItemChecked(index, itemChecked);
            }
        }

        void BtnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ckbDataErrors.Items.Count; i++)
            {
                ckbDataErrors.SetItemChecked(i, true);
            }
            UpdateDataErrorsDisplay();
        }

        void BtnClearAll_Click(object sender, EventArgs e)
        {
            foreach (int indexChecked in ckbDataErrors.CheckedIndices)
            {
                ckbDataErrors.SetItemChecked(indexChecked, false);
            }
            UpdateDataErrorsDisplay();
        }

        void DgDataErrors_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataError error = (DataError)dgDataErrors.CurrentRowDataBoundItem;
                if (error.IsFamily)
                    ShowFamilyFacts((string)dgDataErrors.CurrentRow.Cells[nameof(IDisplayDataError.Reference)].Value);
                else
                    ShowFacts((string)dgDataErrors.CurrentRow.Cells[nameof(IDisplayDataError.Reference)].Value);
            }
        }

        void SetupDataErrors()
        {
            dgDataErrors.DataSource = DataErrors(ckbDataErrors);
            dgDataErrors.Focus();
            mnuPrint.Enabled = true;
            UpdateDataErrorsDisplay();
        }

        public static SortableBindingList<IDisplayDataError> DataErrors(CheckedListBox list)
        {
            var errors = new List<IDisplayDataError>();
            foreach (int indexChecked in list.CheckedIndices)
            {
                DataErrorGroup item = (DataErrorGroup)list.Items[indexChecked];
                errors.AddRange(item.Errors);
            }
            return new SortableBindingList<IDisplayDataError>(errors);
        }
        #endregion

        void ChildAgeProfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Statistics s = Statistics.Instance;
            Chart chart = new Chart();
            int[,,] stats = s.ChildrenBirthProfiles();
            chart.BuildChildBirthProfile(stats);
            DisposeDuplicateForms(chart);
            chart.Show();
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.BirthProfileEvent);
            MessageBox.Show(s.BuildOutput(stats), "Birth Profile Information");
        }

        void ViewOnlineManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.OnlineManualEvent);
            SpecialMethods.VisitWebsite("http://www.ftanalyzer.com");
        }

        void OnlineGuidesToUsingFTAnalyzerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.OnlineGuideEvent);
            SpecialMethods.VisitWebsite("http://www.ftanalyzer.com/guides");
        }

        void PrivacyPolicyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.PrivacyEvent);
            SpecialMethods.VisitWebsite("http://www.ftanalyzer.com/privacy");
        }

        void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
            Application.Exit();
        }

        void TabCtrlLocations_Selecting(object sender, TabControlCancelEventArgs e)
        {
            try
            {
                HourGlass(true); // turn on when tab selected so all the formatting gets hourglass
            }
            catch (Exception) // attempt to fix font issue
            { }
        }

        #region EventHandlers
        void Options_BaptismChanged(object sender, EventArgs e)
        {
            // do anything that needs doing when option changes
        }

        async void Options_ReloadData(object sender, EventArgs e) => await QueryReloadData().ConfigureAwait(true);

        void Options_MinimumParentalAgeChanged(object sender, EventArgs e)
        {
            ft.ResetLooseFacts();
            if (tabSelector.SelectedTab == tabErrorsFixes && tabErrorFixSelector.SelectedTab.Equals(tabLooseBirths))
                SetupLooseBirths();
            if (tabSelector.SelectedTab == tabErrorsFixes && tabErrorFixSelector.SelectedTab.Equals(tabLooseDeaths))
                SetupLooseDeaths();
        }

        void Options_AliasInNameChanged(object sender, EventArgs e) => ft.SetFullNames();

        void Options_GlobalFontChanged(object sender, EventArgs e)
        {
            HourGlass(true);
            SetupFonts();
            HourGlass(false);
        }
        #endregion

        #region Reload Data
        async Task QueryReloadData()
        {
            if (GeneralSettings.Default.ReloadRequired && ft.DataLoaded)
            {
                DialogResult dr = MessageBox.Show("This option requires the data to be refreshed.\n\nDo you want to reload now?\n\nClicking no will keep the data with the old option.", "Reload GEDCOM File", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                GeneralSettings.Default.ReloadRequired = false;
                GeneralSettings.Default.Save();
                if (dr == DialogResult.Yes)
                {
                    await LoadFileAsync(filename).ConfigureAwait(true);
                }
            }
        }

        async void ReloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GeneralSettings.Default.ReloadRequired = false;
            GeneralSettings.Default.Save();
            await LoadFileAsync(filename).ConfigureAwait(true);
        }
        #endregion

        bool preventExpand;

        void TreeViewLocations_BeforeCollapse(object sender, TreeViewCancelEventArgs e) => e.Cancel = preventExpand && e.Action == TreeViewAction.Collapse;

        void TreeViewLocations_BeforeExpand(object sender, TreeViewCancelEventArgs e) => e.Cancel = preventExpand && e.Action == TreeViewAction.Expand;

        void TreeViewLocations_MouseDown(object sender, MouseEventArgs e) => preventExpand = e.Clicks > 1;

        void DisplayOptionsOnLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GeneralSettings.Default.ReportOptions = displayOptionsOnLoadToolStripMenuItem.Checked;
            GeneralSettings.Default.Save();
        }

        void ReportAnIssueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.ReportIssueEvent);
            SpecialMethods.VisitWebsite("https://github.com/ShammyLevva/FTAnalyzer/issues");
        }

        void WhatsNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.WhatsNewEvent);
            SpecialMethods.VisitWebsite("http://ftanalyzer.com/Whats%20New%20in%20this%20Release");
        }

        #region Tab Control
        void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                mnuPrint.Enabled = false;
                tsCountLabel.Text = string.Empty;
                tsHintsLabel.Text = string.Empty;
                tspbTabProgress.Visible = false;
                if (ft.Loading)
                {
                    tabSelector.SelectedTab = tabDisplayProgress;
                }
                else
                {
                    if (!ft.DataLoaded)
                    {   // do not process anything if no GEDCOM yet loaded
                        if (tabSelector.SelectedTab != tabDisplayProgress)
                        {
                            tabSelector.SelectedTab = tabDisplayProgress;
                            MessageBox.Show(ErrorMessages.FTA_0002, "FTAnalyzer Error : FTA_0002");
                        }
                        return;
                    }
                    HourGlass(true);
                    if (tabSelector.SelectedTab == tabDisplayProgress)
                    {
                        mnuPrint.Enabled = true;
                    }
                    if (tabSelector.SelectedTab == tabMainLists)
                    {
                        if (dgIndividuals.DataSource == null)
                            SetupIndividualsTab(); // select individuals tab if first time opening main lists tab
                        Analytics.TrackAction(Analytics.MainFormAction, Analytics.MainListsEvent);
                    }
                    if (tabSelector.SelectedTab == tabErrorsFixes)
                    {
                        if (dgDataErrors.DataSource == null)
                            SetupDataErrors(); // select data errors tab if first time opening errors fixes tab
                        Analytics.TrackAction(Analytics.MainFormAction, Analytics.ErrorsFixesEvent);
                    }
                    HourGlass(false);
                }
            }
            catch (Exception) { }
        }

        void TabMainListSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabMainListsSelector.SelectedTab == tabIndividuals)
            {
                SetupIndividualsTab();
                Analytics.TrackAction(Analytics.MainListsAction, Analytics.IndividualsTabEvent);
            }
            else if (tabMainListsSelector.SelectedTab == tabFamilies)
            {
                SortableBindingList<IDisplayFamily> list = ft.AllDisplayFamilies;
                dgFamilies.DataSource = list;
                dgFamilies.Sort(dgFamilies.Columns[nameof(IDisplayFamily.FamilyID)], ListSortDirection.Ascending);
                dgFamilies.Focus();
                mnuPrint.Enabled = true;
                tsCountLabel.Text = Messages.Count + list.Count.ToString("N0");
                tsHintsLabel.Text = Messages.Hints_Family;
                Analytics.TrackAction(Analytics.MainListsAction, Analytics.FamilyTabEvent);
            }
            else if (tabMainListsSelector.SelectedTab == tabSources)
            {
                SortableBindingList<IDisplaySource> list = ft.AllDisplaySources;
                dgSources.DataSource = list;
                dgSources.Sort(dgSources.Columns[nameof(IDisplaySource.SourceID)], ListSortDirection.Ascending);
                dgSources.Focus();
                mnuPrint.Enabled = true;
                tsCountLabel.Text = Messages.Count + list.Count.ToString("N0");
                tsHintsLabel.Text = Messages.Hints_Sources;
                Analytics.TrackAction(Analytics.MainListsAction, Analytics.SourcesTabEvent);
            }
        }

        void SetupIndividualsTab()
        {
            SortableBindingList<IDisplayIndividual> list = ft.AllDisplayIndividuals;
            dgIndividuals.DataSource = list;
            dgIndividuals.Sort(dgIndividuals.Columns[nameof(IDisplayIndividual.IndividualID)], ListSortDirection.Ascending);
            dgIndividuals.AllowUserToResizeColumns = true;
            dgIndividuals.Focus();
            mnuPrint.Enabled = true;
            tsCountLabel.Text = Messages.Count + list.Count.ToString("N0");
            tsHintsLabel.Text = Messages.Hints_Individual;
        }

        async void TabErrorFixSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabErrorFixSelector.SelectedTab == tabDataErrors)
                SetupDataErrors();
            else if (tabErrorFixSelector.SelectedTab == tabDuplicates)
            {
                rfhDuplicates.LoadColumnLayout("DuplicatesColumns.xml");
                ckbHideIgnoredDuplicates.Checked = GeneralSettings.Default.HideIgnoredDuplicates;
                await SetPossibleDuplicates().ConfigureAwait(true);
                dgDuplicates.Focus();
                mnuPrint.Enabled = true;
                await Analytics.TrackAction(Analytics.ErrorsFixesAction, Analytics.DuplicatesTabEvent).ConfigureAwait(true);
            }
            if (tabErrorFixSelector.SelectedTab == tabLooseBirths)
            {
                if (dgLooseBirths.DataSource == null)
                    SetupLooseBirths();
                await Analytics.TrackAction(Analytics.ErrorsFixesAction, Analytics.LooseBirthsEvent).ConfigureAwait(true);
            }
            else if (tabErrorFixSelector.SelectedTab == tabLooseDeaths)
            {
                if (dgLooseDeaths.DataSource == null)
                    SetupLooseDeaths();
                await Analytics.TrackAction(Analytics.ErrorsFixesAction, Analytics.LooseDeathsEvent).ConfigureAwait(true);
            }
            else if (tabErrorFixSelector.SelectedTab == tabLooseInfo)
            {
                if (dgLooseInfo.DataSource == null)
                    SetupLooseInfo();
                await Analytics.TrackAction(Analytics.ErrorsFixesAction, Analytics.LooseInfoEvent).ConfigureAwait(true);
            }
        }

        #endregion

        #region ToolStrip Clicks
        void AboutToolStripMenuItem_Click(object sender, EventArgs e) => MessageBox.Show($"This is Family Tree Analyzer version {VERSION}", "FTAnalyzer");

        void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Options options = new Options();
                options.ShowDialog(this);
                options.Dispose();
                Analytics.TrackAction(Analytics.MainFormAction, Analytics.OptionsEvent);
            }
            catch (Exception) { }
        }

        #endregion

        #region Print Routines
        void MnuPrint_Click(object sender, EventArgs e)
        {
            try
            {
                printDocument = new PrintDocument();
                printDocument.DefaultPageSettings.Margins = new Margins(50, 50, 50, 25);
                printDocument.DefaultPageSettings.Landscape = true;
                printDialog.PrinterSettings.DefaultPageSettings.Margins = new Margins(50, 50, 50, 25);
                printDialog.PrinterSettings.DefaultPageSettings.Landscape = true;

                if (tabSelector.SelectedTab == tabDisplayProgress && ft.DataLoaded)
                {
                    if (printDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        using (Utilities.Printing p = new Utilities.Printing(rtbOutput))
                        {
                            printDocument.PrintPage += new PrintPageEventHandler(p.PrintPage);
                            printDocument.PrinterSettings = printDialog.PrinterSettings;
                            printDocument.DocumentName = "GEDCOM Load Results";
                            printDocument.Print();
                        }
                    }
                }
                if (tabSelector.SelectedTab == tabMainLists)
                {
                    if (tabMainListsSelector.SelectedTab == tabIndividuals)
                        PrintDataGrid(Orientation.Landscape, dgIndividuals, "List of Individuals");
                    else if (tabMainListsSelector.SelectedTab == tabFamilies)
                        PrintDataGrid(Orientation.Landscape, dgFamilies, "List of Families");
                    else if (tabMainListsSelector.SelectedTab == tabSources)
                        PrintDataGrid(Orientation.Landscape, dgSources, "List of Sources");
                }
                else if (tabSelector.SelectedTab == tabErrorsFixes)
                {
                    if (tabErrorFixSelector.SelectedTab == tabDuplicates)
                        PrintDataGrid(Orientation.Landscape, dgDuplicates, "ist of Potential Duplicates");
                    else if (tabErrorFixSelector.SelectedTab == tabDataErrors)
                        PrintDataGrid(Orientation.Landscape, dgDataErrors, "List of Data Errors");
                    else if (tabErrorFixSelector.SelectedTab == tabLooseBirths)
                        PrintDataGrid(Orientation.Landscape, dgLooseBirths, "List of Loose Births");
                    else if (tabErrorFixSelector.SelectedTab == tabLooseDeaths)
                        PrintDataGrid(Orientation.Landscape, dgLooseDeaths, "List of Loose Deaths");
                    else if (tabErrorFixSelector.SelectedTab == tabLooseInfo)
                        PrintDataGrid(Orientation.Landscape, dgLooseInfo, "List of Loose Births/Deaths");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Printing : {ex.Message}");
            }
        }

        enum Orientation { Landscape, Portrait }

        void PrintDataGrid(Orientation orientation, DataGridView dg, string title)
        {
            PrintingDataGridViewProvider.Create(printDocument, dg, true, true, true, new TitlePrintBlock(title), null, null);
            printDialog.PrinterSettings.DefaultPageSettings.Landscape = (orientation == Orientation.Landscape);
            printDialog.PrinterSettings.DefaultPageSettings.Margins.Left = 50;
            printDialog.PrinterSettings.DefaultPageSettings.Margins.Right = 50;
            printDialog.PrinterSettings.DefaultPageSettings.Margins.Top = 50;
            printDialog.PrinterSettings.DefaultPageSettings.Margins.Bottom = 50;
            if (printDialog.ShowDialog(this) == DialogResult.OK)
            {
                printDocument.DocumentName = title;
                printDocument.PrinterSettings = printDialog.PrinterSettings;
                printDocument.Print();
            }
        }
        #endregion

        #region Dispose Routines
        void DisposeIndividualForms()
        {
            try
            {
                List<Form> toDispose = new List<Form>();
                foreach (Form f in Application.OpenForms)
                {
                    if (!ReferenceEquals(f, this))
                        toDispose.Add(f);
                }
                foreach (Form f in toDispose)
                    f.Dispose();
            }
            catch (Exception) { }
        }

        public static void DisposeDuplicateForms(object form)
        {
            try
            {
                List<Form> toDispose = new List<Form>();
                foreach (Form f in Application.OpenForms)
                {
                    if (!ReferenceEquals(f, form) && f.GetType() == form.GetType())
                        if (form is Facts)
                        {
                            Facts newForm = form as Facts;
                            Facts oldForm = f as Facts;
                            if (oldForm.Individual != null && oldForm.Individual.Equals(newForm.Individual))
                                toDispose.Add(f);
                            if (oldForm.Family != null && oldForm.Family.Equals(newForm.Family))
                                toDispose.Add(f);
                        }
                        else
                            toDispose.Add(f);
                }
                foreach (Form f in toDispose)
                {
                    GC.SuppressFinalize(f);
                    if (f.Visible)
                        f.Close(); // call close method to force tidy up of forms & dispose
                    else
                        f.Dispose();
                }
            }
            catch (Exception) { }
        }
        #endregion

        #region Recent File List
        void ClearRecentFileListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearRecentList();
            BuildRecentList();
        }

        static void ClearRecentList()
        {
            Settings.Default.RecentFiles.Clear();
            for (int i = 0; i < 5; i++)
            {
                Settings.Default.RecentFiles.Add(string.Empty);
            }
            Settings.Default.Save();
        }

        void BuildRecentList()
        {
            if (Settings.Default.RecentFiles == null || Settings.Default.RecentFiles.Count != 5)
                ClearRecentList();
            bool added = false;
            int count = 0;
            for (int i = 0; i < 5; i++)
            {
                string name = Settings.Default.RecentFiles[i];
                if (name != null && name.Length > 0 && File.Exists(name))
                {
                    added = true;
                    mnuRecent.DropDownItems[i].Visible = true;
                    mnuRecent.DropDownItems[i].Text = ++count + ". " + name;
                    mnuRecent.DropDownItems[i].Tag = name;
                }
                else
                    mnuRecent.DropDownItems[i].Visible = false;
            }
            toolStripSeparator7.Visible = added;
            clearRecentFileListToolStripMenuItem.Visible = added;
            mnuRecent.Enabled = added;
        }

        void AddFileToRecentList(string filename)
        {
            string[] recent = new string[5];

            if (Settings.Default.RecentFiles != null)
            {
                int j = 1;
                for (int i = 0; i < Settings.Default.RecentFiles.Count; i++)
                {
                    if (Settings.Default.RecentFiles[i] != filename && File.Exists(Settings.Default.RecentFiles[i]))
                    {
                        recent[j++] = Settings.Default.RecentFiles[i];
                        if (j == 5) break;
                    }
                }
            }

            recent[0] = filename;
            Settings.Default.RecentFiles = new StringCollection();
            Settings.Default.RecentFiles.AddRange(recent);
            Settings.Default.Save();

            BuildRecentList();
        }

        async void OpenRecentFile_Click(object sender, EventArgs e)
        {
            string filename = (string)(sender as ToolStripMenuItem).Tag;
            await LoadFileAsync(filename).ConfigureAwait(true);
        }

        void MnuRecent_DropDownOpening(object sender, EventArgs e) => BuildRecentList();
        #endregion

        void DgFamilies_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string famID = (string)dgFamilies.CurrentRow.Cells[nameof(IDisplayFamily.FamilyID)].Value;
                Family fam = ft.GetFamily(famID);
                if (fam != null)
                {
                    Facts factForm = new Facts(fam);
                    DisposeDuplicateForms(factForm);
                    factForm.Show();
                }
            }
        }

        void DgLooseDeaths_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                ShowFacts((string)dgLooseDeaths.CurrentRow.Cells[nameof(IDisplayLooseDeath.IndividualID)].Value);
        }

        void DgLooseBirths_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                ShowFacts((string)dgLooseBirths.CurrentRow.Cells[nameof(IDisplayLooseBirth.IndividualID)].Value);
        }

        void DgLooseInfo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                ShowFacts((string)dgLooseInfo.CurrentRow.Cells[nameof(IDisplayLooseInfo.IndividualID)].Value);
        }

        void DgIndividuals_MouseDown(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hti = dgIndividuals.HitTest(e.Location.X, e.Location.Y);
            if (e.Button == MouseButtons.Right)
            {
                var ht = dgIndividuals.HitTest(e.X, e.Y);
                if (ht.Type != DataGridViewHitTestType.ColumnHeader)
                {
                    if (hti.RowIndex >= 0 && hti.ColumnIndex >= 0)
                    {
                        dgIndividuals.CurrentCell = dgIndividuals.Rows[hti.RowIndex].Cells[hti.ColumnIndex];
                        // Can leave these here - doesn't hurt
                        dgIndividuals.Rows[hti.RowIndex].Selected = true;
                        dgIndividuals.Focus();
                        mnuSetRoot.Show(MousePosition);
                    }
                }
            }
            if (hti.RowIndex >= 0 && hti.ColumnIndex >= 0)
            {
                if (dgIndividuals.Rows[hti.RowIndex].Cells[hti.ColumnIndex].GetType() == typeof(DataGridViewLinkCell))
                {
                    string familySearchID = dgIndividuals.Rows[hti.RowIndex].Cells[hti.ColumnIndex].Value.ToString();
                    if (!string.IsNullOrEmpty(familySearchID))
                    {
                        string url = $"https://www.familysearch.org/tree/person/details/{familySearchID}";
                        SpecialMethods.VisitWebsite(url);
                    }
                }
                else if (e.Clicks == 2)
                {
                    string indID = (string)dgIndividuals.CurrentRow.Cells[nameof(IDisplayIndividual.IndividualID)].Value;
                    ShowFacts(indID);
                }
            }
        }

        void DgIndividuals_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string indID = (string)dgIndividuals.CurrentRow.Cells[nameof(IDisplayIndividual.IndividualID)].Value;
                ShowFacts(indID);
            }
        }

        void DgSources_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                FactSource source = (FactSource)dgSources.CurrentRowDataBoundItem;
                Facts factForm = new Facts(source);
                DisposeDuplicateForms(factForm);
                factForm.Show();
            }
        }

        void DgDuplicates_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (pbDuplicates.Visible || e.RowIndex < 0 || e.ColumnIndex < 0)
                return; // do nothing if progress bar still visible
            string indA_ID = (string)dgDuplicates.CurrentRow.Cells[nameof(IDisplayDuplicateIndividual.IndividualID)].Value;
            string indB_ID = (string)dgDuplicates.CurrentRow.Cells[nameof(IDisplayDuplicateIndividual.MatchIndividualID)].Value;
            if (GeneralSettings.Default.MultipleFactForms)
            {
                ShowFacts(indA_ID);
                ShowFacts(indB_ID, true);
            }
            else
            {
                List<Individual> dupInd = new List<Individual>
                {
                    ft.GetIndividual(indA_ID),
                    ft.GetIndividual(indB_ID)
                };
                Facts f = new Facts(dupInd, null, null);
                DisposeDuplicateForms(f);
                f.Show();
            }
        }

        #region Facts Tab
        void ShowFacts(string indID, bool offset = false)
        {
            Individual ind = ft.GetIndividual(indID);
            if (ind != null)
            {
                Facts factForm = new Facts(ind);
                DisposeDuplicateForms(factForm);
                factForm.Show();
                if (offset)
                {
                    factForm.Left += 200;
                    factForm.Top += 100;
                }
            }
        }

        void ShowFamilyFacts(string famID, bool offset = false)
        {
            Family fam = ft.GetFamily(famID);
            if (fam != null)
            {
                Facts factForm = new Facts(fam);
                DisposeDuplicateForms(factForm);
                factForm.Show();
                if (offset)
                {
                    factForm.Left += 200;
                    factForm.Top += 100;
                }
            }
        }

        #endregion

        #region Form Drag Drop
        async void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            bool fileLoaded = false;
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            foreach (string filename in files)
            {
                if (Path.GetExtension(filename.ToLower()) == ".ged")
                {
                    fileLoaded = true;
                    await LoadFileAsync(filename).ConfigureAwait(true);
                    break;
                }
            }
            if (!fileLoaded)
                if (files.Length > 1)
                    MessageBox.Show("Unable to load File. None of the files dragged and dropped were *.ged files", "FTAnalyzer");
                else
                    MessageBox.Show("Unable to load File. The file dragged and dropped wasn't a *.ged file", "FTAnalyzer");
        }

        void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }
        #endregion

        #region Manage Form Position
        void ResetToDefaultFormSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDefaultPosition();
            SavePosition();
        }

        void LoadDefaultPosition()
        {
            loading = true;
            Height = 561;
            Width = 1114;
            Top = 50 + NativeMethods.TopTaskbarOffset;
            Left = 50;
            loading = false;
        }

        void MainForm_Resize(object sender, EventArgs e)
        {
            try
            {
                splitGedcom.Height = 100;
                SavePosition();
            }
            catch (Exception) { }
        }

        void MainForm_Move(object sender, EventArgs e) => SavePosition();

        void SavePosition()
        {
            if (!loading && WindowState != FormWindowState.Minimized)
            {  //only save window size if not minimised
                try
                {
                    Application.UserAppDataRegistry.SetValue("Mainform size - width", Width);
                    Application.UserAppDataRegistry.SetValue("Mainform size - height", Height);
                    Application.UserAppDataRegistry.SetValue("Mainform position - top", Top);
                    Application.UserAppDataRegistry.SetValue("Mainform position - left", Left);
                    string maxState = (WindowState == FormWindowState.Maximized).ToString();
                    Application.UserAppDataRegistry.SetValue("Mainform maximised", maxState);
                }
                catch (IOException)
                {
                    UIHelpers.ShowMessage("Unable to save window permissions please check App has rights to save user preferences to registry");
                }
            }
        }
        #endregion

        #region Duplicates Tab
        CancellationTokenSource cts;
        SortableBindingList<IDisplayDuplicateIndividual> duplicateData;

        async Task SetPossibleDuplicates()
        {
            SetDuplicateControlsVisibility(true);
            rfhDuplicates.SaveColumnLayout("DuplicatesColumns.xml");
            var progress = new Progress<int>(value =>
            {
                if (value < 0)
                    value = 0;
                if (value > pbDuplicates.Maximum)
                    value = pbDuplicates.Maximum;
                pbDuplicates.Value = value;
            });
            var progressText = new Progress<string>(value => labCompletion.Text = value);
            var maxScore = new Progress<int>(value =>
            {
                tbDuplicateScore.TickFrequency = value / 20;
                tbDuplicateScore.SetRange(1, value);
            });
            cts = new CancellationTokenSource();
            int score = tbDuplicateScore.Value;
            labDuplicateSlider.Text = $"Match Quality : {tbDuplicateScore.Value}  ";
            tsCountLabel.Text = "Calculating Duplicates this may take some considerable time";
            tsHintsLabel.Text = string.Empty;
            duplicateData = await Task.Run(() => ft.GenerateDuplicatesList(score, progress, progressText, maxScore, cts.Token)).ConfigureAwait(true);
            cts = null;
            if (duplicateData != null)
            {
                dgDuplicates.DataSource = duplicateData;
                rfhDuplicates.LoadColumnLayout("DuplicatesColumns.xml");
                tsCountLabel.Text = $"Possible Duplicate Count : {dgDuplicates.RowCount:N0}.  {Messages.Hints_Duplicates}";
                dgDuplicates.UseWaitCursor = false;
            }
            SetDuplicateControlsVisibility(false);
            HourGlass(false);
        }

        void SetDuplicateControlsVisibility(bool visible)
        {
            btnCancelDuplicates.Visible = visible;
            labCalcDuplicates.Visible = visible;
            pbDuplicates.Visible = visible;
            labCompletion.Visible = visible;
        }

        void ResetDuplicatesTable()
        {
            if (dgDuplicates.RowCount > 0)
            {
                dgDuplicates.Sort(dgDuplicates.Columns[nameof(IDisplayDuplicateIndividual.Score)], ListSortDirection.Descending);
            }
        }

        async void TbDuplicateScore_Scroll(object sender, EventArgs e)
        {
            // do nothing if progress bar still visible
            if (!pbDuplicates.Visible)
                await SetPossibleDuplicates().ConfigureAwait(true);
        }

        void BtnCancelDuplicates_Click(object sender, EventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
                MessageBox.Show("Possible Duplicate Search Cancelled", "FTAnalyzer");
            }
        }

        void DgDuplicates_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0 && !pbDuplicates.Visible) // don't do anything if progressbar still loading duplicates
            {
                DisplayDuplicateIndividual dupInd = (DisplayDuplicateIndividual)dgDuplicates.Rows[e.RowIndex].DataBoundItem;
                NonDuplicate nonDup = new NonDuplicate(dupInd);
                dupInd.IgnoreNonDuplicate = !dupInd.IgnoreNonDuplicate; // flip state of checkbox
                if (dupInd.IgnoreNonDuplicate)
                {  //ignoring this record so add it to the list if its not already present
                    if (!ft.NonDuplicates.ContainsDuplicate(nonDup))
                        ft.NonDuplicates.Add(nonDup);
                }
                else
                    ft.NonDuplicates.Remove(nonDup); // no longer ignoring so remove from list
                ft.SerializeNonDuplicates();
            }
        }

        async void CkbHideIgnoredDuplicates_CheckedChanged(object sender, EventArgs e)
        {
            if (pbDuplicates.Visible)
                return; // do nothing if progress bar still visible
            GeneralSettings.Default.HideIgnoredDuplicates = ckbHideIgnoredDuplicates.Checked;
            GeneralSettings.Default.Save();
            await SetPossibleDuplicates().ConfigureAwait(true);
        }

        #endregion

        #region Loose Birth/Death Tabs
        void SetupLooseBirths()
        {
            try
            {
                SortableBindingList<IDisplayLooseBirth> looseBirthList = ft.LooseBirths();
                dgLooseBirths.DataSource = looseBirthList;
                dgLooseBirths.Sort(dgLooseBirths.Columns["Forenames"], ListSortDirection.Ascending);
                dgLooseBirths.Sort(dgLooseBirths.Columns["Surname"], ListSortDirection.Ascending);
                dgLooseBirths.Focus();
                mnuPrint.Enabled = true;
                tsCountLabel.Text = Messages.Count + looseBirthList.Count;
                tsHintsLabel.Text = Messages.Hints_Loose_Births + Messages.Hints_Individual;

            }
            catch (LooseDataException ex)
            {
                MessageBox.Show(ex.Message, "FTAnalyzer");
            }
        }

        void SetupLooseDeaths()
        {
            try
            {
                SortableBindingList<IDisplayLooseDeath> looseDeathList = ft.LooseDeaths();
                dgLooseDeaths.DataSource = looseDeathList;
                dgLooseDeaths.Sort(dgLooseDeaths.Columns["Forenames"], ListSortDirection.Ascending);
                dgLooseDeaths.Sort(dgLooseDeaths.Columns["Surname"], ListSortDirection.Ascending);
                dgLooseDeaths.Focus();
                mnuPrint.Enabled = true;
                tsCountLabel.Text = Messages.Count + looseDeathList.Count;
                tsHintsLabel.Text = Messages.Hints_Loose_Deaths + Messages.Hints_Individual;
            }
            catch (LooseDataException ex)
            {
                MessageBox.Show(ex.Message, "FTAnalyzer");
            }
        }

        void SetupLooseInfo()
        {
            try
            {
                SortableBindingList<IDisplayLooseInfo> looseInfoList = ft.LooseInfo();
                dgLooseInfo.DataSource = looseInfoList;
                dgLooseInfo.Sort(dgLooseInfo.Columns["Forenames"], ListSortDirection.Ascending);
                dgLooseInfo.Sort(dgLooseInfo.Columns["Surname"], ListSortDirection.Ascending);
                dgLooseInfo.Focus();
                mnuPrint.Enabled = true;
                tsCountLabel.Text = Messages.Count + looseInfoList.Count;
                tsHintsLabel.Text = "Double click to view records. " + Messages.Hints_Individual;
            }
            catch (LooseDataException ex)
            {
                MessageBox.Show(ex.Message, "FTAnalyzer");
            }
        }

        #endregion

        #region View Notes
        void CtxViewNotes_Opening(object sender, CancelEventArgs e)
        {
            Individual ind = GetContextIndividual(sender);
            if (ind != null)
                mnuViewNotes.Enabled = ind.HasNotes;
            else
                e.Cancel = true;
        }

        Individual GetContextIndividual(object sender)
        {
            Individual ind = null;
            ContextMenuStrip cms = null;
            if (sender is ContextMenuStrip strip)
                cms = strip;
            if (sender is ToolStripMenuItem tsmi)
                cms = (ContextMenuStrip)tsmi.Owner;
            if (cms != null && cms.Tag != null)
                ind = (Individual)cms.Tag;
            return ind;
        }

        void MnuViewNotes_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            Individual ind = GetContextIndividual(sender);
            if (ind != null)
            {
                Notes notes = new Notes(ind);
                notes.Show();
            }
            HourGlass(false);
        }
        #endregion

        #region Referrals
        void CmbReferrals_Click(object sender, EventArgs e)
        {
            if (cmbReferrals.Items.Count == 0)
            {
                HourGlass(true);
                List<Individual> list = ft.AllIndividuals.ToList();
                list.Sort(new NameComparer<Individual>(true, false));
                foreach (Individual ind in list)
                    cmbReferrals.Items.Add(ind);
                btnReferrals.Enabled = true;
                HourGlass(false);
            }
        }

        void BtnReferrals_Click(object sender, EventArgs e)
        {
            if (cmbReferrals.SelectedItem is Individual selected)
            {
                HourGlass(true);
                Individual root = ft.RootPerson;
                ft.SetRelations(selected.IndividualID, null);
                ft.SetRelations(root.IndividualID, null);
                HourGlass(false);
            }
        }
        #endregion

        FactDate AliveDate { get; set; }

        void TxtAliveDates_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAliveDates.Text))
            {
                txtAliveDates.Text = "Enter valid GEDCOM date/date range";
                AliveDate = FactDate.UNKNOWN_DATE;
                return;
            }
            FactDate aliveDate = FactDate.UNKNOWN_DATE;
            HourGlass(true);
            try
            {
                aliveDate = new FactDate(txtAliveDates.Text);
            }
            catch (FactDateException)
            {
                aliveDate = FactDate.UNKNOWN_DATE;
            }
            HourGlass(false);
            if (aliveDate == FactDate.UNKNOWN_DATE)
            {
                e.Cancel = true;
                MessageBox.Show($"{txtAliveDates.Text} is not a valid GEDCOM date.", "FTAnalyzer");
                return;
            }
            AliveDate = aliveDate;
        }

        void TxtAliveDates_Enter(object sender, EventArgs e)
        {
            if (txtAliveDates.Text.StartsWith("Enter"))
                txtAliveDates.Text = string.Empty;
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var openGramps = new OpenFileDialog();
            XDocument doc;

            if (string.IsNullOrEmpty(Settings.Default.LoadLocation))
                openGramps.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            else
                openGramps.InitialDirectory = Settings.Default.LoadLocation;
            openGramps.FileName = "*.gramps";
            openGramps.Filter = "Gramps files (*.gramps)|*.gramps|All files (*.*)|*.*";
            openGramps.FilterIndex = 1;
            openGramps.RestoreDirectory = true;

            if (openGramps.ShowDialog() == DialogResult.OK)
            {
                var grampsParser = new GrampsProject.GrampsXML();

                doc = grampsParser.Load(openGramps.FileName);
                Settings.Default.LoadLocation = Path.GetFullPath(openGramps.FileName);
                Settings.Default.Save();
            }
        }
    }
}