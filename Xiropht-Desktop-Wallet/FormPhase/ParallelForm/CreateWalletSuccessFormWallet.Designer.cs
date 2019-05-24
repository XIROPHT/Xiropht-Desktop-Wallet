namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    partial class CreateWalletSuccessFormWallet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateWalletSuccessFormWallet));
            this.buttonAcceptAndCloseWalletInformation = new System.Windows.Forms.Button();
            this.labelYourPublicKey = new System.Windows.Forms.Label();
            this.labelYourPrivateKey = new System.Windows.Forms.Label();
            this.labelYourPinCode = new System.Windows.Forms.Label();
            this.buttonCopyWalletInformation = new System.Windows.Forms.Button();
            this.labelCreateWalletInformation = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonAcceptAndCloseWalletInformation
            // 
            this.buttonAcceptAndCloseWalletInformation.Location = new System.Drawing.Point(150, 372);
            this.buttonAcceptAndCloseWalletInformation.Name = "buttonAcceptAndCloseWalletInformation";
            this.buttonAcceptAndCloseWalletInformation.Size = new System.Drawing.Size(511, 74);
            this.buttonAcceptAndCloseWalletInformation.TabIndex = 3;
            this.buttonAcceptAndCloseWalletInformation.Text = "I have copy/past my wallet informations before to close this window.";
            this.buttonAcceptAndCloseWalletInformation.Click += new System.EventHandler(this.ButtonAcceptAndCloseWalletInformation_Click);
            // 
            // labelYourPublicKey
            // 
            this.labelYourPublicKey.AutoSize = true;
            this.labelYourPublicKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelYourPublicKey.Location = new System.Drawing.Point(12, 193);
            this.labelYourPublicKey.Name = "labelYourPublicKey";
            this.labelYourPublicKey.Size = new System.Drawing.Size(146, 16);
            this.labelYourPublicKey.TabIndex = 4;
            this.labelYourPublicKey.Text = "Your Public Wallet Key:";
            // 
            // labelYourPrivateKey
            // 
            this.labelYourPrivateKey.AutoSize = true;
            this.labelYourPrivateKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelYourPrivateKey.Location = new System.Drawing.Point(12, 220);
            this.labelYourPrivateKey.Name = "labelYourPrivateKey";
            this.labelYourPrivateKey.Size = new System.Drawing.Size(151, 16);
            this.labelYourPrivateKey.TabIndex = 5;
            this.labelYourPrivateKey.Text = "Your Private Wallet Key:";
            // 
            // labelYourPinCode
            // 
            this.labelYourPinCode.AutoSize = true;
            this.labelYourPinCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelYourPinCode.Location = new System.Drawing.Point(12, 248);
            this.labelYourPinCode.Name = "labelYourPinCode";
            this.labelYourPinCode.Size = new System.Drawing.Size(97, 16);
            this.labelYourPinCode.TabIndex = 6;
            this.labelYourPinCode.Text = "Your Pin Code:";
            // 
            // buttonCopyWalletInformation
            // 
            this.buttonCopyWalletInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCopyWalletInformation.Location = new System.Drawing.Point(15, 281);
            this.buttonCopyWalletInformation.Name = "buttonCopyWalletInformation";
            this.buttonCopyWalletInformation.Size = new System.Drawing.Size(190, 52);
            this.buttonCopyWalletInformation.TabIndex = 7;
            this.buttonCopyWalletInformation.Text = "Copy my wallet informations";
            this.buttonCopyWalletInformation.UseVisualStyleBackColor = true;
            this.buttonCopyWalletInformation.Click += new System.EventHandler(this.ButtonCopyWalletInformation_Click);
            // 
            // labelCreateWalletInformation
            // 
            this.labelCreateWalletInformation.AutoSize = true;
            this.labelCreateWalletInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCreateWalletInformation.Location = new System.Drawing.Point(2, 9);
            this.labelCreateWalletInformation.Name = "labelCreateWalletInformation";
            this.labelCreateWalletInformation.Size = new System.Drawing.Size(815, 128);
            this.labelCreateWalletInformation.TabIndex = 11;
            this.labelCreateWalletInformation.Text = resources.GetString("labelCreateWalletInformation.Text");
            this.labelCreateWalletInformation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CreateWalletSuccessForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(820, 458);
            this.ControlBox = false;
            this.Controls.Add(this.labelCreateWalletInformation);
            this.Controls.Add(this.buttonCopyWalletInformation);
            this.Controls.Add(this.labelYourPinCode);
            this.Controls.Add(this.labelYourPrivateKey);
            this.Controls.Add(this.labelYourPublicKey);
            this.Controls.Add(this.buttonAcceptAndCloseWalletInformation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateWalletSuccessForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CreateWalletSuccessForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.CreateWalletSuccessForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonAcceptAndCloseWalletInformation;
        private System.Windows.Forms.Label labelYourPublicKey;
        private System.Windows.Forms.Label labelYourPrivateKey;
        private System.Windows.Forms.Label labelYourPinCode;
        private System.Windows.Forms.Button buttonCopyWalletInformation;
        private System.Windows.Forms.Label labelCreateWalletInformation;
    }
}