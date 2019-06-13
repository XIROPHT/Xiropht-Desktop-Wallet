using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xiropht_Wallet.FormCustom;
using Xiropht_Wallet.Wallet;

namespace Xiropht_Wallet.FormPhase.MainForm
{
    public sealed partial class TransactionHistoryWallet : Form
    {
        private ClassPanel _panelWaitingSync;
        public bool IsShowed;
        private Label _labelWaitingText = new Label();
        public bool IsShowedWaitingTransaction;

        public TransactionHistoryWallet()
        {
            InitializeComponent();
            AutoScroll = true;
            IsShowed = false;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);

        }


        public void ResyncTransaction()
        {
            ClassWalletObject.BlockTransactionSync = true;
            ClassFormPhase.WalletXiropht.StopUpdateTransactionHistory(true, true);
            if (ClassWalletTransactionCache.RemoveWalletCache(ClassWalletObject.WalletConnect.WalletAddress))
            {
                if (ClassWalletTransactionAnonymityCache.RemoveWalletCache(ClassWalletObject.WalletConnect.WalletAddress))
                {
                    ClassWalletTransactionCache.ListTransaction.Clear();
                    ClassWalletTransactionAnonymityCache.ListTransaction.Clear();
                    ClassWalletObject.InSyncTransaction = false;
                    ClassWalletObject.InSyncTransactionAnonymity = false;
                    ClassWalletObject.BlockTransactionSync = false;
                    ClassWalletObject.DisconnectWholeRemoteNodeSyncAsync(true, true);
                }
            }
        }

        public void AutoResizeColumns(ListView lv)
        {
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            ListView.ColumnHeaderCollection cc = lv.Columns;
            for (int i = 0; i < cc.Count; i++)
            {
                int colWidth = TextRenderer.MeasureText(cc[i].Text, lv.Font).Width + 30;
                if (colWidth > cc[i].Width || cc[i].Text == ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_FEE") || cc[i].Text == ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_AMOUNT"))
                {
                    cc[i].Width = colWidth + 30;
                }
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
                Width = (int)(Width / 1.5f),
                Height = (int)(Height / 5.5f),
                BackColor = Color.LightBlue
            };
            _panelWaitingSync.Location = new Point()
            {
                X = ClassFormPhase.WalletXiropht.TransactionHistoryWalletForm.Width / 2 - _panelWaitingSync.Width / 2,
                Y = ClassFormPhase.WalletXiropht.TransactionHistoryWalletForm.Height / 2 - _panelWaitingSync.Height / 2
            };

