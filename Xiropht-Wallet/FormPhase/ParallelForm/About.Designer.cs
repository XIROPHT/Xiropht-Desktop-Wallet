namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    partial class About
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
            this.labelWalletGuiVersion = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panelLanguageContributor = new System.Windows.Forms.Panel();
            this.labelLanguageContributors = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabelWebsite = new System.Windows.Forms.LinkLabel();
            this.panelLanguageContributor.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelWalletGuiVersion
            // 
            this.labelWalletGuiVersion.AutoSize = true;
            this.labelWalletGuiVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWalletGuiVersion.Location = new System.Drawing.Point(198, 23);
            this.labelWalletGuiVersion.Name = "labelWalletGuiVersion";
            this.labelWalletGuiVersion.Size = new System.Drawing.Size(296, 60);
            this.labelWalletGuiVersion.TabIndex = 0;
            this.labelWalletGuiVersion.Text = "GUI Wallet Xiropht v0.4.3.4b\r\n\r\nCopyright © 2018 Xiropht Developer\r\n";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 200);
            this.label2.TabIndex = 1;
            this.label2.Text = "Official testers: \r\n\r\n- DigitalTwister\r\n- Sniperviperman\r\n- AlpHA\r\n- Maxy86\r\n- Wo" +
    "lfierawr\r\n- Rashed\r\n- Sabrar\r\n- Xiropht";
            // 
            // panelLanguageContributor
            // 
            this.panelLanguageContributor.AutoScroll = true;
            this.panelLanguageContributor.Controls.Add(this.labelLanguageContributors);
            this.panelLanguageContributor.Location = new System.Drawing.Point(174, 155);
            this.panelLanguageContributor.Name = "panelLanguageContributor";
            this.panelLanguageContributor.Size = new System.Drawing.Size(462, 245);
            this.panelLanguageContributor.TabIndex = 2;
            // 
            // labelLanguageContributors
            // 
            this.labelLanguageContributors.AutoSize = true;
            this.labelLanguageContributors.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLanguageContributors.Location = new System.Drawing.Point(25, 33);
            this.labelLanguageContributors.Name = "labelLanguageContributors";
            this.labelLanguageContributors.Size = new System.Drawing.Size(0, 16);
            this.labelLanguageContributors.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(285, 132);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(233, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Language files contributors:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(170, 415);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Official Website: ";
            // 
            // linkLabelWebsite
            // 
            this.linkLabelWebsite.AutoSize = true;
            this.linkLabelWebsite.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelWebsite.Location = new System.Drawing.Point(308, 414);
            this.linkLabelWebsite.Name = "linkLabelWebsite";
            this.linkLabelWebsite.Size = new System.Drawing.Size(162, 20);
            this.linkLabelWebsite.TabIndex = 5;
            this.linkLabelWebsite.TabStop = true;
            this.linkLabelWebsite.Text = "https://xiropht.com/";
            this.linkLabelWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelWebsite_LinkClicked);
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 454);
            this.Controls.Add(this.linkLabelWebsite);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panelLanguageContributor);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelWalletGuiVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.Load += new System.EventHandler(this.About_Load);
            this.panelLanguageContributor.ResumeLayout(false);
            this.panelLanguageContributor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelWalletGuiVersion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelLanguageContributor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelLanguageContributors;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabelWebsite;
    }
}