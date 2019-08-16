namespace Xiropht_Wallet.FormPhase.MainForm
{
    partial class OpenWallet
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
            this.buttonSearchWalletFile = new System.Windows.Forms.Button();
            this.labelOpenYourWallet = new System.Windows.Forms.Label();
            this.buttonOpenYourWallet = new System.Windows.Forms.Button();
            this.textBoxPasswordWallet = new System.Windows.Forms.TextBox();
            this.labelWriteYourWalletPassword = new System.Windows.Forms.Label();
            this.labelOpenFileSelected = new System.Windows.Forms.Label();
            this.checkBoxEnableTokenMode = new System.Windows.Forms.CheckBox();
            this.buttonTokenNetworkHelp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonSearchWalletFile
            // 
            this.buttonSearchWalletFile.Location = new System.Drawing.Point(152, 177);
            this.buttonSearchWalletFile.Name = "buttonSearchWalletFile";
            this.buttonSearchWalletFile.Size = new System.Drawing.Size(106, 40);
            this.buttonSearchWalletFile.TabIndex = 11;
            this.buttonSearchWalletFile.Text = "Search";
            this.buttonSearchWalletFile.Click += new System.EventHandler(this.ButtonSearchWalletFile_Click);
            // 
            // labelOpenYourWallet
            // 
            this.labelOpenYourWallet.AutoSize = true;
            this.labelOpenYourWallet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOpenYourWallet.Location = new System.Drawing.Point(342, 19);
            this.labelOpenYourWallet.Name = "labelOpenYourWallet";
            this.labelOpenYourWallet.Size = new System.Drawing.Size(106, 13);
            this.labelOpenYourWallet.TabIndex = 10;
            this.labelOpenYourWallet.Text = "Open your wallet:";
            // 
            // buttonOpenYourWallet
            // 
            this.buttonOpenYourWallet.Location = new System.Drawing.Point(574, 285);
            this.buttonOpenYourWallet.Name = "buttonOpenYourWallet";
            this.buttonOpenYourWallet.Size = new System.Drawing.Size(61, 44);
            this.buttonOpenYourWallet.TabIndex = 9;
            this.buttonOpenYourWallet.Text = "OK";
            this.buttonOpenYourWallet.Click += new System.EventHandler(this.ButtonOpenYourWallet_Click);
            // 
            // textBoxPasswordWallet
            // 
            this.textBoxPasswordWallet.Location = new System.Drawing.Point(152, 298);
            this.textBoxPasswordWallet.Name = "textBoxPasswordWallet";
            this.textBoxPasswordWallet.PasswordChar = '*';
            this.textBoxPasswordWallet.Size = new System.Drawing.Size(416, 20);
            this.textBoxPasswordWallet.TabIndex = 8;
            this.textBoxPasswordWallet.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxPasswordWallet_KeyDownAsync);
            // 
            // labelWriteYourWalletPassword
            // 
            this.labelWriteYourWalletPassword.AutoSize = true;
            this.labelWriteYourWalletPassword.Location = new System.Drawing.Point(152, 276);
            this.labelWriteYourWalletPassword.Name = "labelWriteYourWalletPassword";
            this.labelWriteYourWalletPassword.Size = new System.Drawing.Size(106, 13);
            this.labelWriteYourWalletPassword.TabIndex = 12;
            this.labelWriteYourWalletPassword.Text = "Write your password:";
            // 
            // labelOpenFileSelected
            // 
            this.labelOpenFileSelected.AutoSize = true;
            this.labelOpenFileSelected.Location = new System.Drawing.Point(152, 240);
            this.labelOpenFileSelected.Name = "labelOpenFileSelected";
            this.labelOpenFileSelected.Size = new System.Drawing.Size(71, 13);
            this.labelOpenFileSelected.TabIndex = 13;
            this.labelOpenFileSelected.Text = "File Selected:";
            // 
            // checkBoxEnableTokenMode
            // 
            this.checkBoxEnableTokenMode.AutoSize = true;
            this.checkBoxEnableTokenMode.Location = new System.Drawing.Point(152, 335);
            this.checkBoxEnableTokenMode.Name = "checkBoxEnableTokenMode";
            this.checkBoxEnableTokenMode.Size = new System.Drawing.Size(158, 17);
            this.checkBoxEnableTokenMode.TabIndex = 14;
            this.checkBoxEnableTokenMode.Text = "Use Token Network Mode. ";
            this.checkBoxEnableTokenMode.UseVisualStyleBackColor = true;
            this.checkBoxEnableTokenMode.CheckedChanged += new System.EventHandler(this.checkBoxEnableTokenMode_CheckedChanged);
            // 
            // buttonTokenNetworkHelp
            // 
            this.buttonTokenNetworkHelp.Location = new System.Drawing.Point(306, 330);
            this.buttonTokenNetworkHelp.Name = "buttonTokenNetworkHelp";
            this.buttonTokenNetworkHelp.Size = new System.Drawing.Size(26, 25);
            this.buttonTokenNetworkHelp.TabIndex = 15;
            this.buttonTokenNetworkHelp.Text = "?";
            this.buttonTokenNetworkHelp.UseVisualStyleBackColor = true;
            this.buttonTokenNetworkHelp.Click += new System.EventHandler(this.buttonTokenNetworkHelp_Click);
            // 
            // OpenWallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 496);
            this.Controls.Add(this.buttonTokenNetworkHelp);
            this.Controls.Add(this.checkBoxEnableTokenMode);
            this.Controls.Add(this.labelOpenFileSelected);
            this.Controls.Add(this.labelWriteYourWalletPassword);
            this.Controls.Add(this.buttonSearchWalletFile);
            this.Controls.Add(this.labelOpenYourWallet);
            this.Controls.Add(this.buttonOpenYourWallet);
            this.Controls.Add(this.textBoxPasswordWallet);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OpenWallet";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "OpenWallet";
            this.Load += new System.EventHandler(this.OpenWallet_Load);
            this.Resize += new System.EventHandler(this.OpenWallet_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBoxPasswordWallet;
        public System.Windows.Forms.Button buttonSearchWalletFile;
        public System.Windows.Forms.Label labelOpenYourWallet;
        public System.Windows.Forms.Button buttonOpenYourWallet;
        public System.Windows.Forms.Label labelWriteYourWalletPassword;
        public System.Windows.Forms.Label labelOpenFileSelected;
        private System.Windows.Forms.CheckBox checkBoxEnableTokenMode;
        private System.Windows.Forms.Button buttonTokenNetworkHelp;
    }
}