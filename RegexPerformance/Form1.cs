using FTAnalyzer;
using System;
using System.Windows.Forms;

namespace RegexPerformance
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void BtnStart_Click(object sender, EventArgs e)
        {
            rtbOutput.Text = string.Empty;
            RunTests();
        }

        void RunTests()
        {
            var outputText = new Progress<string>(value => { rtbOutput.AppendText(value); });
        }
    }
}
