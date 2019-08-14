using System;
using System.Windows.Forms;
using Xiropht_Wallet.Features;
using Xiropht_Wallet.Utility;

namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    public partial class CreateWalletSuccessFormWallet : Form
    {
        public string PinCode;
        public string PrivateKey;
        public string PublicKey;

        public CreateWalletSuccessFormWallet()
        {
            InitializeComponent();
        }

        private void ButtonAcceptAndCloseWalletInformation_Click(object sender, EventArgs e)
        {
#if WINDOWS
            if (ClassFormPhase.MessageBoxInterface(
                    ClassTranslation.GetLanguageTextFromOrder(
                        ClassTranslationEnumeration.createwalletsubmenubuttonacceptwalletinformationmessagecontenttext),
                    ClassTranslation.GetLanguageTextFromOrder(
                        ClassTranslationEnumeration.createwalletsubmenubuttonacceptwalletinformationmessagetitletext),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
#else
            if (MessageBox.Show(Program.WalletXiropht,
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletsubmenubuttonacceptwalletinformationmessagecontenttext),
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletsubmenubuttonacceptwalletinformationmessagetitletext), MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
#endif
            {
                labelYourPrivateKey.Text =
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletsubmenulabelprivatekeytext);
                labelYourPublicKey.Text =
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletsubmenulabelpublickeytext);
                labelYourPinCode.Text =
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletsubmenulabelpincodetext);
                PrivateKey = string.Empty;
                PublicKey = string.Empty;
                PinCode = string.Empty;
                Close();
            }
            else
            {
#if WINDOWS
                ClassFormPhase.MessageBoxInterface(
                    ClassTranslation.GetLanguageTextFromOrder(
                        ClassTranslationEnumeration.createwalletsubmenubuttonacceptwalletinformationmessagesafecontenttext),
                    string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                MessageBox.Show(Program.WalletXiropht,
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletsubmenubuttonacceptwalletinformationmessagesafecontenttext));
#endif
            }
        }

        private void ButtonCopyWalletInformation_Click(object sender, EventArgs e)
        {
#if WINDOWS
            Clipboard.SetText(labelYourPublicKey.Text + " " + Environment.NewLine +
                              labelYourPrivateKey.Text + " " + Environment.NewLine +
                              labelYourPinCode.Text);


            ClassFormPhase.MessageBoxInterface(
                ClassTranslation.GetLanguageTextFromOrder(
                    ClassTranslationEnumeration.createwalletsubmenubuttoncopywalletinformationcontenttext),
                ClassTranslation.GetLanguageTextFromOrder(
                    ClassTranslationEnumeration.createwalletsubmenubuttoncopywalletinformationtitletext), MessageBoxButtons.OK,
                MessageBoxIcon.Question);
#else
            LinuxClipboard.SetText(labelYourPublicKey.Text + " " + Environment.NewLine +
                             labelYourPrivateKey.Text + " " + Environment.NewLine +
                             labelYourPinCode.Text);


            MessageBox.Show(Program.WalletXiropht,
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletsubmenubuttoncopywalletinformationcontenttext),
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletsubmenubuttoncopywalletinformationtitletext), MessageBoxButtons.OK, MessageBoxIcon.Question);
#endif
        }

        private void CreateWalletSuccessForm_Load(object sender, EventArgs e)
        {
            labelCreateWalletInformation.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletsubmenulabelinformationtext);
            labelYourPinCode.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletsubmenulabelpincodetext) + " " + PinCode;
            labelYourPrivateKey.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletsubmenulabelprivatekeytext) + " " +
                PrivateKey;
            labelYourPublicKey.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletsubmenulabelpublickeytext) + " " +
                PublicKey;
            buttonAcceptAndCloseWalletInformation.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletsubmenubuttonacceptwalletinformationtext);
            buttonCopyWalletInformation.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletsubmenubuttoncopywalletinformationtext);
        }
    }
}