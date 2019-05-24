namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    partial class FirstStartWallet
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
            this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
            this.labelWelcomeText = new System.Windows.Forms.Label();
            this.buttonEndSetting = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBoxLanguage
            // 
            this.comboBoxLanguage.FormattingEnabled = true;
            this.comboBoxLanguage.Location = new System.Drawing.Point(204, 104);
            this.comboBoxLanguage.Name = "comboBoxLanguage";
            this.comboBoxLanguage.Size = new System.Drawing.Size(351, 21);
            this.comboBoxLanguage.TabIndex = 0;
            this.comboBoxLanguage.SelectedIndexChanged += new System.EventHandler(this.comboBoxLanguage_SelectedIndexChanged);
            // 
            // labelWelcomeText
            // 
            this.labelWelcomeText.AutoSize = true;
            this.labelWelcomeText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWelcomeText.Location = new System.Drawing.Point(90, 31);
            this.labelWelcomeText.Name = "labelWelcomeText";
            this.labelWelcomeText.Size = new System.Drawing.Size(576, 40);
            this.labelWelcomeText.TabIndex = 1;
            this.labelWelcomeText.Text = "Welcome on Xiropht, it seems this is your first running of the gui wallet.\r\nPleas" +
    "e select your language:";
            this.labelWelcomeText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonEndSetting
            // 
            this.buttonEndSetting.Location = new System.Drawing.Point(287, 162);
            this.buttonEndSetting.Name = "buttonEndSetting";
            this.buttonEndSetting.Size = new System.Drawing.Size(197, 38);
            this.buttonEndSetting.TabIndex = 2;
            this.buttonEndSetting.Text = "OK";
            this.buttonEndSetting.UseVisualStyleBackColor = true;
            this.buttonEndSetting.Click += new System.EventHandler(this.buttonEndSetting_Click);
            // 
            // FirstStartWallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 222);
            this.Controls.Add(this.buttonEndSetting);
            this.Controls.Add(this.labelWelcomeText);
            this.Controls.Add(this.comboBoxLanguage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimizeBox = false;
            this.Name = "FirstStartWallet";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FirstStartWallet";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxLanguage;
        private System.Windows.Forms.Label labelWelcomeText;
        private System.Windows.Forms.Button buttonEndSetting;
    }
}