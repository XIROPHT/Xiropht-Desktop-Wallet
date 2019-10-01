using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xiropht_Connector_All.Setting;
using Xiropht_Wallet.Debug;
using Xiropht_Wallet.Features;
using Xiropht_Wallet.FormCustom;
using Xiropht_Wallet.Utility;
using Xiropht_Wallet.Wallet.Sync;

namespace Xiropht_Wallet.FormPhase.MainForm
{
    public sealed partial class BlockExplorerWallet : Form
    {
        public Label _labelWaitingText = new Label();
        private ClassPanel _panelWaitingSync;
        public bool IsShowed;
        public bool IsShowedWaitingBlock;

        public BlockExplorerWallet()
        {
            InitializeComponent();
            SetStyle(
                ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            AutoScroll = true;
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

        public void GetListControl()
        {
            if (Program.WalletXiropht.ListControlSizeBlock.Count == 0)
                for (var i = 0; i < Controls.Count; i++)
                    if (i < Controls.Count)
                        Program.WalletXiropht.ListControlSizeBlock.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
        }

        /// <summary>
        ///     Force to resync blocks.
        /// </summary>
        public async void ResyncBlock()
        {
            if (ClassBlockCache.RemoveWalletBlockCache())
            {
                MethodInvoker invoke = () =>
                {
                    try
                    {
                        listViewBlockExplorer.Items.Clear();
                    }
                    catch
                    {
                    }
                };
                Program.WalletXiropht.BeginInvoke(invoke);
                try
                {
                    Program.WalletXiropht.ListBlockHashShowed.Clear();
                }
                catch
                {
                }


                await Program.WalletXiropht.ClassWalletObject.DisconnectRemoteNodeTokenSync();
                Program.WalletXiropht.ClassWalletObject.WalletOnUseSync = false;


                Program.WalletXiropht.StopUpdateBlockHistory(true, false);
            }
        }

        private void Block_Load(object sender, EventArgs e)
        {
            IsShowed = true;
            UpdateStyles();
            listViewBlockExplorer.ListViewItemSorter = new ListViewComparer(0, SortOrder.Descending);


            _panelWaitingSync = new ClassPanel
            {
                Width = (int)(Width / 1.5f),
                Height = (int)(Height / 5.5f),
                BackColor = Color.LightBlue
            };
            _panelWaitingSync.Location = new Point
            {
                X = Program.WalletXiropht.BlockWalletForm.Width / 2 - _panelWaitingSync.Width / 2,
                Y = Program.WalletXiropht.BlockWalletForm.Height / 2 - _panelWaitingSync.Height / 2
            };

            _labelWaitingText.AutoSize = true;
            _labelWaitingText.Font = new Font(_labelWaitingText.Font.FontFamily, 9f, FontStyle.Bold);
            _labelWaitingText.Text = ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.waitingmenulabeltext);
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
            Program.WalletXiropht.ResizeWalletInterface();
        }

        private void Block_Resize(object sender, EventArgs e)
        {
            UpdateStyles();
        }

        protected override void OnResize(EventArgs e)
        {
            if (_panelWaitingSync != null)
            {
                _panelWaitingSync.Width = (int)(Width / 1.5f);
                _panelWaitingSync.Height = (int)(Height / 5.5f);
                _panelWaitingSync.Location = new Point
                {
                    X = Program.WalletXiropht.BlockWalletForm.Width / 2 -
                        _panelWaitingSync.Width / 2,
                    Y = Program.WalletXiropht.BlockWalletForm.Height / 2 -
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

        public void ShowWaitingSyncBlockPanel()
        {
            if (!IsShowedWaitingBlock)
                Program.WalletXiropht.Invoke((MethodInvoker)delegate
               {
                   _panelWaitingSync.Visible = true;
                   _panelWaitingSync.Show();
                   _panelWaitingSync.BringToFront();
                   _panelWaitingSync.Width = (int)(Width / 1.5f);
                   _panelWaitingSync.Height = (int)(Height / 5.5f);
                   _panelWaitingSync.Location = new Point
                   {
                       X = Program.WalletXiropht.BlockWalletForm.Width / 2 -
                           _panelWaitingSync.Width / 2,
                       Y = Program.WalletXiropht.BlockWalletForm.Height / 2 -
                           _panelWaitingSync.Height / 2
                   };
                   _labelWaitingText.Location = new Point
                   {
                       X = _panelWaitingSync.Width / 2 - _labelWaitingText.Width / 2,
                       Y = _panelWaitingSync.Height / 2 - _labelWaitingText.Height / 2
                   };
                   IsShowedWaitingBlock = true;
                   UpdateStyles();
               });
        }

        public void HideWaitingSyncBlockPanel()
        {
            if (IsShowedWaitingBlock)
                Program.WalletXiropht.Invoke((MethodInvoker)delegate
               {
                   _panelWaitingSync.Visible = false;
                   _panelWaitingSync.Hide();
                   IsShowedWaitingBlock = false;
                   UpdateStyles();
               });
        }


        private void listViewBlockExplorer_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                var item = listViewBlockExplorer.GetItemAt(0, e.Y);

                var found = false;
                if (item == null) return;
                for (var ix = item.SubItems.Count - 1; ix >= 0; --ix)
                    if (item.SubItems[ix].Bounds.Contains(e.Location))
                        if (!found)
                        {
                            found = true;
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
                            return;
                        }
            }
            catch
            {
            }
        }


        /// <summary>
        ///     Sort block explorer by block id
        /// </summary>
        public void SortingBlockExplorer()
        {
            MethodInvoker invoke = () => listViewBlockExplorer.Sort();
            listViewBlockExplorer.BeginInvoke(invoke);
        }

        private void timerShowBlockExplorer_Tick(object sender, EventArgs e)
        {
            try
            {
                StartUpdateBlockSync(Program.WalletXiropht);
            }
            catch
            {

            }
        }

        /// <summary>
        ///     Start update block sync.
        /// </summary>
        public void StartUpdateBlockSync(WalletXiropht walletXiropht)
        {

            if (!ClassBlockCache.OnLoad)
            {
                try
                {
                    if (!walletXiropht.ClassWalletObject.InSyncBlock || ClassBlockCache.ListBlock.Count >= walletXiropht.ClassWalletObject.TotalBlockInSync)
                    {
                        if (this.IsShowed)
                        {
                            this.HideWaitingSyncBlockPanel();

                            var minShow = (walletXiropht.CurrentBlockExplorerPage - 1) * walletXiropht.MaxBlockPerPage;
                            var maxShow = walletXiropht.CurrentBlockExplorerPage * walletXiropht.MaxBlockPerPage;

                            for (var i = minShow; i < maxShow; i++)
                            {
                                if (i < ClassBlockCache.ListBlock.Count)
                                {
                                    var blockTarget = ClassBlockCache.ListBlock.Count - 1 - i;
                                    var blockObject = ClassBlockCache.ListBlock
                                        .ElementAt(blockTarget)
                                        .Value;
                                    var blockId = int.Parse(blockObject.BlockHeight);
                                    if (!walletXiropht.ListBlockHashShowed.ContainsKey(blockId))
                                    {
                                        walletXiropht.ListBlockHashShowed.Add(blockId, blockObject.BlockHash);

                                        var dateTimeCreate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                        dateTimeCreate =
                                            dateTimeCreate.AddSeconds(
                                                int.Parse(blockObject.BlockTimestampCreate));
                                        dateTimeCreate = dateTimeCreate.ToLocalTime();
                                        var dateTimeFound = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                        dateTimeFound =
                                            dateTimeFound.AddSeconds(
                                                int.Parse(blockObject.BlockTimestampFound));
                                        dateTimeFound = dateTimeFound.ToLocalTime();

                                        string[] row =
                                        {
                                                                blockObject.BlockHeight, blockObject.BlockHash,
                                                                blockObject.BlockReward, blockObject.BlockDifficulty,
                                                                dateTimeCreate.ToString(CultureInfo.InvariantCulture),
                                                                dateTimeFound.ToString(CultureInfo.InvariantCulture),
                                                                blockObject.BlockTransactionHash
                                                            };
                                        var listViewItem = new ListViewItem(row);


                                        if (walletXiropht.TotalBlockRead < maxShow)
                                        {
                                            void Invoker()
                                            {
                                                this.listViewBlockExplorer.Items.Add(
                                                    listViewItem);
                                            }

                                            BeginInvoke((MethodInvoker)Invoker);
                                            walletXiropht.TotalBlockRead++;
                                        }
                                        else
                                        {
                                            void Invoker()
                                            {
                                                this.listViewBlockExplorer.Items.Insert(
                                                    0,
                                                    listViewItem);
                                            }

                                            BeginInvoke((MethodInvoker)Invoker);
                                            walletXiropht.TotalBlockRead++;
                                        }
                                    }
                                }

                                if (ClassFormPhase.FormPhase ==
                                    ClassFormPhaseEnumeration.BlockExplorer)
                                {
                                    if (walletXiropht.ClassWalletObject.InSyncBlock)
                                        walletXiropht.UpdateLabelSyncInformation(
                                            "Total blocks synced: " +
                                            ClassBlockCache.ListBlock.Count + "/" + walletXiropht.ClassWalletObject.TotalBlockInSync + ".");
                                    else
                                        walletXiropht.UpdateLabelSyncInformation(
                                            "Total blocks loaded: " +
                                            ClassBlockCache.ListBlock.Count +
                                            "/" + walletXiropht.ClassWalletObject.TotalBlockInSync + ".");
                                }
                            }

                            if (this.IsShowed)
                                if (this.listViewBlockExplorer.Items.Count > 0)
                                    this.SortingBlockExplorer();
                        }
                    }
                    else
                    {
                        if (this.IsShowed) this.ShowWaitingSyncBlockPanel();
                    }
                }
                catch (Exception error)
                {
#if DEBUG
                    Log.WriteLine("Error loading blocks: " + error.Message);
#endif
                    Console.WriteLine("Error loading blocks: " + error.Message);
                }

                try
                {
                    if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.BlockExplorer)
                    {
                        if (walletXiropht.ClassWalletObject.InSyncBlock)
                            walletXiropht.UpdateLabelSyncInformation(
                                "Total blocks synced: " + ClassBlockCache.ListBlock.Count + "/" + walletXiropht.ClassWalletObject.TotalBlockInSync + ".");
                        else
                            walletXiropht.UpdateLabelSyncInformation(
                                "Total blocks loaded: " + ClassBlockCache.ListBlock.Count + "/" + walletXiropht.ClassWalletObject.TotalBlockInSync + ".");
                    }

                    if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.Overview)
                        if (walletXiropht.ClassWalletObject.InSyncBlock &&
                            !walletXiropht.ClassWalletObject.InSyncTransactionAnonymity &&
                            !walletXiropht.ClassWalletObject.InSyncTransaction)
                            try
                            {
                                if (ClassConnectorSetting.SeedNodeIp.ContainsKey(walletXiropht.ClassWalletObject
                                    .ListWalletConnectToRemoteNode[9].RemoteNodeHost))
                                    walletXiropht.UpdateLabelSyncInformation(
                                        "Total blocks synced: " + ClassBlockCache.ListBlock.Count +
                                        "/" + walletXiropht.ClassWalletObject.TotalBlockInSync + " from Seed Node: " + walletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode[9]
                                            .RemoteNodeHost +
                                        " | " + ClassConnectorSetting
                                            .SeedNodeIp[walletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode[0]
                                                .RemoteNodeHost].Item1 + ".");
                                else
                                    walletXiropht.UpdateLabelSyncInformation(
                                        "Total blocks synced: " + ClassBlockCache.ListBlock.Count +
                                        "/" + walletXiropht.ClassWalletObject.TotalBlockInSync + " from node: " + walletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode[0]
                                            .RemoteNodeHost +
                                        ".");
                            }
                            catch
                            {
                            }
                }
                catch
                {
                }
            }
        }
    }
}