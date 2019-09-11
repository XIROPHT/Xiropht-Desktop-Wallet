using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xiropht_Connector_All.Setting;
using Xiropht_Connector_All.Utils;
using Xiropht_Wallet.Features;
using Xiropht_Wallet.FormCustom;
using Xiropht_Wallet.Utility;
using Xiropht_Wallet.Wallet;
using Xiropht_Wallet.Wallet.Sync;
using Timer = System.Windows.Forms.Timer;
#if DEBUG
using Xiropht_Wallet.Debug;
#endif
namespace Xiropht_Wallet.FormPhase.MainForm
{
    public sealed partial class TransactionHistoryWallet : Form
    {
        public Label _labelWaitingText = new Label();
        private ClassPanel _panelWaitingSync;
        public bool IsShowed;
        public bool IsShowedWaitingTransaction;

        public TransactionHistoryWallet()
        {
            InitializeComponent();
            AutoScroll = true;
            IsShowed = false;
            SetStyle(
                ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
        }


        public async void ResyncTransactionAsync()
        {
            Program.WalletXiropht.ClassWalletObject.BlockTransactionSync = true;
            Program.WalletXiropht.StopUpdateTransactionHistory(true, true);
            if (ClassWalletTransactionCache.RemoveWalletCache(Program.WalletXiropht.ClassWalletObject.WalletConnect
                .WalletAddress))
                if (ClassWalletTransactionAnonymityCache.RemoveWalletCache(Program.WalletXiropht.ClassWalletObject
                    .WalletConnect.WalletAddress))
                {
                    ClassWalletTransactionCache.ListTransaction.Clear();
                    ClassWalletTransactionAnonymityCache.ListTransaction.Clear();
                    Program.WalletXiropht.ClassWalletObject.InSyncTransaction = false;
                    Program.WalletXiropht.ClassWalletObject.InSyncTransactionAnonymity = false;
                    Program.WalletXiropht.ClassWalletObject.BlockTransactionSync = false;
                    await Program.WalletXiropht.ClassWalletObject.DisconnectRemoteNodeTokenSync();
                    Program.WalletXiropht.ClassWalletObject.WalletOnUseSync = false;
                }
        }

        public void AutoResizeColumns(ListView lv)
        {
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            var cc = lv.Columns;
            for (var i = 0; i < cc.Count; i++)
            {
                var colWidth = TextRenderer.MeasureText(cc[i].Text, lv.Font).Width + 30;
                if (colWidth > cc[i].Width ||
                    cc[i].Text == ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.transactionhistorywalletcolumnfee) ||
                    cc[i].Text == ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.transactionhistorywalletcolumnamount))
                    cc[i].Width = colWidth + 30;
            }
        }

        private void Transaction_Load(object sender, EventArgs e)
        {
            listViewAnonymityReceivedTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewAnonymityReceivedTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            listViewAnonymitySendTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewAnonymitySendTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            listViewBlockRewardTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewBlockRewardTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            listViewNormalReceivedTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewNormalReceivedTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);


            listViewNormalSendTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewNormalSendTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            listViewAnonymityReceivedTransactionHistory.MultiSelect = true;
            listViewAnonymitySendTransactionHistory.MultiSelect = true;
            listViewBlockRewardTransactionHistory.MultiSelect = true;
            listViewNormalReceivedTransactionHistory.MultiSelect = true;
            listViewNormalSendTransactionHistory.MultiSelect = true;


            _panelWaitingSync = new ClassPanel
            {
                Width = (int) (Width / 1.5f),
                Height = (int) (Height / 5.5f),
                BackColor = Color.LightBlue
            };
            _panelWaitingSync.Location = new Point
            {
                X = Width / 2 - _panelWaitingSync.Width / 2,
                Y = Height / 2 - _panelWaitingSync.Height / 2
            };

            _labelWaitingText.AutoSize = true;
            _labelWaitingText.Font = new Font(_labelWaitingText.Font.FontFamily, 9f, FontStyle.Bold);
            _labelWaitingText.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.transactionhistorywalletwaitingmessagesynctext);
            _panelWaitingSync.Controls.Add(_labelWaitingText);
            _labelWaitingText.Location = new Point
            {
                X = _panelWaitingSync.Width / 2 - _labelWaitingText.Width / 2,
                Y = _panelWaitingSync.Height / 2 - _labelWaitingText.Height / 2
            };
            _labelWaitingText.Show();
            Controls.Add(_panelWaitingSync);
            _panelWaitingSync.Show();
            IsShowed = true;

            listViewNormalSendTransactionHistory.Show();
            listViewBlockRewardTransactionHistory.Hide();
            listViewAnonymityReceivedTransactionHistory.Hide();
            listViewAnonymitySendTransactionHistory.Hide();
            listViewNormalReceivedTransactionHistory.Hide();
            UpdateStyles();
        }

        public void ShowWaitingSyncTransactionPanel()
        {
            if (!IsShowedWaitingTransaction)
            {
                _panelWaitingSync.Visible = true;
                _panelWaitingSync.Show();
                _panelWaitingSync.BringToFront();
                _panelWaitingSync.Width = (int) (Width / 1.5f);
                _panelWaitingSync.Height = (int) (Height / 5.5f);
                _panelWaitingSync.Location = new Point
                {
                    X = Width / 2 -
                        _panelWaitingSync.Width / 2,
                    Y = Height / 2 -
                        _panelWaitingSync.Height / 2
                };
                _labelWaitingText.Location = new Point
                {
                    X = _panelWaitingSync.Width / 2 - _labelWaitingText.Width / 2,
                    Y = _panelWaitingSync.Height / 2 - _labelWaitingText.Height / 2
                };
                IsShowedWaitingTransaction = true;
                UpdateStyles();
            }
        }

        public void HideWaitingSyncTransactionPanel()
        {
            if (IsShowedWaitingTransaction)
            {
                _panelWaitingSync.Visible = false;
                _panelWaitingSync.Hide();
                IsShowedWaitingTransaction = false;
                UpdateStyles();
            }

        }

        protected override void OnResize(EventArgs e)
        {
            if (_panelWaitingSync != null)
            {
                _panelWaitingSync.Width = (int) (Width / 1.5f);
                _panelWaitingSync.Height = (int) (Height / 5.5f);
                _panelWaitingSync.Location = new Point
                {
                    X = Width / 2 -
                        _panelWaitingSync.Width / 2,
                    Y = Height / 2 -
                        _panelWaitingSync.Height / 2
                };
                _labelWaitingText.Location = new Point
                {
                    X = _panelWaitingSync.Width / 2 - _labelWaitingText.Width / 2,
                    Y = _panelWaitingSync.Height / 2 - _labelWaitingText.Height / 2
                };
                UpdateStyles();
            }

            base.OnResize(e);
        }

        public void GetListControl()
        {
            if (Program.WalletXiropht.ListControlSizeTransaction.Count == 0)
                for (var i = 0; i < Controls.Count; i++)
                    if (i < Controls.Count)
                        Program.WalletXiropht.ListControlSizeTransaction.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));

            if (Program.WalletXiropht.ListControlSizeTransactionTabPage.Count == 0)
                for (var i = 0; i < tabPageTransactionHistory.Controls.Count; i++)
                    if (i < tabPageTransactionHistory.Controls.Count)
                        Program.WalletXiropht.ListControlSizeTransactionTabPage.Add(
                            new Tuple<Size, Point>(tabPageTransactionHistory.Controls[i].Size,
                                tabPageTransactionHistory.Controls[i].Location));

        }

        private void listViewNormalSendTransactionHistory_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                var item = listViewNormalSendTransactionHistory.GetItemAt(0, e.Y);

                if (item == null) return;
                for (var ix = item.SubItems.Count - 1; ix >= 0; --ix)
                    if (item.SubItems[ix].Bounds.Contains(e.Location))
                    {


                        if (ClassContact.CheckContactName(item.SubItems[ix].Text))
                        {
                            Clipboard.SetText(
                                ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                ClassFormPhase.MessageBoxInterface(
                                    ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2 + " " +
                                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.transactionhistorywalletcopytext), string.Empty, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information)).ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker =
 () => MessageBox.Show(Program.WalletXiropht, ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2 + " " + ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.transactionhistorywalletcopytext));
                                    BeginInvoke(invoker);
                                }).ConfigureAwait(false);
#endif
                        }
                        else
                        {
                            Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                ClassFormPhase.MessageBoxInterface(
                                    item.SubItems[ix].Text + " " +
                                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.transactionhistorywalletcopytext), string.Empty, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information)).ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(item.SubItems[ix].Text);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker =
 () => MessageBox.Show(Program.WalletXiropht, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.transactionhistorywalletcopytext));
                                    BeginInvoke(invoker);
                                }).ConfigureAwait(false);
#endif
                        }


                        return;
                    }
            }
            catch
            {
            }
        }

        private void listViewNormalReceivedTransactionHistory_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                var item = listViewNormalReceivedTransactionHistory.GetItemAt(5, e.Y);
                if (item == null) return;
                for (var ix = item.SubItems.Count - 1; ix >= 0; --ix)
                    if (item.SubItems[ix].Bounds.Contains(e.Location))
                    {
                        if (ClassContact.CheckContactName(item.SubItems[ix].Text))
                        {
                            Clipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                    ClassFormPhase.MessageBoxInterface(
                                        ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2 + " " +
                                        ClassTranslation.GetLanguageTextFromOrder(
                                            ClassTranslationEnumeration.transactionhistorywalletcopytext),
                                        string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information))
                                .ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker =
 () => MessageBox.Show(Program.WalletXiropht, ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2 + " " + ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.transactionhistorywalletcopytext));
                                    BeginInvoke(invoker);
                                }).ConfigureAwait(false);
