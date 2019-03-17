namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    partial class ChangeWalletPasswordWallet
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
            this.labelChangePasswordWallet = new System.Windows.Forms.Label();
            this.textBoxWalletOldPassword = new System.Windows.Forms.TextBox();
            this.textBoxPinCode = new System.Windows.Forms.TextBox();
            this.labelChangePasswordPinCode = new System.Windows.Forms.Label();
            this.textBoxWalletNewPassword = new System.Windows.Forms.TextBox();
            this.labelChangePasswordNewPassword = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelChangePasswordWallet
            // 
            this.labelChangePasswordWallet.AutoSize = true;
            this.labelChangePasswordWallet.Location = new System.Drawing.Point(12, 68);
            this.labelChangePasswordWallet.Name = "labelChangePasswordWallet";
            this.labelChangePasswordWallet.Size = new System.Drawing.Size(123, 13);
            this.labelChangePasswordWallet.TabIndex = 0;
            this.labelChangePasswordWallet.Text = "Write your old password:";
            // 
            // textBoxWalletOldPassword
            // 
            this.textBoxWalletOldPassword.Location = new System.Drawing.Point(196, 65);
            this.textBoxWalletOldPassword.Name = "textBoxWalletOldPassword";
            this.textBoxWalletOldPassword.PasswordChar = '*';
            this.textBoxWalletOldPassword.Size = new System.Drawing.Size(211, 20);
            this.textBoxWalletOldPassword.TabIndex = 1;
            // 
            // textBoxPinCode
            // 
            this.textBoxPinCode.Location = new System.Drawing.Point(196, 105);
            this.textBoxPinCode.Name = "textBoxPinCode";
            this.textBoxPinCode.PasswordChar = '*';
            this.textBoxPinCode.Size = new System.Drawing.Size(211, 20);
            this.textBoxPinCode.TabIndex = 3;
            // 
            // labelChangePasswordPinCode
            // 
            this.labelChangePasswordPinCode.AutoSize = true;
            this.labelChangePasswordPinCode.Location = new System.Drawing.Point(12, 108);
            this.labelChangePasswordPinCode.Name = "labelChangePasswordPinCode";
            this.labelChangePasswordPinCode.Size = new System.Drawing.Size(102, 13);
            this.labelChangePasswordPinCode.TabIndex = 2;
            this.labelChangePasswordPinCode.Text = "Write your pin code:";
            // 
            // textBoxWalletNewPassword
            // 
            this.textBoxWalletNewPassword.Location = new System.Drawing.Point(196, 142);
            this.textBoxWalletNewPassword.Name = "textBoxWalletNewPassword";
            this.textBoxWalletNewPassword.PasswordChar = '*';
            this.textBoxWalletNewPassword.Size = new System.Drawing.Size(211, 20);
            this.textBoxWalletNewPassword.TabIndex = 5;
            // 
            // labelChangePasswordNewPassword
            // 
            this.labelChangePasswordNewPassword.AutoSize = true;
            this.labelChangePasswordNewPassword.Location = new System.Drawing.Point(12, 145);
            this.labelChangePasswordNewPassword.Name = "labelChangePasswordNewPassword";
            this.labelChangePasswordNewPassword.Size = new System.Drawing.Size(129, 13);
            this.labelChangePasswordNewPassword.TabIndex = 4;
            this.labelChangePasswordNewPassword.Text = "Write your new password:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(196, 196);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // ChangeWalletPasswordWallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 231);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxWalletNewPassword);
            this.Controls.Add(this.labelChangePasswordNewPassword);
            this.Controls.Add(this.textBoxPinCode);
            this.Controls.Add(this.labelChangePasswordPinCode);
            this.Controls.Add(this.textBoxWalletOldPassword);
            this.Controls.Add(this.labelChangePasswordWallet);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangeWalletPasswordWallet";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Xiropht - Change password wallet";
            this.Load += new System.EventHandler(this.ChangeWalletPassword_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelChangePasswordWallet;
        private System.Windows.Forms.TextBox textBoxWalletOldPassword;
        private System.Windows.Forms.TextBox textBoxPinCode;
        private System.Windows.Forms.Label labelChangePasswordPinCode;
        private System.Windows.Forms.TextBox textBoxWalletNewPassword;
        private System.Windows.Forms.Label labelChangePasswordNewPassword;
        private System.Windows.Forms.Button button1;
    }
}