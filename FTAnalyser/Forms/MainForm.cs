using FTAnalyzer.Forms.Controls;
using FTAnalyzer.Exports;
using FTAnalyzer.Filters;
using FTAnalyzer.Forms;
using FTAnalyzer.Properties;
using FTAnalyzer.UserControls;
using FTAnalyzer.Utilities;
using Ionic.Zip;
using Printing.DataGridViewPrint.Tools;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Net;
using System.Diagnostics;
using GeneGenie.Gedcom;
using GeneGenie.Gedcom.Parser;

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
        Font boldFont;
        Font normalFont;
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
                DatabaseHelper.Instance.CheckDatabaseVersion(new Version(ver));
                CheckSystemVersion();
                if (!Application.ExecutablePath.Contains("WindowsApps"))
                    CheckWebVersion(); // check for web version if not windows store app
                SetSavePath();
                BuildRecentList();
            } catch (Exception e)
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
                boldFont = new Font(dgCountries.DefaultCellStyle.Font, FontStyle.Bold);
                normalFont = new Font(dgCountries.DefaultCellStyle.Font, FontStyle.Regular);
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
                        SetupFactsCheckboxes();
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
            databaseToolStripMenuItem.Enabled = true;
            mnuRestore.Enabled = false;
            mnuLoadLocationsCSV.Enabled = false;
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
            rtbToday.Text = string.Empty;
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
            databaseToolStripMenuItem.Enabled = false;
            mnuRecent.Enabled = false;
            tabMainListsSelector.SelectedTab = tabIndividuals; // force back to first tab
            tabErrorFixSelector.SelectedTab = tabDataErrors; //force tab back to data errors tab
            tabCtrlLocations.SelectedTab = tabTreeView; // otherwise totals etc look wrong
            treeViewLocations.Nodes.Clear();
            Text = "Family Tree Analyzer v" + VERSION;
        }

        void SetupGridControls()
        {
            dgPlaces.DataSource = null; // set datasources for locations in reverse order to avoid null pointer cell formatting race condition
            dgAddresses.DataSource = null;
            dgSubRegions.DataSource = null;
            dgRegions.DataSource = null;
            dgCountries.DataSource = null;
            dgIndividuals.DataSource = null;
            dgFamilies.DataSource = null;
            dgLooseBirths.DataSource = null;
            dgLooseDeaths.DataSource = null;
            dgLooseInfo.DataSource = null;
            dgDataErrors.DataSource = null;
            dgOccupations.DataSource = null;
            dgSurnames.DataSource = null;
            dgDuplicates.DataSource = null;
            dgSources.DataSource = null;
            ExtensionMethods.DoubleBuffered(dgPlaces, true);
            ExtensionMethods.DoubleBuffered(dgAddresses, true);
            ExtensionMethods.DoubleBuffered(dgSubRegions, true);
            ExtensionMethods.DoubleBuffered(dgRegions, true);
            ExtensionMethods.DoubleBuffered(dgCountries, true);
            ExtensionMethods.DoubleBuffered(dgIndividuals, true);
            ExtensionMethods.DoubleBuffered(dgFamilies, true);
            ExtensionMethods.DoubleBuffered(dgLooseBirths, true);
            ExtensionMethods.DoubleBuffered(dgLooseDeaths, true);
            ExtensionMethods.DoubleBuffered(dgLooseInfo, true);
            ExtensionMethods.DoubleBuffered(dgDataErrors, true);
            ExtensionMethods.DoubleBuffered(dgOccupations, true);
            ExtensionMethods.DoubleBuffered(dgSurnames, true);
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
            mnuRestore.Enabled = true;
            mnuLoadLocationsCSV.Enabled = true;
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
            mnuFactsToExcel.Enabled = enabled;
            mnuIndividualsToExcel.Enabled = enabled;
            mnuFamiliesToExcel.Enabled = enabled;
            MnuExportLocations.Enabled = enabled;
            mnuSourcesToExcel.Enabled = enabled;
            mnuDataErrorsToExcel.Enabled = enabled;
            mnuSurnamesToExcel.Enabled = enabled;
            mnuLooseBirthsToExcel.Enabled = enabled;
            mnuLooseDeathsToExcel.Enabled = enabled;
            mnuChildAgeProfiles.Enabled = enabled;
            mnuOlderParents.Enabled = enabled;
            mnuBirthdayEffect.Enabled = enabled;
            mnuPossibleCensusFacts.Enabled = enabled;
            mnuPossiblyMissingChildReport.Enabled = enabled;
            mnuShowTimeline.Enabled = enabled;
            mnuGeocodeLocations.Enabled = enabled;
            mnuOSGeocoder.Enabled = enabled;
            mnuLocationsGeocodeReport.Enabled = enabled;
            mnuLifelines.Enabled = enabled;
            mnuPlaces.Enabled = enabled;
            mnuCousinsCountReport.Enabled = enabled;
            mnuHowManyGreats.Enabled = enabled;
            MnuAgedOver99Report.Enabled = enabled;
            mnuLookupBlankFoundLocations.Enabled = enabled;
            MnuSingleParentsReport.Enabled = enabled;
            mnuDNA_GEDCOM.Enabled = enabled;
            mnuJSON.Enabled = enabled;
        }

        void HourGlass(bool on)
        {
            Cursor = on ? Cursors.WaitCursor : Cursors.Default;
            Application.DoEvents();
        }

        void DgCountries_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                HourGlass(true);
                var loc = (FactLocation)dgCountries.CurrentRow.DataBoundItem;
                var frmInd = new People();
                frmInd.SetLocation(loc, FactLocation.COUNTRY);
                DisposeDuplicateForms(frmInd);
                frmInd.Show();
                HourGlass(false);
            }
        }

        void DgRegions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                HourGlass(true);
                var loc = dgRegions.CurrentRow == null ? FactLocation.BLANK_LOCATION : (FactLocation)dgRegions.CurrentRow.DataBoundItem;
                var frmInd = new People();
                frmInd.SetLocation(loc, FactLocation.REGION);
                DisposeDuplicateForms(frmInd);
                frmInd.Show();
                HourGlass(false);
            }
        }

        void DgSubRegions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                HourGlass(true);
                var loc = (FactLocation)dgSubRegions.CurrentRow.DataBoundItem;
                var frmInd = new People();
                frmInd.SetLocation(loc, FactLocation.SUBREGION);
                DisposeDuplicateForms(frmInd);
                frmInd.Show();
                HourGlass(false);
            }
        }

        void DgAddresses_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                HourGlass(true);
                var loc = (FactLocation)dgAddresses.CurrentRow.DataBoundItem;
                var frmInd = new People();
                frmInd.SetLocation(loc, FactLocation.ADDRESS);
                DisposeDuplicateForms(frmInd);
                frmInd.Show();
                HourGlass(false);
            }
        }

        void DgPlaces_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                HourGlass(true);
                var loc = (FactLocation)dgPlaces.CurrentRow.DataBoundItem;
                var frmInd = new People();
                frmInd.SetLocation(loc, FactLocation.PLACE);
                DisposeDuplicateForms(frmInd);
                frmInd.Show();
                HourGlass(false);
            }
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

        void DgOccupations_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                HourGlass(true);
                var occ = (DisplayOccupation)dgOccupations.CurrentRowDataBoundItem;
                var frmInd = new People();
                frmInd.SetWorkers(occ.Occupation, ft.AllWorkers(occ.Occupation));
                DisposeDuplicateForms(frmInd);
                frmInd.Show();
                HourGlass(false);
            }
        }

        void DgCustomFacts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                HourGlass(true);
                var customFacts = (DisplayCustomFact)dgCustomFacts.CurrentRowDataBoundItem;
                var frmInd = new People();
                frmInd.SetCustomFacts(customFacts.CustomFactName, ft.AllCustomFactIndividuals(customFacts.CustomFactName));
                DisposeDuplicateForms(frmInd);
                frmInd.Show();
                HourGlass(false);
            }
        }

        void DgCustomFacts_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var customFact = (DisplayCustomFact)dgCustomFacts.CurrentRowDataBoundItem;
            DatabaseHelper.IgnoreCustomFact(customFact.CustomFactName, customFact.Ignore);
        }

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

        void BtnShowMap_Click(object sender, EventArgs e)
        {
            float zoom = GetMapZoomLevel(out FactLocation loc);
            if (loc != null && loc.IsGeoCoded(false))
            {
                string URL = $"https://www.google.com/maps/@{loc.Latitude},{loc.Longitude},{zoom}z";
                SpecialMethods.VisitWebsite(URL);
            }
            else
                MessageBox.Show($"{loc} is not yet geocoded so can't be displayed.");
        }

        void BtnOSMap_Click(object sender, EventArgs e)
        {
            bool oldOSMap = (sender as Button).Name == "btnOldOSMap";
            {
                float zoom = GetMapZoomLevel(out FactLocation loc);
                if (loc != null && loc.IsGeoCoded(false))
                {
                    if (loc.IsWithinUKBounds)
                    {
                        if (oldOSMap)
                        {
                            string URL = $"https://maps.nls.uk/geo/explore/#zoom={zoom}&lat={loc.Latitude}&lon={loc.Longitude}&layers=1&b=1";
                            SpecialMethods.VisitWebsite(URL);
                        }
                    }
                    else
                        MessageBox.Show($"{loc} is outwith the UK so cannot be shown on a UK OS Map.");
                }
                else
                    MessageBox.Show($"{loc} is not yet geocoded so can't be displayed.");
            }
        }

        float GetMapZoomLevel(out FactLocation loc)
        {
            // get the tab
            loc = null;
            try
            {
                switch (tabCtrlLocations.SelectedTab.Text)
                {
                    case "Tree View":
                        TreeNode node = treeViewLocations.SelectedNode;
                        if (node != null)
                            loc = node.Text == "<blank>" ? null : ((FactLocation)node.Tag).GetLocation(node.Level);
                        break;
                    case "Countries":
                        loc = dgCountries.CurrentRow == null ? null : (FactLocation)dgCountries.CurrentRow.DataBoundItem;
                        break;
                    case "Regions":
                        loc = dgRegions.CurrentRow == null ? null : (FactLocation)dgRegions.CurrentRow.DataBoundItem;
                        break;
                    case "SubRegions":
                        loc = dgSubRegions.CurrentRow == null ? null : (FactLocation)dgSubRegions.CurrentRow.DataBoundItem;
                        break;
                    case "Addresses":
                        loc = dgAddresses.CurrentRow == null ? null : (FactLocation)dgAddresses.CurrentRow.DataBoundItem;
                        break;
                    case "Places":
                        loc = dgPlaces.CurrentRow == null ? null : (FactLocation)dgPlaces.CurrentRow.DataBoundItem;
                        break;
                }
                if (loc == null)
                {
                    if (tabCtrlLocations.SelectedTab.Text == "Tree View")
                        MessageBox.Show("Location selected isn't valid to show on the map.", "FTAnalyzer");
                    else
                        MessageBox.Show("Nothing selected. Please select a location to show on the map.", "FTAnalyzer");
                    return 0f;
                }
                return loc.ZoomLevel;
            }
            catch (NullReferenceException)
            {
                return 0f;
            }
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

        void OlderParentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            People frmInd = new People();
            string inputAge = "50";
            DialogResult result = DialogResult.Cancel;
            int age = 0;
            do
            {
                try
                {
                    result = InputBox.Show("Enter age between 13 and 90", "Please select minimum age to report on", ref inputAge);
                    age = int.Parse(inputAge);
                }
                catch (Exception)
                {
                    if (result != DialogResult.Cancel)
                        MessageBox.Show("Invalid Age entered", "FTAnalyzer");
                }
                if (age < 13 || age > 90)
                    MessageBox.Show("Please enter an age between 13 and 90", "FTAnalyzer");
            } while ((result != DialogResult.Cancel) && (age < 13 || age > 90));
            if (result == DialogResult.OK)
            {
                if (frmInd.OlderParents(age))
                {
                    DisposeDuplicateForms(frmInd);
                    frmInd.Show();
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.OlderParentsEvent);
                }
            }
            HourGlass(false);
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

        void TabCtrlLocations_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                HourGlass(true);
                Application.DoEvents();
                TabPage current = tabCtrlLocations.SelectedTab;
                Control control = current.Controls[0];
                control.Focus();
                if (control is DataGridView)
                {
                    DataGridView dg = control as DataGridView;
                    tsCountLabel.Text = $"{Messages.Count}{dg.RowCount} {dg.Name.Substring(2)}";
                    mnuPrint.Enabled = true;
                }
                else
                {
                    tsCountLabel.Text = string.Empty;
                    mnuPrint.Enabled = false;
                }
                tsHintsLabel.Text = Messages.Hints_Location;
                HourGlass(false);
            }
            catch (Exception) // attempt to fix font issue
            { }
        }

        #region CellFormatting
        void FormatCellLocations(DataGridView grid, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                DataGridViewCell cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (e.ColumnIndex == 0)
                {
                    string country = (string)cell.Value;
                    if (Countries.IsKnownCountry(country))
                        e.CellStyle.Font = boldFont;
                    else
                        e.CellStyle.Font = normalFont;
                }
                else if (e.ColumnIndex == 1)
                {
                    string region = (string)cell.Value;
                    if (region.Length > 0 && Regions.IsKnownRegion(region))
                        e.CellStyle.Font = boldFont;
                    else
                        e.CellStyle.Font = normalFont;
                }
                else
                {
                    FactLocation loc = grid.Rows[e.RowIndex].DataBoundItem as FactLocation;
                    cell.ToolTipText = $"Geocoding Status : {loc.Geocoded}";
                }
            }
            catch (Exception) { }
        }

        void DgCountries_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0 || e.ColumnIndex == dgCountries?.Columns["Icon"].Index)
                FormatCellLocations(dgCountries, e);
        }

        void DgRegions_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex <= 1 || e.ColumnIndex == dgCountries?.Columns["Icon"].Index)
                FormatCellLocations(dgRegions, e);
        }

        void DgSubRegions_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex <= 1 || e.ColumnIndex == dgCountries?.Columns["Icon"].Index)
                FormatCellLocations(dgSubRegions, e);
        }

        void DgAddresses_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex <= 1 || e.ColumnIndex == dgCountries?.Columns["Icon"].Index)
                FormatCellLocations(dgAddresses, e);
        }

        void DgPlaces_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex <= 1 || e.ColumnIndex == dgCountries?.Columns["Icon"].Index)
                FormatCellLocations(dgPlaces, e);
        }
        #endregion

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

        void TreeViewLocations_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            HourGlass(true);
            var location = e.Node.Tag as FactLocation;
            if (location != null)
            {
                if (ft.CountPeopleAtLocation(location, e.Node.Level) == 0)
                    UIHelpers.ShowMessage($"You have no one in your file at {location}.");
                else
                {
                    var frmInd = new People();
                    frmInd.SetLocation(location, e.Node.Level);
                    DisposeDuplicateForms(frmInd);
                    frmInd.Show();
                }
            }
            HourGlass(false);
        }

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

        void MnuShowTimeline_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            TimeLine tl = new TimeLine(new Progress<string>(value => { rtbOutput.AppendText(value); }));
            DisposeDuplicateForms(tl);
            tl.Show();
            HourGlass(false);
            Analytics.TrackAction(Analytics.MapsAction, Analytics.ShowTimelinesEvent);
        }

        enum GecodingType { Google = 1, OS = 2, Reverse = 3 }

        void MnuGeocodeLocations_Click(object sender, EventArgs e)
        {
            StartGeocoding(GecodingType.Google);
            Analytics.TrackAction(Analytics.GeocodingAction, Analytics.GoogleGeocodingEvent);
        }

        void MnuOSGeocoder_Click(object sender, EventArgs e)
        {
            StartGeocoding(GecodingType.OS);
            Analytics.TrackAction(Analytics.GeocodingAction, Analytics.OSGeocodingEvent);
        }

        void MnuLookupBlankFoundLocations_Click(object sender, EventArgs e)
        {
            StartGeocoding(GecodingType.Reverse);
            Analytics.TrackAction(Analytics.GeocodingAction, Analytics.ReverseGeocodingEvent);
        }

        void StartGeocoding(GecodingType type)
        {
            if (!ft.Geocoding) // don't geocode if another geocode session in progress
            {
                try
                {
                    HourGlass(true);
                    GeocodeLocations geo = null;
                    foreach (Form f in Application.OpenForms)
                    {
                        if (f is GeocodeLocations)
                        {
                            geo = f as GeocodeLocations;
                            break;
                        }
                    }
                    if (geo == null)
                        geo = new GeocodeLocations(new Progress<string>(value => { rtbOutput.AppendText(value); }));
                    geo.Show();
                    geo.Focus();
                    Application.DoEvents();
                    switch (type)
                    {
                        case GecodingType.Google:
                            geo.StartGoogleGeoCoding(false);
                            break;
                        case GecodingType.OS:
                            geo.StartOSGeoCoding();
                            break;
                        case GecodingType.Reverse:
                            geo.StartReverseGeoCoding();
                            break;
                    }
                    HourGlass(false);
                }
                catch (Exception) { }
            }
        }

        void LocationsGeocodeReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            GeocodeLocations geo = new GeocodeLocations(new Progress<string>(value => { rtbOutput.AppendText(value); }));
            DisposeDuplicateForms(geo);
            geo.Show();
            HourGlass(false);
            Analytics.TrackAction(Analytics.MapsAction, Analytics.GeocodesEvent);
        }

        void TreeViewLocations_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (treeViewLocations.SelectedNode != e.Node && e.Button.Equals(MouseButtons.Right))
                    treeViewLocations.SelectedNode = e.Node;
            }
            catch (Exception) { }
        }

        void TreeViewLocations_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                treeViewLocations.SelectedImageIndex = e.Node.ImageIndex;
            }
            catch (Exception) { }
        }

        void MnuLifelines_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            LifeLine l = new LifeLine(new Progress<string>(value => { rtbOutput.AppendText(value); }));
            DisposeDuplicateForms(l);
            l.Show();
            HourGlass(false);
            Analytics.TrackAction(Analytics.MapsAction, Analytics.LifelinesEvent);
        }

        void MnuPlaces_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            Places p = new Places(new Progress<string>(value => { rtbOutput.AppendText(value); }));
            DisposeDuplicateForms(p);
            p.Show();
            HourGlass(false);
            Analytics.TrackAction(Analytics.MapsAction, Analytics.ShowPlacesEvent);
        }

        void DgSurnames_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                HourGlass(true);
                IDisplaySurnames stat = dgSurnames.CurrentRowDataBoundItem;
                People frmInd = new People();
                frmInd.SetSurnameStats(stat, chkSurnamesIgnoreCase.Checked);
                DisposeDuplicateForms(frmInd);
                frmInd.Show();
                HourGlass(false);
                Analytics.TrackAction(Analytics.MainFormAction, Analytics.ViewAllSurnameEvent);
            }
        }

        void DgSurnames_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                DataGridViewCell cell = dgSurnames.Rows[e.RowIndex].Cells[nameof(IDisplaySurnames.Surname)];
                if (cell.Value != null)
                {
                    Statistics.DisplayGOONSpage(cell.Value.ToString());
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.GOONSEvent);
                }
            }
        }

        void PossibleCensusFactsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            var predicate = new Predicate<Individual>(x => x.Notes.ToLower().Contains("census"));
            var censusNotes = ft.AllIndividuals.Filter(predicate).ToList<Individual>();
            var people = new People();
            people.SetIndividuals(censusNotes, "List of Possible Census records incorrectly recorded as notes");
            DisposeDuplicateForms(people);
            people.Show();
            HourGlass(false);
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.PossibleCensusEvent);
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
                            mnuRestore.Enabled = true;
                            mnuLoadLocationsCSV.Enabled = true;
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
                    else if (tabSelector.SelectedTab == tabFacts)
                    {
                        // already cleared text don't need to do anything else
                        Analytics.TrackAction(Analytics.MainFormAction, Analytics.FactsTabEvent);
                    }
                    else if (tabSelector.SelectedTab == tabSurnames)
                    {
                        // show empty form click button to load
                        Analytics.TrackAction(Analytics.MainFormAction, Analytics.SurnamesTabEvent);
                    }
                    else if (tabSelector.SelectedTab == tabToday)
                    {
                        bool todaysMonth = Application.UserAppDataRegistry.GetValue("Todays Events Month", "False").Equals("True");
                        int todaysStep = int.Parse(Application.UserAppDataRegistry.GetValue("Todays Events Step", "5").ToString());
                        rbTodayMonth.Checked = todaysMonth;
                        nudToday.Value = todaysStep;
                        Analytics.TrackAction(Analytics.MainFormAction, Analytics.TodayTabEvent);
                    }
                    else if (tabSelector.SelectedTab == tabLocations)
                    {
                        HourGlass(true);
                        tabCtrlLocations.SelectedIndex = 0;
                        tsCountLabel.Text = string.Empty;
                        tsHintsLabel.Text = Messages.Hints_Location;
                        tspbTabProgress.Visible = true;
                        treeViewLocations.Nodes.Clear();
                        Application.DoEvents();
                        TreeNode[] nodes = TreeViewHandler.Instance.GetAllLocationsTreeNodes(treeViewLocations.Font, true, tspbTabProgress);
                        try
                        {
                            treeViewLocations.Nodes.AddRange(nodes);
                        }
                        catch (ArgumentException fEx)
                        {
                            Console.WriteLine(fEx.Message); // typically font loading error
                        }
                        mnuPrint.Enabled = false;
                        dgCountries.DataSource = ft.AllDisplayCountries;
                        dgRegions.DataSource = ft.AllDisplayRegions;
                        dgSubRegions.DataSource = ft.AllDisplaySubRegions;
                        dgAddresses.DataSource = ft.AllDisplayAddresses;
                        dgPlaces.DataSource = ft.AllDisplayPlaces;
                        tspbTabProgress.Visible = false;
                        Analytics.TrackAction(Analytics.MainFormAction, Analytics.LocationTabViewed);
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
            else if (tabMainListsSelector.SelectedTab == tabOccupations)
            {
                SortableBindingList<IDisplayOccupation> list = ft.AllDisplayOccupations;
                dgOccupations.DataSource = list;
                dgOccupations.Sort(dgOccupations.Columns[nameof(IDisplayOccupation.Occupation)], ListSortDirection.Ascending);
                dgOccupations.Focus();
                mnuPrint.Enabled = true;
                tsCountLabel.Text = Messages.Count + list.Count.ToString("N0");
                tsHintsLabel.Text = Messages.Hints_Occupation;
                Analytics.TrackAction(Analytics.MainListsAction, Analytics.OccupationsTabEvent);
            }
            else if (tabMainListsSelector.SelectedTab == tabCustomFacts)
            {
                SortableBindingList<IDisplayCustomFact> list = ft.AllCustomFacts;
                dgCustomFacts.DataSource = list;
                dgCustomFacts.Sort(dgCustomFacts.Columns[nameof(IDisplayCustomFact.CustomFactName)], ListSortDirection.Ascending);
                dgCustomFacts.Focus();
                dgCustomFacts.Columns[nameof(IDisplayCustomFact.Ignore)].ReadOnly = false;
                dgCustomFacts.Columns[nameof(IDisplayCustomFact.Ignore)].ToolTipText = "Tick box to ignore warnings for this custom fact type.";
                mnuPrint.Enabled = true;
                tsCountLabel.Text = Messages.Count + list.Count.ToString("N0");
                tsHintsLabel.Text = Messages.Hints_CustomFacts;
                Analytics.TrackAction(Analytics.MainListsAction, Analytics.CustomFactTabEvent);
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

        #region Filters
        Predicate<ExportFact> CreateFactsFilter()
        {
            var filter = relTypesFacts.BuildFilter<ExportFact>(x => x.RelationType);
            if (txtFactsSurname.Text.Length > 0)
            {
                var surnameFilter = FilterUtils.StringFilter<ExportFact>(x => x.Surname, txtFactsSurname.Text.Trim());
                filter = FilterUtils.AndFilter(filter, surnameFilter);
            }
            return filter;
        }

        Predicate<Individual> CreateAliveatDateFilter(FactDate aliveDate, string surname)
        {
            var relationFilter = relTypesCensus.BuildFilter<Individual>(x => x.RelationType);
            var dateFilter = new Predicate<Individual>(x => x.IsPossiblyAlive(aliveDate));
            Predicate<Individual> filter = FilterUtils.AndFilter(relationFilter, dateFilter);
            if (surname.Length > 0)
            {
                Predicate<Individual> surnameFilter = FilterUtils.StringFilter<Individual>(x => x.Surname, surname);
                filter = FilterUtils.AndFilter(filter, surnameFilter);
            }
            if (chkExcludeUnknownBirths.Checked)
                filter = FilterUtils.AndFilter(x => x.BirthDate.IsKnown, filter);
            return filter;
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
                    else if (tabMainListsSelector.SelectedTab == tabOccupations)
                        PrintDataGrid(Orientation.Portrait, dgOccupations, "List of Occupations");
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
                else if (tabSelector.SelectedTab == tabLocations)
                {
                    if (tabCtrlLocations.SelectedTab == tabCountries)
                        PrintDataGrid(Orientation.Portrait, dgCountries, "List of Countries");
                    if (tabCtrlLocations.SelectedTab == tabRegions)
                        PrintDataGrid(Orientation.Portrait, dgRegions, "List of Regions");
                    if (tabCtrlLocations.SelectedTab == tabSubRegions)
                        PrintDataGrid(Orientation.Portrait, dgSubRegions, "List of Sub Regions");
                    if (tabCtrlLocations.SelectedTab == tabAddresses)
                        PrintDataGrid(Orientation.Portrait, dgAddresses, "List of Addresses");
                    if (tabCtrlLocations.SelectedTab == tabPlaces)
                        PrintDataGrid(Orientation.Portrait, dgPlaces, "List of Places");
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

        #region Backup/Restore Database
        void BackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ft.Geocoding)
                MessageBox.Show("You need to stop Geocoding before you can export the database", "FTAnalyzer");
            else
            {
                DatabaseHelper.Instance.BackupDatabase(saveDatabase, $"FTAnalyzer zip file created by v{VERSION}");
                Analytics.TrackAction(Analytics.MainFormAction, Analytics.DBBackupEvent);
            }
        }

        void RestoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ft.Geocoding)
                MessageBox.Show("You need to stop Geocoding before you can import the database", "FTAnalyzer");
            else
            {
                string directory = Application.UserAppDataRegistry.GetValue("Geocode Backup Directory", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)).ToString();
                restoreDatabase.FileName = "*.zip";
                restoreDatabase.InitialDirectory = directory;
                DialogResult result = restoreDatabase.ShowDialog();
                if (result == DialogResult.OK && File.Exists(restoreDatabase.FileName))
                {
                    HourGlass(true);
                    bool failed = false;
                    using (ZipFile zip = new ZipFile(restoreDatabase.FileName))
                    {
                        if (zip.Count == 1 && zip.ContainsEntry("Geocodes.s3db"))
                        {
                            DatabaseHelper dbh = DatabaseHelper.Instance;
                            if (DatabaseHelper.StartBackupRestoreDatabase())
                            {
                                File.Copy(dbh.DatabaseFile, dbh.CurrentFilename, true); // copy exisiting file to safety
                                zip.ExtractAll(dbh.DatabasePath, ExtractExistingFileAction.OverwriteSilently);
                                if (dbh.RestoreDatabase(new Progress<string>(value => { rtbOutput.AppendText(value); })))
                                    MessageBox.Show("Database restored from " + restoreDatabase.FileName, "FTAnalyzer Database Restore Complete");
                                else
                                {
                                    File.Copy(dbh.CurrentFilename, dbh.DatabaseFile, true);
                                    dbh.RestoreDatabase(new Progress<string>(value => { rtbOutput.AppendText(value); })); // restore original database
                                    failed = true;
                                }
                            }
                            else
                                MessageBox.Show("Database file could not be extracted", "FTAnalyzer Database Restore Error");
                        }
                        else
                        {
                            failed = true;
                        }
                        if (failed)
                            MessageBox.Show(restoreDatabase.FileName + " doesn't appear to be an FTAnalyzer database", "FTAnalyzer Database Restore Error");
                        else
                            Analytics.TrackAction(Analytics.MainFormAction, Analytics.DBRestoreEvent);
                    }
                    HourGlass(false);
                }
            }
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
        void SetupFactsCheckboxes()
        {
            Predicate<ExportFact> filter = CreateFactsFilter();
            SetFactTypeList(ckbFactSelect, ckbFactExclude, filter);
            SetShowFactsButton();
        }

        void RelTypesFacts_RelationTypesChanged(object sender, EventArgs e) => SetupFactsCheckboxes();

        void TxtFactsSurname_TextChanged(object sender, EventArgs e) => SetupFactsCheckboxes();

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

        void BtnShowFacts_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            Predicate<Individual> filter = relTypesFacts.BuildFilter<Individual>(x => x.RelationType);
            if (txtFactsSurname.Text.Length > 0)
            {
                Predicate<Individual> surnameFilter = FilterUtils.StringFilter<Individual>(x => x.Surname, txtFactsSurname.Text);
                filter = FilterUtils.AndFilter<Individual>(filter, surnameFilter);
            }
            Facts facts = new Facts(ft.AllIndividuals.Filter(filter), BuildFactTypeList(ckbFactSelect, true), BuildFactTypeList(ckbFactExclude, true));
            facts.Show();
            HourGlass(false);
        }

        List<string> BuildFactTypeList(CheckedListBox list, bool includeCreated)
        {
            List<string> result = new List<string>();
            if (list == ckbFactExclude && ckbFactExclude.Visible == false)
                return result; // if we aren't looking to exclude facts don't pass anything to list of exclusions
            int index = 0;
            foreach (string factType in list.Items)
            {
                if (list.GetItemChecked(index++))
                {
                    if (includeCreated)
                        result.Add(factType);
                    else
                        if (factType != Fact.GetFactTypeDescription(Fact.PARENT) && factType != Fact.GetFactTypeDescription(Fact.CHILDREN))
                        result.Add(factType);
                }
            }
            return result;
        }

        void BtnSelectAllFactTypes_Click(object sender, EventArgs e) => SetFactTypes(ckbFactSelect, true, "Fact: ");

        void BtnDeselectAllFactTypes_Click(object sender, EventArgs e) => SetFactTypes(ckbFactSelect, false, "Fact: ");

        void SetFactTypes(CheckedListBox list, bool selected, string registryPrefix)
        {
            for (int index = 0; index < list.Items.Count; index++)
            {
                string factType = list.Items[index].ToString();
                list.SetItemChecked(index, selected);
                try
                {
                    Application.UserAppDataRegistry.SetValue(registryPrefix + factType, selected);
                }
                catch (IOException)
                {
                    UIHelpers.ShowMessage("Unable to save fact selection preferences. Please check App has permission to save user preferences to registry.");
                }
            }
            SetShowFactsButton();
        }

        void CkbFactSelect_MouseClick(object sender, MouseEventArgs e)
        {
            int index = ckbFactSelect.IndexFromPoint(e.Location);
            if (index >= 0)
            {
                string factType = ckbFactSelect.Items[index].ToString();
                bool selected = ckbFactSelect.GetItemChecked(index);
                ckbFactSelect.SetItemChecked(index, !selected);
                try
                {
                    Application.UserAppDataRegistry.SetValue("Fact: " + factType, !selected);
                }
                catch (IOException)
                {
                    UIHelpers.ShowMessage("Unable to save fact selection preferences. Please check App has permission to save user preferences to registry.");
                }
                SetShowFactsButton();
            }
        }

        void SetShowFactsButton()
        {
            if (ckbFactSelect.CheckedItems.Count == 0 && ckbFactExclude.CheckedItems.Count > 0)
                btnShowFacts.Text = "Show all Facts for Individuals who are missing the selected excluded Fact Types";
            else
                btnShowFacts.Text = "Show only the selected Facts for Individuals" + (ckbFactExclude.Visible ? " who don't have any of the excluded Fact Types" : string.Empty);
            btnShowFacts.Enabled = ckbFactSelect.CheckedItems.Count > 0 || (ckbFactExclude.Visible && ckbFactExclude.CheckedItems.Count > 0);
        }

        void BtnExcludeAllFactTypes_Click(object sender, EventArgs e) => SetFactTypes(ckbFactExclude, true, "Exclude Fact: ");

        void BtnDeselectExcludeAllFactTypes_Click(object sender, EventArgs e) => SetFactTypes(ckbFactExclude, false, "Exclude Fact: ");

        void BtnShowExclusions_Click(object sender, EventArgs e)
        {
            bool visible = !ckbFactExclude.Visible;
            ckbFactExclude.Visible = visible;
            btnExcludeAllFactTypes.Visible = visible;
            btnDeselectExcludeAllFactTypes.Visible = visible;
            lblExclude.Visible = visible;
            SetShowFactsButton();
        }

        void CkbFactExclude_MouseClick(object sender, MouseEventArgs e)
        {
            int index = ckbFactExclude.IndexFromPoint(e.Location);
            string factType = ckbFactExclude.Items[index].ToString();
            bool selected = ckbFactExclude.GetItemChecked(index);
            ckbFactExclude.SetItemChecked(index, !selected);
            try
            {
                Application.UserAppDataRegistry.SetValue("Exclude Fact: " + factType, !selected);
            }
            catch (IOException)
            {
                UIHelpers.ShowMessage("Unable to save fact exclusion preferences. Please check App has permission to save user preferences to registry.");
            }
            SetShowFactsButton();
        }

        void BtnDuplicateFacts_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            Predicate<Individual> filter = relTypesFacts.BuildFilter<Individual>(x => x.RelationType);
            if (txtFactsSurname.Text.Length > 0)
            {
                Predicate<Individual> surnameFilter = FilterUtils.StringFilter<Individual>(x => x.Surname, txtFactsSurname.Text);
                filter = FilterUtils.AndFilter<Individual>(filter, surnameFilter);
            }
            Facts facts = new Facts(ft.AllIndividuals.Filter(filter), BuildFactTypeList(ckbFactSelect, false));
            facts.Show();
            HourGlass(false);
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
                rtbToday.Top = dpToday.Top + 30;
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

        void ShowViewNotesMenu(VirtualDataGridView<IDisplayIndividual> dg, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hti = dg.HitTest(e.Location.X, e.Location.Y);
            if (e.Button == MouseButtons.Right)
            {
                var ht = dg.HitTest(e.X, e.Y);
                if (ht.Type != DataGridViewHitTestType.ColumnHeader)
                {
                    if (hti.RowIndex >= 0 && hti.ColumnIndex >= 0)
                    {
                        dg.CurrentCell = dg.Rows[hti.RowIndex].Cells[hti.ColumnIndex];
                        // Can leave these here - doesn't hurt
                        dg.Rows[hti.RowIndex].Selected = true;
                        dg.Focus();
                        ctxViewNotes.Tag = dg.CurrentRowDataBoundItem;
                        ctxViewNotes.Show(MousePosition);
                    }
                }
            }
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

        #region Export To Excel
        void IndividualsToExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            ListtoDataTableConvertor convertor = new ListtoDataTableConvertor();
            using (DataTable dt = convertor.ToDataTable(new List<IExportIndividual>((IEnumerable<IExportIndividual>)ft.AllIndividuals)))
                ExportToExcel.Export(dt);
            Analytics.TrackAction(Analytics.ExportAction, Analytics.ExportIndEvent);
            HourGlass(false);
        }

        void FamiliesToExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            ListtoDataTableConvertor convertor = new ListtoDataTableConvertor();
            using (DataTable dt = convertor.ToDataTable(new List<IDisplayFamily>(ft.AllFamilies)))
                ExportToExcel.Export(dt);
            Analytics.TrackAction(Analytics.ExportAction, Analytics.ExportFamEvent);
            HourGlass(false);
        }

        void FactsToExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            ListtoDataTableConvertor convertor = new ListtoDataTableConvertor();
            using (DataTable dt = convertor.ToDataTable(new List<ExportFact>(ft.AllExportFacts)))
                ExportToExcel.Export(dt);
            Analytics.TrackAction(Analytics.ExportAction, Analytics.ExportFactsEvent);
            HourGlass(false);
        }

        void LooseBirthsToExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            try
            {
                ListtoDataTableConvertor convertor = new ListtoDataTableConvertor();
                List<IDisplayLooseBirth> list = ft.LooseBirths().ToList();
                list.Sort(new LooseBirthComparer());
                using (DataTable dt = convertor.ToDataTable(list))
                    ExportToExcel.Export(dt);
                Analytics.TrackAction(Analytics.ExportAction, Analytics.ExportLooseBirthsEvent);
            }
            catch (LooseDataException ex)
            {
                MessageBox.Show(ex.Message, "FTAnalyzer");
            }
            HourGlass(false);
        }

        void LooseDeathsToExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            try
            {
                ListtoDataTableConvertor convertor = new ListtoDataTableConvertor();
                List<IDisplayLooseDeath> list = ft.LooseDeaths().ToList();
                list.Sort(new LooseDeathComparer());
                using (DataTable dt = convertor.ToDataTable(list))
                    ExportToExcel.Export(dt);
                Analytics.TrackAction(Analytics.ExportAction, Analytics.ExportLooseDeathsEvent);
            }
            catch (LooseDataException ex)
            {
                MessageBox.Show(ex.Message, "FTAnalyzer");
            }
            HourGlass(false);
        }

        void MnuExportLocations_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            ListtoDataTableConvertor convertor = new ListtoDataTableConvertor();
            using (DataTable dt = convertor.ToDataTable(new List<IDisplayLocation>(ft.AllDisplayPlaces)))
                ExportToExcel.Export(dt);
            Analytics.TrackAction(Analytics.ExportAction, Analytics.ExportLocationsEvent);
            HourGlass(false);
        }

        void MnuSourcesToExcel_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            ListtoDataTableConvertor convertor = new ListtoDataTableConvertor();
            using (DataTable dt = convertor.ToDataTable(new List<IDisplaySource>(ft.AllSources)))
                ExportToExcel.Export(dt);
            Analytics.TrackAction(Analytics.ExportAction, Analytics.ExportSourcesEvent);
            HourGlass(false);
        }

        void MnuDataErrorsToExcel_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            ListtoDataTableConvertor convertor = new ListtoDataTableConvertor();
            using (DataTable dt = convertor.ToDataTable(new List<IDisplayDataError>(DataErrors(ckbDataErrors))))
                ExportToExcel.Export(dt);
            Analytics.TrackAction(Analytics.ExportAction, Analytics.ExportDataErrorsEvent);
            HourGlass(false);
        }

        async void MnuSurnamesToExcel_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            ListtoDataTableConvertor convertor = new ListtoDataTableConvertor();
            SortableBindingList<IDisplaySurnames> stats;
            if (dgSurnames.DataSource != null)
                stats = dgSurnames.DataSource;
            else
            {
                tspbTabProgress.Visible = true;
                Predicate<Individual> indFilter = reltypesSurnames.BuildFilter<Individual>(x => x.RelationType);
                Predicate<Family> famFilter = reltypesSurnames.BuildFamilyFilter<Family>(x => x.RelationTypes);
                var progress = new Progress<int>(value => { tspbTabProgress.Value = value; });
                stats = await Task.Run(() =>
                    new SortableBindingList<IDisplaySurnames>(Statistics.Instance.Surnames(indFilter, famFilter, progress, chkSurnamesIgnoreCase.Checked))).ConfigureAwait(true);
                tspbTabProgress.Visible = false;
            }
            List<IDisplaySurnames> list = new List<IDisplaySurnames>(stats);
            using (DataTable dt = convertor.ToDataTable(list))
                ExportToExcel. Export(dt);
            await Analytics.TrackAction(Analytics.ExportAction, Analytics.ExportSurnamesEvent);
            HourGlass(false);
        }
        #endregion

        #region Today

        async Task ShowTodaysEvents()
        {
            pbToday.Visible = true;
            labToday.Visible = true;
            rtbToday.ResetText();
            Progress<int> progress = new Progress<int>(value => { pbToday.Value = value; });
            Progress<string> outputText = new Progress<string>(text => { rtbToday.Rtf = text; });
            await Task.Run(() => ft.AddTodaysFacts(dpToday.Value, rbTodayMonth.Checked, (int)nudToday.Value, progress, outputText)).ConfigureAwait(true);
            labToday.Visible = false;
            pbToday.Visible = false;
            await Analytics.TrackAction(Analytics.MainFormAction, Analytics.TodayClickedEvent).ConfigureAwait(true);
        }

        void RbTodayMonth_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Application.UserAppDataRegistry.SetValue("Todays Events Month", rbTodayMonth.Checked);
            }
            catch (IOException)
            {
                UIHelpers.ShowMessage("Unable to save Today preference. Please check App has rights to save user preferences to registry.");
            }
        }

        void RbTodaySingle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Application.UserAppDataRegistry.SetValue("Todays Events Month", !rbTodaySingle.Checked);
            }
            catch (IOException)
            {
                UIHelpers.ShowMessage("Unable to save Today preference. Please check App has rights to save user preferences to registry.");
            }
        }

        async void BtnUpdateTodaysEvents_Click(object sender, EventArgs e) => await ShowTodaysEvents().ConfigureAwait(true);

        void NudToday_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Application.UserAppDataRegistry.SetValue("Todays Events Step", nudToday.Value);
            }
            catch (IOException)
            {
                UIHelpers.ShowMessage("Unable to save Today preference. Please check App has rights to save user preferences to registry.");
            }
        }
        #endregion

        public void SetFactTypeList(CheckedListBox ckbFactSelect, CheckedListBox ckbFactExclude, Predicate<ExportFact> filter)
        {
            List<string> factTypes = ft.AllExportFacts.Filter(filter).Select(x => x.FactType).Distinct().ToList();
            factTypes.Sort();
            ckbFactSelect.Items.Clear();
            ckbFactExclude.Items.Clear();
            foreach (string factType in factTypes)
            {
                if (!ckbFactSelect.Items.Contains(factType))
                {
                    int index = ckbFactSelect.Items.Add(factType);
                    bool itemChecked = Application.UserAppDataRegistry.GetValue("Fact: " + factType, "True").Equals("True");
                    ckbFactSelect.SetItemChecked(index, itemChecked);
                }
                if (!ckbFactExclude.Items.Contains(factType))
                {
                    int index = ckbFactExclude.Items.Add(factType);
                    bool itemChecked = Application.UserAppDataRegistry.GetValue("Exlude Fact: " + factType, "False").Equals("True");
                    ckbFactExclude.SetItemChecked(index, itemChecked);
                }
            }
        }

        void MnuLoadLocationsCSV_Click(object sender, EventArgs e) => LoadLocations(tspbTabProgress, tsStatusLabel, 1);

        void MnuLoadLocationsTNG_Click(object sender, EventArgs e) => LoadLocations(tspbTabProgress, tsStatusLabel, 2);

        #region Load CSV Location Data

        public static void LoadLocationData(ToolStripProgressBar pb, ToolStripStatusLabel label, int defaultIndex)
        {
            string csvFilename = string.Empty;
            pb.Visible = true;
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    string initialDir = (string)Application.UserAppDataRegistry.GetValue("Excel Export Individual Path");
                    openFileDialog.InitialDirectory = initialDir ?? Environment.SpecialFolder.MyDocuments.ToString();
                    openFileDialog.Filter = "Comma Separated Value (*.csv)|*.csv|TNG format (*.tng)|*.tng";
                    openFileDialog.FilterIndex = defaultIndex;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        csvFilename = openFileDialog.FileName;
                        label.Text = "Loading " + csvFilename;
                        string path = Path.GetDirectoryName(csvFilename);
                        Application.UserAppDataRegistry.SetValue("Excel Export Individual Path", path);
                        if (csvFilename.EndsWith("TNG", StringComparison.InvariantCultureIgnoreCase))
                            ReadTNGdata(pb, csvFilename);
                        else
                            ReadCSVdata(pb, csvFilename);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading CSV location data from {csvFilename}\nError was {ex.Message}", "FTAnalyzer");
            }
            pb.Visible = false;
            label.Text = string.Empty;
        }

        public static void ReadTNGdata(ToolStripProgressBar pb, string tngFilename)
        {
            int rowCount = 0;
            int lineCount = File.ReadLines(tngFilename).Count();
            pb.Maximum = lineCount;
            pb.Minimum = 0;
            pb.Value = rowCount;
            using (CsvFileReader reader = new CsvFileReader(tngFilename, ';'))
            {
                CsvRow row = new CsvRow();
                while (reader.ReadRow(row))
                {
                    if (row.Count == 4)
                    {
                        FactLocation.GetLocation(row[1], row[3], row[2], FactLocation.Geocode.NOT_SEARCHED, true, true);
                        rowCount++;
                    }
                    pb.Value++;
                    if (pb.Value % 10 == 0)
                        Application.DoEvents();
                }
                MessageBox.Show($"Loaded {rowCount} locations from TNG file {tngFilename}", "FTAnalyzer");
            }
        }

        public static void ReadCSVdata(ToolStripProgressBar pb, string csvFilename)
        {
            int rowCount = 0;
            int lineCount = File.ReadLines(csvFilename).Count();
            pb.Maximum = lineCount;
            pb.Minimum = 0;
            pb.Value = rowCount;
            using (CsvFileReader reader = new CsvFileReader(csvFilename))
            {
                CsvRow headerRow = new CsvRow();
                CsvRow row = new CsvRow();

                reader.ReadRow(headerRow);
                if (headerRow.Count != 3)
                    throw new InvalidLocationCSVFileException("Location file should have 3 values per line.");
                if (!headerRow[0].Trim().ToUpper().Equals("LOCATION"))
                    throw new InvalidLocationCSVFileException("No Location header record. Header should be Location, Latitude, Longitude");
                if (!headerRow[1].Trim().ToUpper().Equals("LATITUDE"))
                    throw new InvalidLocationCSVFileException("No Latitude header record. Header should be Location, Latitude, Longitude");
                if (!headerRow[2].Trim().ToUpper().Equals("LONGITUDE"))
                    throw new InvalidLocationCSVFileException("No Longitude header record. Header should be Location, Latitude, Longitude");
                while (reader.ReadRow(row))
                {
                    if (row.Count == 3)
                    {
                        FactLocation loc = FactLocation.GetLocation(row[0], row[1], row[2], FactLocation.Geocode.NOT_SEARCHED, true, true);
                        rowCount++;
                    }
                    pb.Value++;
                    if (pb.Value % 10 == 0)
                        Application.DoEvents();
                }
            }
            MessageBox.Show($"Loaded {rowCount} locations from file {csvFilename}", "FTAnalyzer");
        }
        #endregion

        void LoadLocations(ToolStripProgressBar pb, ToolStripStatusLabel label, int defaultIndex)
        {
            DialogResult result = MessageBox.Show("It is recommended you backup your Geocoding database first.\nDo you want to backup now?", "FTAnalyzer", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
                DatabaseHelper.Instance.BackupDatabase(saveDatabase, "FTAnalyzer zip file created by v" + VERSION);
            if (result != DialogResult.Cancel)
                LoadLocationData(pb, label, defaultIndex);
        }

        async void BtnShowSurnames_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            tsCountLabel.Text = string.Empty;
            tsHintsLabel.Text = string.Empty;
            tspbTabProgress.Visible = true;
            Predicate<Individual> indFilter = reltypesSurnames.BuildFilter<Individual>(x => x.RelationType);
            Predicate<Family> famFilter = reltypesSurnames.BuildFamilyFilter<Family>(x => x.RelationTypes);
            var progress = new Progress<int>(value => { tspbTabProgress.Value = value; });
            var list = await Task.Run(() =>
                new SortableBindingList<IDisplaySurnames>(Statistics.Instance.Surnames(indFilter, famFilter, progress, chkSurnamesIgnoreCase.Checked))).ConfigureAwait(true);
            tspbTabProgress.Visible = false;
            dgSurnames.DataSource = list;
            dgSurnames.Sort(dgSurnames.Columns[nameof(IDisplaySurnames.Surname)], ListSortDirection.Ascending);
            dgSurnames.Focus();
            tsCountLabel.Text = $"{Messages.Count}{list.Count} Surnames.";
            tsHintsLabel.Text = Messages.Hints_Surname;
            HourGlass(false);
            await Analytics.TrackAction(Analytics.MainFormAction, Analytics.ShowSurnamesEvent).ConfigureAwait(true);
        }

        void CousinsCountReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            StatisticsForm f = new StatisticsForm(StatisticsForm.StatisticType.CousinCount);
            DisposeDuplicateForms(f);
            f.Show();
            HourGlass(false);
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.CousinCountEvent);
        }

        void HowManyDirectsReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            StatisticsForm f = new StatisticsForm(StatisticsForm.StatisticType.HowManyDirects);
            DisposeDuplicateForms(f);
            f.Show();
            HourGlass(false);
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.DirectsReportEvent);
        }

        void FacebookSupportGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpecialMethods.VisitWebsite("https://www.facebook.com/ftanalyzer");
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.FacebookSupportEvent);
        }

        void FacebookUserGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpecialMethods.VisitWebsite("https://www.facebook.com/groups/ftanalyzer");
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.FacebookUsersEvent);
        }

        void MnuDNA_GEDCOM_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            DNA_GEDCOM.Export();
            HourGlass(false);
        }

        void GetGoogleAPIKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpecialMethods.VisitWebsite("https://developers.google.com/maps/documentation/embed/get-api-key");
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.GoogleAPIKey);
        }

        void GoogleAPISetupGuideToolStripMenuItem_Click(object sender, EventArgs e) => SpecialMethods.VisitWebsite("http://www.ftanalyzer.com/GoogleAPI");

        void BirthdayEffectReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            StatisticsForm f = new StatisticsForm(StatisticsForm.StatisticType.BirthdayEffect);
            DisposeDuplicateForms(f);
            f.Show();
            HourGlass(false);
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.BirthdayEffectEvent);
        }

        void PossiblyMissingChildReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            People people = new People();
            people.SetupPossiblyMissingChildrenReport();
            DisposeDuplicateForms(people);
            people.Show();
            Analytics.TrackAction(Analytics.ReportsAction, Analytics.PossiblyMissingChildren);
            HourGlass(false);
        }

        void MnuAgedOver99Report_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            People people = new People();
            people.SetupAgedOver99Report();
            DisposeDuplicateForms(people);
            people.Show();
            Analytics.TrackAction(Analytics.ReportsAction, Analytics.AgedOver99Report);
            HourGlass(false);
        }

        void MnuSingleParentsReport_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            People people = new People();
            people.SingleParents();
            DisposeDuplicateForms(people);
            people.Show();
            Analytics.TrackAction(Analytics.ReportsAction, Analytics.AgedOver99Report);
            HourGlass(false);
        }

        void MnuJSON_Click(object sender, EventArgs e)
        {
            HourGlass(true);
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    string initialDir = (string)Application.UserAppDataRegistry.GetValue("JSON Export Path");
                    saveFileDialog.InitialDirectory = initialDir ?? Environment.SpecialFolder.MyDocuments.ToString();
                    saveFileDialog.Filter = "JavaScript Object Notation (*.json)|*.json";
                    saveFileDialog.FilterIndex = 1;
                    DialogResult dr = saveFileDialog.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        string path = Path.GetDirectoryName(saveFileDialog.FileName);
                        Application.UserAppDataRegistry.SetValue("JSON Export Path", path);
                        using (StreamWriter output = new StreamWriter(new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write), Encoding.UTF8))
                        {
                            var data = new JsonExport(filename);
                            data.WriteJsonData(output);
                        }
                        UIHelpers.ShowMessage($"File written to {saveFileDialog.FileName}", "FTAnalyzer");
                    }
                }
            }
            catch (Exception ex)
            {
                UIHelpers.ShowMessage(ex.Message, "FTAnalyzer");
            }
            HourGlass(false);
        }

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

        void BtnAliveOnDate_Click(object sender, EventArgs e)
        {
            if (AliveDate != FactDate.UNKNOWN_DATE)
            {
                HourGlass(true);
                People people = new People();
                Predicate<Individual> filter = CreateAliveatDateFilter(AliveDate, txtCensusSurname.Text);
                people.SetupAliveAtDate(AliveDate, filter);
                DisposeDuplicateForms(people);
                people.Show();
                Analytics.TrackAction(Analytics.CensusTabAction, Analytics.AliveAtDate);
                HourGlass(false);
            }
        }
    }
}