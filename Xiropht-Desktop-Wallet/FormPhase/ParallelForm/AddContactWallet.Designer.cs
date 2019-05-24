namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    partial class AddContactWallet
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
            this.textBoxContactName = new System.Windows.Forms.TextBox();
            this.textBoxContactWalletAddress = new System.Windows.Forms.TextBox();
            this.buttonInsertContact = new System.Windows.Forms.Button();
            this.labelTextContactName = new System.Windows.Forms.Label();
            this.labelTextContactWalletAddress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxContactName
            // 
            this.textBoxContactName.Location = new System.Drawing.Point(278, 69);
            this.textBoxContactName.Name = "textBoxContactName";
            this.textBoxContactName.Size = new System.Drawing.Size(277, 20);
            this.textBoxContactName.TabIndex = 0;
            // 
            // textBoxContactWalletAddress
            // 
            this.textBoxContactWalletAddress.Location = new System.Drawing.Point(278, 121);
            this.textBoxContactWalletAddress.Name = "textBoxContactWalletAddress";
            this.textBoxContactWalletAddress.Size = new System.Drawing.Size(277, 20);
            this.textBoxContactWalletAddress.TabIndex = 1;
            // 
            // buttonInsertContact
            // 
            this.buttonInsertContact.Location = new System.Drawing.Point(170, 175);
            this.buttonInsertContact.Name = "buttonInsertContact";
            this.buttonInsertContact.Size = new System.Drawing.Size(205, 23);
            this.buttonInsertContact.TabIndex = 2;
            this.buttonInsertContact.Text = "OK";
            this.buttonInsertContact.UseVisualStyleBackColor = true;
            this.buttonInsertContact.Click += new System.EventHandler(this.buttonInsertContact_Click);
            // 
            // labelTextContactName
            // 
            this.labelTextContactName.AutoSize = true;
            this.labelTextContactName.Location = new System.Drawing.Point(12, 72);
            this.labelTextContactName.Name = "labelTextContactName";
            this.labelTextContactName.Size = new System.Drawing.Size(78, 13);
            this.labelTextContactName.TabIndex = 3;
            this.labelTextContactName.Text = "Contact Name:";
            // 
            // labelTextContactWalletAddress
            // 
            this.labelTextContactWalletAddress.AutoSize = true;
            this.labelTextContactWalletAddress.Location = new System.Drawing.Point(12, 124);
            this.labelTextContactWalletAddress.Name = "labelTextContactWalletAddress";
            this.labelTextContactWalletAddress.Size = new System.Drawing.Size(121, 13);
            this.labelTextContactWalletAddress.TabIndex = 4;
            this.labelTextContactWalletAddress.Text = "Contact Wallet Address:";
            // 
            // AddContactWallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 210);
            this.Controls.Add(this.labelTextContactWalletAddress);
            this.Controls.Add(this.labelTextContactName);
            this.Controls.Add(this.buttonInsertContact);
            this.Controls.Add(this.textBoxContactWalletAddress);
            this.Controls.Add(this.textBoxContactName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddContactWallet";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Xiropht - Add new contact";
            this.Load += new System.EventHandler(this.AddContactWallet_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxContactName;
        private System.Windows.Forms.TextBox textBoxContactWalletAddress;
        private System.Windows.Forms.Button buttonInsertContact;
        private System.Windows.Forms.Label labelTextContactName;
        private System.Windows.Forms.Label labelTextContactWalletAddress;
    }
}