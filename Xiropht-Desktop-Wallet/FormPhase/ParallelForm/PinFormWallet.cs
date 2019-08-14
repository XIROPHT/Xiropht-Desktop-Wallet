using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;
using Xiropht_Wallet.Features;
#if WINDOWS
#endif

namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    public partial class PinFormWallet : Form
    {
        public PinFormWallet()
        {
            InitializeComponent();
        }

        public object StopWatch { get; private set; }

        private void ButtonSendPinCode_ClickAsync(object sender, EventArgs e)
        {
            if (textBoxPinCode.Text.Length >= 4)
            {
                new Thread(async delegate()
                {
                    if (!await Program.WalletXiropht.ClassWalletObject.WalletConnect
                        .SendPacketWallet(
                            ClassWalletCommand.ClassWalletSendEnumeration.PinPhase + "|" + textBoxPinCode.Text,
                            Program.WalletXiropht.ClassWalletObject.Certificate, true))
                    {
                        Program.WalletXiropht.ClassWalletObject.FullDisconnection(true);
                        ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder("PIN_CODE_SUBMIT_MENU_NETWORK_ERROR_TEXT"),
                            string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        MessageBox.Show(Program.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("PIN_CODE_SUBMIT_MENU_NETWORK_ERROR_TEXT"));
#endif
                    }

                    MethodInvoker invoke = () => textBoxPinCode.Text = string.Empty;
                    BeginInvoke(invoke);
                    ClassParallelForm.PinFormShowed = false;
                    invoke = () => Hide();
                    BeginInvoke(invoke);
                }).Start();
            }
            else
            {
#if WINDOWS
                ClassFormPhase.MessageBoxInterface(
                    ClassTranslation.GetLanguageTextFromOrder("PIN_CODE_SUBMIT_MENU_WARNING_TEXT"), string.Empty,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                MessageBox.Show(Program.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("PIN_CODE_SUBMIT_MENU_WARNING_TEXT"));
#endif
            }
        }

        private void PinForm_Load(object sender, EventArgs e)
        {
            labelNoticePinCode.Text =
                ClassTranslation.GetLanguageTextFromOrder("PIN_CODE_SUBMIT_MENU_LABEL_INFORMATION_TEXT");
            buttonNumberZero.Text = string.Empty;
            buttonNumberOne.Text = string.Empty;
            buttonNumberTwo.Text = string.Empty;
            buttonNumberThree.Text = string.Empty;
            buttonNumberFour.Text = string.Empty;
            buttonNumberFive.Text = string.Empty;
            buttonNumberSix.Text = string.Empty;
            buttonNumberSeven.Text = string.Empty;
            buttonNumberEight.Text = string.Empty;
            buttonNumberNine.Text = string.Empty;
            buttonNumberTen.Text = string.Empty;
            buttonNumberEleven.Text = string.Empty;
            textBoxPinCode.Text = string.Empty;
            var listButtonId = new List<int>();
            for (var i = 0; i < Controls.Count; i++)
                if (i < Controls.Count)
                    if (Controls[i] is Button)
                        listButtonId.Add(i);

            var counter = 0;
            while (counter < 10)
            {
                var randomButtonId = ClassUtils.GetRandomBetween(0, listButtonId.Count - 1);
                if (Controls[listButtonId[randomButtonId]].Text == "" ||
                    Controls[listButtonId[randomButtonId]].Text == string.Empty)
                {
                    Controls[listButtonId[randomButtonId]].Text = "" + counter;
                    counter++;
                }
            }
        }

        private void buttonNumberZero_Click(object sender, EventArgs e)
        {
            if (sender is Button buttonWhoClick) textBoxPinCode.Text += buttonWhoClick.Text;
        }

        private void buttonNumberOne_Click(object sender, EventArgs e)
        {
            if (sender is Button buttonWhoClick) textBoxPinCode.Text += buttonWhoClick.Text;
        }

        private void buttonNumberTwo_Click(object sender, EventArgs e)
        {
            if (sender is Button buttonWhoClick) textBoxPinCode.Text += buttonWhoClick.Text;
        }

        private void buttonNumberThree_Click(object sender, EventArgs e)
        {
            if (sender is Button buttonWhoClick) textBoxPinCode.Text += buttonWhoClick.Text;
        }

        private void buttonNumberFour_Click(object sender, EventArgs e)
        {
            if (sender is Button buttonWhoClick) textBoxPinCode.Text += buttonWhoClick.Text;
        }

        private void buttonNumberFive_Click(object sender, EventArgs e)
        {
            if (sender is Button buttonWhoClick) textBoxPinCode.Text += buttonWhoClick.Text;
        }

        private void buttonNumberSix_Click(object sender, EventArgs e)
        {
            if (sender is Button buttonWhoClick) textBoxPinCode.Text += buttonWhoClick.Text;
        }

        private void buttonNumberSeven_Click(object sender, EventArgs e)
        {
            if (sender is Button buttonWhoClick) textBoxPinCode.Text += buttonWhoClick.Text;
        }

        private void buttonNumberEight_Click(object sender, EventArgs e)
        {
            if (sender is Button buttonWhoClick) textBoxPinCode.Text += buttonWhoClick.Text;
        }

        private void buttonNumberNine_Click(object sender, EventArgs e)
        {
            if (sender is Button buttonWhoClick) textBoxPinCode.Text += buttonWhoClick.Text;
        }

        private void textBoxPinCode_TextChangedAsync(object sender, EventArgs e)
        {
            if (textBoxPinCode.Text.Length >= 4)
                new Thread(async delegate()
                {
                    if (!await Program.WalletXiropht.ClassWalletObject.WalletConnect
                        .SendPacketWallet(
                            ClassWalletCommand.ClassWalletSendEnumeration.PinPhase + "|" + textBoxPinCode.Text,
                            Program.WalletXiropht.ClassWalletObject.Certificate, true))
                    {
                        Program.WalletXiropht.ClassWalletObject.FullDisconnection(true);
                        ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder("PIN_CODE_SUBMIT_MENU_NETWORK_ERROR_TEXT"),
                            string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        MessageBox.Show(Program.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("PIN_CODE_SUBMIT_MENU_NETWORK_ERROR_TEXT"));
#endif
                    }

                    MethodInvoker invoke = () => textBoxPinCode.Text = string.Empty;
                    BeginInvoke(invoke);
                    ClassParallelForm.PinFormShowed = false;
                    invoke = () => Hide();
                    BeginInvoke(invoke);
                }).Start();
        }

        private void buttonNumberTen_Click(object sender, EventArgs e)
        {
            var buttonWhoClick = sender as Button;
            textBoxPinCode.Text += buttonWhoClick.Text;
        }

        private void buttonNumberEleven_Click(object sender, EventArgs e)
        {
            var buttonWhoClick = sender as Button;
            textBoxPinCode.Text += buttonWhoClick.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBoxPinCode.TextLength > 0)
                textBoxPinCode.Text = textBoxPinCode.Text.Substring(0, textBoxPinCode.TextLength - 1);
        }
    }
}