using System.Windows.Forms;

namespace Xiropht_Wallet.FormPhase.MainForm
{
    sealed partial class OverviewWallet : Form
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
            this.labelTextNetworkStats = new System.Windows.Forms.Label();
            this.labelTextCoinMaxSupply = new System.Windows.Forms.Label();
            this.labelTextCoinCirculating = new System.Windows.Forms.Label();
            this.labelTextCoinMined = new System.Windows.Forms.Label();
            this.labelTextTransactionFee = new System.Windows.Forms.Label();
            this.labelTextTotalBlockMined = new System.Windows.Forms.Label();
            this.labelTextNetworkDifficulty = new System.Windows.Forms.Label();
            this.labelTextTotalBlockLeft = new System.Windows.Forms.Label();
            this.labelTextNetworkHashrate = new System.Windows.Forms.Label();
            this.labelTextBlockchainHeight = new System.Windows.Forms.Label();
            this.labelTextLastBlockFound = new System.Windows.Forms.Label();
            this.buttonFeeInformationAccumulated = new System.Windows.Forms.Button();
            this.labelTextTotalCoinInPending = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelTextNetworkStats
            // 
            this.labelTextNetworkStats.AutoSize = true;
            this.labelTextNetworkStats.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTextNetworkStats.Location = new System.Drawing.Point(331, 22);
            this.labelTextNetworkStats.Name = "labelTextNetworkStats";
            this.labelTextNetworkStats.Size = new System.Drawing.Size(87, 13);
            this.labelTextNetworkStats.TabIndex = 0;
            this.labelTextNetworkStats.Text = "Network Stats";
            // 
            // labelTextCoinMaxSupply
            // 
            this.labelTextCoinMaxSupply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTextCoinMaxSupply.AutoSize = true;
            this.labelTextCoinMaxSupply.Location = new System.Drawing.Point(181, 86);
            this.labelTextCoinMaxSupply.Name = "labelTextCoinMaxSupply";
            this.labelTextCoinMaxSupply.Size = new System.Drawing.Size(89, 13);
            this.labelTextCoinMaxSupply.TabIndex = 1;
            this.labelTextCoinMaxSupply.Text = "Coin Max Supply:";
            this.labelTextCoinMaxSupply.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTextCoinCirculating
            // 
            this.labelTextCoinCirculating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTextCoinCirculating.AutoSize = true;
            this.labelTextCoinCirculating.Location = new System.Drawing.Point(181, 119);
            this.labelTextCoinCirculating.Name = "labelTextCoinCirculating";
            this.labelTextCoinCirculating.Size = new System.Drawing.Size(83, 13);
            this.labelTextCoinCirculating.TabIndex = 2;
            this.labelTextCoinCirculating.Text = "Coin Circulating:";
            this.labelTextCoinCirculating.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTextCoinMined
            // 
            this.labelTextCoinMined.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTextCoinMined.AutoSize = true;
            this.labelTextCoinMined.Location = new System.Drawing.Point(181, 228);
            this.labelTextCoinMined.Name = "labelTextCoinMined";
            this.labelTextCoinMined.Size = new System.Drawing.Size(90, 13);
            this.labelTextCoinMined.TabIndex = 3;
            this.labelTextCoinMined.Text = "Total Coin Mined:";
            this.labelTextCoinMined.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTextTransactionFee
            // 
            this.labelTextTransactionFee.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTextTransactionFee.AutoSize = true;
            this.labelTextTransactionFee.Location = new System.Drawing.Point(181, 192);
            this.labelTextTransactionFee.Name = "labelTextTransactionFee";
            this.labelTextTransactionFee.Size = new System.Drawing.Size(163, 13);
            this.labelTextTransactionFee.TabIndex = 4;
            this.labelTextTransactionFee.Text = "Transaction Fee(s) Accumulated:";
            this.labelTextTransactionFee.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTextTotalBlockMined
            // 
            this.labelTextTotalBlockMined.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTextTotalBlockMined.AutoSize = true;
            this.labelTextTotalBlockMined.Location = new System.Drawing.Point(181, 305);
            this.labelTextTotalBlockMined.Name = "labelTextTotalBlockMined";
            this.labelTextTotalBlockMined.Size = new System.Drawing.Size(96, 13);
            this.labelTextTotalBlockMined.TabIndex = 5;
            this.labelTextTotalBlockMined.Text = "Total Block Mined:";
            this.labelTextTotalBlockMined.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTextNetworkDifficulty
            // 
            this.labelTextNetworkDifficulty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTextNetworkDifficulty.AutoSize = true;
            this.labelTextNetworkDifficulty.Location = new System.Drawing.Point(181, 378);
            this.labelTextNetworkDifficulty.Name = "labelTextNetworkDifficulty";
            this.labelTextNetworkDifficulty.Size = new System.Drawing.Size(93, 13);
            this.labelTextNetworkDifficulty.TabIndex = 6;
            this.labelTextNetworkDifficulty.Text = "Network Difficulty:";
            this.labelTextNetworkDifficulty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTextTotalBlockLeft
            // 
            this.labelTextTotalBlockLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTextTotalBlockLeft.AutoSize = true;
            this.labelTextTotalBlockLeft.Location = new System.Drawing.Point(181, 341);
            this.labelTextTotalBlockLeft.Name = "labelTextTotalBlockLeft";
            this.labelTextTotalBlockLeft.Size = new System.Drawing.Size(85, 13);
            this.labelTextTotalBlockLeft.TabIndex = 7;
            this.labelTextTotalBlockLeft.Text = "Total Block Left:";
            this.labelTextTotalBlockLeft.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTextNetworkHashrate
            // 
            this.labelTextNetworkHashrate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTextNetworkHashrate.AutoSize = true;
            this.labelTextNetworkHashrate.Location = new System.Drawing.Point(181, 417);
            this.labelTextNetworkHashrate.Name = "labelTextNetworkHashrate";
            this.labelTextNetworkHashrate.Size = new System.Drawing.Size(96, 13);
            this.labelTextNetworkHashrate.TabIndex = 8;
            this.labelTextNetworkHashrate.Text = "Network Hashrate:";
            this.labelTextNetworkHashrate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTextBlockchainHeight
            // 
            this.labelTextBlockchainHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTextBlockchainHeight.AutoSize = true;
            this.labelTextBlockchainHeight.Location = new System.Drawing.Point(181, 265);
            this.labelTextBlockchainHeight.Name = "labelTextBlockchainHeight";
            this.labelTextBlockchainHeight.Size = new System.Drawing.Size(97, 13);
            this.labelTextBlockchainHeight.TabIndex = 9;
            this.labelTextBlockchainHeight.Text = "Blockchain Height:";
            this.labelTextBlockchainHeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTextLastBlockFound
            // 
            this.labelTextLastBlockFound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTextLastBlockFound.AutoSize = true;
            this.labelTextLastBlockFound.Location = new System.Drawing.Point(181, 458);
            this.labelTextLastBlockFound.Name = "labelTextLastBlockFound";
            this.labelTextLastBlockFound.Size = new System.Drawing.Size(93, 13);
            this.labelTextLastBlockFound.TabIndex = 10;
            this.labelTextLastBlockFound.Text = "Last Block Found:";
            this.labelTextLastBlockFound.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonFeeInformationAccumulated
            // 
            this.buttonFeeInformationAccumulated.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFeeInformationAccumulated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonFeeInformationAccumulated.Location = new System.Drawing.Point(153, 183);
            this.buttonFeeInformationAccumulated.Name = "buttonFeeInformationAccumulated";
            this.buttonFeeInformationAccumulated.Size = new System.Drawing.Size(22, 22);
            this.buttonFeeInformationAccumulated.TabIndex = 25;
            this.buttonFeeInformationAccumulated.Text = "?";
            this.buttonFeeInformationAccumulated.UseVisualStyleBackColor = true;
            this.buttonFeeInformationAccumulated.Click += new System.EventHandler(this.buttonFeeInformationAccumulated_Click);
            this.buttonFeeInformationAccumulated.MouseHover += new System.EventHandler(this.buttonFeeInformationAccumulated_MouseHover);
            // 
            // labelTextTotalCoinInPending
            // 
            this.labelTextTotalCoinInPending.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTextTotalCoinInPending.AutoSize = true;
            this.labelTextTotalCoinInPending.Location = new System.Drawing.Point(181, 155);
            this.labelTextTotalCoinInPending.Name = "labelTextTotalCoinInPending";
            this.labelTextTotalCoinInPending.Size = new System.Drawing.Size(112, 13);
            this.labelTextTotalCoinInPending.TabIndex = 26;
            this.labelTextTotalCoinInPending.Text = "Total Coin In Pending:";
            this.labelTextTotalCoinInPending.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Overview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 496);
            this.ControlBox = false;
            this.Controls.Add(this.labelTextTotalCoinInPending);
            this.Controls.Add(this.buttonFeeInformationAccumulated);
            this.Controls.Add(this.labelTextLastBlockFound);
            this.Controls.Add(this.labelTextBlockchainHeight);
            this.Controls.Add(this.labelTextNetworkHashrate);
            this.Controls.Add(this.labelTextTotalBlockLeft);
            this.Controls.Add(this.labelTextNetworkDifficulty);
            this.Controls.Add(this.labelTextTotalBlockMined);
            this.Controls.Add(this.labelTextTransactionFee);
            this.Controls.Add(this.labelTextCoinMined);
            this.Controls.Add(this.labelTextCoinCirculating);
            this.Controls.Add(this.labelTextCoinMaxSupply);
            this.Controls.Add(this.labelTextNetworkStats);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Overview";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Overview";
            this.Load += new System.EventHandler(this.Overview_Load);
            this.Resize += new System.EventHandler(this.Overview_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public Label labelTextCoinMaxSupply;
        public Label labelTextCoinCirculating;
        public Label labelTextCoinMined;
        public Label labelTextTransactionFee;
        public Label labelTextTotalBlockMined;
        public Label labelTextNetworkDifficulty;
        public Label labelTextTotalBlockLeft;
        public Label labelTextNetworkHashrate;
        public Label labelTextBlockchainHeight;
        public Label labelTextLastBlockFound;
        public Label labelTextNetworkStats;
        public Button buttonFeeInformationAccumulated;
        public Label labelTextTotalCoinInPending;
    }
}