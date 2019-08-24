using System;
using System.Windows.Forms;
using Xiropht_Wallet.Utility;

namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    public partial class SearchWalletExplorer : Form
    {
        public SearchWalletExplorer()
        {
            InitializeComponent();
        }


        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SearchWalletExplorer_Load(object sender, EventArgs e)
        {
            AddContextMenu(richTextBoxResearchResult);
            UpdateStyles();
        }

        /// <summary>
        ///     Add a context menu into richtextbox control.
        /// </summary>
        /// <param name="rtb"></param>
        public void AddContextMenu(RichTextBox rtb)
        {
            if (rtb.ContextMenuStrip == null)
            {
                var cms = new ContextMenuStrip
                {
                    ShowImageMargin = false
                };

                var tsmiUndo = new ToolStripMenuItem("Undo");
                tsmiUndo.Click += (sender, e) => rtb.Undo();
                cms.Items.Add(tsmiUndo);

                var tsmiRedo = new ToolStripMenuItem("Redo");
                tsmiRedo.Click += (sender, e) => rtb.Redo();
                cms.Items.Add(tsmiRedo);

                cms.Items.Add(new ToolStripSeparator());

                var tsmiCut = new ToolStripMenuItem("Cut");
                tsmiCut.Click += (sender, e) =>
                {
#if WINDOWS
                    rtb.Cut();
#endif
#if LINUX
                    LinuxClipboard.SetText(rtb.Text);
                    rtb.Clear();
#endif
                };
                cms.Items.Add(tsmiCut);

                var tsmiCopy = new ToolStripMenuItem("Copy");
                tsmiCopy.Click += (sender, e) =>
                {
#if WINDOWS
                    rtb.Copy();
#endif
#if LINUX
                    LinuxClipboard.SetText(rtb.Text);
#endif
                };
                cms.Items.Add(tsmiCopy);

                var tsmiDelete = new ToolStripMenuItem("Delete");
                tsmiDelete.Click += (sender, e) => rtb.SelectedText = "";
                cms.Items.Add(tsmiDelete);

                cms.Items.Add(new ToolStripSeparator());

                var tsmiSelectAll = new ToolStripMenuItem("Select All");
                tsmiSelectAll.Click += (sender, e) => rtb.SelectAll();
                cms.Items.Add(tsmiSelectAll);

                cms.Opening += (sender, e) =>
                {
                    tsmiUndo.Enabled = !rtb.ReadOnly && rtb.CanUndo;
                    tsmiRedo.Enabled = !rtb.ReadOnly && rtb.CanRedo;
                    tsmiCut.Enabled = !rtb.ReadOnly && rtb.SelectionLength > 0;
                    tsmiCopy.Enabled = rtb.SelectionLength > 0;
                    tsmiDelete.Enabled = !rtb.ReadOnly && rtb.SelectionLength > 0;
                    tsmiSelectAll.Enabled = rtb.TextLength > 0 && rtb.SelectionLength < rtb.TextLength;
                };

                rtb.ContextMenuStrip = cms;
            }
        }

        /// <summary>
        ///     Append Text to the richtextbox control.
        /// </summary>
        /// <param name="text"></param>
        public void AppendText(string text)
        {
            richTextBoxResearchResult.AppendText(text + "\n");
        }

        #region To finish 

        private void richTextBoxResearchResult_MouseClick(object sender, MouseEventArgs e)
        {
            /*
            int index = richTextBoxResearchResult.SelectionStart;
            int lineIndex = richTextBoxResearchResult.GetLineFromCharIndex(index);
            if (richTextBoxResearchResult.Lines[lineIndex].Contains("Page"))
            {
                if (int.TryParse(richTextBoxResearchResult.Lines[lineIndex].Replace("Page ", ""), out var pageNumber))
                {
                    if (ClassFormPhase.MessageBoxInterface("Do you want to reach the page: "+pageNumber+" on your transaction history?", "Research System", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {

                    }
                }
            }*/
        }

        #endregion
    }
}