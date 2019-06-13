namespace Xiropht_Wallet.FormPhase.MainForm
{
    sealed partial class BlockExplorerWallet
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.columnHeaderID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderHash = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderAmount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFee = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDateRecv = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderBlockHeightSrc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewBlockExplorer = new Xiropht_Wallet.FormPhase.MainForm.ListViewEx();
            this.columnHeaderBlockId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderBlockHash = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderBlockReward = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderBlockDifficulty = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderBlockDateCreate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderBlockDateFound = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderBlockTransactionHash = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // columnHeaderID
            // 
            this.columnHeaderID.Text = "ID";
            // 
            // columnHeaderDate
            // 
            this.columnHeaderDate.Text = "Date";
            // 
            // columnHeaderType
            // 
            this.columnHeaderType.Text = "Type";
            // 
            // columnHeaderHash
            // 
            this.columnHeaderHash.Text = "Hash";
            // 
            // columnHeaderAmount
            // 
            this.columnHeaderAmount.Text = "Amount";
            // 
            // columnHeaderFee
            // 
            this.columnHeaderFee.Text = "Fee";
            // 
            // columnHeaderAddress
            // 
            this.columnHeaderAddress.Text = "Address";
            // 
            // columnHeaderDateRecv
            // 
            this.columnHeaderDateRecv.Text = "Date Recv";
            // 
            // columnHeaderBlockHeightSrc
            // 
            this.columnHeaderBlockHeightSrc.Text = "Block Height Src";
            // 
            // listViewBlockExplorer
            // 
            this.listViewBlockExplorer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewBlockExplorer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderBlockId,
            this.columnHeaderBlockHash,
            this.columnHeaderBlockReward,
            this.columnHeaderBlockDifficulty,
            this.columnHeaderBlockDateCreate,
            this.columnHeaderBlockDateFound,
            this.columnHeaderBlockTransactionHash});
            this.listViewBlockExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewBlockExplorer.FullRowSelect = true;
            this.listViewBlockExplorer.GridLines = true;
            this.listViewBlockExplorer.Location = new System.Drawing.Point(0, 0);
            this.listViewBlockExplorer.Name = "listViewBlockExplorer";
            this.listViewBlockExplorer.ShowGroups = false;
            this.listViewBlockExplorer.Size = new System.Drawing.Size(784, 496);
            this.listViewBlockExplorer.TabIndex = 0;
            this.listViewBlockExplorer.UseCompatibleStateImageBehavior = false;
            this.listViewBlockExplorer.View = System.Windows.Forms.View.Details;
            this.listViewBlockExplorer.SelectedIndexChanged += new System.EventHandler(this.listViewBlockExplorer_SelectedIndexChanged);
            this.listViewBlockExplorer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewBlockExplorer_MouseClick);
            // 
            // columnHeaderBlockId
            // 
            this.columnHeaderBlockId.Text = "ID";
            // 
            // columnHeaderBlockHash
            // 
            this.columnHeaderBlockHash.Text = "Hash";
            this.columnHeaderBlockHash.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderBlockHash.Width = 140;
            // 
            // columnHeaderBlockReward
            // 
            this.columnHeaderBlockReward.Text = "Reward";
            this.columnHeaderBlockReward.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderBlockReward.Width = 102;
            // 
            // columnHeaderBlockDifficulty
            // 
            this.columnHeaderBlockDifficulty.Text = "Difficulty";
            this.columnHeaderBlockDifficulty.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderBlockDifficulty.Width = 106;
            // 
            // columnHeaderBlockDateCreate
            // 
            this.columnHeaderBlockDateCreate.Text = "Date Create";
            this.columnHeaderBlockDateCreate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderBlockDateCreate.Width = 112;
            // 
            // columnHeaderBlockDateFound
            // 
            this.columnHeaderBlockDateFound.Text = "Date Found";
            this.columnHeaderBlockDateFound.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderBlockDateFound.Width = 121;
            // 
            // columnHeaderBlockTransactionHash
            // 
            this.columnHeaderBlockTransactionHash.Text = "Transaction Hash";
            this.columnHeaderBlockTransactionHash.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderBlockTransactionHash.Width = 134;
            // 
            // BlockExplorerWallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 496);
            this.ControlBox = false;
            this.Controls.Add(this.listViewBlockExplorer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BlockExplorerWallet";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Block";
            this.Load += new System.EventHandler(this.Block_Load);
            this.Resize += new System.EventHandler(this.Block_Resize);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ColumnHeader columnHeaderID;
        private System.Windows.Forms.ColumnHeader columnHeaderDate;
        private System.Windows.Forms.ColumnHeader columnHeaderType;
        private System.Windows.Forms.ColumnHeader columnHeaderHash;
        private System.Windows.Forms.ColumnHeader columnHeaderAmount;
        private System.Windows.Forms.ColumnHeader columnHeaderFee;
        private System.Windows.Forms.ColumnHeader columnHeaderAddress;
        private System.Windows.Forms.ColumnHeader columnHeaderDateRecv;
        private System.Windows.Forms.ColumnHeader columnHeaderBlockHeightSrc;
        public ListViewEx listViewBlockExplorer;
        private System.Windows.Forms.ColumnHeader columnHeaderBlockId;
        private System.Windows.Forms.ColumnHeader columnHeaderBlockHash;
        private System.Windows.Forms.ColumnHeader columnHeaderBlockReward;
        private System.Windows.Forms.ColumnHeader columnHeaderBlockDifficulty;
        private System.Windows.Forms.ColumnHeader columnHeaderBlockDateCreate;
        private System.Windows.Forms.ColumnHeader columnHeaderBlockDateFound;
        private System.Windows.Forms.ColumnHeader columnHeaderBlockTransactionHash;
    }
}