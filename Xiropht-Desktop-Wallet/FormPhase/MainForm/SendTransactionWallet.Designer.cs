using System.Windows.Forms;

namespace Xiropht_Wallet.FormPhase.MainForm
{
    sealed partial class SendTransactionWallet
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
            this.labelWalletDestination = new System.Windows.Forms.Label();
            this.textBoxWalletDestination = new System.Windows.Forms.TextBox();
            this.labelSendTransaction = new System.Windows.Forms.Label();
            this.labelAmount = new System.Windows.Forms.Label();
            this.labelFeeTransaction = new System.Windows.Forms.Label();
            this.textBoxAmount = new System.Windows.Forms.TextBox();
            this.buttonSendTransaction = new System.Windows.Forms.Button();
            this.textBoxFee = new System.Windows.Forms.TextBox();
            this.textBoxTransactionTime = new System.Windows.Forms.TextBox();
            this.metroLabelEstimatedTimeReceived = new System.Windows.Forms.Label();
            this.checkBoxHideWalletAddress = new System.Windows.Forms.CheckBox();
            this.buttonFeeInformation = new System.Windows.Forms.Button();
            this.buttonEstimatedTimeInformation = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelWalletDestination
            // 
            this.labelWalletDestination.AutoSize = true;
            this.labelWalletDestination.BackColor = System.Drawing.Color.Transparent;
            this.labelWalletDestination.Location = new System.Drawing.Point(105, 156);
            this.labelWalletDestination.Name = "labelWalletDestination";
            this.labelWalletDestination.Size = new System.Drawing.Size(115, 13);
            this.labelWalletDestination.TabIndex = 16;
            this.labelWalletDestination.Text = "Wallet Address Target:";
            // 
            // textBoxWalletDestination
            // 
            this.textBoxWalletDestination.Location = new System.Drawing.Point(298, 153);
            this.textBoxWalletDestination.Name = "textBoxWalletDestination";
            this.textBoxWalletDestination.Size = new System.Drawing.Size(313, 20);
            this.textBoxWalletDestination.TabIndex = 15;
            this.textBoxWalletDestination.TextChanged += new System.EventHandler(this.textBoxWalletDestination_TextChanged);
            // 
            // labelSendTransaction
            // 
            this.labelSendTransaction.AutoSize = true;
            this.labelSendTransaction.BackColor = System.Drawing.Color.Transparent;
            this.labelSendTransaction.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSendTransaction.Location = new System.Drawing.Point(335, 33);
            this.labelSendTransaction.Name = "labelSendTransaction";
            this.labelSendTransaction.Size = new System.Drawing.Size(131, 15);
            this.labelSendTransaction.TabIndex = 14;
            this.labelSendTransaction.Text = "Send a transaction:";
            // 
            // labelAmount
            // 
            this.labelAmount.AutoSize = true;
            this.labelAmount.BackColor = System.Drawing.Color.Transparent;
            this.labelAmount.Location = new System.Drawing.Point(105, 227);
            this.labelAmount.Name = "labelAmount";
            this.labelAmount.Size = new System.Drawing.Size(46, 13);
            this.labelAmount.TabIndex = 13;
            this.labelAmount.Text = "Amount:";
            // 
            // labelFeeTransaction
            // 
            this.labelFeeTransaction.AutoSize = true;
            this.labelFeeTransaction.BackColor = System.Drawing.Color.Transparent;
            this.labelFeeTransaction.Location = new System.Drawing.Point(105, 261);
            this.labelFeeTransaction.Name = "labelFeeTransaction";
            this.labelFeeTransaction.Size = new System.Drawing.Size(28, 13);
            this.labelFeeTransaction.TabIndex = 12;
            this.labelFeeTransaction.Text = "Fee:";
            // 
            // textBoxAmount
            // 
            this.textBoxAmount.Location = new System.Drawing.Point(309, 224);
            this.textBoxAmount.Name = "textBoxAmount";
            this.textBoxAmount.Size = new System.Drawing.Size(264, 20);
            this.textBoxAmount.TabIndex = 11;
            this.textBoxAmount.Text = "0.00000000";
            // 
            // buttonSendTransaction
            // 
            this.buttonSendTransaction.Location = new System.Drawing.Point(309, 410);
            this.buttonSendTransaction.Name = "buttonSendTransaction";
            this.buttonSendTransaction.Size = new System.Drawing.Size(189, 74);
            this.buttonSendTransaction.TabIndex = 10;
            this.buttonSendTransaction.Text = "Send Transaction";
            this.buttonSendTransaction.Click += new System.EventHandler(this.ButtonSendTransaction_ClickAsync);
            // 
            // textBoxFee
            // 
            this.textBoxFee.Location = new System.Drawing.Point(309, 258);
            this.textBoxFee.Name = "textBoxFee";
            this.textBoxFee.Size = new System.Drawing.Size(264, 20);
            this.textBoxFee.TabIndex = 9;
            this.textBoxFee.Text = "0.00001000";
            // 
            // textBoxTransactionTime
            // 
            this.textBoxTransactionTime.Location = new System.Drawing.Point(309, 292);
            this.textBoxTransactionTime.Name = "textBoxTransactionTime";
            this.textBoxTransactionTime.ReadOnly = true;
            this.textBoxTransactionTime.Size = new System.Drawing.Size(264, 20);
            this.textBoxTransactionTime.TabIndex = 18;
            this.textBoxTransactionTime.Text = "N/A";
            // 
            // metroLabelEstimatedTimeReceived
            // 
            this.metroLabelEstimatedTimeReceived.AutoSize = true;
            this.metroLabelEstimatedTimeReceived.BackColor = System.Drawing.Color.Transparent;
            this.metroLabelEstimatedTimeReceived.Location = new System.Drawing.Point(105, 295);
            this.metroLabelEstimatedTimeReceived.Name = "metroLabelEstimatedTimeReceived";
            this.metroLabelEstimatedTimeReceived.Size = new System.Drawing.Size(125, 13);
            this.metroLabelEstimatedTimeReceived.TabIndex = 19;
            this.metroLabelEstimatedTimeReceived.Text = "Estimated Receive Time:";
            // 
            // checkBoxHideWalletAddress
            // 
            this.checkBoxHideWalletAddress.AutoSize = true;
            this.checkBoxHideWalletAddress.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.checkBoxHideWalletAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxHideWalletAddress.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.checkBoxHideWalletAddress.Location = new System.Drawing.Point(250, 334);
            this.checkBoxHideWalletAddress.Name = "checkBoxHideWalletAddress";
            this.checkBoxHideWalletAddress.Size = new System.Drawing.Size(302, 19);
            this.checkBoxHideWalletAddress.TabIndex = 20;
            this.checkBoxHideWalletAddress.Text = "Option - Hide Your Wallet Address on receiver side.";
            this.checkBoxHideWalletAddress.UseVisualStyleBackColor = true;
            this.checkBoxHideWalletAddress.Click += new System.EventHandler(this.CheckBoxHideWalletAddress_Click);
            // 
            // buttonFeeInformation
            // 
            this.buttonFeeInformation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFeeInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonFeeInformation.Location = new System.Drawing.Point(578, 257);
            this.buttonFeeInformation.Name = "buttonFeeInformation";
            this.buttonFeeInformation.Size = new System.Drawing.Size(22, 22);
            this.buttonFeeInformation.TabIndex = 24;
            this.buttonFeeInformation.Text = "?";
            this.buttonFeeInformation.UseVisualStyleBackColor = true;
            this.buttonFeeInformation.Click += new System.EventHandler(this.buttonFeeInformation_Click);
            this.buttonFeeInformation.MouseHover += new System.EventHandler(this.buttonFeeInformation_MouseHover);
            // 
            // buttonEstimatedTimeInformation
            // 
            this.buttonEstimatedTimeInformation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEstimatedTimeInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEstimatedTimeInformation.Location = new System.Drawing.Point(578, 292);
            this.buttonEstimatedTimeInformation.Name = "buttonEstimatedTimeInformation";
            this.buttonEstimatedTimeInformation.Size = new System.Drawing.Size(22, 22);
            this.buttonEstimatedTimeInformation.TabIndex = 25;
            this.buttonEstimatedTimeInformation.Text = "?";
            this.buttonEstimatedTimeInformation.UseVisualStyleBackColor = true;
            this.buttonEstimatedTimeInformation.Click += new System.EventHandler(this.buttonEstimatedTimeInformation_Click);
            this.buttonEstimatedTimeInformation.MouseHover += new System.EventHandler(this.buttonEstimatedTimeInformation_MouseHover);
            // 
            // SendTransactionWallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 496);
            this.ControlBox = false;
            this.Controls.Add(this.buttonEstimatedTimeInformation);
            this.Controls.Add(this.buttonFeeInformation);
            this.Controls.Add(this.checkBoxHideWalletAddress);
            this.Controls.Add(this.metroLabelEstimatedTimeReceived);
            this.Controls.Add(this.textBoxTransactionTime);
            this.Controls.Add(this.labelWalletDestination);
            this.Controls.Add(this.textBoxWalletDestination);
            this.Controls.Add(this.labelSendTransaction);
            this.Controls.Add(this.labelAmount);
            this.Controls.Add(this.labelFeeTransaction);
            this.Controls.Add(this.textBoxAmount);
            this.Controls.Add(this.buttonSendTransaction);
            this.Controls.Add(this.textBoxFee);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SendTransactionWallet";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SendTransaction";
            this.Load += new System.EventHandler(this.SendTransaction_Load);
            this.Resize += new System.EventHandler(this.SendTransaction_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private TextBox textBoxWalletDestination;
        private TextBox textBoxTransactionTime;
        public TextBox textBoxAmount;
        public TextBox textBoxFee;
        private Button buttonFeeInformation;
        private Button buttonEstimatedTimeInformation;
        public Label labelWalletDestination;
        public Label labelSendTransaction;
        public Label labelAmount;
        public Label labelFeeTransaction;
        public Button buttonSendTransaction;
        public Label metroLabelEstimatedTimeReceived;
        public CheckBox checkBoxHideWalletAddress;
    }
}