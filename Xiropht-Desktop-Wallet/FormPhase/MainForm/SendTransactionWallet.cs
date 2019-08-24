using System;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xiropht_Connector_All.Setting;
using Xiropht_Connector_All.Wallet;
using Xiropht_Wallet.Features;
using Xiropht_Wallet.Utility;

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
                var CP = base.CreateParams;
                CP.ExStyle = CP.ExStyle | 0x02000000; // WS_EX_COMPOSITED
                return CP;
            }
        }

        private async void ButtonSendTransaction_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                var amountstring = textBoxAmount.Text.Replace(",", ".");
                var feestring = textBoxFee.Text.Replace(",", ".");

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
                            var destination = ClassUtility.RemoveSpecialCharacters(textBoxWalletDestination.Text);
                            if (destination.Length >= 48 &&
                                destination.Length <= 128 && Regex.IsMatch(
                                    destination,
                                    "[a-z0-9]+", RegexOptions.IgnoreCase))
                            {
#if WINDOWS
                                if (ClassFormPhase.MessageBoxInterface(
                                        ClassTranslation
                                            .GetLanguageTextFromOrder(
                                                "SEND_TRANSACTION_WALLET_MESSAGE_SUBMIT_CONTENT_TEXT")
                                            .Replace(ClassTranslation.AmountSendOrder, "" + amountSend)
                                            .Replace(ClassTranslation.TargetAddressOrder, destination),
                                        ClassTranslation.GetLanguageTextFromOrder(
                                            "SEND_TRANSACTION_WALLET_MESSAGE_SUBMIT_TITLE_TEXT"),
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
                                    DialogResult.Yes)
#else
                                if (MessageBox.Show(Program.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_SUBMIT_CONTENT_TEXT").Replace(ClassTranslation.AmountSendOrder, "" + amountSend).Replace(ClassTranslation.TargetAddressOrder, destination),
                                        ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_SUBMIT_TITLE_TEXT"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
                                    DialogResult.Yes)
#endif
                                {
                                    ClassParallelForm.ShowWaitingFormAsync();

                                    if (checkBoxHideWalletAddress.Checked)
                                    {
                                        if (!Program.WalletXiropht.EnableTokenNetworkMode)
                                            await Program.WalletXiropht.ClassWalletObject.WalletConnect
                                                .SendPacketWallet(
                                                    ClassWalletCommand.ClassWalletSendEnumeration.SendTransaction +
                                                    ClassConnectorSetting.PacketContentSeperator +
                                                    destination + ClassConnectorSetting.PacketContentSeperator + amountSend + ClassConnectorSetting.PacketContentSeperator + feeSend + "|1",
                                                    Program.WalletXiropht.ClassWalletObject.Certificate, true);
                                        else
                                            await Program.WalletXiropht.ClassWalletObject.SendWalletTokenTransaction(
                                                destination, amountSend, feeSend, "1");
                                    }
                                    else
                                    {
                                        if (!Program.WalletXiropht.EnableTokenNetworkMode)
                                            await Program.WalletXiropht.ClassWalletObject.WalletConnect
                                                .SendPacketWallet(
                                                    ClassWalletCommand.ClassWalletSendEnumeration.SendTransaction +
                                                    ClassConnectorSetting.PacketContentSeperator +
                                                    destination + ClassConnectorSetting.PacketContentSeperator + amountSend + ClassConnectorSetting.PacketContentSeperator + feeSend + "|0",
                                                    Program.WalletXiropht.ClassWalletObject.Certificate, true);
                                        else
                                            await Program.WalletXiropht.ClassWalletObject.SendWalletTokenTransaction(
                                                destination, amountSend, feeSend, "0");
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
                                ClassFormPhase.MessageBoxInterface(
                                    ClassTranslation.GetLanguageTextFromOrder(
                                        "SEND_TRANSACTION_WALLET_MESSAGE_ERROR_TARGET_CONTENT_TEXT"), string.Empty,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                                MessageBox.Show(Program.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_ERROR_TARGET_CONTENT_TEXT"));
#endif
                            }
                        }
                        else
                        {
#if WINDOWS
                            ClassFormPhase.MessageBoxInterface(
                                ClassTranslation.GetLanguageTextFromOrder(
                                    "WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_AMOUNT_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder(
                                    "WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_AMOUNT_TITLE_TEXT"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                            MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_AMOUNT_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_AMOUNT_TITLE_TEXT"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                        }
                    }
                    else
                    {
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder(
                                "SEND_TRANSACTION_WALLET_MESSAGE_ERROR_FEE_CONTENT_TEXT"), string.Empty,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        MessageBox.Show(Program.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_ERROR_FEE_CONTENT_TEXT"));
#endif
                    }
                }
                else
                {
#if WINDOWS
                    ClassFormPhase.MessageBoxInterface(
                        ClassTranslation.GetLanguageTextFromOrder(
                            "SEND_TRANSACTION_WALLET_MESSAGE_ERROR_AMOUNT_CONTENT_TEXT"), string.Empty,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                    MessageBox.Show(Program.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_ERROR_AMOUNT_CONTENT_TEXT"));
#endif
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Exception error: " + error.Message);
            }

            UpdateStyles();
        }

        /// <summary>
        ///     Check amount before send.
        /// </summary>
        /// <param name="amount"></param>
        private Tuple<bool, decimal> CheckAmount(string amount)
        {
            try
            {
                var amountParse =
                    decimal.Parse(amount, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat);
                return new Tuple<bool, decimal>(true, amountParse);
            }
            catch
            {
                // ignored
            }

            return new Tuple<bool, decimal>(false, 0);
        }

        /// <summary>
        ///     Check total amount with current balance.
        /// </summary>
        /// <param name="totalAmount"></param>
        /// <returns></returns>
        private bool CheckAmountNetwork(decimal totalAmount)
        {
            try
            {
                var newTotalAmount = Program.WalletXiropht.ClassWalletObject.WalletConnect.WalletAmount;
                var amount = decimal.Parse(newTotalAmount.Replace(".", ","), NumberStyles.Any,
                    Program.GlobalCultureInfo);
                if (amount >= totalAmount) return true;
            }
            catch
            {
                return false;
            }

            return false;
        }


        public void GetListControl()
        {
            if (Program.WalletXiropht.ListControlSizeSendTransaction.Count == 0)
                for (var i = 0; i < Controls.Count; i++)
                    if (i < Controls.Count)
                        Program.WalletXiropht.ListControlSizeSendTransaction.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
        }


        private void SendTransaction_Load(object sender, EventArgs e)
        {
            UpdateStyles();
            Program.WalletXiropht.ResizeWalletInterface();
            if (!AutoUpdateTimeReceived)
            {
                AutoUpdateTimeReceived = true;
                StartAutoUpdateTimeReceived();
            }
        }

        private async void StartAutoUpdateTimeReceived()
        {
            await Task.Factory.StartNew(async delegate
            {
                while (true)
                {
                    await Task.Delay(100);
                    if (Program.WalletXiropht.EnableTokenNetworkMode)
                    {
                        CalculateTimeToReceive();
                    }
                    else
                    {
                        if (Program.WalletXiropht.ClassWalletObject.SeedNodeConnectorWallet != null)
                            if (Program.WalletXiropht.ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                                CalculateTimeToReceive();
                    }
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Current).ConfigureAwait(false);
        }

        private void CalculateTimeToReceive()
        {
            MethodInvoker invoke;
            if (!string.IsNullOrEmpty(textBoxFee.Text))
            {
                if (decimal.TryParse(textBoxFee.Text.Replace(".", ","), out var feeAmount))
                {
                    decimal timePendingFromFee = Program.WalletXiropht.ClassWalletObject
                        .RemoteNodeTotalPendingTransactionInNetwork;


                    if (timePendingFromFee <= 0)
                    {
                        timePendingFromFee = 1;
                    }
                    else
                    {
                        var decreaseTime = feeAmount * 1000 / timePendingFromFee * 100;
                        if (decreaseTime > 0)
                        {
                            timePendingFromFee = timePendingFromFee - decreaseTime;
                            if (timePendingFromFee <= 0)
                                timePendingFromFee = 1;
                            else
                                timePendingFromFee = timePendingFromFee * 1 / 100;
                        }
                    }

                    if (timePendingFromFee < 1) timePendingFromFee = 1;

                    if (timePendingFromFee < 60)
                    {
                        timePendingFromFee = Math.Round(timePendingFromFee, 0);
                        invoke = () =>
                            textBoxTransactionTime.Text =
                                timePendingFromFee + " " +
                                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_TIME_SECOND_TEXT");
                        BeginInvoke(invoke);
                    }
                    else if (timePendingFromFee >= 60 && timePendingFromFee < 3600)
                    {
                        timePendingFromFee = timePendingFromFee / 60;
                        timePendingFromFee = Math.Round(timePendingFromFee, 2);
                        invoke = () =>
                            textBoxTransactionTime.Text =
                                timePendingFromFee + " " +
                                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_TIME_MINUTE_TEXT");
                        BeginInvoke(invoke);
                    }
                    else if (timePendingFromFee >= 3600 && timePendingFromFee < 84600)
                    {
                        timePendingFromFee = timePendingFromFee / 3600;
                        timePendingFromFee = Math.Round(timePendingFromFee, 2);
                        invoke = () =>
                            textBoxTransactionTime.Text =
                                timePendingFromFee + " " +
                                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_TIME_HOUR_TEXT");
                        BeginInvoke(invoke);
                    }
                    else if (timePendingFromFee >= 84600)
                    {
                        timePendingFromFee = timePendingFromFee / 84600;
                        timePendingFromFee = Math.Round(timePendingFromFee, 2);
                        invoke = () =>
                            textBoxTransactionTime.Text =
                                timePendingFromFee + " " +
                                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_TIME_DAY_TEXT");
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

        private void CheckBoxHideWalletAddress_Click(object sender, EventArgs e)
        {
            if (checkBoxHideWalletAddress.Checked)
            {
#if WINDOWS
                if (ClassFormPhase.MessageBoxInterface(
                        ClassTranslation.GetLanguageTextFromOrder(
                            "SEND_TRANSACTION_WALLET_MESSAGE_OPTION_ANONYMITY_CONTENT1_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder(
                            "SEND_TRANSACTION_WALLET_MESSAGE_OPTION_ANONYMITY_TITLE_TEXT"), MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error) == DialogResult.No)
#else
                if (MessageBox.Show(Program.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_OPTION_ANONYMITY_CONTENT1_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_OPTION_ANONYMITY_TITLE_TEXT"), MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
#endif
                    checkBoxHideWalletAddress.Checked = false;
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
                ClassTranslation.GetLanguageTextFromOrder(
                    "SEND_TRANSACTION_WALLET_MESSAGE_FEE_INFORMATION_CONTENT_TEXT"));
        }

        private void buttonFeeInformation_Click(object sender, EventArgs e)
        {
#if WINDOWS
            ClassFormPhase.MessageBoxInterface(
                ClassTranslation.GetLanguageTextFromOrder(
                    "SEND_TRANSACTION_WALLET_MESSAGE_FEE_INFORMATION_CONTENT_TEXT"),
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_FEE_INFORMATION_TITLE_TEXT"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
#else
            MessageBox.Show(Program.WalletXiropht,
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_FEE_INFORMATION_CONTENT_TEXT"),
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_FEE_INFORMATION_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
        }

        private void buttonEstimatedTimeInformation_MouseHover(object sender, EventArgs e)
        {
            var toolTipFeeInformation = new ToolTip();
            toolTipFeeInformation.SetToolTip(buttonEstimatedTimeInformation,
                ClassTranslation.GetLanguageTextFromOrder(
                    "SEND_TRANSACTION_WALLET_MESSAGE_TIME_RECEIVE_INFORMATION_CONTENT_TEXT"));
        }

        private void buttonEstimatedTimeInformation_Click(object sender, EventArgs e)
        {
#if WINDOWS
            ClassFormPhase.MessageBoxInterface(
                ClassTranslation.GetLanguageTextFromOrder(
                    "SEND_TRANSACTION_WALLET_MESSAGE_TIME_RECEIVE_INFORMATION_CONTENT_TEXT"),
                ClassTranslation.GetLanguageTextFromOrder(
                    "SEND_TRANSACTION_WALLET_MESSAGE_TIME_RECEIVE_INFORMATION_TITLE_TEXT"), MessageBoxButtons.OK,
                MessageBoxIcon.Information);
#else
            MessageBox.Show(Program.WalletXiropht,
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_TIME_RECEIVE_INFORMATION_CONTENT_TEXT"),
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_TIME_RECEIVE_INFORMATION_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
        }

        private void textBoxWalletDestination_TextChanged(object sender, EventArgs e)
        {
            textBoxWalletDestination.Text = ClassUtility.RemoveSpecialCharacters(textBoxWalletDestination.Text);
        }

        private void textBoxFee_TextChanged(object sender, EventArgs e)
        {
            CalculateTotalToSpend();
        }

        private void CalculateTotalToSpend()
        {
            try
            {
                var amountstring = textBoxAmount.Text.Replace(",", ".");
                var feestring = textBoxFee.Text.Replace(",", ".");

                var checkAmount = CheckAmount(amountstring);
                var checkFee = CheckAmount(feestring);
                if (checkAmount.Item1)
                {
                    var amountSend = checkAmount.Item2;
                    if (checkFee.Item1)
                    {
                        var feeSend = checkFee.Item2;
                        if (checkBoxHideWalletAddress.Checked)
                        {
                            if (CheckAmountNetwork(amountSend + feeSend +
                                                   ClassConnectorSetting.MinimumWalletTransactionAnonymousFee))
                                textBoxTotalSpend.ForeColor = Color.Green;
                            else
                                textBoxTotalSpend.ForeColor = Color.Red;

                            textBoxTotalSpend.Text = ClassUtility.FormatAmount(
                                (amountSend + feeSend + ClassConnectorSetting.MinimumWalletTransactionAnonymousFee)
                                .ToString().Replace(",", "."));
                        }
                        else
                        {
                            if (CheckAmountNetwork(amountSend + feeSend))
                                textBoxTotalSpend.ForeColor = Color.Green;
                            else
                                textBoxTotalSpend.ForeColor = Color.Red;
                            textBoxTotalSpend.Text =
                                ClassUtility.FormatAmount((amountSend + feeSend).ToString().Replace(",", "."));
                        }
                    }
                    else
                    {
                        textBoxTotalSpend.Text = "N/A";
                        textBoxTotalSpend.ForeColor = Color.Black;
                    }
                }
                else
                {
                    textBoxTotalSpend.Text = "N/A";
                    textBoxTotalSpend.ForeColor = Color.Black;
                }
            }
            catch
            {
                textBoxTotalSpend.Text = "N/A";
                textBoxTotalSpend.ForeColor = Color.Black;
            }
        }

        private void textBoxAmount_TextChanged(object sender, EventArgs e)
        {
            CalculateTotalToSpend();
        }

        private void checkBoxHideWalletAddress_CheckedChanged(object sender, EventArgs e)
        {
            CalculateTotalToSpend();
        }

        private void textBoxTotalSpend_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}