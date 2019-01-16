﻿#if WINDOWS
using MetroFramework;
#endif
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xiropht_Connector_All.Wallet;
using Xiropht_Wallet.Wallet;

namespace Xiropht_Wallet.FormPhase.MainForm
{
    public partial class CreateWallet : Form
    {
        public bool InCreation;

        public CreateWallet()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams CP = base.CreateParams;
                CP.ExStyle = CP.ExStyle | 0x02000000; // WS_EX_COMPOSITED
                return CP;
            }
        }

        public void GetListControl()
        {
            if (ClassFormPhase.WalletXiropht.ListControlSizeCreateWallet.Count == 0)
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (i < Controls.Count)
                    {
                        ClassFormPhase.WalletXiropht.ListControlSizeCreateWallet.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
                    }
                }
            }
        }

        private void SaveWalletFilePath()
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

        private async void ButtonCreateYourWallet_Click(object sender, EventArgs e)
        {
            await CreateWalletAsync();
        }

        private void ButtonSearchNewWalletFile_Click(object sender, EventArgs e)
        {
            SaveWalletFilePath();
        }

        private void CreateWallet_Load(object sender, EventArgs e)
        {
            UpdateStyles();
            ClassFormPhase.WalletXiropht.ResizeWalletInterface();
        }

        private void CreateWallet_Resize(object sender, EventArgs e)
        {
            UpdateStyles();
        }

        private async Task CreateWalletAsync()
        {
            if (InCreation)
            {
                await ClassWalletObject.FullDisconnection(true).ConfigureAwait(false);
                InCreation = false;
            }

            if (textBoxPathWallet.Text != "")
            {
                if (textBoxSelectWalletPassword.Text != "")
                {
                    if (await ClassWalletObject
                        .InitializationWalletConnection("", textBoxSelectWalletPassword.Text, "",
                            ClassWalletPhase.Create))
                    {
                        ClassWalletObject.WalletNewPassword = textBoxSelectWalletPassword.Text;
                        ClassWalletObject.ListenSeedNodeNetworkForWallet();

                        InCreation = true;
                        ClassWalletObject.WalletDataCreationPath = textBoxPathWallet.Text;
                        if (await ClassWalletObject.WalletConnect
                            .SendPacketWallet(ClassWalletObject.Certificate, string.Empty, false))
                        {
                            await Task.Delay(1000);
                            if (!await ClassWalletObject
                                .SendPacketWalletToSeedNodeNetwork(
                                    ClassWalletCommand.ClassWalletSendEnumeration.CreatePhase + "|" +
                                    textBoxSelectWalletPassword.Text))
                            {
#if WINDOWS
                                MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                                    ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT"));
#else
                                MessageBox.Show(ClassFormPhase.WalletXiropht,
                                    ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT"));
#endif
                            }

                            void MethodInvoker() => textBoxSelectWalletPassword.Text = "";
                            BeginInvoke((MethodInvoker)MethodInvoker);
                        }
                        else
                        {
#if WINDOWS
                            MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                               ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT"));
#else
                            MessageBox.Show(ClassFormPhase.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT"));
#endif
                        }

                    }
                }
            }
        }

        private async void textBoxSelectWalletPassword_KeyDownAsync(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                await CreateWalletAsync();
            }
        }
    }
}