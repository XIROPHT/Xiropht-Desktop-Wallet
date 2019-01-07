namespace Xiropht_Wallet.FormPhase.MainForm
{
    partial class Main
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
            this.labelNoticeWelcomeWallet = new System.Windows.Forms.Label();
            this.buttonMainOpenMenuWallet = new MetroFramework.Controls.MetroButton();
            this.buttonMainCreateWallet = new MetroFramework.Controls.MetroButton();
            this.labelNoticeLanguageAndIssue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelNoticeWelcomeWallet
            // 
            this.labelNoticeWelcomeWallet.AutoSize = true;
            this.labelNoticeWelcomeWallet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNoticeWelcomeWallet.Location = new System.Drawing.Point(181, 48);
            this.labelNoticeWelcomeWallet.Name = "labelNoticeWelcomeWallet";
            this.labelNoticeWelcomeWallet.Size = new System.Drawing.Size(439, 26);
            this.labelNoticeWelcomeWallet.TabIndex = 0;
            this.labelNoticeWelcomeWallet.Text = "Welcome to your Xiropht Wallet \r\nOpen your wallet or create a new one and start t" +
    "o send/receive transaction:";
            this.labelNoticeWelcomeWallet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonMainOpenMenuWallet
            // 
            this.buttonMainOpenMenuWallet.Location = new System.Drawing.Point(301, 145);
            this.buttonMainOpenMenuWallet.Name = "buttonMainOpenMenuWallet";
            this.buttonMainOpenMenuWallet.Size = new System.Drawing.Size(140, 50);
            this.buttonMainOpenMenuWallet.Style = MetroFramework.MetroColorStyle.Blue;
            this.buttonMainOpenMenuWallet.TabIndex = 1;
            this.buttonMainOpenMenuWallet.Text = "Open a wallet";
            this.buttonMainOpenMenuWallet.UseSelectable = true;
            this.buttonMainOpenMenuWallet.Click += new System.EventHandler(this.ButtonMainOpenMenuWallet_Click);
            // 
            // buttonMainCreateWallet
            // 
            this.buttonMainCreateWallet.Location = new System.Drawing.Point(301, 247);
            this.buttonMainCreateWallet.Name = "buttonMainCreateWallet";
            this.buttonMainCreateWallet.Size = new System.Drawing.Size(140, 50);
            this.buttonMainCreateWallet.Style = MetroFramework.MetroColorStyle.Red;
            this.buttonMainCreateWallet.TabIndex = 2;
            this.buttonMainCreateWallet.Text = "Create a Wallet";
            this.buttonMainCreateWallet.UseSelectable = true;
            this.buttonMainCreateWallet.Click += new System.EventHandler(this.ButtonMainCreateWallet_Click);
            // 
            // labelNoticeLanguageAndIssue
            // 
            this.labelNoticeLanguageAndIssue.AutoSize = true;
            this.labelNoticeLanguageAndIssue.Location = new System.Drawing.Point(241, 409);
            this.labelNoticeLanguageAndIssue.Name = "labelNoticeLanguageAndIssue";
            this.labelNoticeLanguageAndIssue.Size = new System.Drawing.Size(288, 26);
            this.labelNoticeLanguageAndIssue.TabIndex = 3;
            this.labelNoticeLanguageAndIssue.Text = "You can change the language of the wallet if it\'s necessary.\r\nFor any issues cont" +
    "act the Team.";
            this.labelNoticeLanguageAndIssue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 496);
            this.Controls.Add(this.labelNoticeLanguageAndIssue);
            this.Controls.Add(this.buttonMainCreateWallet);
            this.Controls.Add(this.buttonMainOpenMenuWallet);
            this.Controls.Add(this.labelNoticeWelcomeWallet);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "OpenWallet";
            this.Load += new System.EventHandler(this.Main_Load);
            this.Resize += new System.EventHandler(this.Main_Resize_1);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label labelNoticeWelcomeWallet;
        public MetroFramework.Controls.MetroButton buttonMainOpenMenuWallet;
        public MetroFramework.Controls.MetroButton buttonMainCreateWallet;
        public System.Windows.Forms.Label labelNoticeLanguageAndIssue;
    }
}