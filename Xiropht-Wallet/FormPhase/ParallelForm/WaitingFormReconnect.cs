using System;
using System.Windows.Forms;
using Xiropht_Wallet.Wallet;

namespace Xiropht_Wallet.FormPhase
{
    public partial class WaitingFormReconnect : Form
    {
        public WaitingFormReconnect()
        {
            InitializeComponent();
        }

        private void WaitingFormReconnect_Load(object sender, EventArgs e)
        {
            labelLoadingNetwork.Text = ClassTranslation.GetLanguageTextFromOrder("NETWORK_WAITING_MENU_LABEL_TEXT");
        }

        private async void buttonClose_ClickAsync(object sender, EventArgs e)
        {
            ClassWalletObject.FullDisconnection(true);
            Hide();
        }
    }
}