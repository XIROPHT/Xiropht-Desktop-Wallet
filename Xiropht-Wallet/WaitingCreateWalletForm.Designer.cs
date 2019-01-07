namespace Xiropht_Wallet
{
    partial class WaitingCreateWalletForm
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
            this.labelWaitCreateWallet = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelWaitCreateWallet
            // 
            this.labelWaitCreateWallet.AutoSize = true;
            this.labelWaitCreateWallet.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWaitCreateWallet.Location = new System.Drawing.Point(71, 51);
            this.labelWaitCreateWallet.Name = "labelWaitCreateWallet";
            this.labelWaitCreateWallet.Size = new System.Drawing.Size(275, 40);
            this.labelWaitCreateWallet.TabIndex = 1;
            this.labelWaitCreateWallet.Text = "Your Wallet is on pending create.\r\nPlease wait a little moment.";
            this.labelWaitCreateWallet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // WaitingCreateWalletForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 150);
            this.ControlBox = false;
            this.Controls.Add(this.labelWaitCreateWallet);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WaitingCreateWalletForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "WaitingCreateWalletForm";
            this.Load += new System.EventHandler(this.WaitingCreateWalletForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelWaitCreateWallet;
    }
}