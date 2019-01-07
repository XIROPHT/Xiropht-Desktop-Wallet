using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            labelWaitCreateWallet.Text = ClassTranslation.GetLanguageTextFromOrder("WAITING_CREATE_WALLET_MENU_LABEL_TEXT");
        }
    }
}