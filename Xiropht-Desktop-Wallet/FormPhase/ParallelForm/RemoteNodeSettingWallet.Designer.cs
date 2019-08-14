namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    partial class RemoteNodeSettingWallet : System.Windows.Forms.Form
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
            this.radioButtonEnableSeedNodeSync = new System.Windows.Forms.RadioButton();
            this.radioButtonEnablePublicRemoteNodeSync = new System.Windows.Forms.RadioButton();
            this.radioButtonEnableManualRemoteNodeSync = new System.Windows.Forms.RadioButton();
            this.labelNoticePrivateRemoteNode = new System.Windows.Forms.Label();
            this.labelNoticePublicNodeInformation = new System.Windows.Forms.Label();
            this.textBoxRemoteNodeHost = new System.Windows.Forms.TextBox();
            this.labelNoticeRemoteNodeHost = new System.Windows.Forms.Label();
            this.buttonValidSetting = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // radioButtonEnableSeedNodeSync
            // 
            this.radioButtonEnableSeedNodeSync.AutoSize = true;
            this.radioButtonEnableSeedNodeSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonEnableSeedNodeSync.Location = new System.Drawing.Point(83, 47);
            this.radioButtonEnableSeedNodeSync.Name = "radioButtonEnableSeedNodeSync";
            this.radioButtonEnableSeedNodeSync.Size = new System.Drawing.Size(391, 20);
            this.radioButtonEnableSeedNodeSync.TabIndex = 0;
            this.radioButtonEnableSeedNodeSync.TabStop = true;
            this.radioButtonEnableSeedNodeSync.Text = "Use seed node network for sync your wallet. [Recommended]";
            this.radioButtonEnableSeedNodeSync.UseVisualStyleBackColor = true;
            this.radioButtonEnableSeedNodeSync.Click += new System.EventHandler(this.RadioButtonEnableSeedNodeSync_Click);
            // 
            // radioButtonEnablePublicRemoteNodeSync
            // 
            this.radioButtonEnablePublicRemoteNodeSync.AutoSize = true;
            this.radioButtonEnablePublicRemoteNodeSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonEnablePublicRemoteNodeSync.Location = new System.Drawing.Point(83, 122);
            this.radioButtonEnablePublicRemoteNodeSync.Name = "radioButtonEnablePublicRemoteNodeSync";
            this.radioButtonEnablePublicRemoteNodeSync.Size = new System.Drawing.Size(306, 20);
            this.radioButtonEnablePublicRemoteNodeSync.TabIndex = 1;
            this.radioButtonEnablePublicRemoteNodeSync.TabStop = true;
            this.radioButtonEnablePublicRemoteNodeSync.Text = "Use public remote node list for sync your wallet.";
            this.radioButtonEnablePublicRemoteNodeSync.UseVisualStyleBackColor = true;
            this.radioButtonEnablePublicRemoteNodeSync.Click += new System.EventHandler(this.RadioButtonEnablePublicRemoteNodeSync_Click);
            // 
            // radioButtonEnableManualRemoteNodeSync
            // 
            this.radioButtonEnableManualRemoteNodeSync.AutoSize = true;
            this.radioButtonEnableManualRemoteNodeSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonEnableManualRemoteNodeSync.Location = new System.Drawing.Point(83, 220);
            this.radioButtonEnableManualRemoteNodeSync.Name = "radioButtonEnableManualRemoteNodeSync";
            this.radioButtonEnableManualRemoteNodeSync.Size = new System.Drawing.Size(401, 20);
            this.radioButtonEnableManualRemoteNodeSync.TabIndex = 2;
            this.radioButtonEnableManualRemoteNodeSync.TabStop = true;
            this.radioButtonEnableManualRemoteNodeSync.Text = "Manual sync by using a remote node host [Not Recommended]";
            this.radioButtonEnableManualRemoteNodeSync.UseVisualStyleBackColor = true;
            this.radioButtonEnableManualRemoteNodeSync.Click += new System.EventHandler(this.RadioButtonEnableManualRemoteNodeSync_Click);
            // 
            // labelNoticePrivateRemoteNode
            // 
            this.labelNoticePrivateRemoteNode.AutoSize = true;
            this.labelNoticePrivateRemoteNode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNoticePrivateRemoteNode.Location = new System.Drawing.Point(80, 253);
            this.labelNoticePrivateRemoteNode.Name = "labelNoticePrivateRemoteNode";
            this.labelNoticePrivateRemoteNode.Size = new System.Drawing.Size(386, 15);
            this.labelNoticePrivateRemoteNode.TabIndex = 3;
            this.labelNoticePrivateRemoteNode.Text = "If you use this option, we recommend to use your private remote node.";
            this.labelNoticePrivateRemoteNode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelNoticePublicNodeInformation
            // 
            this.labelNoticePublicNodeInformation.AutoSize = true;
            this.labelNoticePublicNodeInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNoticePublicNodeInformation.Location = new System.Drawing.Point(80, 157);
            this.labelNoticePublicNodeInformation.Name = "labelNoticePublicNodeInformation";
            this.labelNoticePublicNodeInformation.Size = new System.Drawing.Size(482, 30);
            this.labelNoticePublicNodeInformation.TabIndex = 4;
            this.labelNoticePublicNodeInformation.Text = "The list of public remote node is provided by the seed node. \r\nIf no public remot" +
    "e node are available the seed node mode will be enable automaticaly.";
            this.labelNoticePublicNodeInformation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxRemoteNodeHost
            // 
            this.textBoxRemoteNodeHost.Location = new System.Drawing.Point(165, 316);
            this.textBoxRemoteNodeHost.Name = "textBoxRemoteNodeHost";
            this.textBoxRemoteNodeHost.Size = new System.Drawing.Size(309, 20);
            this.textBoxRemoteNodeHost.TabIndex = 5;
            // 
            // labelNoticeRemoteNodeHost
            // 
            this.labelNoticeRemoteNodeHost.AutoSize = true;
            this.labelNoticeRemoteNodeHost.Location = new System.Drawing.Point(12, 319);
            this.labelNoticeRemoteNodeHost.Name = "labelNoticeRemoteNodeHost";
            this.labelNoticeRemoteNodeHost.Size = new System.Drawing.Size(76, 13);
            this.labelNoticeRemoteNodeHost.TabIndex = 6;
            this.labelNoticeRemoteNodeHost.Text = "Hostname/IP: ";
            this.labelNoticeRemoteNodeHost.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonValidSetting
            // 
            this.buttonValidSetting.Location = new System.Drawing.Point(487, 316);
            this.buttonValidSetting.Name = "buttonValidSetting";
            this.buttonValidSetting.Size = new System.Drawing.Size(75, 20);
            this.buttonValidSetting.TabIndex = 7;
            this.buttonValidSetting.Text = "OK";
            this.buttonValidSetting.UseVisualStyleBackColor = true;
            this.buttonValidSetting.Click += new System.EventHandler(this.ButtonValidSetting_Click);
            // 
            // RemoteNodeSettingWallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 358);
            this.Controls.Add(this.buttonValidSetting);
            this.Controls.Add(this.labelNoticeRemoteNodeHost);
            this.Controls.Add(this.textBoxRemoteNodeHost);
            this.Controls.Add(this.labelNoticePublicNodeInformation);
            this.Controls.Add(this.labelNoticePrivateRemoteNode);
            this.Controls.Add(this.radioButtonEnableManualRemoteNodeSync);
            this.Controls.Add(this.radioButtonEnablePublicRemoteNodeSync);
            this.Controls.Add(this.radioButtonEnableSeedNodeSync);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RemoteNodeSettingWallet";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Xiropht - Sync Setting";
            this.Load += new System.EventHandler(this.RemoteNodeSetting_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonEnableSeedNodeSync;
        private System.Windows.Forms.RadioButton radioButtonEnablePublicRemoteNodeSync;
        private System.Windows.Forms.RadioButton radioButtonEnableManualRemoteNodeSync;
        private System.Windows.Forms.Label labelNoticePrivateRemoteNode;
        private System.Windows.Forms.Label labelNoticePublicNodeInformation;
        private System.Windows.Forms.TextBox textBoxRemoteNodeHost;
        private System.Windows.Forms.Label labelNoticeRemoteNodeHost;
        private System.Windows.Forms.Button buttonValidSetting;
    }
}