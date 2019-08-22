using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xiropht_Wallet.Features;

namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    public partial class WaitingForm : Form
    {
        public WaitingForm()
        {
            InitializeComponent();
        }

        private void WaitingForm_Load(object sender, EventArgs e)
        {
            labelLoadingNetwork.Text = ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.waitingmenulabeltext);
        }

        private async void ButtonClose_Click(object sender, EventArgs e)
        {
            await Program.WalletXiropht.ClassWalletObject.FullDisconnection(true);
            Hide();
        }

        private void labelLoadingNetwork_Click(object sender, EventArgs e)
        {
        }
    }
}