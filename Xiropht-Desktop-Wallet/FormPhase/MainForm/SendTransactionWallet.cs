#if WINDOWS
using MetroFramework;
#endif
using System;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xiropht_Connector_All.Wallet;
using Xiropht_Wallet.Threading;
using Xiropht_Wallet.Wallet;

namespace Xiropht_Wallet.FormPhase.MainForm
{
    public sealed partial class SendTransactionWallet : Form
    {
        private bool AutoUpdateTimeReceived;

        public SendTransactionWallet()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams CP = base.CreateParams;
                CP.ExStyle = CP.ExStyle | 0x02000000; // WS_EX_COMPOSITED
                return CP;
            }
        }

        private async void ButtonSendTransaction_ClickAsync(object sender, EventArgs e)
        {

            try
            {
                string amountstring = textBoxAmount.Text.Replace(",", ".");
                string feestring = textBoxFee.Text.Replace(",", ".");

                var checkAmount = CheckAmount(amountstring);
                var checkFee = CheckAmount(feestring);
                if (checkAmount.Item1)
                {
                    var amountSend = checkAmount.Item2;
                    if (checkFee.Item1)
                    {
                        var feeSend = checkFee.Item2;
                        if (CheckAmountNetwork(amountSend + feeSend))
                        {
                            string destination = ClassUtility.RemoveSpecialCharacters(textBoxWalletDestination.Text);
                            if ((destination.Length >= 48 &&
                                 destination.Length <= 128) && Regex.IsMatch(
                                    destination,
                                    "[a-z0-9]+", RegexOptions.IgnoreCase))
                            {
#if WINDOWS
                                if (ClassFormPhase.MessageBoxInterface(ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_SUBMIT_CONTENT_TEXT").Replace(ClassTranslation.AmountSendOrder, "" + amountSend).Replace(ClassTranslation.TargetAddressOrder, destination),
                                        ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_SUBMIT_TITLE_TEXT"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
                                    DialogResult.Yes)
#else
                                if (MessageBox.Show(ClassFormPhase.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_SUBMIT_CONTENT_TEXT").Replace(ClassTranslation.AmountSendOrder, "" + amountSend).Replace(ClassTranslation.TargetAddressOrder, destination),
                                        ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_SUBMIT_TITLE_TEXT"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
                                    DialogResult.Yes)
#endif
                                {

                                    ClassParallelForm.ShowWaitingFormAsync();

                                    if (checkBoxHideWalletAddress.Checked)
                                    {
                                        await ClassFormPhase.WalletXiropht.ClassWalletObject.WalletConnect.SendPacketWallet(
                                                    ClassWalletCommand.ClassWalletSendEnumeration.SendTransaction + "|" +
                                                    destination + "|" + amountSend + "|" + feeSend + "|1", ClassFormPhase.WalletXiropht.ClassWalletObject.Certificate, true);


                                    }
                                    else
                                    {
                                        await ClassFormPhase.WalletXiropht.ClassWalletObject.WalletConnect.SendPacketWallet(
                                            ClassWalletCommand.ClassWalletSendEnumeration.SendTransaction + "|" +
                                            destination + "|" + amountSend + "|" + feeSend + "|0", ClassFormPhase.WalletXiropht.ClassWalletObject.Certificate, true);
                                    }

                                    MethodInvoker invoke = () =>
                                    {
                                        checkBoxHideWalletAddress.Checked = false;
                                        textBoxAmount.Text = "0.00000000";
                                        textBoxFee.Text = "0.00001000";
                                        textBoxWalletDestination.Text = string.Empty;
                                    };
                                    BeginInvoke(invoke);

                                }
                            }
                            else
                            {
#if WINDOWS
                                ClassFormPhase.MessageBoxInterface(ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_ERROR_TARGET_CONTENT_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                                MessageBox.Show(ClassFormPhase.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_ERROR_TARGET_CONTENT_TEXT"));
#endif
                            }
                        }
                    }
                    else
                    {
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_ERROR_FEE_CONTENT_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        MessageBox.Show(ClassFormPhase.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_ERROR_FEE_CONTENT_TEXT"));
#endif
                    }
                }
                else
                {
#if WINDOWS
                    ClassFormPhase.MessageBoxInterface(ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_ERROR_AMOUNT_CONTENT_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                    MessageBox.Show(ClassFormPhase.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_ERROR_AMOUNT_CONTENT_TEXT"));
#endif
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Exception error: " + error.Message);
            }
            Refresh();
        }

        /// <summary>
        /// Check amount before send.
        /// </summary>
        /// <param name="amount"></param>
        private Tuple<bool, Decimal> CheckAmount(string amount)
        {
            try
            {
                Decimal amountParse =
                    Decimal.Parse(amount, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat);
                return new Tuple<bool, Decimal>(true, amountParse);
            }
            catch
            {
                // ignored
            }


            return new Tuple<bool, Decimal>(false, 0);
        }

        /// <summary>
        /// Check total amount with current balance.
        /// </summary>
        /// <param name="totalAmount"></param>
        /// <returns></returns>
        private bool CheckAmountNetwork(Decimal totalAmount)
        {
            try
            {
                string newTotalAmount = ClassFormPhase.WalletXiropht.ClassWalletObject.WalletConnect.WalletAmount;
                Decimal amount = Decimal.Parse(newTotalAmount, NumberStyles.Any,
                    Program.GlobalCultureInfo);
                if (amount >= totalAmount)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }


        public void GetListControl()
        {
            if (ClassFormPhase.WalletXiropht.ListControlSizeSendTransaction.Count == 0)
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (i < Controls.Count)
                    {
                        ClassFormPhase.WalletXiropht.ListControlSizeSendTransaction.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
                    }
                }
            }
        }


        private void SendTransaction_Load(object sender, EventArgs e)
        {
            UpdateStyles();
            ClassFormPhase.WalletXiropht.ResizeWalletInterface();
            if (!AutoUpdateTimeReceived)
            {
                AutoUpdateTimeReceived = true;
                StartAutoUpdateTimeReceived();
            }
        }

        private async void StartAutoUpdateTimeReceived()
        {
            await Task.Factory.StartNew(async delegate()
            {
                while (true)
                {
                    await Task.Delay(100);
                    if (ClassFormPhase.WalletXiropht.ClassWalletObject.SeedNodeConnectorWallet != null)
                    {
                        if (ClassFormPhase.WalletXiropht.ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                        {
                            MethodInvoker invoke;
                            if (!string.IsNullOrEmpty(textBoxFee.Text))
                            {
                                if (Decimal.TryParse(textBoxFee.Text.Replace(".", ","), out var feeAmount))
                                {
                                    Decimal timePendingFromFee = ClassFormPhase.WalletXiropht.ClassWalletObject.RemoteNodeTotalPendingTransactionInNetwork;


                                    if (timePendingFromFee <= 0)
                                    {
                                        timePendingFromFee = 1;
                                    }
                                    else
                                    {
                                        Decimal decreaseTime = ((feeAmount * 1000) / timePendingFromFee) * 100;
                                        if (decreaseTime > 0)
                                        {
                                            timePendingFromFee = timePendingFromFee - decreaseTime;
                                            if (timePendingFromFee <= 0)
                                            {
                                                timePendingFromFee = 1;
                                            }
                                            else
                                            {
                                                timePendingFromFee = (timePendingFromFee * 1) / 100;
                                            }
                                        }
                                    }
                                    if (timePendingFromFee < 1)
                                    {
                                        timePendingFromFee = 1;
                                    }

                                    if (timePendingFromFee < 60)
                                    {
                                        timePendingFromFee = Math.Round(timePendingFromFee, 0);
                                        invoke = () => textBoxTransactionTime.Text = timePendingFromFee + " " + ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_TIME_SECOND_TEXT");
                                        BeginInvoke(invoke);
                                    }
                                    else if (timePendingFromFee >= 60 && timePendingFromFee < 3600)
                                    {
                                        timePendingFromFee = timePendingFromFee / 60;
                                        timePendingFromFee = Math.Round(timePendingFromFee, 2);
                                        invoke = () => textBoxTransactionTime.Text = timePendingFromFee + " " + ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_TIME_MINUTE_TEXT");
                                        BeginInvoke(invoke);
                                    }
                                    else if (timePendingFromFee >= 3600 && timePendingFromFee < 84600)
                                    {
                                        timePendingFromFee = timePendingFromFee / 3600;
                                        timePendingFromFee = Math.Round(timePendingFromFee, 2);
                                        invoke = () => textBoxTransactionTime.Text = timePendingFromFee + " " + ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_TIME_HOUR_TEXT");
                                        BeginInvoke(invoke);
                                    }
                                    else if (timePendingFromFee >= 84600)
                                    {
                                        timePendingFromFee = timePendingFromFee / 84600;
                                        timePendingFromFee = Math.Round(timePendingFromFee, 2);
                                        invoke = () => textBoxTransactionTime.Text = timePendingFromFee + " " + ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_TIME_DAY_TEXT");
                                        BeginInvoke(invoke);
                                    }
                                }
                                else
                                {
                                    invoke = () => textBoxTransactionTime.Text = "N/A";
                                    BeginInvoke(invoke);
                                }
                            }
                        }
                    }
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Current).ConfigureAwait(false);
        }

        private void CheckBoxHideWalletAddress_Click(object sender, EventArgs e)
        {
            if (checkBoxHideWalletAddress.Checked)
            {
#if WINDOWS
                if (ClassFormPhase.MessageBoxInterface(
                        ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_OPTION_ANONYMITY_CONTENT1_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_OPTION_ANONYMITY_TITLE_TEXT"), MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
#else
                if (MessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_OPTION_ANONYMITY_CONTENT1_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_OPTION_ANONYMITY_TITLE_TEXT"), MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
#endif
                {
                    checkBoxHideWalletAddress.Checked = false;
                }
            }
        }

        private void SendTransaction_Resize(object sender, EventArgs e)
        {
            UpdateStyles();
        }


        private void buttonFeeInformation_MouseHover(object sender, EventArgs e)
        {
            var toolTipFeeInformation = new ToolTip();
            toolTipFeeInformation.SetToolTip(buttonFeeInformation,
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_FEE_INFORMATION_CONTENT_TEXT"));
        }

        private void buttonFeeInformation_Click(object sender, EventArgs e)
        {
#if WINDOWS
            ClassFormPhase.MessageBoxInterface(
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_FEE_INFORMATION_CONTENT_TEXT"),
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_FEE_INFORMATION_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
#else
            MessageBox.Show(ClassFormPhase.WalletXiropht,
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_FEE_INFORMATION_CONTENT_TEXT"),
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_FEE_INFORMATION_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
        }

        private void buttonEstimatedTimeInformation_MouseHover(object sender, EventArgs e)
        {
            var toolTipFeeInformation = new ToolTip();
            toolTipFeeInformation.SetToolTip(buttonEstimatedTimeInformation,
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_TIME_RECEIVE_INFORMATION_CONTENT_TEXT"));
        }

        private void buttonEstimatedTimeInformation_Click(object sender, EventArgs e)
        {
#if WINDOWS
            ClassFormPhase.MessageBoxInterface(
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_TIME_RECEIVE_INFORMATION_CONTENT_TEXT"),
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_TIME_RECEIVE_INFORMATION_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
#else
            MessageBox.Show(ClassFormPhase.WalletXiropht,
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_TIME_RECEIVE_INFORMATION_CONTENT_TEXT"),
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_TIME_RECEIVE_INFORMATION_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
        }

        private void textBoxWalletDestination_TextChanged(object sender, EventArgs e)
        {
            textBoxWalletDestination.Text = ClassUtility.RemoveSpecialCharacters(textBoxWalletDestination.Text);
        }
    }
}