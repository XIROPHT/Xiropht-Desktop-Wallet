using System;
using System.Drawing;
using System.Reflection;
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
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
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
                if (colWidth > cc[i].Width || cc[i].Text == "Fee" || cc[i].Text == "Amount")
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

        public void ResyncBlock()
        {
            ClassBlockCache.RemoveWalletBlockCache();
        }

        private void Block_Load(object sender, EventArgs e)
        {
            IsShowed = true;
            UpdateStyles();

            ClassFormPhase.WalletXiropht.ResizeWalletInterface();
        }

        private void Block_Resize(object sender, EventArgs e)
        {
            UpdateStyles();
        }

        private void listViewBlockExplorer_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}