using FTAnalyzer.Filters;
using FTAnalyzer.UserControls;
using FTAnalyzer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FTAnalyzer.Forms
{
    public partial class LostCousinsReferral : Form
    {
        readonly List<ExportReferrals> referrals;

        public LostCousinsReferral(Individual referee, bool onlyInCommon)
        {
            InitializeComponent();
            Top += NativeMethods.TopTaskbarOffset;
            FamilyTree ft = FamilyTree.Instance;
            Text = $"Lost Cousins Referral for {referee}";
            dgLCReferrals.AutoGenerateColumns = false;
            ExtensionMethods.DoubleBuffered(dgLCReferrals, true);
            Predicate<Individual> lostCousinsFact = new Predicate<Individual>(x => x.HasLostCousinsFact);
            List<Individual> lostCousinsFacts = ft.AllIndividuals.Filter(lostCousinsFact).ToList<Individual>();
            referrals = new List<ExportReferrals>();
            foreach (Individual ind in lostCousinsFacts)
            {
                List<Fact> indLCFacts = new List<Fact>();
                indLCFacts.AddRange(ind.GetFacts(Fact.LOSTCOUSINS));
                indLCFacts.AddRange(ind.GetFacts(Fact.LC_FTA));
                foreach (Fact f in indLCFacts)
                {
                    if ((onlyInCommon && ind.IsBloodDirect) || !onlyInCommon)
                        referrals.Add(new ExportReferrals(ind, f));
                }
            }
            tsRecords.Text = GetCountofRecords();
        }

        string GetCountofRecords()
        {
            int total = referrals.Count;
            int direct = referrals.Count(x => x.RelationType.Equals(Properties.Messages.Referral_Direct));
            int blood = referrals.Count(x => x.RelationType.Equals(Properties.Messages.Referral_Blood));
            int marriage = referrals.Count(x => x.RelationType.Equals(Properties.Messages.Referral_Marriage));
            int others = referrals.Count(x => string.IsNullOrEmpty(x.RelationType));
            return total + $" Lost Cousins Records listed made up of {direct} Direct Ancestors, {blood} Blood Relatives, {marriage} Marriage and {others} Others.";
        }

        void LostCousinsReferral_FormClosed(object sender, FormClosedEventArgs e) => Dispose();

        void LostCousinsReferral_Load(object sender, EventArgs e) => SpecialMethods.SetFonts(this);
    }
}
