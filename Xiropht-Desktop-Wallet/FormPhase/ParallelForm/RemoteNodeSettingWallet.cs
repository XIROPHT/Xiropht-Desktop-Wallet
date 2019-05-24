#if WINDOWS
using MetroFramework;
#endif
using System;
using System.Windows.Forms;
using Xiropht_Wallet.Wallet;

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
                ClassWalletObject.WalletSyncMode = (int)ClassWalletSyncMode.WALLET_SYNC_DEFAULT;
            }
            else if (radioButtonEnablePublicRemoteNodeSync.Checked)
            {
                ClassWalletObject.WalletSyncMode = (int)ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE;
            }
            else if (radioButtonEnableManualRemoteNodeSync.Checked)
            {
                ClassWalletObject.WalletSyncMode = (int)ClassWalletSyncMode.WALLET_SYNC_MANUAL_NODE;
                ClassWalletObject.WalletSyncHostname = textBoxRemoteNodeHost.Text;
            }

            ClassWalletSetting.SaveSetting();
            ClassWalletObject.FullDisconnection(true);
            ClassFormPhase.WalletXiropht.SwitchForm(ClassFormPhaseEnumeration.OpenWallet);
#if WINDOWS
            MetroMessageBox.Show(this, ClassTranslation.GetLanguageTextFromOrder("REMOTE_NODE_SETTING_MENU_SAVE_SETTING_TEXT"));
#else
            MessageBox.Show(this, ClassTranslation.GetLanguageTextFromOrder("REMOTE_NODE_SETTING_MENU_SAVE_SETTING_TEXT"));
#endif
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
            radioButtonEnableSeedNodeSync.Text = ClassTranslation.GetLanguageTextFromOrder("REMOTE_NODE_SETTING_MENU_USE_SEED_NODE_NETWORK_ONLY_TEXT");
            radioButtonEnablePublicRemoteNodeSync.Text = ClassTranslation.GetLanguageTextFromOrder("REMOTE_NODE_SETTING_MENU_USE_REMOTE_NODE_TEXT");
            radioButtonEnableManualRemoteNodeSync.Text = ClassTranslation.GetLanguageTextFromOrder("REMOTE_NODE_SETTING_MENU_USE_MANUAL_NODE_TEXT");
            labelNoticeRemoteNodeHost.Text = ClassTranslation.GetLanguageTextFromOrder("REMOTE_NODE_SETTING_MENU_USE_MANUAL_NODE_HOSTNAME_TEXT");
            labelNoticePublicNodeInformation.Text = ClassTranslation.GetLanguageTextFromOrder("REMOTE_NODE_SETTING_MENU_USE_REMOTE_NODE_INFORMATION_TEXT");
            labelNoticePrivateRemoteNode.Text = ClassTranslation.GetLanguageTextFromOrder("REMOTE_NODE_SETTING_MENU_USE_MANUAL_NODE_INFORMATION_TEXT");
            switch(ClassWalletObject.WalletSyncMode)
            {
                case (int)ClassWalletSyncMode.WALLET_SYNC_DEFAULT:
                    radioButtonEnableSeedNodeSync.Checked = true;
                    radioButtonEnablePublicRemoteNodeSync.Checked = false;
                    radioButtonEnableManualRemoteNodeSync.Checked = false;
                    break;
                case (int)ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE:
                    radioButtonEnableSeedNodeSync.Checked = false;
                    radioButtonEnablePublicRemoteNodeSync.Checked = true;
                    radioButtonEnableManualRemoteNodeSync.Checked = false;
                    break;
                case (int)ClassWalletSyncMode.WALLET_SYNC_MANUAL_NODE:
                    radioButtonEnableSeedNodeSync.Checked = false;
                    radioButtonEnablePublicRemoteNodeSync.Checked = false;
                    radioButtonEnableManualRemoteNodeSync.Checked = true;
                    textBoxRemoteNodeHost.Text = ClassWalletObject.WalletSyncHostname;
                    break;
            }

        }
    }
}