#endif
                        }
                        else
                        {
                            Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                    ClassFormPhase.MessageBoxInterface(
                                        item.SubItems[ix].Text + " " +
                                        ClassTranslation.GetLanguageTextFromOrder(
                                            ClassTranslationEnumeration.transactionhistorywalletcopytext),
                                        string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information))
                                .ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(item.SubItems[ix].Text);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker =
 () => MessageBox.Show(Program.WalletXiropht, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.transactionhistorywalletcopytext));
                                    BeginInvoke(invoker);
                                }).ConfigureAwait(false);
#endif
                        }

                        return;
                    }
            }
            catch
            {
            }
        }

        private void listViewAnonymitySendTransactionHistory_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                var item = listViewAnonymitySendTransactionHistory.GetItemAt(5, e.Y);
                if (item == null) return;
                for (var ix = item.SubItems.Count - 1; ix >= 0; --ix)
                    if (item.SubItems[ix].Bounds.Contains(e.Location))
                    {
                        if (ClassContact.CheckContactName(item.SubItems[ix].Text))
                        {
                            Clipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                    ClassFormPhase.MessageBoxInterface(
                                        ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2 + " " +
                                        ClassTranslation.GetLanguageTextFromOrder(
                                            ClassTranslationEnumeration.transactionhistorywalletcopytext),
                                        string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information))
                                .ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker =
 () => MessageBox.Show(Program.WalletXiropht, ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2 + " " + ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.transactionhistorywalletcopytext));
                                    BeginInvoke(invoker);
                                }).ConfigureAwait(false);
#endif
                        }
                        else
                        {
                            Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                    ClassFormPhase.MessageBoxInterface(
                                        item.SubItems[ix].Text + " " +
                                        ClassTranslation.GetLanguageTextFromOrder(
                                            ClassTranslationEnumeration.transactionhistorywalletcopytext),
                                        string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information))
                                .ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(item.SubItems[ix].Text);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker =
 () => MessageBox.Show(Program.WalletXiropht, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.transactionhistorywalletcopytext));
                                    BeginInvoke(invoker);
                                }).ConfigureAwait(false);
#endif
                        }

                        return;
                    }
            }
            catch
            {
            }
        }

        private void listViewAnonymityReceivedTransactionHistory_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                var item = listViewAnonymityReceivedTransactionHistory.GetItemAt(5, e.Y);
                if (item == null) return;
                for (var ix = item.SubItems.Count - 1; ix >= 0; --ix)
                    if (item.SubItems[ix].Bounds.Contains(e.Location))
                    {
                        if (ClassContact.CheckContactName(item.SubItems[ix].Text))
                        {
                            Clipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                    ClassFormPhase.MessageBoxInterface(
                                        ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2 + " " +
                                        ClassTranslation.GetLanguageTextFromOrder(
                                            ClassTranslationEnumeration.transactionhistorywalletcopytext),
                                        string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information))
                                .ConfigureAwait(false);
#else
                            LinuxClipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2);
                            Task.Factory.StartNew(() =>
                            {
                                MethodInvoker invoker =
 () => MessageBox.Show(Program.WalletXiropht, ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2 + " " + ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.transactionhistorywalletcopytext));
                                BeginInvoke(invoker);
                            }).ConfigureAwait(false);
#endif
                        }
                        else
                        {
                            Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                    ClassFormPhase.MessageBoxInterface(
                                        item.SubItems[ix].Text + " " +
                                        ClassTranslation.GetLanguageTextFromOrder(
                                            ClassTranslationEnumeration.transactionhistorywalletcopytext),
                                        string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information))
                                .ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(item.SubItems[ix].Text);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker =
 () => MessageBox.Show(Program.WalletXiropht, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.transactionhistorywalletcopytext));
                                    BeginInvoke(invoker);
                                }).ConfigureAwait(false);
#endif
                        }

                        return;
                    }
            }
            catch
            {
            }
        }

        private void listViewBlockRewardTransactionHistory_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                var item = listViewBlockRewardTransactionHistory.GetItemAt(5, e.Y);
                if (item == null) return;
                for (var ix = item.SubItems.Count - 1; ix >= 0; --ix)
                    if (item.SubItems[ix].Bounds.Contains(e.Location))
                    {
                        if (ClassContact.CheckContactName(item.SubItems[ix].Text))
                        {
                            Clipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                    ClassFormPhase.MessageBoxInterface(
                                        ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2 + " " +
                                        ClassTranslation.GetLanguageTextFromOrder(
                                            ClassTranslationEnumeration.transactionhistorywalletcopytext),
                                        string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information))
                                .ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker =
 () => MessageBox.Show(Program.WalletXiropht, ClassContact.ListContactWallet[item.SubItems[ix].Text.ToLower()].Item2 + " " + ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.transactionhistorywalletcopytext));
                                    BeginInvoke(invoker);
                                }).ConfigureAwait(false);
#endif
                        }
                        else
                        {
                            Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                    ClassFormPhase.MessageBoxInterface(
                                        item.SubItems[ix].Text + " " +
                                        ClassTranslation.GetLanguageTextFromOrder(
                                            ClassTranslationEnumeration.transactionhistorywalletcopytext),
                                        string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information))
                                .ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(item.SubItems[ix].Text);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker =
 () => MessageBox.Show(Program.WalletXiropht, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.transactionhistorywalletcopytext));
                                    BeginInvoke(invoker);
                                }).ConfigureAwait(false);
