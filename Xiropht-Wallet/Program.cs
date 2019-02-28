using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Xiropht_Wallet
{
    static class Program
    {
        public static CultureInfo GlobalCultureInfo = new CultureInfo("fr-FR"); // Set the global culture info, I don't suggest to change this, this one is used by the blockchain and by the whole network.

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Thread.CurrentThread.Name = Path.GetFileName(Environment.GetCommandLineArgs()[0]);

#if DEBUG
            Log.InitializeLog(); // Initialization of log system.
            Log.AutoWriteLog(); // Start the automatic write of log lines.
#endif
            AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs args)
            {
                var filePath = ClassUtils.ConvertPath(Directory.GetCurrentDirectory()+"\\error_wallet.txt");
                var exception = (Exception) args.ExceptionObject;
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
                System.Diagnostics.Trace.TraceError(exception.StackTrace);

                Environment.Exit(1);
            };
#if WINDOWS
            ClassMemory.CleanMemory();
#endif

            bool firstStart = ClassWalletSetting.LoadSetting(); // Load the setting file.
            ClassTranslation.InitializationLanguage(); // Initialization of language system.
            ClassContact.InitializationContactList(); // Initialization of contact system.
#if WINDOWS
            Application.EnableVisualStyles();
#endif
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WalletXiropht(firstStart)); // Start the main interface.


        }
    }
}