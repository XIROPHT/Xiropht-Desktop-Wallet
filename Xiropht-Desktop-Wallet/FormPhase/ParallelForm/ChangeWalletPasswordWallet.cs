using System;
using System.Windows.Forms;
using Xiropht_Connector_All.Wallet;
using Xiropht_Wallet.Wallet;

namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    public partial class ChangeWalletPasswordWallet : Form
    {
        public ChangeWalletPasswordWallet()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ClassWalletObject.WalletNewPassword = textBoxWalletNewPassword.Text;
            ClassWalletObject.WalletConnect.SendPacketWallet(
                ClassWalletCommand.ClassWalletSendEnumeration.ChangePassword + "|" + textBoxWalletOldPassword.Text +
                "|" + textBoxPinCode.Text + "|" + textBoxWalletNewPassword.Text + "", ClassWalletObject.Certificate,
                true);
            textBoxPinCode.Text = "";
            textBoxWalletNewPassword.Text = "";
            textBoxWalletOldPassword.Text = "";
            Close();
        }

        private void ChangeWalletPassword_Load(object sender, EventArgs e)
        {
            labelChangePasswordWallet.Text = ClassTranslation.GetLanguageTextFromOrder("CHANGE_PASSWORD_WALLET_LABEL_OLD_PASSWORD_TEXT");
            labelChangePasswordNewPassword.Text = ClassTranslation.GetLanguageTextFromOrder("CHANGE_PASSWORD_WALLET_LABEL_NEW_PASSWORD_TEXT");
            labelChangePasswordPinCode.Text = ClassTranslation.GetLanguageTextFromOrder("CHANGE_PASSWORD_WALLET_LABEL_PIN_CODE_TEXT");
        }
    }
}