#endif
                        }

                        return;
                    }
            }
            catch
            {
            }
        }

        private void Transaction_Resize(object sender, EventArgs e)
        {
            if (_panelWaitingSync != null)
            {
                _panelWaitingSync.Width = (int)(Width / 1.5f);
                _panelWaitingSync.Height = (int)(Height / 5.5f);
                _panelWaitingSync.Location = new Point
                {
                    X = Width / 2 -
                        _panelWaitingSync.Width / 2,
                    Y = Height / 2 -
                        _panelWaitingSync.Height / 2
                };
                _labelWaitingText.Location = new Point
                {
                    X = _panelWaitingSync.Width / 2 - _labelWaitingText.Width / 2,
                    Y = _panelWaitingSync.Height / 2 - _labelWaitingText.Height / 2
                };
                UpdateStyles();
            }
        }


        private void tabPageTransactionHistory_Selected(object sender, TabControlEventArgs e)
        {
            Program.WalletXiropht.UpdateCurrentPageNumberTransactionHistory();
            if (tabPageNormalTransactionSend.Visible) //  Normal transaction send list
            {
                listViewNormalSendTransactionHistory.Show();
                listViewBlockRewardTransactionHistory.Hide();
                listViewAnonymityReceivedTransactionHistory.Hide();
                listViewAnonymitySendTransactionHistory.Hide();
                listViewNormalReceivedTransactionHistory.Hide();
            }

            if (tabPageNormalTransactionReceived.Visible) // Normal transaction received list
            {
                listViewNormalSendTransactionHistory.Hide();
                listViewBlockRewardTransactionHistory.Hide();
                listViewAnonymityReceivedTransactionHistory.Hide();
                listViewAnonymitySendTransactionHistory.Hide();
                listViewNormalReceivedTransactionHistory.Show();
            }

            if (tabPageAnonymityTransactionSend.Visible) // Anonymous transaction send list 
            {
                listViewNormalSendTransactionHistory.Hide();
                listViewBlockRewardTransactionHistory.Hide();
                listViewAnonymityReceivedTransactionHistory.Hide();
                listViewAnonymitySendTransactionHistory.Show();
                listViewNormalReceivedTransactionHistory.Hide();
            }

            if (tabPageAnonymityTransactionReceived.Visible) // Anonymous transaction received list 
            {
                listViewNormalSendTransactionHistory.Hide();
                listViewBlockRewardTransactionHistory.Hide();
                listViewAnonymityReceivedTransactionHistory.Show();
                listViewAnonymitySendTransactionHistory.Hide();
                listViewNormalReceivedTransactionHistory.Hide();
            }

            if (tabPageBlockRewardTransaction.Visible) // block reward transaction list 
            {
                listViewNormalSendTransactionHistory.Hide();
                listViewBlockRewardTransactionHistory.Show();
                listViewAnonymityReceivedTransactionHistory.Hide();
                listViewAnonymitySendTransactionHistory.Hide();
                listViewNormalReceivedTransactionHistory.Hide();
            }
        }

        private void tabPageTransactionHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.WalletXiropht.UpdateCurrentPageNumberTransactionHistory();
            if (tabPageNormalTransactionSend.Visible) //  Normal transaction send list
            {
                listViewNormalSendTransactionHistory.Show();
                listViewBlockRewardTransactionHistory.Hide();
                listViewAnonymityReceivedTransactionHistory.Hide();
                listViewAnonymitySendTransactionHistory.Hide();
                listViewNormalReceivedTransactionHistory.Hide();
            }

            if (tabPageNormalTransactionReceived.Visible) // Normal transaction received list
            {
                listViewNormalSendTransactionHistory.Hide();
                listViewBlockRewardTransactionHistory.Hide();
                listViewAnonymityReceivedTransactionHistory.Hide();
                listViewAnonymitySendTransactionHistory.Hide();
                listViewNormalReceivedTransactionHistory.Show();
            }

            if (tabPageAnonymityTransactionSend.Visible) // Anonymous transaction send list 
            {
                listViewNormalSendTransactionHistory.Hide();
                listViewBlockRewardTransactionHistory.Hide();
                listViewAnonymityReceivedTransactionHistory.Hide();
                listViewAnonymitySendTransactionHistory.Show();
                listViewNormalReceivedTransactionHistory.Hide();
            }

            if (tabPageAnonymityTransactionReceived.Visible) // Anonymous transaction received list 
            {
                listViewNormalSendTransactionHistory.Hide();
                listViewBlockRewardTransactionHistory.Hide();
                listViewAnonymityReceivedTransactionHistory.Show();
                listViewAnonymitySendTransactionHistory.Hide();
                listViewNormalReceivedTransactionHistory.Hide();
            }

            if (tabPageBlockRewardTransaction.Visible) // block reward transaction list 
            {
                listViewNormalSendTransactionHistory.Hide();
                listViewBlockRewardTransactionHistory.Show();
                listViewAnonymityReceivedTransactionHistory.Hide();
                listViewAnonymitySendTransactionHistory.Hide();
                listViewNormalReceivedTransactionHistory.Hide();
            }
        }

        /// <summary>
        /// Update tx showed.
        /// </summary>
        public void UpdateTxShowed()
        {
            if (IsShowed)
            {
                #region Update transaction normal send color

                if (listViewNormalSendTransactionHistory.Items.Count >0)
                {
                    ListViewItem[] cloneItemsTx =
                        new ListViewItem[listViewNormalSendTransactionHistory.Items.Count];

                    listViewNormalSendTransactionHistory.Items
                        .CopyTo(cloneItemsTx, 0);

                    for (var i = 0; i < cloneItemsTx.Length; i++)
                    {
                        var i1 = i;


                        try
                        {
                            if (i1 < cloneItemsTx.Length)
                            {
                                if (Program.WalletXiropht.CurrentTransactionHistoryPageNormalSend == 1)
                                {
                                    if (cloneItemsTx[i1].SubItems[0].Text !=
                                        (i1 + 1).ToString("F0"))
                                    {

                                        listViewNormalSendTransactionHistory
                                                .Items[i1]
                                                .SubItems[0].Text =
                                            (i1 + 1).ToString("F0");
                                    }
                                }

                                var transactionWalletDateRecv =
                                    DateTime.Parse(cloneItemsTx[i1].SubItems[7].Text);

                                if (ClassUtils.DateUnixTimeNowSecondConvertDate(
                                        transactionWalletDateRecv) >
                                    ClassUtils.DateUnixTimeNowSecond())
                                {
                                    if (cloneItemsTx[i1].BackColor !=
                                        Color.FromArgb(255, 153, 102))
                                    {

                                        listViewNormalSendTransactionHistory.Items[i1]
                                            .BackColor = Color.FromArgb(255, 153, 102);


                                    }
                                }
                                else
                                {
                                    if (cloneItemsTx[i1].BackColor !=
                                        Color.FromArgb(0, 255, 153))
                                    {

                                        listViewNormalSendTransactionHistory.Items[i1]
                                            .BackColor = Color.FromArgb(0, 255, 153);

                                    }

                                }

                                if (cloneItemsTx[i1].SubItems[6].Text !=
                                    ClassWalletTransactionType.AnonymousTransaction)
                                {
                                    if (ClassContact.CheckContactNameFromWalletAddress(
                                        cloneItemsTx[i1].SubItems[6].Text))
                                    {
                                        string contactName = ClassContact
                                            .GetContactNameFromWalletAddress(
                                                cloneItemsTx[i1].SubItems[6].Text);
                                        if (cloneItemsTx[i1].SubItems[6].Text != contactName
                                        )
                                        {

                                            listViewNormalSendTransactionHistory
                                                .Items[i1]
                                                .SubItems[6].Text = contactName;

                                        }
                                    }

                                }
                            }
                        }
                        catch
                        {
                        }

                    }

                    Array.Clear(cloneItemsTx, 0, cloneItemsTx.Length);
                }

                #endregion

                #region Update transaction normal recv color

                if (listViewNormalReceivedTransactionHistory.Items.Count > 0)
                {
                    ListViewItem[] cloneItemsTx =
                        new ListViewItem[listViewNormalReceivedTransactionHistory.Items.Count];

                    listViewNormalReceivedTransactionHistory.Items
                        .CopyTo(cloneItemsTx, 0);

                    for (var i = 0; i < cloneItemsTx.Length; i++)
                    {
                        var i1 = i;


                        try
                        {
                            if (i1 < cloneItemsTx.Length)
                            {
                                if (Program.WalletXiropht.CurrentTransactionHistoryPageNormalReceive == 1)
                                {
                                    if (cloneItemsTx[i1].SubItems[0].Text !=
                                        (i1 + 1).ToString("F0"))
                                    {

                                        listViewNormalReceivedTransactionHistory
                                                .Items[i1]
                                                .SubItems[0].Text =
                                            (i1 + 1).ToString("F0");
                                    }
                                }

                                var transactionWalletDateRecv =
                                    DateTime.Parse(cloneItemsTx[i1].SubItems[7].Text);

                                if (ClassUtils.DateUnixTimeNowSecondConvertDate(
                                        transactionWalletDateRecv) >
                                    ClassUtils.DateUnixTimeNowSecond())
                                {
                                    if (cloneItemsTx[i1].BackColor !=
                                        Color.FromArgb(255, 153, 102))
                                    {

                                        listViewNormalReceivedTransactionHistory.Items[i1]
                                            .BackColor = Color.FromArgb(255, 153, 102);


                                    }
                                }
                                else
                                {
                                    if (cloneItemsTx[i1].BackColor !=
                                        Color.FromArgb(0, 255, 153))
                                    {

                                        listViewNormalReceivedTransactionHistory.Items[i1]
                                            .BackColor = Color.FromArgb(0, 255, 153);

                                    }

                                }

                                if (cloneItemsTx[i1].SubItems[6].Text !=
                                    ClassWalletTransactionType.AnonymousTransaction)
                                {
                                    if (ClassContact.CheckContactNameFromWalletAddress(
                                        cloneItemsTx[i1].SubItems[6].Text))
                                    {
                                        string contactName = ClassContact
                                            .GetContactNameFromWalletAddress(
                                                cloneItemsTx[i1].SubItems[6].Text);
                                        if (cloneItemsTx[i1].SubItems[6].Text != contactName
                                        )
                                        {

                                            listViewNormalReceivedTransactionHistory
                                                .Items[i1]
                                                .SubItems[6].Text = contactName;

                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }

                    }

                    Array.Clear(cloneItemsTx, 0, cloneItemsTx.Length);
                }

                #endregion

                #region Update transaction block reward color

                if (listViewBlockRewardTransactionHistory.Items.Count > 0)
                {
                    ListViewItem[] cloneItemsTx =
                        new ListViewItem[listViewBlockRewardTransactionHistory.Items.Count];

                    listViewBlockRewardTransactionHistory.Items
                        .CopyTo(cloneItemsTx, 0);

                    for (var i = 0; i < cloneItemsTx.Length; i++)
                    {
                        var i1 = i;


                        try
                        {
                            if (i1 < cloneItemsTx.Length)
                            {
                                if (Program.WalletXiropht.CurrentTransactionHistoryPageBlockReward == 1)
                                {
                                    if (cloneItemsTx[i1].SubItems[0].Text !=
                                        (i1 + 1).ToString("F0"))
                                    {

                                        listViewBlockRewardTransactionHistory
                                                .Items[i1]
                                                .SubItems[0].Text =
                                            (i1 + 1).ToString("F0");
                                    }
                                }

                                var transactionWalletDateRecv =
                                    DateTime.Parse(cloneItemsTx[i1].SubItems[7].Text);

                                if (ClassUtils.DateUnixTimeNowSecondConvertDate(
                                        transactionWalletDateRecv) >
                                    ClassUtils.DateUnixTimeNowSecond())
                                {
                                    if (cloneItemsTx[i1].BackColor !=
                                        Color.FromArgb(255, 153, 102))
                                    {

                                        listViewBlockRewardTransactionHistory.Items[i1]
                                            .BackColor = Color.FromArgb(255, 153, 102);


                                    }
                                }
                                else
                                {
                                    if (cloneItemsTx[i1].BackColor !=
                                        Color.FromArgb(0, 255, 153))
                                    {

                                        listViewBlockRewardTransactionHistory.Items[i1]
                                            .BackColor = Color.FromArgb(0, 255, 153);

                                    }

                                }
                            }
                        }
                        catch
                        {
                        }

                    }

                    Array.Clear(cloneItemsTx, 0, cloneItemsTx.Length);
                }

                #endregion

                #region Update transaction anonymous send color

                if (listViewAnonymitySendTransactionHistory.Items.Count > 0)
                {
                    ListViewItem[] cloneItemsTx =
                        new ListViewItem[listViewAnonymitySendTransactionHistory.Items.Count];

                    listViewAnonymitySendTransactionHistory.Items
                        .CopyTo(cloneItemsTx, 0);

                    for (var i = 0; i < cloneItemsTx.Length; i++)
                    {
                        var i1 = i;


                        try
                        {
                            if (i1 < cloneItemsTx.Length)
                            {
                                if (Program.WalletXiropht.CurrentTransactionHistoryPageAnonymousSend == 1)
                                {
                                    if (cloneItemsTx[i1].SubItems[0].Text !=
                                        (i1 + 1).ToString("F0"))
                                    {

                                        listViewAnonymitySendTransactionHistory
                                                .Items[i1]
                                                .SubItems[0].Text =
                                            (i1 + 1).ToString("F0");
                                    }
                                }

                                var transactionWalletDateRecv =
                                    DateTime.Parse(cloneItemsTx[i1].SubItems[7].Text);

                                if (ClassUtils.DateUnixTimeNowSecondConvertDate(
                                        transactionWalletDateRecv) >
                                    ClassUtils.DateUnixTimeNowSecond())
                                {
                                    if (cloneItemsTx[i1].BackColor !=
                                        Color.FromArgb(255, 153, 102))
                                    {

                                        listViewAnonymitySendTransactionHistory.Items[i1]
                                            .BackColor = Color.FromArgb(255, 153, 102);


                                    }
                                }
                                else
                                {
                                    if (cloneItemsTx[i1].BackColor !=
                                        Color.FromArgb(0, 255, 153))
                                    {

                                        listViewAnonymitySendTransactionHistory.Items[i1]
                                            .BackColor = Color.FromArgb(0, 255, 153);

                                    }

                                }

                                if (cloneItemsTx[i1].SubItems[6].Text !=
                                    ClassWalletTransactionType.AnonymousTransaction)
                                {
                                    if (ClassContact.CheckContactNameFromWalletAddress(
                                        cloneItemsTx[i1].SubItems[6].Text))
                                    {
                                        string contactName = ClassContact
                                            .GetContactNameFromWalletAddress(
                                                cloneItemsTx[i1].SubItems[6].Text);
                                        if (cloneItemsTx[i1].SubItems[6].Text != contactName
                                        )
                                        {

                                            listViewAnonymitySendTransactionHistory
                                                .Items[i1]
                                                .SubItems[6].Text = contactName;

                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }

                    }

                    Array.Clear(cloneItemsTx, 0, cloneItemsTx.Length);
                }
                #endregion

                #region Update transaction anonymous recv color

                if (listViewAnonymityReceivedTransactionHistory.Items.Count > 0)
                {
                    ListViewItem[] cloneItemsTx =
                        new ListViewItem[listViewAnonymityReceivedTransactionHistory.Items.Count];

                    listViewAnonymityReceivedTransactionHistory.Items
                        .CopyTo(cloneItemsTx, 0);

                    for (var i = 0; i < cloneItemsTx.Length; i++)
                    {
                        var i1 = i;


                        try
                        {
                            if (i1 < cloneItemsTx.Length)
                            {
                                if (Program.WalletXiropht.CurrentTransactionHistoryPageAnonymousReceived == 1)
                                {
                                    if (cloneItemsTx[i1].SubItems[0].Text !=
                                        (i1 + 1).ToString("F0"))
                                    {

                                        listViewAnonymityReceivedTransactionHistory
                                                .Items[i1]
                                                .SubItems[0].Text =
                                            (i1 + 1).ToString("F0");
                                    }
                                }

                                var transactionWalletDateRecv =
                                    DateTime.Parse(cloneItemsTx[i1].SubItems[7].Text);

                                if (ClassUtils.DateUnixTimeNowSecondConvertDate(
                                        transactionWalletDateRecv) >
                                    ClassUtils.DateUnixTimeNowSecond())
                                {
                                    if (cloneItemsTx[i1].BackColor !=
                                        Color.FromArgb(255, 153, 102))
                                    {

                                        listViewAnonymityReceivedTransactionHistory.Items[i1]
                                            .BackColor = Color.FromArgb(255, 153, 102);


                                    }
                                }
                                else
                                {
                                    if (cloneItemsTx[i1].BackColor !=
                                        Color.FromArgb(0, 255, 153))
                                    {

                                        listViewAnonymityReceivedTransactionHistory.Items[i1]
                                            .BackColor = Color.FromArgb(0, 255, 153);

                                    }

                                }
                            }
                        }
                        catch
                        {
                        }

                    }

                    Array.Clear(cloneItemsTx, 0, cloneItemsTx.Length);
                }

                #endregion

            }

        }



        #region Ordering events.

        private ColumnHeader SortingColumn;

        private void listViewNormalSendTransactionHistory_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var new_sorting_column = listViewNormalSendTransactionHistory.Columns[e.Column];

            SortOrder sort_order;
            if (SortingColumn == null)
            {
                sort_order = SortOrder.Ascending;
            }
            else
            {
                if (new_sorting_column == SortingColumn)
                    sort_order = SortingColumn.Text.StartsWith("> ") ? SortOrder.Descending : SortOrder.Ascending;
                else
                    sort_order = SortOrder.Ascending;

                SortingColumn.Text = SortingColumn.Text.Substring(2);
            }

            SortingColumn = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
                SortingColumn.Text = "> " + SortingColumn.Text;
            else
                SortingColumn.Text = "< " + SortingColumn.Text;

            listViewNormalSendTransactionHistory.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            listViewNormalSendTransactionHistory.Sort();
        }

        private ColumnHeader SortingColumn2;

        private void listViewNormalReceivedTransactionHistory_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var new_sorting_column = listViewNormalReceivedTransactionHistory.Columns[e.Column];

            SortOrder sort_order;
            if (SortingColumn2 == null)
            {
                sort_order = SortOrder.Ascending;
            }
            else
            {
                if (new_sorting_column == SortingColumn2)
                    sort_order = SortingColumn2.Text.StartsWith("> ") ? SortOrder.Descending : SortOrder.Ascending;
                else
                    sort_order = SortOrder.Ascending;

                SortingColumn2.Text = SortingColumn2.Text.Substring(2);
            }

            SortingColumn2 = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
                SortingColumn2.Text = "> " + SortingColumn2.Text;
            else
                SortingColumn2.Text = "< " + SortingColumn2.Text;

            listViewNormalReceivedTransactionHistory.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            listViewNormalReceivedTransactionHistory.Sort();
        }

        private ColumnHeader SortingColumn3;

        private void listViewAnonymitySendTransactionHistory_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var new_sorting_column = listViewAnonymitySendTransactionHistory.Columns[e.Column];

            SortOrder sort_order;
            if (SortingColumn3 == null)
            {
                sort_order = SortOrder.Ascending;
            }
            else
            {
                if (new_sorting_column == SortingColumn3)
                    sort_order = SortingColumn3.Text.StartsWith("> ") ? SortOrder.Descending : SortOrder.Ascending;
                else
                    sort_order = SortOrder.Ascending;

                SortingColumn3.Text = SortingColumn3.Text.Substring(2);
            }

            SortingColumn3 = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
                SortingColumn3.Text = "> " + SortingColumn3.Text;
            else
                SortingColumn3.Text = "< " + SortingColumn3.Text;

            listViewAnonymitySendTransactionHistory.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            listViewAnonymitySendTransactionHistory.Sort();
        }

        private ColumnHeader SortingColumn4;

        private void listViewAnonymityReceivedTransactionHistory_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var new_sorting_column = listViewAnonymityReceivedTransactionHistory.Columns[e.Column];

            SortOrder sort_order;
            if (SortingColumn4 == null)
            {
                sort_order = SortOrder.Ascending;
            }
            else
            {
                if (new_sorting_column == SortingColumn4)
                    sort_order = SortingColumn4.Text.StartsWith("> ") ? SortOrder.Descending : SortOrder.Ascending;
                else
                    sort_order = SortOrder.Ascending;

                SortingColumn4.Text = SortingColumn4.Text.Substring(2);
            }

            SortingColumn4 = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
                SortingColumn4.Text = "> " + SortingColumn4.Text;
            else
                SortingColumn4.Text = "< " + SortingColumn4.Text;

            listViewAnonymityReceivedTransactionHistory.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            listViewAnonymityReceivedTransactionHistory.Sort();
        }

        private ColumnHeader SortingColumn5;

        private void listViewBlockRewardTransactionHistory_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var new_sorting_column = listViewBlockRewardTransactionHistory.Columns[e.Column];

            SortOrder sort_order;
            if (SortingColumn5 == null)
            {
                sort_order = SortOrder.Ascending;
            }
            else
            {
                if (new_sorting_column == SortingColumn5)
                    sort_order = SortingColumn5.Text.StartsWith("> ") ? SortOrder.Descending : SortOrder.Ascending;
                else
                    sort_order = SortOrder.Ascending;

                SortingColumn5.Text = SortingColumn5.Text.Substring(2);
            }

            SortingColumn5 = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
                SortingColumn5.Text = "> " + SortingColumn5.Text;
            else
                SortingColumn5.Text = "< " + SortingColumn5.Text;

            listViewBlockRewardTransactionHistory.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            listViewBlockRewardTransactionHistory.Sort();
        }

        #endregion

        private void timerUpdateTxColor_Tick(object sender, EventArgs e)
        {
            try
            {
                UpdateTxShowed();
            }
            catch
            {

            }
        }

        private void timerInsertTx_Tick(object sender, EventArgs e)
        {
            try
            {
                StartUpdateTransactionHistory(Program.WalletXiropht);
            }
            catch
            {

            }
        }

        /// <summary>
        ///     Start update transaction history.
        /// </summary>
        public void StartUpdateTransactionHistory(WalletXiropht walletXiropht)
        {
            if (!walletXiropht.EnableTokenNetworkMode)
                if (walletXiropht.ClassWalletObject.SeedNodeConnectorWallet == null)
                    return;



            if (walletXiropht.ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus() ||
                walletXiropht.EnableTokenNetworkMode && !walletXiropht.ClassWalletObject.WalletClosed)
            {
                if (!ClassWalletTransactionCache.OnLoad && !ClassWalletTransactionAnonymityCache.OnLoad)
                {
                    var errorSyncList = false;
                    try
                    {
                        if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.TransactionHistory)
                        {
                            if (ClassWalletTransactionCache.ListTransaction.Count +
                                ClassWalletTransactionAnonymityCache.ListTransaction.Count <
                                walletXiropht.ClassWalletObject.TotalTransactionInSync +
                                walletXiropht.ClassWalletObject.TotalTransactionInSyncAnonymity)
                            {
                                walletXiropht.UpdateLabelSyncInformation(
                                    "Total transactions downloaded: " +
                                    (ClassWalletTransactionCache.ListTransaction.Count +
                                     ClassWalletTransactionAnonymityCache.ListTransaction.Count) + "/" +
                                    (walletXiropht.ClassWalletObject.TotalTransactionInSync +
                                     walletXiropht.ClassWalletObject.TotalTransactionInSyncAnonymity));
                                if (this.IsShowed)
                                    this.ShowWaitingSyncTransactionPanel();
                            }
                            else
                            {
                                walletXiropht.UpdateLabelSyncInformation(
                                    "Total transactions loaded successfully: " +
                                    (ClassWalletTransactionCache.ListTransaction.Count +
                                     ClassWalletTransactionAnonymityCache.ListTransaction.Count) + "/" +
                                    (walletXiropht.ClassWalletObject.TotalTransactionInSync +
                                     walletXiropht.ClassWalletObject.TotalTransactionInSyncAnonymity));
                            }
                        }
                        else if (ClassFormPhase.FormPhase != ClassFormPhaseEnumeration.TransactionHistory &&
                                 ClassFormPhase.FormPhase != ClassFormPhaseEnumeration.BlockExplorer)
                        {
                            try
                            {
                                bool noNode = true;
                                if (walletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode != null)
                                {
                                    if (walletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode.Count > 0)
                                    {
                                        noNode = false;
                                        string node = walletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode[0]
                                            .RemoteNodeHost;
                                        if (!string.IsNullOrEmpty(node))
                                        {
                                            if (walletXiropht.ClassWalletObject.InSyncTransaction ||
                                                walletXiropht.ClassWalletObject.InSyncTransactionAnonymity)
                                            {
                                                try
                                                {
                                                    walletXiropht.UpdateLabelSyncInformation(
                                                        "Your wallet currently sync transactions with node: " +
                                                        walletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode[0]
                                                            .RemoteNodeHost +
                                                        " " +
                                                        (ClassWalletTransactionCache.ListTransaction.Count +
                                                         ClassWalletTransactionAnonymityCache
                                                             .ListTransaction
                                                             .Count) +
                                                        "/" + (walletXiropht.ClassWalletObject.TotalTransactionInSync +
                                                               walletXiropht.ClassWalletObject
                                                                   .TotalTransactionInSyncAnonymity
                                                        ) +
                                                        ".");
                                                }
                                                catch (Exception error)
                                                {
#if DEBUG
                                                            Log.WriteLine("Error on seed node list: " + error.Message);
#endif
                                                }
                                            }
                                            else if (!walletXiropht.ClassWalletObject.InSyncBlock)
                                            {
                                                if (ClassConnectorSetting.SeedNodeIp.ContainsKey(node))
                                                {
                                                    walletXiropht.UpdateLabelSyncInformation(
                                                        "Your wallet currently sync with the Seed Node IP: " +
                                                        node +
                                                        " Country: " +
                                                        ClassConnectorSetting.SeedNodeIp[node].Item1);
                                                }
                                                else
                                                {
                                                    walletXiropht.UpdateLabelSyncInformation(
                                                        "Your wallet currently sync with the Node IP: " +
                                                        node);
                                                }
                                            }
                                        }
                                    }
                                }

                             
                            }
                            catch
                            {

                            }
                        }
                    }
                    catch (Exception error)
                    {
#if DEBUG
                                            Log.WriteLine("Error on listing remote node connection: " + error.Message);
#endif
                        walletXiropht.StopUpdateTransactionHistory(true, true);
                        errorSyncList = true;
                    }

                    if (!errorSyncList)
                        try
                        {
                            walletXiropht.UpdateCurrentPageNumberTransactionHistory();

                            if (!walletXiropht.ClassWalletObject.InSyncTransaction &&
                                !walletXiropht.ClassWalletObject.InSyncTransactionAnonymity)
                            {
                                if (walletXiropht.ClassWalletObject.TotalTransactionInSyncAnonymity ==
                                    ClassWalletTransactionAnonymityCache.ListTransaction.Count)
                                    if (walletXiropht.ClassWalletObject.TotalTransactionInSync ==
                                        ClassWalletTransactionCache.ListTransaction.Count)
                                        if (this.IsShowed)
                                        {
                                            if (
                                                ClassWalletTransactionCache.ListTransaction.Count ==
                                                walletXiropht.ClassWalletObject.TotalTransactionInSync &&
                                                ClassWalletTransactionAnonymityCache.ListTransaction
                                                    .Count == walletXiropht.ClassWalletObject
                                                    .TotalTransactionInSyncAnonymity)
                                            {
                                                if (ClassWalletTransactionAnonymityCache.ListTransaction
                                                        .Count == walletXiropht.ClassWalletObject
                                                        .TotalTransactionInSyncAnonymity)
                                                    if (ClassWalletTransactionCache.ListTransaction.Count ==
                                                        walletXiropht.ClassWalletObject.TotalTransactionInSync)
                                                        this
                                                            .HideWaitingSyncTransactionPanel();


                                                #region show transaction received/sent by the normal unique wallet id of the wallet

                                                if (ClassWalletTransactionCache.ListTransaction.Count > 0)
                                                    if (walletXiropht.TotalTransactionRead !=
                                                        ClassWalletTransactionCache
                                                            .ListTransaction.Count)
                                                    {
                                                        for (var i =
                                                                ClassWalletTransactionCache.ListTransaction
                                                                    .Count - 1;
                                                            i >= walletXiropht.TotalTransactionRead;
                                                            i--)
                                                        {
                                                            var transactionObject =
                                                                ClassWalletTransactionCache.ListTransaction
                                                                    .ElementAt(i);

                                                            if (transactionObject.Value != null)
                                                            {
                                                                var dateTimeSend = new DateTime(1970, 1, 1,
                                                                    0,
                                                                    0, 0, 0);
                                                                dateTimeSend =
                                                                    dateTimeSend.AddSeconds(
                                                                        transactionObject
                                                                            .Value
                                                                            .TransactionTimestampSend);
                                                                dateTimeSend = dateTimeSend.ToLocalTime();


                                                                var dateTimeRecv = new DateTime(1970, 1, 1,
                                                                    0,
                                                                    0, 0, 0);
                                                                dateTimeRecv =
                                                                    dateTimeRecv.AddSeconds(
                                                                        transactionObject
                                                                            .Value
                                                                            .TransactionTimestampRecv);
                                                                dateTimeRecv = dateTimeRecv.ToLocalTime();


                                                                #region show anonymous transaction received

                                                                if (transactionObject.Value
                                                                        .TransactionWalletAddress ==
                                                                    ClassWalletTransactionType
                                                                        .AnonymousTransaction)
                                                                {
                                                                    var minShow =
                                                                        (walletXiropht.CurrentTransactionHistoryPageAnonymousReceived -
                                                                         1) * walletXiropht.MaxTransactionPerPage;
                                                                    var maxShow =
                                                                        walletXiropht
                                                                            .CurrentTransactionHistoryPageAnonymousReceived *
                                                                        walletXiropht.MaxTransactionPerPage;

                                                                    if (!walletXiropht.NormalTransactionLoaded)
                                                                        if (walletXiropht
                                                                                .TotalTransactionAnonymousReceived >=
                                                                            minShow && walletXiropht
                                                                                .TotalTransactionAnonymousReceived <
                                                                            maxShow &&
                                                                            !walletXiropht.NormalTransactionLoaded)
                                                                        {

                                                                            string[] row =
                                                                            {
                                                                                (walletXiropht
                                                                                     .TotalTransactionAnonymousReceived +
                                                                                 1).ToString(),
                                                                                dateTimeSend.ToString(),
                                                                                transactionObject.Value
                                                                                    .TransactionType,
                                                                                transactionObject.Value
                                                                                    .TransactionHash,
                                                                                "" + transactionObject
                                                                                    .Value
                                                                                    .TransactionAmount,
                                                                                "" + transactionObject
                                                                                    .Value
                                                                                    .TransactionFee,
                                                                                transactionObject.Value
                                                                                    .TransactionWalletAddress,
                                                                                dateTimeRecv.ToString(),
                                                                                transactionObject.Value
                                                                                    .TransactionBlockchainHeight
                                                                            };
                                                                            var listViewItem =
                                                                                new ListViewItem(row);
                                                                            listViewAnonymityReceivedTransactionHistory
                                                                                    .Items
                                                                                    .Add(listViewItem);

                                                                            if (ClassUtils
                                                                                    .DateUnixTimeNowSecondConvertDate(
                                                                                        dateTimeRecv) >
                                                                                ClassUtils
                                                                                    .DateUnixTimeNowSecond()
                                                                            )
                                                                            {
                                                                                listViewAnonymityReceivedTransactionHistory
                                                                                            .Items[listViewAnonymityReceivedTransactionHistory
                                                                                                    .Items
                                                                                                    .Count -
                                                                                                1]
                                                                                            .BackColor
                                                                                        =
                                                                                        Color.FromArgb(
                                                                                            255,
                                                                                            153, 102);
                                                                            }
                                                                            else
                                                                            {
                                                                                listViewAnonymityReceivedTransactionHistory
                                                                                            .Items[listViewAnonymityReceivedTransactionHistory
                                                                                                    .Items
                                                                                                    .Count -
                                                                                                1]
                                                                                            .BackColor
                                                                                        =
                                                                                        Color.FromArgb(
                                                                                            0,
                                                                                            255, 153);
                                                                            }

                                                                        }

                                                                    if (walletXiropht.NormalTransactionLoaded &&
                                                                        walletXiropht
                                                                            .CurrentTransactionHistoryPageAnonymousReceived ==
                                                                        1)
                                                                    {

                                                                        string[] row =
                                                                        {
                                                                            (walletXiropht
                                                                                 .TotalTransactionAnonymousReceived +
                                                                             1).ToString(),
                                                                            dateTimeSend.ToString(),
                                                                            transactionObject.Value
                                                                                .TransactionType,
                                                                            transactionObject.Value
                                                                                .TransactionHash,
                                                                            "" + transactionObject.Value
                                                                                .TransactionAmount,
                                                                            "" + transactionObject.Value
                                                                                .TransactionFee,
                                                                            transactionObject.Value
                                                                                .TransactionWalletAddress,
                                                                            dateTimeRecv.ToString(),
                                                                            transactionObject.Value
                                                                                .TransactionBlockchainHeight
                                                                        };
                                                                        var listViewItem =
                                                                            new ListViewItem(row);
                                                                        listViewAnonymityReceivedTransactionHistory
                                                                                .Items
                                                                                .Insert(0,
                                                                                    listViewItem);

                                                                    }

                                                                    if (walletXiropht
                                                                            .TotalTransactionAnonymousReceived ==
                                                                        minShow)
                                                                    {
                                                                        AutoResizeColumns(listViewAnonymityReceivedTransactionHistory);

                                                                    }

                                                                    walletXiropht.TotalTransactionAnonymousReceived++;
                                                                }

                                                                #endregion

                                                                #region Show block reward transaction.

                                                                else if (transactionObject.Value
                                                                    .TransactionWalletAddress
                                                                    .Contains(ClassWalletTransactionType
                                                                        .BlockchainTransaction))
                                                                {
                                                                    var minShow =
                                                                        (walletXiropht
                                                                             .CurrentTransactionHistoryPageBlockReward -
                                                                         1) * walletXiropht.MaxTransactionPerPage;
                                                                    var maxShow =
                                                                        walletXiropht
                                                                            .CurrentTransactionHistoryPageBlockReward *
                                                                        walletXiropht.MaxTransactionPerPage;

                                                                    if (walletXiropht.TotalTransactionBlockReward >=
                                                                        minShow && walletXiropht
                                                                            .TotalTransactionBlockReward <
                                                                        maxShow &&
                                                                        !walletXiropht.NormalTransactionLoaded)
                                                                    {

                                                                        string[] row =
                                                                        {
                                                                            (walletXiropht.TotalTransactionBlockReward +
                                                                             1
                                                                            ).ToString(),
                                                                            dateTimeSend.ToString(),
                                                                            transactionObject.Value
                                                                                .TransactionType,
                                                                            transactionObject.Value
                                                                                .TransactionHash,
                                                                            "" + transactionObject.Value
                                                                                .TransactionAmount,
                                                                            "" + transactionObject.Value
                                                                                .TransactionFee,
                                                                            transactionObject.Value
                                                                                .TransactionWalletAddress,
                                                                            dateTimeRecv.ToString(),
                                                                            transactionObject.Value
                                                                                .TransactionBlockchainHeight
                                                                        };
                                                                        var listViewItem =
                                                                            new ListViewItem(row);

                                                                       
                                                                                listViewBlockRewardTransactionHistory
                                                                                .Items
                                                                                .Add(listViewItem);
                                          

                                                                        if (ClassUtils
                                                                                .DateUnixTimeNowSecondConvertDate(
                                                                                    dateTimeRecv) >
                                                                            ClassUtils
                                                                                .DateUnixTimeNowSecond()
                                                                        )
                                                                        {
                                                                            listViewBlockRewardTransactionHistory
                                                                                        .Items[listViewBlockRewardTransactionHistory
                                                                                                .Items
                                                                                                .Count -
                                                                                            1]
                                                                                        .BackColor =
                                                                                    Color.FromArgb(255,
                                                                                        153,
                                                                                        102);
                                                                        }
                                                                        else
                                                                        {
                                                                           listViewBlockRewardTransactionHistory
                                                                                        .Items[listViewBlockRewardTransactionHistory
                                                                                                .Items
                                                                                                .Count -
                                                                                            1]
                                                                                        .BackColor =
                                                                                    Color.FromArgb(0,
                                                                                        255,
                                                                                        153);
                                                                        }

                                                                    }

                                                                    if (walletXiropht.NormalTransactionLoaded &&
                                                                        walletXiropht
                                                                            .CurrentTransactionHistoryPageBlockReward ==
                                                                        1)
                                                                    {

                                                                        string[] row =
                                                                        {
                                                                            (walletXiropht.TotalTransactionBlockReward +
                                                                             1
                                                                            ).ToString(),
                                                                            dateTimeSend.ToString(),
                                                                            transactionObject.Value
                                                                                .TransactionType,
                                                                            transactionObject.Value
                                                                                .TransactionHash,
                                                                            "" + transactionObject.Value
                                                                                .TransactionAmount,
                                                                            "" + transactionObject.Value
                                                                                .TransactionFee,
                                                                            transactionObject.Value
                                                                                .TransactionWalletAddress,
                                                                            dateTimeRecv.ToString(),
                                                                            transactionObject.Value
                                                                                .TransactionBlockchainHeight
                                                                        };
                                                                        var listViewItem =
                                                                            new ListViewItem(row);

                                                                        listViewBlockRewardTransactionHistory
                                                                                .Items
                                                                                .Insert(0,
                                                                                    listViewItem);

                                                                    }

                                                                    if (walletXiropht.TotalTransactionBlockReward ==
                                                                        minShow)
                                                                    {
                                                                        AutoResizeColumns(listViewBlockRewardTransactionHistory);
                                                                    }

                                                                    walletXiropht.TotalTransactionBlockReward++;
                                                                }

                                                                #endregion

                                                                else
                                                                {
                                                                    #region show normal transaction sent

                                                                    if (transactionObject.Value
                                                                            .TransactionType ==
                                                                        ClassWalletTransactionType
                                                                            .SendTransaction &&
                                                                        transactionObject.Value
                                                                            .TransactionWalletAddress !=
                                                                        ClassWalletTransactionType
                                                                            .AnonymousTransaction)
                                                                    {
                                                                        var minShow =
                                                                            (walletXiropht
                                                                                 .CurrentTransactionHistoryPageNormalSend -
                                                                             1) * walletXiropht.MaxTransactionPerPage;
                                                                        var maxShow =
                                                                            walletXiropht
                                                                                .CurrentTransactionHistoryPageNormalSend *
                                                                            walletXiropht.MaxTransactionPerPage;

                                                                        if (walletXiropht.TotalTransactionNormalSend >=
                                                                            minShow && walletXiropht
                                                                                .TotalTransactionNormalSend <
                                                                            maxShow &&
                                                                            !walletXiropht.NormalTransactionLoaded)
                                                                        {
                                                                            var walletAddress =
                                                                                transactionObject.Value
                                                                                    .TransactionWalletAddress;
                                                                            if (ClassContact
                                                                                .CheckContactNameFromWalletAddress(
                                                                                    transactionObject
                                                                                        .Value
                                                                                        .TransactionWalletAddress)
                                                                            )
                                                                                walletAddress =
                                                                                    ClassContact
                                                                                        .GetContactNameFromWalletAddress(
                                                                                            transactionObject
                                                                                                .Value
                                                                                                .TransactionWalletAddress);
                                                                            string[] row =
                                                                            {
                                                                                (walletXiropht
                                                                                     .TotalTransactionNormalSend +
                                                                                 1).ToString(),
                                                                                dateTimeSend.ToString(),
                                                                                transactionObject.Value
                                                                                    .TransactionType,
                                                                                transactionObject.Value
                                                                                    .TransactionHash,
                                                                                "" + transactionObject
                                                                                    .Value
                                                                                    .TransactionAmount,
                                                                                "" + transactionObject
                                                                                    .Value
                                                                                    .TransactionFee,
                                                                                walletAddress,
                                                                                dateTimeRecv.ToString(),
                                                                                transactionObject.Value
                                                                                    .TransactionBlockchainHeight
                                                                            };
                                                                            var listViewItem =
                                                                                new ListViewItem(row);


                                                                           listViewNormalSendTransactionHistory
                                                                                    .Items
                                                                                    .Add(listViewItem);


                                                                            if (ClassUtils
                                                                                    .DateUnixTimeNowSecondConvertDate(
                                                                                        dateTimeRecv) >
                                                                                ClassUtils
                                                                                    .DateUnixTimeNowSecond()
                                                                            )
                                                                            {
                                                                                listViewNormalSendTransactionHistory
                                                                                            .Items[listViewNormalSendTransactionHistory
                                                                                                    .Items
                                                                                                    .Count -
                                                                                                1]
                                                                                            .BackColor
                                                                                        =
                                                                                        Color.FromArgb(
                                                                                            255,
                                                                                            153, 102);
                                                                            }
                                                                            else
                                                                            {
                                                                               listViewNormalSendTransactionHistory
                                                                                            .Items[listViewNormalSendTransactionHistory
                                                                                                    .Items
                                                                                                    .Count -
                                                                                                1]
                                                                                            .BackColor
                                                                                        =
                                                                                        Color.FromArgb(
                                                                                            0,
                                                                                            255, 153);
       
                                                                            }
                                                                        }


                                                                        if (walletXiropht.NormalTransactionLoaded &&
                                                                            walletXiropht
                                                                                .CurrentTransactionHistoryPageNormalSend ==
                                                                            1)
                                                                        {

                                                                            var walletAddress =
                                                                                transactionObject.Value
                                                                                    .TransactionWalletAddress;
                                                                            if (ClassContact
                                                                                .CheckContactNameFromWalletAddress(
                                                                                    transactionObject
                                                                                        .Value
                                                                                        .TransactionWalletAddress)
                                                                            )
                                                                                walletAddress =
                                                                                    ClassContact
                                                                                        .GetContactNameFromWalletAddress(
                                                                                            transactionObject
                                                                                                .Value
                                                                                                .TransactionWalletAddress);
                                                                            string[] row =
                                                                            {
                                                                                (walletXiropht
                                                                                     .TotalTransactionNormalSend +
                                                                                 1).ToString(),
                                                                                dateTimeSend.ToString(),
                                                                                transactionObject.Value
                                                                                    .TransactionType,
                                                                                transactionObject.Value
                                                                                    .TransactionHash,
                                                                                "" + transactionObject
                                                                                    .Value
                                                                                    .TransactionAmount,
                                                                                "" + transactionObject
                                                                                    .Value
                                                                                    .TransactionFee,
                                                                                walletAddress,
                                                                                dateTimeRecv.ToString(),
                                                                                transactionObject.Value
                                                                                    .TransactionBlockchainHeight
                                                                            };
                                                                            var listViewItem =
                                                                                new ListViewItem(row);


                                                                            listViewNormalSendTransactionHistory
                                                                                    .Items
                                                                                    .Insert(0,
                                                                                        listViewItem);


                                                                        }


                                                                        if (walletXiropht.TotalTransactionNormalSend ==
                                                                            minShow)
                                                                        {
                                                                            AutoResizeColumns(listViewNormalSendTransactionHistory);

                                                                        }


                                                                        walletXiropht.TotalTransactionNormalSend++;
                                                                    }

                                                                    #endregion

                                                                    #region Show normal transaction received.

                                                                    else if (transactionObject.Value
                                                                                 .TransactionType ==
                                                                             ClassWalletTransactionType
                                                                                 .ReceiveTransaction &&
                                                                             transactionObject.Value
                                                                                 .TransactionWalletAddress !=
                                                                             ClassWalletTransactionType
                                                                                 .AnonymousTransaction)
                                                                    {
                                                                        var minShow =
                                                                            (walletXiropht
                                                                                 .CurrentTransactionHistoryPageNormalReceive -
                                                                             1) * walletXiropht.MaxTransactionPerPage;
                                                                        var maxShow =
                                                                            walletXiropht
                                                                                .CurrentTransactionHistoryPageNormalReceive *
                                                                            walletXiropht.MaxTransactionPerPage;

                                                                        if (walletXiropht
                                                                                .TotalTransactionNormalReceived >=
                                                                            minShow && walletXiropht
                                                                                .TotalTransactionNormalReceived <
                                                                            maxShow &&
                                                                            !walletXiropht.NormalTransactionLoaded)
                                                                        {
                                                                            var walletAddress =
                                                                                transactionObject.Value
                                                                                    .TransactionWalletAddress;
                                                                            if (ClassContact
                                                                                .CheckContactNameFromWalletAddress(
                                                                                    transactionObject
                                                                                        .Value
                                                                                        .TransactionWalletAddress)
                                                                            )
                                                                                walletAddress =
                                                                                    ClassContact
                                                                                        .GetContactNameFromWalletAddress(
                                                                                            transactionObject
                                                                                                .Value
                                                                                                .TransactionWalletAddress);
                                                                            string[] row =
                                                                            {
                                                                                (walletXiropht
                                                                                     .TotalTransactionNormalReceived +
                                                                                 1).ToString(),
                                                                                dateTimeSend.ToString(),
                                                                                transactionObject.Value
                                                                                    .TransactionType,
                                                                                transactionObject.Value
                                                                                    .TransactionHash,
                                                                                "" + transactionObject
                                                                                    .Value
                                                                                    .TransactionAmount,
                                                                                "" + transactionObject
                                                                                    .Value
                                                                                    .TransactionFee,
                                                                                walletAddress,
                                                                                dateTimeRecv.ToString(),
                                                                                transactionObject.Value
                                                                                    .TransactionBlockchainHeight
                                                                            };
                                                                            var listViewItem =
                                                                                new ListViewItem(row);


                                                                            listViewNormalReceivedTransactionHistory
                                                                                    .Items
                                                                                    .Add(listViewItem);


                                                                            if (ClassUtils
                                                                                    .DateUnixTimeNowSecondConvertDate(
                                                                                        dateTimeRecv) >
                                                                                ClassUtils
                                                                                    .DateUnixTimeNowSecond()
                                                                            )
                                                                            {
                                                                                listViewNormalReceivedTransactionHistory
                                                                                            .Items[listViewNormalReceivedTransactionHistory
                                                                                                    .Items
                                                                                                    .Count -
                                                                                                1]
                                                                                            .BackColor
                                                                                        =
                                                                                        Color.FromArgb(
                                                                                            255,
                                                                                            153, 102);

                                                                            }
                                                                            else
                                                                            {
                                                                                listViewNormalReceivedTransactionHistory
                                                                                            .Items[listViewNormalReceivedTransactionHistory
                                                                                                    .Items
                                                                                                    .Count -
                                                                                                1]
                                                                                            .BackColor
                                                                                        =
                                                                                        Color.FromArgb(
                                                                                            0,
                                                                                            255, 153);
                                                                            }

                                                                        }

                                                                        if (walletXiropht.NormalTransactionLoaded &&
                                                                            walletXiropht
                                                                                .CurrentTransactionHistoryPageNormalReceive ==
                                                                            1)
                                                                        {
                                                                            var walletAddress =
                                                                                transactionObject.Value
                                                                                    .TransactionWalletAddress;
                                                                            if (ClassContact
                                                                                .CheckContactNameFromWalletAddress(
                                                                                    transactionObject
                                                                                        .Value
                                                                                        .TransactionWalletAddress)
                                                                            )
                                                                                walletAddress =
                                                                                    ClassContact
                                                                                        .GetContactNameFromWalletAddress(
                                                                                            transactionObject
                                                                                                .Value
                                                                                                .TransactionWalletAddress);
                                                                            string[] row =
                                                                            {
                                                                                (walletXiropht
                                                                                     .TotalTransactionNormalReceived +
                                                                                 1).ToString(),
                                                                                dateTimeSend.ToString(),
                                                                                transactionObject.Value
                                                                                    .TransactionType,
                                                                                transactionObject.Value
                                                                                    .TransactionHash,
                                                                                "" + transactionObject
                                                                                    .Value
                                                                                    .TransactionAmount,
                                                                                "" + transactionObject
                                                                                    .Value
                                                                                    .TransactionFee,
                                                                                walletAddress,
                                                                                dateTimeRecv.ToString(),
                                                                                transactionObject.Value
                                                                                    .TransactionBlockchainHeight
                                                                            };
                                                                            var listViewItem =
                                                                                new ListViewItem(row);

                                                                            listViewNormalReceivedTransactionHistory
                                                                                    .Items
                                                                                    .Insert(0,
                                                                                        listViewItem);


                                                                        }

                                                                        if (walletXiropht
                                                                                .TotalTransactionNormalReceived ==
                                                                            minShow)
                                                                        {
                                                                            AutoResizeColumns(listViewNormalReceivedTransactionHistory);
                                                                        }

                                                                        walletXiropht.TotalTransactionNormalReceived++;
                                                                    }

                                                                    #endregion
                                                                }
                                                            }
                                                        }

                                                        walletXiropht.TotalTransactionRead = ClassWalletTransactionCache
                                                            .ListTransaction.Count;
                                                        walletXiropht.NormalTransactionLoaded = true;
                                                    }
                                            }
                                            else
                                            {
                                                if (this.IsShowed)
                                                    this
                                                        .ShowWaitingSyncTransactionPanel();
                                            }

                                            #endregion


                                            if (
                                                ClassWalletTransactionAnonymityCache.ListTransaction
                                                    .Count == walletXiropht.ClassWalletObject
                                                    .TotalTransactionInSyncAnonymity &&
                                                ClassWalletTransactionCache.ListTransaction.Count ==
                                                walletXiropht.ClassWalletObject.TotalTransactionInSync)
                                            {
                                                if (ClassWalletTransactionAnonymityCache.ListTransaction
                                                        .Count == walletXiropht.ClassWalletObject
                                                        .TotalTransactionInSyncAnonymity)
                                                    if (ClassWalletTransactionCache.ListTransaction.Count ==
                                                        walletXiropht.ClassWalletObject.TotalTransactionInSync)
                                                        this
                                                            .HideWaitingSyncTransactionPanel();


                                                if (ClassWalletTransactionAnonymityCache.ListTransaction
                                                        .Count >
                                                    0)
                                                {
                                                    #region Show transaction anonymity sent with the unique wallet anonymity id of the wallet

                                                    if (walletXiropht.TotalAnonymityTransactionRead !=
                                                        ClassWalletTransactionAnonymityCache.ListTransaction
                                                            .Count)
                                                    {
                                                        for (var i =
                                                                ClassWalletTransactionAnonymityCache
                                                                    .ListTransaction.Count - 1;
                                                            i >= walletXiropht.TotalAnonymityTransactionRead;
                                                            i--)
                                                        {
                                                            var transactionObject =
                                                                ClassWalletTransactionAnonymityCache
                                                                    .ListTransaction.ElementAt(i);

                                                            if (transactionObject.Value != null)
                                                            {
                                                                var dateTimeSend = new DateTime(1970, 1, 1,
                                                                    0,
                                                                    0, 0, 0);
                                                                dateTimeSend =
                                                                    dateTimeSend.AddSeconds(
                                                                        transactionObject
                                                                            .Value
                                                                            .TransactionTimestampSend);
                                                                dateTimeSend = dateTimeSend.ToLocalTime();

                                                                DateTime dateTimeRecv;

                                                                dateTimeRecv = new DateTime(1970, 1, 1, 0,
                                                                    0, 0,
                                                                    0);
                                                                dateTimeRecv =
                                                                    dateTimeRecv.AddSeconds(
                                                                        transactionObject
                                                                            .Value
                                                                            .TransactionTimestampRecv);
                                                                dateTimeRecv = dateTimeRecv.ToLocalTime();

                                                                var minShow =
                                                                    (walletXiropht
                                                                         .CurrentTransactionHistoryPageAnonymousSend -
                                                                     1) * walletXiropht.MaxTransactionPerPage;
                                                                var maxShow =
                                                                    walletXiropht
                                                                        .CurrentTransactionHistoryPageAnonymousSend *
                                                                    walletXiropht.MaxTransactionPerPage;

                                                                if (walletXiropht.TotalTransactionAnonymousSend >=
                                                                    minShow && walletXiropht
                                                                        .TotalTransactionAnonymousSend <
                                                                    maxShow &&
                                                                    !walletXiropht.AnonymousTransactionLoaded)
                                                                {
                                                                    var walletAddress =
                                                                        transactionObject.Value
                                                                            .TransactionWalletAddress;
                                                                    if (ClassContact
                                                                        .CheckContactNameFromWalletAddress(
                                                                            transactionObject.Value
                                                                                .TransactionWalletAddress)
                                                                    )
                                                                        walletAddress =
                                                                            ClassContact
                                                                                .GetContactNameFromWalletAddress(
                                                                                    transactionObject
                                                                                        .Value
                                                                                        .TransactionWalletAddress);
                                                                    string[] row =
                                                                    {
                                                                        (walletXiropht.TotalTransactionAnonymousSend +
                                                                         1)
                                                                        .ToString(),
                                                                        dateTimeSend.ToString(),
                                                                        transactionObject.Value
                                                                            .TransactionType,
                                                                        transactionObject.Value
                                                                            .TransactionHash,
                                                                        "" + transactionObject.Value
                                                                            .TransactionAmount,
                                                                        "" + transactionObject.Value
                                                                            .TransactionFee,
                                                                        walletAddress,
                                                                        dateTimeRecv.ToString(),
                                                                        transactionObject.Value
                                                                            .TransactionBlockchainHeight
                                                                    };
                                                                    var listViewItem =
                                                                        new ListViewItem(row);


                                                                    listViewAnonymitySendTransactionHistory
                                                                            .Items
                                                                            .Add(listViewItem);

                                                                    if (ClassUtils
                                                                            .DateUnixTimeNowSecondConvertDate(
                                                                                dateTimeRecv) >=
                                                                        ClassUtils
                                                                            .DateUnixTimeNowSecond())
                                                                    {
                                                                        listViewAnonymitySendTransactionHistory
                                                                                    .Items[listViewAnonymitySendTransactionHistory
                                                                                            .Items
                                                                                            .Count - 1]
                                                                                    .BackColor =
                                                                                Color.FromArgb(255, 153,
                                                                                    102);
                                                                    }
                                                                    else
                                                                    {
                                                                        listViewAnonymitySendTransactionHistory
                                                                                    .Items[listViewAnonymitySendTransactionHistory
                                                                                            .Items
                                                                                            .Count - 1]
                                                                                    .BackColor =
                                                                                Color.FromArgb(0, 255,
                                                                                    153);

                                                                    }
                                                                }

                                                                if (walletXiropht.AnonymousTransactionLoaded &&
                                                                    walletXiropht
                                                                        .CurrentTransactionHistoryPageAnonymousSend ==
                                                                    1)
                                                                {

                                                                    var walletAddress =
                                                                        transactionObject.Value
                                                                            .TransactionWalletAddress;
                                                                    if (ClassContact
                                                                        .CheckContactNameFromWalletAddress(
                                                                            transactionObject.Value
                                                                                .TransactionWalletAddress)
                                                                    )
                                                                        walletAddress =
                                                                            ClassContact
                                                                                .GetContactNameFromWalletAddress(
                                                                                    transactionObject
                                                                                        .Value
                                                                                        .TransactionWalletAddress);

                                                                    string[] row =
                                                                    {
                                                                        (walletXiropht.TotalTransactionAnonymousSend +
                                                                         1)
                                                                        .ToString(),
                                                                        dateTimeSend.ToString(),
                                                                        transactionObject.Value
                                                                            .TransactionType,
                                                                        transactionObject.Value
                                                                            .TransactionHash,
                                                                        "" + transactionObject.Value
                                                                            .TransactionAmount,
                                                                        "" + transactionObject.Value
                                                                            .TransactionFee,
                                                                        walletAddress,
                                                                        dateTimeRecv.ToString(),
                                                                        transactionObject.Value
                                                                            .TransactionBlockchainHeight
                                                                    };

                                                                    var listViewItem =
                                                                        new ListViewItem(row);


                                                                    listViewAnonymitySendTransactionHistory
                                                                            .Items
                                                                            .Insert(0, listViewItem);
          

                                                                }

                                                                if (walletXiropht.TotalTransactionAnonymousSend ==
                                                                    minShow
                                                                )
                                                                {
                                                                    AutoResizeColumns(listViewAnonymitySendTransactionHistory);
                                                                    
                                                                }

                                                                walletXiropht.TotalTransactionAnonymousSend++;
                                                            }
                                                        }

                                                        walletXiropht.TotalAnonymityTransactionRead =
                                                            ClassWalletTransactionAnonymityCache
                                                                .ListTransaction
                                                                .Count;
                                                        walletXiropht.AnonymousTransactionLoaded = true;
                                                    }

                                                    #endregion
                                                }
                                            }
                                            else
                                            {
                                                if (this.IsShowed)
                                                    this
                                                        .ShowWaitingSyncTransactionPanel();
                                            }

      

                                            if (this
                                                    .listViewNormalSendTransactionHistory.Items.Count +
                                                this
                                                    .listViewAnonymityReceivedTransactionHistory.Items
                                                    .Count +
                                                this
                                                    .listViewBlockRewardTransactionHistory.Items.Count +
                                                this
                                                    .listViewNormalReceivedTransactionHistory.Items.Count >
                                                ClassWalletTransactionCache.ListTransaction.Count)
                                            {
                                                walletXiropht.AnonymousTransactionLoaded = false;
                                                walletXiropht.NormalTransactionLoaded = false;
                                                walletXiropht.StopUpdateTransactionHistory(false, false);
                                            }

                                            if (this
                                                    .listViewAnonymitySendTransactionHistory.Items.Count >
                                                ClassWalletTransactionAnonymityCache.ListTransaction.Count)
                                            {
                                                walletXiropht.AnonymousTransactionLoaded = false;
                                                walletXiropht.NormalTransactionLoaded = false;
                                                walletXiropht.StopUpdateTransactionHistory(false, false);
                                            }
                                        }
                            }
                            else
                            {
                                if (this.IsShowed)
                                    this.ShowWaitingSyncTransactionPanel();
                                walletXiropht.StopUpdateTransactionHistory(true, true);
                            }
                        }
                        catch (Exception error)
                        {
#if DEBUG
                                                Log.WriteLine("Error on showing transaction history: " + error.Message);
#endif
                            walletXiropht.StopUpdateTransactionHistory(true, true);
                        }

                }
                else
                {
                    if (this.IsShowed)
                        this.ShowWaitingSyncTransactionPanel();
                }

            }

        }
    }

    public class ListViewComparer : IComparer
    {
        private readonly int _columnNumber;
        private readonly SortOrder _sortOrder;

        public ListViewComparer(int columnNumber,
            SortOrder sortOrder)
        {
            _columnNumber = columnNumber;
            _sortOrder = sortOrder;
        }

        public int Compare(object objectX, object objectY)
        {
            var itemX = objectX as ListViewItem;
            var itemY = objectY as ListViewItem;

            string stringX = null;
            if (itemX != null && itemX.SubItems.Count <= _columnNumber)
            {
                stringX = "";
            }
            else
            {
                if (itemX != null) stringX = itemX.SubItems[_columnNumber].Text;
            }

            string stringY = null;
            if (itemY != null && itemY.SubItems.Count <= _columnNumber)
            {
                stringY = "";
            }
            else
            {
                if (itemY != null) stringY = itemY.SubItems[_columnNumber].Text;
            }

            var result = 0;
            if (double.TryParse(stringX, out var double_x) &&
                double.TryParse(stringY, out var double_y))
            {
                result = double_x.CompareTo(double_y);
            }
            else
            {
                if (DateTime.TryParse(stringX, out var date_x) &&
                    DateTime.TryParse(stringY, out var date_y))
                {
                    result = date_x.CompareTo(date_y);
                }
                else
                {
                    if (stringX != null) result = stringX.CompareTo(stringY);
                }
            }

            if (_sortOrder == SortOrder.Ascending)
                return result;
            return -result;
        }
    }


    public sealed class TabControlEx : TabControl
    {
        private Timer _timeUpdateStyle;

        public TabControlEx()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            DoubleBuffered = true;
            _timeUpdateStyle = new Timer { Interval = 100, Enabled = true };
            _timeUpdateStyle.Tick += TimeUpdateStyleEvent;
            _timeUpdateStyle.Start();
        }

        private void TimeUpdateStyleEvent(object sender, EventArgs e)
        {
            UpdateStyles();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

    }


    public sealed class TabPageEx : TabPage
    {
        private Timer _timeUpdateStyle;

        public TabPageEx()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            DoubleBuffered = true;
            _timeUpdateStyle = new Timer { Interval = 100, Enabled = true };
            _timeUpdateStyle.Tick += TimeUpdateStyleEvent;
            _timeUpdateStyle.Start();
        }

        private void TimeUpdateStyleEvent(object sender, EventArgs e)
        {
            UpdateStyles();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

    }

    public sealed class ListViewEx : ListView
    {
        private const int WM_LBUTTONDOWN = 0x0201;
        private Timer _timeUpdateStyle;

        public ListViewEx()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint, true);

            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
            _timeUpdateStyle = new Timer {Interval = 100, Enabled = true};
            _timeUpdateStyle.Tick += TimeUpdateStyleEvent;
            _timeUpdateStyle.Start();
        }

        private void TimeUpdateStyleEvent(object sender, EventArgs e)
        {
            UpdateStyles();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN)
            {
                var x = (short) m.LParam;

                var y = (short) ((int) m.LParam >> 16);


                var e = new MouseEventArgs(MouseButtons.Left, 2, x, y, 0);
#if WINDOWS
                UpdateStyles();
#endif
            }

            base.WndProc(ref m);
        }
    }
}