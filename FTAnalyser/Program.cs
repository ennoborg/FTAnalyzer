using FTAnalyzer.Utilities;
using SharpMap;
using System;
using System.Windows.Forms;

namespace FTAnalyzer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()

        {
            if (Environment.OSVersion.Version.Major >= 10) NativeMethods.SetProcessDPIAware();
            SharpMapUtility.Configure();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
