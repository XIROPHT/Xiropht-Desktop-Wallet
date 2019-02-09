namespace Xiropht_Wallet.FormPhase.MainForm
{
    partial class ContactWallet
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
            this.listViewExContact = new Xiropht_Wallet.FormPhase.MainForm.ListViewEx();
            this.buttonAddContact = new System.Windows.Forms.Button();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();

            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 200;
            // 
            // columnHeaderAddress
            // 
            this.columnHeaderAddress.Text = "Address";
            this.columnHeaderAddress.Width = 450;
            // 
            // columnHeaderAction
            // 
            this.columnHeaderAction.Text = "Action";
            this.columnHeaderAction.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderAction.Width = 128;

            // 
            // listViewExContact
            // 
            this.listViewExContact.AutoArrange = false;
            this.listViewExContact.FullRowSelect = true;
            this.listViewExContact.GridLines = true;
            this.listViewExContact.Location = new System.Drawing.Point(0, 0);
            this.listViewExContact.Name = "listViewExContact";
            this.listViewExContact.ShowGroups = false;
            this.listViewExContact.Size = new System.Drawing.Size(784, 436);
            this.listViewExContact.TabIndex = 0;
            this.listViewExContact.UseCompatibleStateImageBehavior = false;
            this.listViewExContact.View = System.Windows.Forms.View.Details;
            this.listViewExContact.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewExContact_MouseClick);
            this.listViewExContact.Columns.Add(columnHeaderName);
            this.listViewExContact.Columns.Add(columnHeaderAddress);
            this.listViewExContact.Columns.Add(columnHeaderAction);

            // 
            // buttonAddContact
            // 
            this.buttonAddContact.Location = new System.Drawing.Point(275, 442);
            this.buttonAddContact.Name = "buttonAddContact";
            this.buttonAddContact.Size = new System.Drawing.Size(219, 42);
            this.buttonAddContact.TabIndex = 1;
            this.buttonAddContact.Text = "Add Contact";
            this.buttonAddContact.UseVisualStyleBackColor = true;
            this.buttonAddContact.Click += new System.EventHandler(this.buttonAddContact_Click);

            // 
            // ContactWallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 496);
            this.ControlBox = false;
            this.Controls.Add(this.listViewExContact);
            this.Controls.Add(this.buttonAddContact);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ContactWallet";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "ContactWallet";
            this.Load += new System.EventHandler(this.ContactWallet_Load);
            this.Resize += new System.EventHandler(this.ContactWallet_Resize);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderAddress;
        public ListViewEx listViewExContact;
        private System.Windows.Forms.ColumnHeader columnHeaderAction;
        public System.Windows.Forms.Button buttonAddContact;
    }
}