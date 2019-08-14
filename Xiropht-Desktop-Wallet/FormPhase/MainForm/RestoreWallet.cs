using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xiropht_Connector_All.Setting;
using Xiropht_Connector_All.Wallet;
using Xiropht_Wallet.Features;
using Xiropht_Wallet.Properties;
using Xiropht_Wallet.Tcp.Option;
using Xiropht_Wallet.Utility;

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
                for (var i = 0; i < Controls.Count; i++)
                    if (i < Controls.Count)
                        Program.WalletXiropht.ListControlSizeRestoreWallet.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
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
                if (saveFileDialogWallet.FileName != "")
                    textBoxPathWallet.Text = saveFileDialogWallet.FileName;
        }

        private async void buttonRestoreYourWallet_ClickAsync(object sender, EventArgs e)
        {
            if (CheckPasswordValidity())
            {
                ClassParallelForm.ShowWaitingFormAsync();

                var walletPath = textBoxPathWallet.Text;
                var walletPassword = textBoxPassword.Text;
                var walletKey = textBoxPrivateKey.Text;
                walletKey = Regex.Replace(walletKey, @"\s+", string.Empty);
                textBoxPassword.Text = string.Empty;
                textBoxPathWallet.Text = string.Empty;
                textBoxPrivateKey.Text = string.Empty;
                CheckPasswordValidity();


                await Task.Factory.StartNew(async () =>
                {
                    var walletRestoreFunctions = new ClassWalletRestoreFunctions();

                    var requestRestoreQrCodeEncrypted =
                        walletRestoreFunctions.GenerateQRCodeKeyEncryptedRepresentation(walletKey, walletPassword);

                    if (Program.WalletXiropht.ClassWalletObject != null)
                        Program.WalletXiropht.InitializationWalletObject();

                    if (await Program.WalletXiropht.ClassWalletObject.InitializationWalletConnection(string.Empty,
                        walletPassword, string.Empty, ClassWalletPhase.Restore))
                    {
                        Program.WalletXiropht.ClassWalletObject.ListenSeedNodeNetworkForWallet();

                        Program.WalletXiropht.ClassWalletObject.WalletDataCreationPath = walletPath;
                        if (await Program.WalletXiropht.ClassWalletObject.WalletConnect.SendPacketWallet(
                            Program.WalletXiropht.ClassWalletObject.Certificate, string.Empty, false))
                        {
                            if (requestRestoreQrCodeEncrypted != null)
                            {
                                Program.WalletXiropht.ClassWalletObject.WalletNewPassword = walletPassword;
                                Program.WalletXiropht.ClassWalletObject.WalletPrivateKeyEncryptedQRCode = walletKey;

                                await Task.Delay(1000);

                                if (!await Program.WalletXiropht.ClassWalletObject.SeedNodeConnectorWallet
                                    .SendPacketToSeedNodeAsync(
                                        ClassWalletCommand.ClassWalletSendEnumeration.AskPhase + "|" +
                                        requestRestoreQrCodeEncrypted,
                                        Program.WalletXiropht.ClassWalletObject.Certificate, false, true))
                                {
                                    ClassParallelForm.HideWaitingFormAsync();
#if WINDOWS
                                    ClassFormPhase.MessageBoxInterface(
                                        ClassTranslation.GetLanguageTextFromOrder(
                                            ClassTranslationEnumeration.createwalleterrorcantconnectmessagecontenttext), string.Empty,
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                  ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalleterrorcantconnectmessagecontenttext));
                            Program.WalletXiropht.BeginInvoke(invoke);
#endif
                                }
                            }
                            else
                            {
                                ClassParallelForm.HideWaitingFormAsync();

#if WINDOWS
                                ClassFormPhase.MessageBoxInterface("Invalid private key inserted.", string.Empty,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                            MethodInvoker invoke =
 () => MessageBox.Show(Program.WalletXiropht,"Invalid private key inserted.");
                            Program.WalletXiropht.BeginInvoke(invoke);
#endif
                            }
                        }
                        else
                        {
                            ClassParallelForm.HideWaitingFormAsync();

#if WINDOWS
                            ClassFormPhase.MessageBoxInterface(
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration.createwalleterrorcantconnectmessagecontenttext), string.Empty,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalleterrorcantconnectmessagecontenttext));
                        Program.WalletXiropht.BeginInvoke(invoke);
#endif
                        }
                    }
                    else
                    {
                        ClassParallelForm.HideWaitingFormAsync();

#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.createwalleterrorcantconnectmessagecontenttext), string.Empty,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalleterrorcantconnectmessagecontenttext));
                        Program.WalletXiropht.BeginInvoke(invoke);
#endif
                    }

                    walletRestoreFunctions.Dispose();
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Current);
            }
            else
            {
#if WINDOWS
                ClassFormPhase.MessageBoxInterface(
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletlabelpasswordrequirementtext),
                    ClassTranslation.GetLanguageTextFromOrder(
                        ClassTranslationEnumeration.walletnetworkobjectcreatewalletpassworderror2titletext), MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletlabelpasswordrequirementtext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.walletnetworkobjectcreatewalletpassworderror2titletext), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
            }
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

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            CheckPasswordValidity();
        }

        /// <summary>
        ///     Check password validity
        /// </summary>
        private bool CheckPasswordValidity()
        {
            if (textBoxPassword.Text.Length >= ClassConnectorSetting.WalletMinPasswordLength)
            {
                if (ClassUtility.CheckPassword(textBoxPassword.Text))
                {
                    MethodInvoker invoke = () => pictureBoxPasswordStatus.BackgroundImage = Resources.valid;
                    BeginInvoke(invoke);
                    return true;
                }
                else
                {
                    MethodInvoker invoke = () => pictureBoxPasswordStatus.BackgroundImage = Resources.error;
                    BeginInvoke(invoke);
                }
            }
            else
            {
                MethodInvoker invoke = () => pictureBoxPasswordStatus.BackgroundImage = Resources.error;
                BeginInvoke(invoke);
            }

            return false;
        }
    }
}