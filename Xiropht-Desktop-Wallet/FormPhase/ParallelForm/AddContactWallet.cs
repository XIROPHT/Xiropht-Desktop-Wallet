using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    public partial class AddContactWallet : Form
    {
        public AddContactWallet()
        {
            InitializeComponent();
        }

        private void buttonInsertContact_Click(object sender, EventArgs e)
        {
            if (CheckContactInformations())
            {
                if(!ClassContact.InsertContact(textBoxContactName.Text, textBoxContactWalletAddress.Text))
                {
#if WINDOWS
                    ClassFormPhase.MessageBoxInterface(ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_INSERT_CONTACT_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_INSERT_CONTACT_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
#else
                    MessageBox.Show(ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_INSERT_CONTACT_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_INSERT_CONTACT_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
                }
                else
                {
#if WINDOWS
                    ClassFormPhase.MessageBoxInterface(ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_SUCCESS_INSERT_CONTACT_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_SUCCESS_INSERT_CONTACT_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
#else
                    MessageBox.Show(ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_SUCCESS_INSERT_CONTACT_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_SUCCESS_INSERT_CONTACT_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
                    string[] objectContact = { textBoxContactName.Text, textBoxContactWalletAddress.Text, "X" };
                    ListViewItem itemContact = new ListViewItem(objectContact);
                    Program.WalletXiropht.ContactWalletForm.listViewExContact.Items.Add(itemContact);
                    Close();
                }
            }
        }

        /// <summary>
        /// Check contact informations to insert.
        /// </summary>
        /// <returns></returns>
        private bool CheckContactInformations()
        {
            if (string.IsNullOrEmpty(textBoxContactName.Text))
            {
#if WINDOWS
                ClassFormPhase.MessageBoxInterface(ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_EMPTY_CONTACT_NAME_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_EMPTY_CONTACT_NAME_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                MessageBox.Show(ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_EMPTY_CONTACT_NAME_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_EMPTY_CONTACT_NAME_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                return false;
            }
            if (textBoxContactName.Text.Contains("|")) // Don't allow to insert character separator inside.
            {
#if WINDOWS
                ClassFormPhase.MessageBoxInterface(ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_INVALID_CONTACT_NAME_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_INVALID_CONTACT_NAME_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                MessageBox.Show(ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_INVALID_CONTACT_NAME_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_INVALID_CONTACT_NAME_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                return false;
            }

            if (string.IsNullOrEmpty(textBoxContactWalletAddress.Text))
            {
#if WINDOWS
                ClassFormPhase.MessageBoxInterface(ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_EMPTY_CONTACT_WALLET_ADDRESS_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_EMPTY_CONTACT_WALLET_ADDRESS_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                MessageBox.Show(ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_EMPTY_CONTACT_WALLET_ADDRESS_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_EMPTY_CONTACT_WALLET_ADDRESS_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                return false;
            }

            if (CheckSpecialCharacters(textBoxContactWalletAddress.Text))
            {
#if WINDOWS
                ClassFormPhase.MessageBoxInterface(ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_EMPTY_CONTACT_WALLET_ADDRESS_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_INVALID_CONTACT_WALLET_ADDRESS_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                MessageBox.Show(ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_INVALID_CONTACT_WALLET_ADDRESS_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_ERROR_INVALID_CONTACT_WALLET_ADDRESS_CONTENT_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check special characters.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private bool CheckSpecialCharacters(string word)
        {
            var regx = new Regex("[^a-zA-Z0-9_.]");
            return regx.IsMatch(word);
        }

        /// <summary>
        /// Translate on loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddContactWallet_Load(object sender, EventArgs e)
        {
            labelTextContactName.Text = ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_LABEL_CONTACT_NAME_TEXT");
            labelTextContactWalletAddress.Text = ClassTranslation.GetLanguageTextFromOrder("CONTACT_SUBMENU_LABEL_CONTACT_WALLET_ADDRESS_TEXT");
        }
    }
}
