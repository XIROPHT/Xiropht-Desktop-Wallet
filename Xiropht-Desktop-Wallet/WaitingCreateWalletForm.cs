using System;
using System.Windows.Forms;
using Xiropht_Wallet.Features;

namespace Xiropht_Wallet
{
    public partial class WaitingCreateWalletForm : Form
    {
        public WaitingCreateWalletForm()
        {
            InitializeComponent();
        }

        private void WaitingCreateWalletForm_Load(object sender, EventArgs e)
        {
            labelWaitCreateWallet.Text =
                ClassTranslation.GetLanguageTextFromOrder("WAITING_CREATE_WALLET_MENU_LABEL_TEXT");
        }
    }
}