            _labelWaitingText.AutoSize = true;
            _labelWaitingText.Font = new Font(_labelWaitingText.Font.FontFamily, 9f, FontStyle.Bold);
            _labelWaitingText.Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_WAITING_MESSAGE_SYNC_TEXT");
            _panelWaitingSync.Controls.Add(_labelWaitingText);
            _labelWaitingText.Location = new Point()
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
            Refresh();
        }

        public void ShowWaitingSyncTransactionPanel()
        {
            if (!IsShowedWaitingTransaction)
            {
                ClassFormPhase.WalletXiropht.Invoke((MethodInvoker)delegate ()
                {
                    _panelWaitingSync.Visible = true;
                    _panelWaitingSync.Show();
                    _panelWaitingSync.BringToFront();
                    _panelWaitingSync.Width = (int)(Width / 1.5f);
                    _panelWaitingSync.Height = (int)(Height / 5.5f);
                    _panelWaitingSync.Location = new Point()
                    {
                        X = ClassFormPhase.WalletXiropht.TransactionHistoryWalletForm.Width / 2 -
                            _panelWaitingSync.Width / 2,
                        Y = ClassFormPhase.WalletXiropht.TransactionHistoryWalletForm.Height / 2 -
                            _panelWaitingSync.Height / 2
                    };
                    _labelWaitingText.Location = new Point()
                    {
                        X = _panelWaitingSync.Width / 2 - _labelWaitingText.Width / 2,
                        Y = _panelWaitingSync.Height / 2 - _labelWaitingText.Height / 2
                    };
                    IsShowedWaitingTransaction = true;
                    Refresh();

                });
            }
        }

        public void HideWaitingSyncTransactionPanel()
        {
            if (IsShowedWaitingTransaction)
            {
                ClassFormPhase.WalletXiropht.Invoke((MethodInvoker)delegate ()
                {
                    _panelWaitingSync.Visible = false;
                    _panelWaitingSync.Hide();
                    IsShowedWaitingTransaction = false;
                    Refresh();
                });
            }
        }

        protected override void OnResize(EventArgs e)
        {
            if (_panelWaitingSync != null)
            {
                _panelWaitingSync.Width = (int)(Width / 1.5f);
                _panelWaitingSync.Height = (int)(Height / 5.5f);
                _panelWaitingSync.Location = new Point()
                {
                    X = ClassFormPhase.WalletXiropht.TransactionHistoryWalletForm.Width / 2 -
                        _panelWaitingSync.Width / 2,
                    Y = ClassFormPhase.WalletXiropht.TransactionHistoryWalletForm.Height / 2 -
                        _panelWaitingSync.Height / 2
                };
                _labelWaitingText.Location = new Point()
                {
                    X = _panelWaitingSync.Width / 2 - _labelWaitingText.Width / 2,
                    Y = _panelWaitingSync.Height / 2 - _labelWaitingText.Height / 2
                };
                Refresh();
            }

            base.OnResize(e);
        }

        public void GetListControl()
        {
            if (ClassFormPhase.WalletXiropht.ListControlSizeTransaction.Count == 0)
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (i < Controls.Count)
                    {
                        ClassFormPhase.WalletXiropht.ListControlSizeTransaction.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
                    }
                }
            }

            if (ClassFormPhase.WalletXiropht.ListControlSizeTransactionTabPage.Count == 0)
            {
                for (int i = 0; i < tabPageTransactionHistory.Controls.Count; i++)
                {
                    if (i < tabPageTransactionHistory.Controls.Count)
                    {
                        ClassFormPhase.WalletXiropht.ListControlSizeTransactionTabPage.Add(
                            new Tuple<Size, Point>(tabPageTransactionHistory.Controls[i].Size,
                                tabPageTransactionHistory.Controls[i].Location));
                    }
                }
            }

            ClassFormPhase.WalletXiropht.ResizeWalletInterface();
        }


        public bool CheckContainKeyInvokerNormalReceive(string hash)
        {
            bool check = false;
            MethodInvoker invoke = () => check = listViewNormalReceivedTransactionHistory.Items.ContainsKey(hash);
            BeginInvoke(invoke);
            return check;
        }

        public bool CheckContainKeyInvokerAnonymousReceive(string hash)
        {
            bool check = false;
            MethodInvoker invoke = () => check = listViewAnonymityReceivedTransactionHistory.Items.ContainsKey(hash);
            BeginInvoke(invoke);
            return check;
        }


        public bool CheckContainKeyInvokerNormalSend(string hash)
        {
            bool check = false;
            MethodInvoker invoke = () => check = listViewNormalSendTransactionHistory.Items.ContainsKey(hash);
            BeginInvoke(invoke);
            return check;
        }

        public bool CheckContainKeyInvokerAnonymousSend(string hash)
        {
            bool check = false;
            MethodInvoker invoke = () => check = listViewAnonymitySendTransactionHistory.Items.ContainsKey(hash);
            BeginInvoke(invoke);
            return check;
        }

        public bool CheckContainKeyInvokerBlockReward(string hash)
        {
            bool check = false;
            MethodInvoker invoke = () => check = listViewBlockRewardTransactionHistory.Items.ContainsKey(hash);
            BeginInvoke(invoke);
            return check;
        }

        private void listViewNormalSendTransactionHistory_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                ListViewItem item = listViewNormalSendTransactionHistory.GetItemAt(0, e.Y);

                bool found = false;
                if (item == null) return;
                for (int ix = item.SubItems.Count - 1; ix >= 0; --ix)
                    if (item.SubItems[ix].Bounds.Contains(e.Location))
                    {
                        if (!found)
                        {
                            found = true;

                            if (ClassContact.ListContactWallet.ContainsKey(item.SubItems[ix].Text))
                            {
                                Clipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text].Item2);
#if WINDOWS
                                Task.Factory.StartNew(() =>
                                        ClassFormPhase.MessageBoxInterface(ClassContact.ListContactWallet[item.SubItems[ix].Text] + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information)).ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text].Item2);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker = () => MessageBox.Show(ClassFormPhase.WalletXiropht, ClassContact.ListContactWallet[item.SubItems[ix].Text].Item2 + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"));
                                    BeginInvoke(invoker);
                                }).ConfigureAwait(false);
