using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xiropht_Connector_All.Setting;
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
            if (Program.WalletXiropht.ListControlSizeRestoreWallet.Count == 0)
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (i < Controls.Count)
                    {
                        Program.WalletXiropht.ListControlSizeRestoreWallet.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
                    }
                }
            }
        }

        private void buttonSearchNewWalletFile_Click(object sender, EventArgs e)
        {
            var saveFileDialogWallet = new SaveFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
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
            ClassParallelForm.ShowWaitingFormAsync();

            string walletPath = textBoxPathWallet.Text;
            string walletPassword = textBoxPassword.Text;
            string walletKey = textBoxPrivateKey.Text;
            walletKey = Regex.Replace(walletKey, @"\s+", string.Empty);
            textBoxPassword.Text = string.Empty;
            textBoxPathWallet.Text = string.Empty;
            textBoxPrivateKey.Text = string.Empty;



            await Task.Factory.StartNew(async () =>
            {

                ClassWalletRestoreFunctions walletRestoreFunctions = new ClassWalletRestoreFunctions();

                string requestRestoreQrCodeEncrypted = walletRestoreFunctions.GenerateQRCodeKeyEncryptedRepresentation(walletKey, walletPassword);

                if (Program.WalletXiropht.ClassWalletObject != null)
                {
                    Program.WalletXiropht.InitializationWalletObject();
                }

                if (await Program.WalletXiropht.ClassWalletObject.InitializationWalletConnection(string.Empty, walletPassword, string.Empty, ClassWalletPhase.Restore))
                {

                    Program.WalletXiropht.ClassWalletObject.ListenSeedNodeNetworkForWallet();

                    Program.WalletXiropht.ClassWalletObject.WalletDataCreationPath = walletPath;
                    if (await Program.WalletXiropht.ClassWalletObject.WalletConnect.SendPacketWallet(Program.WalletXiropht.ClassWalletObject.Certificate, string.Empty, false))
                    {
                         if (requestRestoreQrCodeEncrypted != null)
                        {
                            Program.WalletXiropht.ClassWalletObject.WalletNewPassword = walletPassword;
                            Program.WalletXiropht.ClassWalletObject.WalletPrivateKeyEncryptedQRCode = walletKey;

                            await Task.Delay(1000);

                            if (!await Program.WalletXiropht.ClassWalletObject.SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassWalletCommand.ClassWalletSendEnumeration.AskPhase + "|" + requestRestoreQrCodeEncrypted, Program.WalletXiropht.ClassWalletObject.Certificate, false, true))
                            {
                                ClassParallelForm.HideWaitingFormAsync();
#if WINDOWS
                                ClassFormPhase.MessageBoxInterface(ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                  ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT"));
                            Program.WalletXiropht.BeginInvoke(invoke);
#endif
                            }
                        }
                        else
                        {
                            ClassParallelForm.HideWaitingFormAsync();

#if WINDOWS
                            ClassFormPhase.MessageBoxInterface("Invalid private key inserted.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,"Invalid private key inserted.");
                            Program.WalletXiropht.BeginInvoke(invoke);
#endif

                        }

                    }
                    else
                    {
                        ClassParallelForm.HideWaitingFormAsync();

#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                                 ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT"));
                        Program.WalletXiropht.BeginInvoke(invoke);
#endif
                    }
                }
                else
                {
                    ClassParallelForm.HideWaitingFormAsync();

#if WINDOWS
                    ClassFormPhase.MessageBoxInterface(
                        ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT"));
                        Program.WalletXiropht.BeginInvoke(invoke);
#endif
                }
                walletRestoreFunctions.Dispose();

            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Current);

        }

        private void textBoxPassword_Resize(object sender, EventArgs e)
        {
            UpdateStyles();
        }

        private void RestoreWallet_Load(object sender, EventArgs e)
        {
            UpdateStyles();
            Program.WalletXiropht.ResizeWalletInterface();
        }
    }
}
