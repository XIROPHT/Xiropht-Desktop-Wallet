using System;
using System.Windows.Forms;

namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    public partial class FirstStartWallet : Form
    {
        public FirstStartWallet()
        {
            InitializeComponent();
            UpdateLangueForm();
            initializationFirstStartForm();
        }

        /// <summary>
        /// Initilization of the first start form.
        /// </summary>
        private void initializationFirstStartForm()
        {
            foreach (string key in ClassTranslation.LanguageDatabases.Keys)
            {
                comboBoxLanguage.Items.Add(ClassTranslation.UppercaseFirst(key));
            }
        }

        private void UpdateLangueForm()
        {
            labelWelcomeText.Text = ClassTranslation.GetLanguageTextFromOrder("FIRST_START_MENU_LABEL_WELCOME");
        }

        private void comboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClassTranslation.ChangeCurrentLanguage(comboBoxLanguage.Items[comboBoxLanguage.SelectedIndex].ToString());
            UpdateLangueForm();
            ClassFormPhase.WalletXiropht.UpdateGraphicLanguageText();
        }

        private void buttonEndSetting_Click(object sender, EventArgs e)
        {
            ClassWalletSetting.SaveSetting();
            Close();
        }
    }
}