#endif
                            }
                            else
                            {
                                Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                                Task.Factory.StartNew(() =>
                                        ClassFormPhase.MessageBoxInterface(item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information)).ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(item.SubItems[ix].Text);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker = () => MessageBox.Show(ClassFormPhase.WalletXiropht, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"));
                                    BeginInvoke(invoker);
                                }).ConfigureAwait(false);
#endif
                            }


                            return;
                        }
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
                ListViewItem item = listViewNormalReceivedTransactionHistory.GetItemAt(5, e.Y);
                if (item == null) return;
                for (int ix = item.SubItems.Count - 1; ix >= 0; --ix)
                    if (item.SubItems[ix].Bounds.Contains(e.Location))
                    {
                        if (ClassContact.ListContactWallet.ContainsKey(item.SubItems[ix].Text))
                        {
                            Clipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text].Item2);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                    ClassFormPhase.MessageBoxInterface(ClassContact.ListContactWallet[item.SubItems[ix].Text] + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information)).ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text].Item2);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker = () => MessageBox.Show(ClassFormPhase.WalletXiropht, ClassContact.ListContactWallet[item.SubItems[ix].Text].Item2 + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"));
                                    BeginInvoke(invoker);
                                }).ConfigureAwait(false);
#endif
                        }
                        else
                        {
                            Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                    ClassFormPhase.MessageBoxInterface(item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information)).ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(item.SubItems[ix].Text);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker = () => MessageBox.Show(ClassFormPhase.WalletXiropht, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"));
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
                ListViewItem item = listViewAnonymitySendTransactionHistory.GetItemAt(5, e.Y);
                if (item == null) return;
                for (int ix = item.SubItems.Count - 1; ix >= 0; --ix)
                    if (item.SubItems[ix].Bounds.Contains(e.Location))
                    {
                        if (ClassContact.ListContactWallet.ContainsKey(item.SubItems[ix].Text))
                        {
                            Clipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text].Item2);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                    ClassFormPhase.MessageBoxInterface(ClassContact.ListContactWallet[item.SubItems[ix].Text] + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information)).ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text].Item2);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker = () => MessageBox.Show(ClassFormPhase.WalletXiropht, ClassContact.ListContactWallet[item.SubItems[ix].Text].Item2 + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"));
                                    BeginInvoke(invoker);
                                }).ConfigureAwait(false);
#endif
                        }
                        else
                        {
                            Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                    ClassFormPhase.MessageBoxInterface(item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information)).ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(item.SubItems[ix].Text);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker = () => MessageBox.Show(ClassFormPhase.WalletXiropht, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"));
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
                ListViewItem item = listViewAnonymityReceivedTransactionHistory.GetItemAt(5, e.Y);
                if (item == null) return;
                for (int ix = item.SubItems.Count - 1; ix >= 0; --ix)
                    if (item.SubItems[ix].Bounds.Contains(e.Location))
                    {
                        if (ClassContact.ListContactWallet.ContainsKey(item.SubItems[ix].Text))
                        {
                            Clipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text].Item2);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                    ClassFormPhase.MessageBoxInterface(ClassContact.ListContactWallet[item.SubItems[ix].Text].Item2 + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information)).ConfigureAwait(false);
