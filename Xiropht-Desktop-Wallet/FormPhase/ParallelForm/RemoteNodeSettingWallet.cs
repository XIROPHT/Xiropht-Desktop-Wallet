using System;
using System.Windows.Forms;
using Xiropht_Wallet.Features;
using Xiropht_Wallet.Wallet.Setting;
using Xiropht_Wallet.Wallet.Tcp;

namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    public partial class RemoteNodeSettingWallet : Form
    {
        public RemoteNodeSettingWallet()
        {
            InitializeComponent();
        }

        private void ButtonValidSetting_Click(object sender, EventArgs e)
        {
            if (radioButtonEnableSeedNodeSync.Checked)
            {
                Program.WalletXiropht.WalletSyncMode = ClassWalletSyncMode.WALLET_SYNC_DEFAULT;
            }
            else if (radioButtonEnablePublicRemoteNodeSync.Checked)
            {
                Program.WalletXiropht.WalletSyncMode = ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE;
            }
            else if (radioButtonEnableManualRemoteNodeSync.Checked)
            {
                Program.WalletXiropht.WalletSyncMode = ClassWalletSyncMode.WALLET_SYNC_MANUAL_NODE;
                Program.WalletXiropht.WalletSyncHostname = textBoxRemoteNodeHost.Text;
            }

            ClassWalletSetting.SaveSetting();


            if (Program.WalletXiropht.ClassWalletObject.WalletConnect != null)
            {
                if (!Program.WalletXiropht.EnableTokenNetworkMode)
                {
                    if (!string.IsNullOrEmpty(Program.WalletXiropht.ClassWalletObject.WalletConnect.WalletPhase))
                        if (!Program.WalletXiropht.ClassWalletObject.WalletClosed)
                            Program.WalletXiropht.ClassWalletObject.DisconnectWholeRemoteNodeSyncAsync(true, true);
                }
                else
                {
                    if (!Program.WalletXiropht.ClassWalletObject.WalletClosed)
                    {
                        Program.WalletXiropht.ClassWalletObject.DisconnectRemoteNodeTokenSync();
                        Program.WalletXiropht.ClassWalletObject.WalletOnUseSync = false;
                        Program.WalletXiropht.ClassWalletObject.EnableWalletTokenSync();
                    }
                }
            }

            Close();
        }

        private void RadioButtonEnableManualRemoteNodeSync_Click(object sender, EventArgs e)
        {
            radioButtonEnableSeedNodeSync.Checked = false;
            radioButtonEnablePublicRemoteNodeSync.Checked = false;
            radioButtonEnableManualRemoteNodeSync.Checked = true;
        }

        private void RadioButtonEnablePublicRemoteNodeSync_Click(object sender, EventArgs e)
        {
            radioButtonEnableManualRemoteNodeSync.Checked = false;
            radioButtonEnableSeedNodeSync.Checked = false;
            radioButtonEnablePublicRemoteNodeSync.Checked = true;
        }

        private void RadioButtonEnableSeedNodeSync_Click(object sender, EventArgs e)
        {
            radioButtonEnablePublicRemoteNodeSync.Checked = false;
            radioButtonEnableManualRemoteNodeSync.Checked = false;
            radioButtonEnableSeedNodeSync.Checked = true;
        }

        private void RemoteNodeSetting_Load(object sender, EventArgs e)
        {
            radioButtonEnableSeedNodeSync.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.remotenodesettingmenuuseseednodenetworkonlytext);
            radioButtonEnablePublicRemoteNodeSync.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.remotenodesettingmenuuseremotenodetext);
            radioButtonEnableManualRemoteNodeSync.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.remotenodesettingmenuusemanualnodetext);
            labelNoticeRemoteNodeHost.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.remotenodesettingmenuusemanualnodehostnametext);
            labelNoticePublicNodeInformation.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.remotenodesettingmenuuseremotenodeinformationtext);
            labelNoticePrivateRemoteNode.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.remotenodesettingmenuusemanualnodeinformationtext);
            switch (Program.WalletXiropht.WalletSyncMode)
            {
                case ClassWalletSyncMode.WALLET_SYNC_DEFAULT:
                    radioButtonEnableSeedNodeSync.Checked = true;
                    radioButtonEnablePublicRemoteNodeSync.Checked = false;
                    radioButtonEnableManualRemoteNodeSync.Checked = false;
                    break;
                case ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE:
                    radioButtonEnableSeedNodeSync.Checked = false;
                    radioButtonEnablePublicRemoteNodeSync.Checked = true;
                    radioButtonEnableManualRemoteNodeSync.Checked = false;
                    break;
                case ClassWalletSyncMode.WALLET_SYNC_MANUAL_NODE:
                    radioButtonEnableSeedNodeSync.Checked = false;
                    radioButtonEnablePublicRemoteNodeSync.Checked = false;
                    radioButtonEnableManualRemoteNodeSync.Checked = true;
                    textBoxRemoteNodeHost.Text = Program.WalletXiropht.WalletSyncHostname;
                    break;
            }
        }
    }
}