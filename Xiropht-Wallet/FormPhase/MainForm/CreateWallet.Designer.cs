using System.Windows.Forms;

namespace Xiropht_Wallet.FormPhase.MainForm
{
    partial class CreateWallet
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
            this.textBoxPathWallet = new MetroFramework.Controls.MetroTextBox();
            this.labelCreateSelectSavingPathWallet = new System.Windows.Forms.Label();
            this.buttonSearchNewWalletFile = new MetroFramework.Controls.MetroButton();
            this.buttonCreateYourWallet = new MetroFramework.Controls.MetroButton();
            this.labelCreateSelectWalletPassword = new System.Windows.Forms.Label();
            this.textBoxSelectWalletPassword = new MetroFramework.Controls.MetroTextBox();
            this.labelCreateYourWallet = new System.Windows.Forms.Label();
            this.labelCreateNoticePasswordRequirement = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxPathWallet
            // 
            // 
            // 
            // 
            this.textBoxPathWallet.CustomButton.Image = null;
            this.textBoxPathWallet.CustomButton.Location = new System.Drawing.Point(225, 2);
            this.textBoxPathWallet.CustomButton.Name = "";
            this.textBoxPathWallet.CustomButton.Size = new System.Drawing.Size(15, 15);
            this.textBoxPathWallet.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxPathWallet.CustomButton.TabIndex = 1;
            this.textBoxPathWallet.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textBoxPathWallet.CustomButton.UseSelectable = true;
            this.textBoxPathWallet.CustomButton.Visible = false;
            this.textBoxPathWallet.Lines = new string[0];
            this.textBoxPathWallet.Location = new System.Drawing.Point(261, 255);
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
            this.textBoxPathWallet.TabIndex = 22;
            this.textBoxPathWallet.UseSelectable = true;
            this.textBoxPathWallet.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textBoxPathWallet.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // labelCreateSelectSavingPathWallet
            // 
            this.labelCreateSelectSavingPathWallet.AutoSize = true;
            this.labelCreateSelectSavingPathWallet.Location = new System.Drawing.Point(261, 237);
            this.labelCreateSelectSavingPathWallet.Name = "labelCreateSelectSavingPathWallet";
            this.labelCreateSelectSavingPathWallet.Size = new System.Drawing.Size(216, 13);
            this.labelCreateSelectSavingPathWallet.TabIndex = 21;
            this.labelCreateSelectSavingPathWallet.Text = "Select the path directory for your new wallet:";
            // 
            // buttonSearchNewWalletFile
            // 
            this.buttonSearchNewWalletFile.Location = new System.Drawing.Point(510, 253);
            this.buttonSearchNewWalletFile.Name = "buttonSearchNewWalletFile";
            this.buttonSearchNewWalletFile.Size = new System.Drawing.Size(27, 22);
            this.buttonSearchNewWalletFile.TabIndex = 20;
            this.buttonSearchNewWalletFile.Text = "...";
            this.buttonSearchNewWalletFile.UseSelectable = true;
            this.buttonSearchNewWalletFile.Click += new System.EventHandler(this.ButtonSearchNewWalletFile_Click);
            // 
            // buttonCreateYourWallet
            // 
            this.buttonCreateYourWallet.Location = new System.Drawing.Point(313, 358);
            this.buttonCreateYourWallet.Name = "buttonCreateYourWallet";
            this.buttonCreateYourWallet.Size = new System.Drawing.Size(133, 49);
            this.buttonCreateYourWallet.TabIndex = 18;
            this.buttonCreateYourWallet.Text = "Create my Wallet";
            this.buttonCreateYourWallet.UseSelectable = true;
            this.buttonCreateYourWallet.Click += new System.EventHandler(this.ButtonCreateYourWallet_Click);
            // 
            // labelCreateSelectWalletPassword
            // 
            this.labelCreateSelectWalletPassword.AutoSize = true;
            this.labelCreateSelectWalletPassword.Location = new System.Drawing.Point(261, 300);
            this.labelCreateSelectWalletPassword.Name = "labelCreateSelectWalletPassword";
            this.labelCreateSelectWalletPassword.Size = new System.Drawing.Size(136, 13);
            this.labelCreateSelectWalletPassword.TabIndex = 17;
            this.labelCreateSelectWalletPassword.Text = "Write your wallet password:";
            // 
            // textBoxSelectWalletPassword
            // 
            // 
            // 
            // 
            this.textBoxSelectWalletPassword.CustomButton.Image = null;
            this.textBoxSelectWalletPassword.CustomButton.Location = new System.Drawing.Point(225, 2);
            this.textBoxSelectWalletPassword.CustomButton.Name = "";
            this.textBoxSelectWalletPassword.CustomButton.Size = new System.Drawing.Size(15, 15);
            this.textBoxSelectWalletPassword.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxSelectWalletPassword.CustomButton.TabIndex = 1;
            this.textBoxSelectWalletPassword.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textBoxSelectWalletPassword.CustomButton.UseSelectable = true;
            this.textBoxSelectWalletPassword.CustomButton.Visible = false;
            this.textBoxSelectWalletPassword.Lines = new string[0];
            this.textBoxSelectWalletPassword.Location = new System.Drawing.Point(261, 318);
            this.textBoxSelectWalletPassword.MaxLength = 32767;
            this.textBoxSelectWalletPassword.Name = "textBoxSelectWalletPassword";
            this.textBoxSelectWalletPassword.PasswordChar = '*';
            this.textBoxSelectWalletPassword.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textBoxSelectWalletPassword.SelectedText = "";
            this.textBoxSelectWalletPassword.SelectionLength = 0;
            this.textBoxSelectWalletPassword.SelectionStart = 0;
            this.textBoxSelectWalletPassword.ShortcutsEnabled = true;
            this.textBoxSelectWalletPassword.Size = new System.Drawing.Size(243, 20);
            this.textBoxSelectWalletPassword.TabIndex = 16;
            this.textBoxSelectWalletPassword.UseSelectable = true;
            this.textBoxSelectWalletPassword.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textBoxSelectWalletPassword.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.textBoxSelectWalletPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxSelectWalletPassword_KeyDownAsync);
            // 
            // labelCreateYourWallet
            // 
            this.labelCreateYourWallet.AutoSize = true;
            this.labelCreateYourWallet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCreateYourWallet.Location = new System.Drawing.Point(333, 20);
            this.labelCreateYourWallet.Name = "labelCreateYourWallet";
            this.labelCreateYourWallet.Size = new System.Drawing.Size(113, 13);
            this.labelCreateYourWallet.TabIndex = 15;
            this.labelCreateYourWallet.Text = "Create your wallet:";
            // 
            // labelCreateNoticePasswordRequirement
            // 
            this.labelCreateNoticePasswordRequirement.AutoSize = true;
            this.labelCreateNoticePasswordRequirement.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCreateNoticePasswordRequirement.Location = new System.Drawing.Point(171, 174);
            this.labelCreateNoticePasswordRequirement.Name = "labelCreateNoticePasswordRequirement";
            this.labelCreateNoticePasswordRequirement.Size = new System.Drawing.Size(441, 13);
            this.labelCreateNoticePasswordRequirement.TabIndex = 23;
            this.labelCreateNoticePasswordRequirement.Text = "Password must be a least 8 characters and contain letters, numbers, and special c" +
    "haracters.";
            // 
            // CreateWallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 496);
            this.ControlBox = false;
            this.Controls.Add(this.labelCreateNoticePasswordRequirement);
            this.Controls.Add(this.textBoxPathWallet);
            this.Controls.Add(this.labelCreateSelectSavingPathWallet);
            this.Controls.Add(this.buttonSearchNewWalletFile);
            this.Controls.Add(this.buttonCreateYourWallet);
            this.Controls.Add(this.labelCreateSelectWalletPassword);
            this.Controls.Add(this.textBoxSelectWalletPassword);
            this.Controls.Add(this.labelCreateYourWallet);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateWallet";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "CreateWallet";
            this.Load += new System.EventHandler(this.CreateWallet_Load);
            this.Resize += new System.EventHandler(this.CreateWallet_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroTextBox textBoxPathWallet;
        private MetroFramework.Controls.MetroTextBox textBoxSelectWalletPassword;
        public Label labelCreateSelectSavingPathWallet;
        public MetroFramework.Controls.MetroButton buttonSearchNewWalletFile;
        public MetroFramework.Controls.MetroButton buttonCreateYourWallet;
        public Label labelCreateSelectWalletPassword;
        public Label labelCreateYourWallet;
        public Label labelCreateNoticePasswordRequirement;
    }
}