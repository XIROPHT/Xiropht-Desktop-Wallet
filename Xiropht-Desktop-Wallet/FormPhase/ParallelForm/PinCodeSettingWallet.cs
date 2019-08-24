using System;
using System.Windows.Forms;
using Xiropht_Connector_All.Setting;
using Xiropht_Connector_All.Wallet;
using Xiropht_Wallet.Features;

namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    public partial class PinCodeSettingWallet : Form
    {
        public PinCodeSettingWallet()
        {
            InitializeComponent();
        }

        private void PinCodeSetting_Load(object sender, EventArgs e)
        {
            if (Program.WalletXiropht.ClassWalletObject.WalletPinDisabled) // Pin disabled
            {
                labelPinCodeStatus.Text =
                    ClassTranslation.GetLanguageTextFromOrder("PIN_CODE_SETTING_MENU_LABEL_STATUS_DISABLED_TEXT");
                buttonChangePinCodeStatus.Text =
                    ClassTranslation.GetLanguageTextFromOrder("PIN_CODE_SETTING_MENU_BUTTON_STATUS_ENABLE_TEXT");
            }
            else
            {
                labelPinCodeStatus.Text =
                    ClassTranslation.GetLanguageTextFromOrder("PIN_CODE_SETTING_MENU_LABEL_STATUS_ENABLED_TEXT");
                buttonChangePinCodeStatus.Text =
                    ClassTranslation.GetLanguageTextFromOrder("PIN_CODE_SETTING_MENU_BUTTON_STATUS_DISABLE_TEXT");
            }

            labelNoticePinInformation.Text =
                ClassTranslation.GetLanguageTextFromOrder("PIN_CODE_SETTING_MENU_LABEL_INFORMATION_TEXT");
            labelNoticePinCodeStatus.Text =
                ClassTranslation.GetLanguageTextFromOrder("PIN_CODE_SETTING_MENU_LABEL_STATUS_INFORMATION_TEXT");
            labelYourPinCode.Text =
                ClassTranslation.GetLanguageTextFromOrder("PIN_CODE_SETTING_MENU_LABEL_WRITE_PIN_CODE_TEXT");
            labelYourPassword.Text =
                ClassTranslation.GetLanguageTextFromOrder("PIN_CODE_SETTING_MENU_LABEL_WRITE_PASSWORD_TEXT");
            buttonChangePinCodeStatus.Text =
                ClassTranslation.GetLanguageTextFromOrder("PIN_CODE_SETTING_MENU_BUTTON_SUBMIT_CHANGE_TEXT");
        }

        private void ButtonChangePinCodeStatus_Click(object sender, EventArgs e)
        {
            if (Program.WalletXiropht.ClassWalletObject.WalletPinDisabled) // Pin disabled
                Program.WalletXiropht.ClassWalletObject.WalletConnect.SendPacketWallet(
                    ClassWalletCommand.ClassWalletSendEnumeration.DisablePinCode + ClassConnectorSetting.PacketContentSeperator + textBoxWalletOldPassword.Text +
                    ClassConnectorSetting.PacketContentSeperator + textBoxPinCode.Text + "|1", Program.WalletXiropht.ClassWalletObject.Certificate, true);
            else
                Program.WalletXiropht.ClassWalletObject.WalletConnect.SendPacketWallet(
                    ClassWalletCommand.ClassWalletSendEnumeration.DisablePinCode + ClassConnectorSetting.PacketContentSeperator + textBoxWalletOldPassword.Text +
                    ClassConnectorSetting.PacketContentSeperator + textBoxPinCode.Text + "|0", Program.WalletXiropht.ClassWalletObject.Certificate, true);

            textBoxWalletOldPassword.Text = "";
            textBoxPinCode.Text = "";
            Close();
        }
    }
}