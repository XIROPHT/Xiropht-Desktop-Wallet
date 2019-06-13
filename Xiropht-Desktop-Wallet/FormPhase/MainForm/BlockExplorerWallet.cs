using System;
using System.Collections;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xiropht_Wallet.Wallet;

namespace Xiropht_Wallet.FormPhase.MainForm
{
    public sealed partial class BlockExplorerWallet : Form
    {
        public bool IsShowed;

        public BlockExplorerWallet()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            AutoScroll = true;
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

        public void AutoResizeColumns(ListView lv)
        {
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            ListView.ColumnHeaderCollection cc = lv.Columns;
            for (int i = 0; i < cc.Count; i++)
            {
                int colWidth = TextRenderer.MeasureText(cc[i].Text, lv.Font).Width + 30;
                if (colWidth > cc[i].Width)
                {
                    cc[i].Width = colWidth + 30;
                }
            }
        }

        public void GetListControl()
        {
            if (ClassFormPhase.WalletXiropht.ListControlSizeBlock.Count == 0)
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (i < Controls.Count)
                    {
                        ClassFormPhase.WalletXiropht.ListControlSizeBlock.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
                    }
                }
            }
        }

        /// <summary>
        /// Force to resync blocks.
        /// </summary>
        public void ResyncBlock()
        {
            if (ClassBlockCache.RemoveWalletBlockCache())
            {
                ClassFormPhase.WalletXiropht.ListBlockHashShowed.Clear();
                ClassFormPhase.WalletXiropht.ClassWalletObject.DisconnectWholeRemoteNodeSyncAsync(true, true);
            }
        }

        private void Block_Load(object sender, EventArgs e)
        {
            IsShowed = true;
            UpdateStyles();
            listViewBlockExplorer.ListViewItemSorter = new ListViewComparer(0, SortOrder.Descending);

            ClassFormPhase.WalletXiropht.ResizeWalletInterface();
        }

        private void Block_Resize(object sender, EventArgs e)
        {
            UpdateStyles();
        }


        private void listViewBlockExplorer_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                ListViewItem item = listViewBlockExplorer.GetItemAt(0, e.Y);

                bool found = false;
                if (item == null) return;
                for (int ix = item.SubItems.Count - 1; ix >= 0; --ix)
                    if (item.SubItems[ix].Bounds.Contains(e.Location))
                    {
                        if (!found)
                        {
                            found = true;
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
                            return;
                        }
                    }
            }
            catch
            {

            }
        }



        /// <summary>
        /// Sort block explorer by block id
        /// </summary>
        public void SortingBlockExplorer()
        {


            MethodInvoker invoke = () => listViewBlockExplorer.Sort();
            listViewBlockExplorer.BeginInvoke(invoke);
        }
    }



}