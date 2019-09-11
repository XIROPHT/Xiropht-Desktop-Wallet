using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Xiropht_Wallet.Features;
using Xiropht_Wallet.Utility;

#if DEBUG
using Xiropht_Wallet.Debug;
#endif

namespace Xiropht_Wallet
{
    internal static class Program
    {
        public static CultureInfo
            GlobalCultureInfo =
                new CultureInfo(
                    "fr-FR"); // Set the global culture info, I don't suggest to change this, this one is used by the blockchain and by the whole network.

        public static WalletXiropht WalletXiropht;

        /// <summary>
        ///     Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Thread.CurrentThread.CurrentUICulture = GlobalCultureInfo;
            Thread.CurrentThread.Name = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
            ServicePointManager.DefaultConnectionLimit = 65535;
#if DEBUG
            Log.InitializeLog(); // Initialization of log system.
            Log.AutoWriteLog(); // Start the automatic write of log lines.
#endif
            AppDomain.CurrentDomain.UnhandledException += Application_ThreadException;

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

#if WINDOWS
            ClassMemory.CleanMemory();
#endif

            ClassTranslation.InitializationLanguage(); // Initialization of language system.
            ClassContact.InitializationContactList(); // Initialization of contact system.
            ClassPeerList.LoadPeerList();
#if WINDOWS
            Application.EnableVisualStyles();
#endif
            Application.SetCompatibleTextRenderingDefault(false);
            WalletXiropht = new WalletXiropht();
            Application.Run(WalletXiropht); // Start the main interface.
        }

        private static void Application_ThreadException(object sender, UnhandledExceptionEventArgs e)
        {
            var filePath = ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + "\\error_wallet.txt");
            var exception = (Exception) e.ExceptionObject;
            using (var writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Message :" + exception.Message + "<br/>" + Environment.NewLine +
                                 "StackTrace :" +
                                 exception.StackTrace +
                                 "" + Environment.NewLine + "Date :" + DateTime.Now);
                writer.WriteLine(Environment.NewLine +
                                 "-----------------------------------------------------------------------------" +
                                 Environment.NewLine);
            }

            MessageBox.Show(
                @"An error has been detected, send the file error_wallet.txt to the Team for fix the issue.");
            Trace.TraceError(exception.StackTrace);

            Environment.Exit(1);
        }
    }
}