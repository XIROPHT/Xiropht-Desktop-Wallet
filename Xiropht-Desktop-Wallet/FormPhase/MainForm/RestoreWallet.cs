﻿using System;
using System.Drawing;
using System.Text.RegularExpressions;
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
            string walletPath = textBoxPathWallet.Text;
            string walletPassword = textBoxPassword.Text;
            string walletKey = textBoxPrivateKey.Text;
            walletKey = Regex.Replace(walletKey, @"\s+", string.Empty);
            textBoxPassword.Text = string.Empty;
            textBoxPathWallet.Text = string.Empty;
            textBoxPrivateKey.Text = string.Empty;

            if (ClassFormPhase.WalletXiropht.ClassWalletObject != null)
            {
                ClassFormPhase.WalletXiropht.InitializationWalletObject();
            }
            if (await ClassFormPhase.WalletXiropht.ClassWalletObject.InitializationWalletConnection(string.Empty, walletPassword, string.Empty, ClassWalletPhase.Restore))
            {

                ClassFormPhase.WalletXiropht.ClassWalletObject.ListenSeedNodeNetworkForWallet();

                ClassFormPhase.WalletXiropht.ClassWalletObject.WalletDataCreationPath = walletPath;

                await Task.Factory.StartNew(async () =>
                 {

                     if (await ClassFormPhase.WalletXiropht.ClassWalletObject.WalletConnect.SendPacketWallet(ClassFormPhase.WalletXiropht.ClassWalletObject.Certificate, string.Empty, false))
                     {

                         string requestRestoreQrCodeEncrypted = null;

                         using (ClassWalletRestoreFunctions walletRestoreFunctions = new ClassWalletRestoreFunctions())
                         {
                             requestRestoreQrCodeEncrypted = walletRestoreFunctions.GenerateQRCodeKeyEncryptedRepresentation(walletKey, walletPassword);
                             if (requestRestoreQrCodeEncrypted != null)
                             {
                                 ClassFormPhase.WalletXiropht.ClassWalletObject.WalletNewPassword = walletPassword;
                                 ClassFormPhase.WalletXiropht.ClassWalletObject.WalletPrivateKeyEncryptedQRCode = walletKey;

                                 await Task.Delay(100);
                                 if (!await ClassFormPhase.WalletXiropht.ClassWalletObject.SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassWalletCommand.ClassWalletSendEnumeration.AskPhase + "|" + requestRestoreQrCodeEncrypted, ClassFormPhase.WalletXiropht.ClassWalletObject.Certificate, false, true))
                                 {
#if WINDOWS
                                    ClassFormPhase.MessageBoxInterface(ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                ClassFormPhase.MessageBoxInterface("Invalid private key inserted.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                            MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,"Invalid private key inserted.");
                            ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
#endif

                             }
                         }
                     }
                     else
                     {
                         ClassParallelForm.ShowWaitingFormAsync();
#if WINDOWS
                         ClassFormPhase.MessageBoxInterface(
                             ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT"));
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
#endif
                    }
                 }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current);
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
