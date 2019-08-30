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
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewPeerList = new System.Windows.Forms.DataGridView();
            this.ColumnPeer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPeerStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPeerAction = new System.Windows.Forms.DataGridViewButtonColumn();
            this.checkBoxEnablePeerTrustSystem = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableProxyMode = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPeerList)).BeginInit();
            this.SuspendLayout();
            // 
            // radioButtonEnableSeedNodeSync
            // 
            this.radioButtonEnableSeedNodeSync.AutoSize = true;
            this.radioButtonEnableSeedNodeSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonEnableSeedNodeSync.Location = new System.Drawing.Point(20, 40);
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
            this.radioButtonEnablePublicRemoteNodeSync.Location = new System.Drawing.Point(20, 99);
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
            this.radioButtonEnableManualRemoteNodeSync.Location = new System.Drawing.Point(20, 195);
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
            this.labelNoticePrivateRemoteNode.Location = new System.Drawing.Point(17, 228);
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
            this.labelNoticePublicNodeInformation.Location = new System.Drawing.Point(17, 134);
            this.labelNoticePublicNodeInformation.Name = "labelNoticePublicNodeInformation";
            this.labelNoticePublicNodeInformation.Size = new System.Drawing.Size(482, 30);
            this.labelNoticePublicNodeInformation.TabIndex = 4;
            this.labelNoticePublicNodeInformation.Text = "The list of public remote node is provided by the seed node. \r\nIf no public remot" +
    "e node are available the seed node mode will be enable automaticaly.";
            this.labelNoticePublicNodeInformation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxRemoteNodeHost
            // 
            this.textBoxRemoteNodeHost.Location = new System.Drawing.Point(20, 311);
            this.textBoxRemoteNodeHost.Name = "textBoxRemoteNodeHost";
            this.textBoxRemoteNodeHost.Size = new System.Drawing.Size(309, 20);
            this.textBoxRemoteNodeHost.TabIndex = 5;
            // 
            // labelNoticeRemoteNodeHost
            // 
            this.labelNoticeRemoteNodeHost.AutoSize = true;
            this.labelNoticeRemoteNodeHost.Location = new System.Drawing.Point(17, 295);
            this.labelNoticeRemoteNodeHost.Name = "labelNoticeRemoteNodeHost";
            this.labelNoticeRemoteNodeHost.Size = new System.Drawing.Size(76, 13);
            this.labelNoticeRemoteNodeHost.TabIndex = 6;
            this.labelNoticeRemoteNodeHost.Text = "Hostname/IP: ";
            this.labelNoticeRemoteNodeHost.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonValidSetting
            // 
            this.buttonValidSetting.Location = new System.Drawing.Point(424, 375);
            this.buttonValidSetting.Name = "buttonValidSetting";
            this.buttonValidSetting.Size = new System.Drawing.Size(75, 20);
            this.buttonValidSetting.TabIndex = 7;
            this.buttonValidSetting.Text = "OK";
            this.buttonValidSetting.UseVisualStyleBackColor = true;
            this.buttonValidSetting.Click += new System.EventHandler(this.ButtonValidSetting_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(694, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 20);
            this.label1.TabIndex = 9;
            this.label1.Text = "Peer List:";
            // 
            // dataGridViewPeerList
            // 
            this.dataGridViewPeerList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPeerList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnPeer,
            this.ColumnPeerStatus,
            this.ColumnPeerAction});
            this.dataGridViewPeerList.Location = new System.Drawing.Point(535, 62);
            this.dataGridViewPeerList.Name = "dataGridViewPeerList";
            this.dataGridViewPeerList.Size = new System.Drawing.Size(420, 276);
            this.dataGridViewPeerList.TabIndex = 10;
            this.dataGridViewPeerList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewPeerList_CellContentClick);
            // 
            // ColumnPeer
            // 
            this.ColumnPeer.HeaderText = "Peer";
            this.ColumnPeer.Name = "ColumnPeer";
            this.ColumnPeer.Width = 140;
            // 
            // ColumnPeerStatus
            // 
            this.ColumnPeerStatus.HeaderText = "Status";
            this.ColumnPeerStatus.Name = "ColumnPeerStatus";
            this.ColumnPeerStatus.Width = 120;
            // 
            // ColumnPeerAction
            // 
            this.ColumnPeerAction.HeaderText = "Action";
            this.ColumnPeerAction.Name = "ColumnPeerAction";
            this.ColumnPeerAction.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnPeerAction.Text = "Remove";
            this.ColumnPeerAction.Width = 115;
            // 
            // checkBoxEnablePeerTrustSystem
            // 
            this.checkBoxEnablePeerTrustSystem.AutoSize = true;
            this.checkBoxEnablePeerTrustSystem.Location = new System.Drawing.Point(535, 344);
            this.checkBoxEnablePeerTrustSystem.Name = "checkBoxEnablePeerTrustSystem";
            this.checkBoxEnablePeerTrustSystem.Size = new System.Drawing.Size(148, 17);
            this.checkBoxEnablePeerTrustSystem.TabIndex = 11;
            this.checkBoxEnablePeerTrustSystem.Text = "Enable Peer Trust System";
            this.checkBoxEnablePeerTrustSystem.UseVisualStyleBackColor = true;
            this.checkBoxEnablePeerTrustSystem.CheckedChanged += new System.EventHandler(this.checkBoxEnablePeerTrustSystem_CheckedChanged);
            // 
            // checkBoxEnableProxyMode
            // 
            this.checkBoxEnableProxyMode.AutoSize = true;
            this.checkBoxEnableProxyMode.Location = new System.Drawing.Point(20, 344);
            this.checkBoxEnableProxyMode.Name = "checkBoxEnableProxyMode";
            this.checkBoxEnableProxyMode.Size = new System.Drawing.Size(393, 17);
            this.checkBoxEnableProxyMode.TabIndex = 12;
            this.checkBoxEnableProxyMode.Text = "Enable Proxy Mode - Connect to the network by a node instead of seed node.";
            this.checkBoxEnableProxyMode.UseVisualStyleBackColor = true;
            this.checkBoxEnableProxyMode.CheckedChanged += new System.EventHandler(this.checkBoxEnableProxyMode_CheckedChanged);
            this.checkBoxEnableProxyMode.Click += new System.EventHandler(this.checkBoxEnableProxyMode_Click);
            // 
            // RemoteNodeSettingWallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(971, 407);
            this.Controls.Add(this.checkBoxEnableProxyMode);
            this.Controls.Add(this.checkBoxEnablePeerTrustSystem);
            this.Controls.Add(this.dataGridViewPeerList);
            this.Controls.Add(this.label1);
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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPeerList)).EndInit();
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridViewPeerList;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPeer;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPeerStatus;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnPeerAction;
        private System.Windows.Forms.CheckBox checkBoxEnablePeerTrustSystem;
        private System.Windows.Forms.CheckBox checkBoxEnableProxyMode;
    }
}