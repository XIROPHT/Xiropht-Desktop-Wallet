using System;
using System.Linq;
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
                if (!Program.WalletXiropht.ClassWalletObject.WalletClosed)
                {
                    Program.WalletXiropht.ClassWalletObject.DisconnectRemoteNodeTokenSync();
                    Program.WalletXiropht.ClassWalletObject.WalletOnUseSync = false;
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
            dataGridViewPeerList.AllowUserToAddRows = false;

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
            textBoxRemoteNodeHost.Text = Program.WalletXiropht.WalletSyncHostname;
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

            if (ClassPeerList.PeerList.Count > 0)
            {
                try
                {
                    foreach (var peer in ClassPeerList.PeerList.ToArray())
                    {
                        dataGridViewPeerList.Rows.Add(peer.Value.peer_host, peer.Value.peer_status, "Remove");

                    }
                }
                catch
                {

                }
            }

            checkBoxEnablePeerTrustSystem.Checked = ClassPeerList.PeerEnableTrustSystem;
        }

        private void dataGridViewPeerList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                try
                {
                    string peer = senderGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                    if (ClassPeerList.PeerList.ContainsKey(peer))
                    {
                        ClassPeerList.PeerList.Remove(peer);
                    }

                    dataGridViewPeerList.Rows.RemoveAt(e.RowIndex);
                }
                catch
                {

                }
            }
        }

        private void checkBoxEnablePeerTrustSystem_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEnablePeerTrustSystem.Checked)
            {
                ClassPeerList.PeerEnableTrustSystem = true;
            }
            else
            {
                ClassPeerList.PeerEnableTrustSystem = false;
            }
        }
    }
}