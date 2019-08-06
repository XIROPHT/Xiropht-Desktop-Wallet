namespace Xiropht_Wallet.FormPhase.MainForm
{
    partial class RestoreWallet
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
#if WINDOWS
            this.textBoxPathWallet = new MetroFramework.Controls.MetroTextBox();
#else
            this.textBoxPathWallet = new System.Windows.Forms.TextBox();
#endif
            this.buttonSearchNewWalletFile = new MetroFramework.Controls.MetroButton();
            this.labelTextRestore = new System.Windows.Forms.Label();
            this.labelCreateSelectSavingPathWallet = new System.Windows.Forms.Label();
            this.textBoxPrivateKey = new System.Windows.Forms.TextBox();
            this.labelPrivateKey = new System.Windows.Forms.Label();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.buttonRestoreYourWallet = new System.Windows.Forms.Button();
            this.pictureBoxPasswordStatus = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPasswordStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxPathWallet
            // 
            // 
            // 
            // 
#if WINDOWS
            this.textBoxPathWallet.CustomButton.Image = null;
            this.textBoxPathWallet.CustomButton.Location = new System.Drawing.Point(225, 2);
            this.textBoxPathWallet.CustomButton.Name = "";
            this.textBoxPathWallet.CustomButton.Size = new System.Drawing.Size(15, 15);
            this.textBoxPathWallet.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxPathWallet.CustomButton.TabIndex = 1;
            this.textBoxPathWallet.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textBoxPathWallet.CustomButton.UseSelectable = true;
            this.textBoxPathWallet.CustomButton.Visible = false;
#endif
            this.textBoxPathWallet.Lines = new string[0];
            this.textBoxPathWallet.Location = new System.Drawing.Point(247, 169);
            this.textBoxPathWallet.MaxLength = 32767;
            this.textBoxPathWallet.Name = "textBoxPathWallet";
            this.textBoxPathWallet.PasswordChar = '\0';
            this.textBoxPathWallet.ReadOnly = true;
            this.textBoxPathWallet.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textBoxPathWallet.SelectedText = "";
            this.textBoxPathWallet.SelectionLength = 0;
            this.textBoxPathWallet.SelectionStart = 0;
            this.textBoxPathWallet.ShortcutsEnabled = true;
            this.textBoxPathWallet.Size = new System.Drawing.Size(243, 20);
            this.textBoxPathWallet.TabIndex = 25;
#if WINDOWS
            this.textBoxPathWallet.UseSelectable = true;
            this.textBoxPathWallet.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textBoxPathWallet.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
#endif
            // 
            // buttonSearchNewWalletFile
            // 
            this.buttonSearchNewWalletFile.Location = new System.Drawing.Point(496, 168);
            this.buttonSearchNewWalletFile.Name = "buttonSearchNewWalletFile";
            this.buttonSearchNewWalletFile.Size = new System.Drawing.Size(27, 22);
            this.buttonSearchNewWalletFile.TabIndex = 23;
            this.buttonSearchNewWalletFile.Text = "...";
#if WINDOWS
            this.buttonSearchNewWalletFile.UseSelectable = true;
#endif
            this.buttonSearchNewWalletFile.Click += new System.EventHandler(this.buttonSearchNewWalletFile_Click);
            // 
            // labelTextRestore
            // 
            this.labelTextRestore.AutoSize = true;
            this.labelTextRestore.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTextRestore.Location = new System.Drawing.Point(312, 27);
            this.labelTextRestore.Name = "labelTextRestore";
            this.labelTextRestore.Size = new System.Drawing.Size(120, 13);
            this.labelTextRestore.TabIndex = 0;
            this.labelTextRestore.Text = "Restore your wallet:";
            // 
            // labelCreateSelectSavingPathWallet
            // 
            this.labelCreateSelectSavingPathWallet.AutoSize = true;
            this.labelCreateSelectSavingPathWallet.Location = new System.Drawing.Point(253, 151);
            this.labelCreateSelectSavingPathWallet.Name = "labelCreateSelectSavingPathWallet";
            this.labelCreateSelectSavingPathWallet.Size = new System.Drawing.Size(228, 13);
            this.labelCreateSelectSavingPathWallet.TabIndex = 24;
            this.labelCreateSelectSavingPathWallet.Text = "Select the path directory for restore your wallet:";
            // 
            // textBoxPrivateKey
            // 
            this.textBoxPrivateKey.Location = new System.Drawing.Point(247, 227);
            this.textBoxPrivateKey.Name = "textBoxPrivateKey";
            this.textBoxPrivateKey.PasswordChar = '*';
            this.textBoxPrivateKey.Size = new System.Drawing.Size(243, 20);
            this.textBoxPrivateKey.TabIndex = 26;
            // 
            // labelPrivateKey
            // 
            this.labelPrivateKey.AutoSize = true;
            this.labelPrivateKey.Location = new System.Drawing.Point(253, 208);
            this.labelPrivateKey.Name = "labelPrivateKey";
            this.labelPrivateKey.Size = new System.Drawing.Size(113, 13);
            this.labelPrivateKey.TabIndex = 27;
            this.labelPrivateKey.Text = "Write your private key:";
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(253, 273);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(136, 13);
            this.labelPassword.TabIndex = 29;
            this.labelPassword.Text = "Write your wallet password:";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(247, 292);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(243, 20);
            this.textBoxPassword.TabIndex = 28;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxPassword_TextChanged);
            this.textBoxPassword.Resize += new System.EventHandler(this.textBoxPassword_Resize);
            // 
            // buttonRestoreYourWallet
            // 
            this.buttonRestoreYourWallet.Location = new System.Drawing.Point(285, 332);
            this.buttonRestoreYourWallet.Name = "buttonRestoreYourWallet";
            this.buttonRestoreYourWallet.Size = new System.Drawing.Size(166, 44);
            this.buttonRestoreYourWallet.TabIndex = 30;
            this.buttonRestoreYourWallet.Text = "Restore";
            this.buttonRestoreYourWallet.Click += new System.EventHandler(this.buttonRestoreYourWallet_ClickAsync);
            // 
            // pictureBoxPasswordStatus
            // 
            this.pictureBoxPasswordStatus.BackgroundImage = global::Xiropht_Wallet.Properties.Resources.error;
            this.pictureBoxPasswordStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxPasswordStatus.Location = new System.Drawing.Point(496, 290);
            this.pictureBoxPasswordStatus.Name = "pictureBoxPasswordStatus";
            this.pictureBoxPasswordStatus.Size = new System.Drawing.Size(25, 25);
            this.pictureBoxPasswordStatus.TabIndex = 31;
            this.pictureBoxPasswordStatus.TabStop = false;
            // 
            // RestoreWallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(748, 447);
            this.Controls.Add(this.pictureBoxPasswordStatus);
            this.Controls.Add(this.buttonRestoreYourWallet);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.labelPrivateKey);
            this.Controls.Add(this.textBoxPrivateKey);
            this.Controls.Add(this.textBoxPathWallet);
            this.Controls.Add(this.labelCreateSelectSavingPathWallet);
            this.Controls.Add(this.buttonSearchNewWalletFile);
            this.Controls.Add(this.labelTextRestore);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RestoreWallet";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "RestoreWallet";
            this.Load += new System.EventHandler(this.RestoreWallet_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPasswordStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

#endregion

#if WINDOWS
        private MetroFramework.Controls.MetroTextBox textBoxPathWallet;
        private MetroFramework.Controls.MetroButton buttonSearchNewWalletFile;
#else
        private System.Windows.Forms.TextBox textBoxPathWallet;
        public System.Windows.Forms.Button buttonSearchNewWalletFile;
#endif
        public System.Windows.Forms.Label labelCreateSelectSavingPathWallet;
        private System.Windows.Forms.TextBox textBoxPrivateKey;
        public System.Windows.Forms.Label labelPrivateKey;
        public System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox textBoxPassword;
        public System.Windows.Forms.Button buttonRestoreYourWallet;
        public System.Windows.Forms.Label labelTextRestore;
        private System.Windows.Forms.PictureBox pictureBoxPasswordStatus;
    }
}