#else
                            LinuxClipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text].Item2);
                            Task.Factory.StartNew(() =>
                            {
                                MethodInvoker invoker = () => MessageBox.Show(ClassFormPhase.WalletXiropht, ClassContact.ListContactWallet[item.SubItems[ix].Text].Item2 + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"));
                                BeginInvoke(invoker);
                            }).ConfigureAwait(false);
#endif
                        }
                        else
                        {
                            Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                    ClassFormPhase.MessageBoxInterface(item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information)).ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(item.SubItems[ix].Text);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker = () => MessageBox.Show(ClassFormPhase.WalletXiropht, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"));
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
                ListViewItem item = listViewBlockRewardTransactionHistory.GetItemAt(5, e.Y);
                if (item == null) return;
                for (int ix = item.SubItems.Count - 1; ix >= 0; --ix)
                    if (item.SubItems[ix].Bounds.Contains(e.Location))
                    {
                        if (ClassContact.ListContactWallet.ContainsKey(item.SubItems[ix].Text))
                        {
                            Clipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text].Item2);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                    ClassFormPhase.MessageBoxInterface(ClassContact.ListContactWallet[item.SubItems[ix].Text].Item2 + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information)).ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(ClassContact.ListContactWallet[item.SubItems[ix].Text].Item2);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker = () => MessageBox.Show(ClassFormPhase.WalletXiropht, ClassContact.ListContactWallet[item.SubItems[ix].Text].Item2 + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"));
                                    BeginInvoke(invoker);
                                }).ConfigureAwait(false);
#endif
                        }
                        else
                        {
                            Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                    ClassFormPhase.MessageBoxInterface(item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information)).ConfigureAwait(false);
