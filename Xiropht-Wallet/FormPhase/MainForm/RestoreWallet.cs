#if WINDOWS
using MetroFramework;
#endif
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xiropht_Connector_All.Wallet;
using Xiropht_Wallet.Wallet;

namespace Xiropht_Wallet.FormPhase.MainForm
{
    public partial class RestoreWallet : Form
    {

        public RestoreWallet()
        {
            InitializeComponent();
        }

        public void GetListControl()
        {
            if (ClassFormPhase.WalletXiropht.ListControlSizeRestoreWallet.Count == 0)
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (i < Controls.Count)
                    {
                        ClassFormPhase.WalletXiropht.ListControlSizeRestoreWallet.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
                    }
                }
            }
        }

        private void buttonSearchNewWalletFile_Click(object sender, EventArgs e)
        {
            var saveFileDialogWallet = new SaveFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Filter = @"Wallet File (*.xir) | *.xir",
                FilterIndex = 2,
                RestoreDirectory = true
            };
            if (saveFileDialogWallet.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialogWallet.FileName != "")
                {
                    textBoxPathWallet.Text = saveFileDialogWallet.FileName;
                }
            }
        }

        private async void buttonRestoreYourWallet_ClickAsync(object sender, EventArgs e)
        {
            string walletPath = textBoxPathWallet.Text;
            string walletPassword = textBoxPassword.Text;
            string walletKey = textBoxPrivateKey.Text;
            textBoxPassword.Text = string.Empty;
            textBoxPathWallet.Text = string.Empty;
            textBoxPrivateKey.Text = string.Empty;

            if (await ClassWalletObject
                .InitializationWalletConnection("", walletPassword, "",
                    ClassWalletPhase.Create))
            {
                ClassWalletObject.WalletNewPassword = walletPassword;
                ClassWalletObject.ListenSeedNodeNetworkForWallet();

                ClassWalletObject.WalletDataCreationPath = walletPath;

                new Thread(async delegate ()
                {
                    Stopwatch packetSpeedCalculator = new Stopwatch();

                    packetSpeedCalculator.Start();
                    if (await ClassWalletObject.WalletConnect
                        .SendPacketWallet(ClassWalletObject.Certificate, string.Empty, false))
                    {
                        packetSpeedCalculator.Stop();
                        if (packetSpeedCalculator.ElapsedMilliseconds > 0)
                        {
                            Thread.Sleep((int)packetSpeedCalculator.ElapsedMilliseconds);
                        }
                        if (!await ClassWalletObject
                            .SendPacketWalletToSeedNodeNetwork(
                                ClassWalletCommand.ClassWalletSendEnumeration.AskPhase + "|" +
                                walletKey + "|" + walletPassword))
                        {
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                            MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                                  ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT"));
                            ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
#endif
                        }
                    }
                    else
                    {
#if WINDOWS
                    ClassFormPhase.MessageBoxInterface(
                        ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT"));
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
#endif
                    }
                }).Start();
            }
        }

        private void textBoxPassword_Resize(object sender, EventArgs e)
        {
            UpdateStyles();
        }

        private void RestoreWallet_Load(object sender, EventArgs e)
        {
            UpdateStyles();
            ClassFormPhase.WalletXiropht.ResizeWalletInterface();
        }
    }
}
