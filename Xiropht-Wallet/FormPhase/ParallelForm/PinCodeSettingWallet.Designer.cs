namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    partial class PinCodeSettingWallet
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
            this.textBoxPinCode = new System.Windows.Forms.TextBox();
            this.labelYourPinCode = new System.Windows.Forms.Label();
            this.textBoxWalletOldPassword = new System.Windows.Forms.TextBox();
            this.labelYourPassword = new System.Windows.Forms.Label();
            this.labelNoticePinCodeStatus = new System.Windows.Forms.Label();
            this.labelPinCodeStatus = new System.Windows.Forms.Label();
            this.buttonChangePinCodeStatus = new System.Windows.Forms.Button();
            this.labelNoticePinInformation = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxPinCode
            // 
            this.textBoxPinCode.Location = new System.Drawing.Point(178, 123);
            this.textBoxPinCode.Name = "textBoxPinCode";
            this.textBoxPinCode.PasswordChar = '*';
            this.textBoxPinCode.Size = new System.Drawing.Size(271, 20);
            this.textBoxPinCode.TabIndex = 7;
            // 
            // labelYourPinCode
            // 
            this.labelYourPinCode.AutoSize = true;
            this.labelYourPinCode.Location = new System.Drawing.Point(70, 130);
            this.labelYourPinCode.Name = "labelYourPinCode";
            this.labelYourPinCode.Size = new System.Drawing.Size(102, 13);
            this.labelYourPinCode.TabIndex = 6;
            this.labelYourPinCode.Text = "Write your pin code:";
            // 
            // textBoxWalletOldPassword
            // 
            this.textBoxWalletOldPassword.Location = new System.Drawing.Point(178, 83);
            this.textBoxWalletOldPassword.Name = "textBoxWalletOldPassword";
            this.textBoxWalletOldPassword.PasswordChar = '*';
            this.textBoxWalletOldPassword.Size = new System.Drawing.Size(271, 20);
            this.textBoxWalletOldPassword.TabIndex = 5;
            // 
            // labelYourPassword
            // 
            this.labelYourPassword.AutoSize = true;
            this.labelYourPassword.Location = new System.Drawing.Point(66, 90);
            this.labelYourPassword.Name = "labelYourPassword";
            this.labelYourPassword.Size = new System.Drawing.Size(106, 13);
            this.labelYourPassword.TabIndex = 4;
            this.labelYourPassword.Text = "Write your password:";
            // 
            // labelNoticePinCodeStatus
            // 
            this.labelNoticePinCodeStatus.AutoSize = true;
            this.labelNoticePinCodeStatus.Location = new System.Drawing.Point(175, 21);
            this.labelNoticePinCodeStatus.Name = "labelNoticePinCodeStatus";
            this.labelNoticePinCodeStatus.Size = new System.Drawing.Size(120, 13);
            this.labelNoticePinCodeStatus.TabIndex = 8;
            this.labelNoticePinCodeStatus.Text = "Your pin code status is: ";
            // 
            // labelPinCodeStatus
            // 
            this.labelPinCodeStatus.AutoSize = true;
            this.labelPinCodeStatus.Location = new System.Drawing.Point(296, 21);
            this.labelPinCodeStatus.Name = "labelPinCodeStatus";
            this.labelPinCodeStatus.Size = new System.Drawing.Size(46, 13);
            this.labelPinCodeStatus.TabIndex = 9;
            this.labelPinCodeStatus.Text = "Enabled";
            // 
            // buttonChangePinCodeStatus
            // 
            this.buttonChangePinCodeStatus.Location = new System.Drawing.Point(209, 179);
            this.buttonChangePinCodeStatus.Name = "buttonChangePinCodeStatus";
            this.buttonChangePinCodeStatus.Size = new System.Drawing.Size(133, 23);
            this.buttonChangePinCodeStatus.TabIndex = 10;
            this.buttonChangePinCodeStatus.Text = "Disable";
            this.buttonChangePinCodeStatus.UseVisualStyleBackColor = true;
            this.buttonChangePinCodeStatus.Click += new System.EventHandler(this.ButtonChangePinCodeStatus_Click);
            // 
            // labelNoticePinInformation
            // 
            this.labelNoticePinInformation.AutoSize = true;
            this.labelNoticePinInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNoticePinInformation.Location = new System.Drawing.Point(12, 49);
            this.labelNoticePinInformation.Name = "labelNoticePinInformation";
            this.labelNoticePinInformation.Size = new System.Drawing.Size(534, 13);
            this.labelNoticePinInformation.TabIndex = 11;
            this.labelNoticePinInformation.Text = "Remember, your setting is available only for your wallet, each wallet setting are" +
    " independant.";
            // 
            // PinCodeSettingWallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 206);
            this.Controls.Add(this.labelNoticePinInformation);
            this.Controls.Add(this.buttonChangePinCodeStatus);
            this.Controls.Add(this.labelPinCodeStatus);
            this.Controls.Add(this.labelNoticePinCodeStatus);
            this.Controls.Add(this.textBoxPinCode);
            this.Controls.Add(this.labelYourPinCode);
            this.Controls.Add(this.textBoxWalletOldPassword);
            this.Controls.Add(this.labelYourPassword);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PinCodeSettingWallet";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Xiropht - Change Pin Code Status";
            this.Load += new System.EventHandler(this.PinCodeSetting_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxPinCode;
        private System.Windows.Forms.Label labelYourPinCode;
        private System.Windows.Forms.TextBox textBoxWalletOldPassword;
        private System.Windows.Forms.Label labelYourPassword;
        private System.Windows.Forms.Label labelNoticePinCodeStatus;
        private System.Windows.Forms.Label labelPinCodeStatus;
        private System.Windows.Forms.Button buttonChangePinCodeStatus;
        private System.Windows.Forms.Label labelNoticePinInformation;
    }
}