#else
                                LinuxClipboard.SetText(item.SubItems[ix].Text);
                                Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoker = () => MessageBox.Show(ClassFormPhase.WalletXiropht, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"));
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

        #region Ordering events.

        private ColumnHeader SortingColumn = null;

        private void listViewNormalSendTransactionHistory_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnHeader new_sorting_column = listViewNormalSendTransactionHistory.Columns[e.Column];

            SortOrder sort_order;
            if (SortingColumn == null)
            {
                sort_order = SortOrder.Ascending;
            }
            else
            {
                if (new_sorting_column == SortingColumn)
                {
                    sort_order = SortingColumn.Text.StartsWith("> ") ? SortOrder.Descending : SortOrder.Ascending;
                }
                else
                {
                    sort_order = SortOrder.Ascending;
                }

                SortingColumn.Text = SortingColumn.Text.Substring(2);
            }

            SortingColumn = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn.Text = "> " + SortingColumn.Text;
            }
            else
            {
                SortingColumn.Text = "< " + SortingColumn.Text;
            }

            listViewNormalSendTransactionHistory.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            listViewNormalSendTransactionHistory.Sort();
        }

        private ColumnHeader SortingColumn2 = null;

        private void listViewNormalReceivedTransactionHistory_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnHeader new_sorting_column = listViewNormalReceivedTransactionHistory.Columns[e.Column];

            SortOrder sort_order;
            if (SortingColumn2 == null)
            {
                sort_order = SortOrder.Ascending;
            }
            else
            {
                if (new_sorting_column == SortingColumn2)
                {
                    sort_order = SortingColumn2.Text.StartsWith("> ") ? SortOrder.Descending : SortOrder.Ascending;
                }
                else
                {
                    sort_order = SortOrder.Ascending;
                }

                SortingColumn2.Text = SortingColumn2.Text.Substring(2);
            }

            SortingColumn2 = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn2.Text = "> " + SortingColumn2.Text;
            }
            else
            {
                SortingColumn2.Text = "< " + SortingColumn2.Text;
            }

            listViewNormalReceivedTransactionHistory.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            listViewNormalReceivedTransactionHistory.Sort();
        }

        private ColumnHeader SortingColumn3 = null;

        private void listViewAnonymitySendTransactionHistory_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnHeader new_sorting_column = listViewAnonymitySendTransactionHistory.Columns[e.Column];

            SortOrder sort_order;
            if (SortingColumn3 == null)
            {
                sort_order = SortOrder.Ascending;
            }
            else
            {
                if (new_sorting_column == SortingColumn3)
                {
                    sort_order = SortingColumn3.Text.StartsWith("> ") ? SortOrder.Descending : SortOrder.Ascending;
                }
                else
                {
                    sort_order = SortOrder.Ascending;
                }

                SortingColumn3.Text = SortingColumn3.Text.Substring(2);
            }

            SortingColumn3 = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn3.Text = "> " + SortingColumn3.Text;
            }
            else
            {
                SortingColumn3.Text = "< " + SortingColumn3.Text;
            }

            listViewAnonymitySendTransactionHistory.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            listViewAnonymitySendTransactionHistory.Sort();
        }

        private ColumnHeader SortingColumn4 = null;

        private void listViewAnonymityReceivedTransactionHistory_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnHeader new_sorting_column = listViewAnonymityReceivedTransactionHistory.Columns[e.Column];

            SortOrder sort_order;
            if (SortingColumn4 == null)
            {
                sort_order = SortOrder.Ascending;
            }
            else
            {
                if (new_sorting_column == SortingColumn4)
                {
                    sort_order = SortingColumn4.Text.StartsWith("> ") ? SortOrder.Descending : SortOrder.Ascending;
                }
                else
                {
                    sort_order = SortOrder.Ascending;
                }

                SortingColumn4.Text = SortingColumn4.Text.Substring(2);
            }

            SortingColumn4 = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn4.Text = "> " + SortingColumn4.Text;
            }
            else
            {
                SortingColumn4.Text = "< " + SortingColumn4.Text;
            }

            listViewAnonymityReceivedTransactionHistory.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            listViewAnonymityReceivedTransactionHistory.Sort();
        }

        private ColumnHeader SortingColumn5 = null;

        private void listViewBlockRewardTransactionHistory_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnHeader new_sorting_column = listViewBlockRewardTransactionHistory.Columns[e.Column];

            SortOrder sort_order;
            if (SortingColumn5 == null)
            {
                sort_order = SortOrder.Ascending;
            }
            else
            {
                if (new_sorting_column == SortingColumn5)
                {
                    sort_order = SortingColumn5.Text.StartsWith("> ") ? SortOrder.Descending : SortOrder.Ascending;
                }
                else
                {
                    sort_order = SortOrder.Ascending;
                }

                SortingColumn5.Text = SortingColumn5.Text.Substring(2);
            }

            SortingColumn5 = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn5.Text = "> " + SortingColumn5.Text;
            }
            else
            {
                SortingColumn5.Text = "< " + SortingColumn5.Text;
            }

            listViewBlockRewardTransactionHistory.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            listViewBlockRewardTransactionHistory.Sort();
        }

        #endregion

        private void Transaction_Resize(object sender, EventArgs e)
        {
            UpdateStyles();
        }


        private void tabPageTransactionHistory_Selected(object sender, TabControlEventArgs e)
        {
            ClassFormPhase.WalletXiropht.UpdateCurrentPageNumberTransactionHistory();
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
            ClassFormPhase.WalletXiropht.UpdateCurrentPageNumberTransactionHistory();
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

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
#if LINUX
            foreach (Control control in Controls)
            {
                control.Refresh();
            }
#endif
        }
    }

    public class ListViewComparer : System.Collections.IComparer
    {
        private int _columnNumber;
        private SortOrder _sortOrder;

        public ListViewComparer(int columnNumber,
            SortOrder sortOrder)
        {
            _columnNumber = columnNumber;
            _sortOrder = sortOrder;
        }

        public int Compare(object objectX, object objectY)
        {
            ListViewItem itemX = objectX as ListViewItem;
            ListViewItem itemY = objectY as ListViewItem;

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

            int result = 0;
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
            {
                return result;
            }
            else
            {
                return -result;
            }
        }
    }

    public sealed class ListViewEx : ListView
    {
        private const int WM_LBUTTONDOWN = 0x0201;

        public ListViewEx() : base()
        {

            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN)
            {
                short x = (short)m.LParam;

                short y = (short)((int)m.LParam >> 16);


                MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 2, x, y, 0);
#if WINDOWS
                UpdateStyles();
#endif
            }

            base.WndProc(ref m);
        }
    }
}