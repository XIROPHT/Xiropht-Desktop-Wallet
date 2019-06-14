namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    partial class SearchWalletExplorer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchWalletExplorer));
            this.richTextBoxResearchResult = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // richTextBoxResearchResult
            // 
            this.richTextBoxResearchResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxResearchResult.Location = new System.Drawing.Point(12, 12);
            this.richTextBoxResearchResult.Name = "richTextBoxResearchResult";
            this.richTextBoxResearchResult.ReadOnly = true;
            this.richTextBoxResearchResult.Size = new System.Drawing.Size(960, 416);
            this.richTextBoxResearchResult.TabIndex = 0;
            this.richTextBoxResearchResult.Text = "";
            this.richTextBoxResearchResult.MouseClick += new System.Windows.Forms.MouseEventHandler(this.richTextBoxResearchResult_MouseClick);
            // 
            // SearchWalletExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 440);
            this.Controls.Add(this.richTextBoxResearchResult);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchWalletExplorer";
            this.ShowInTaskbar = false;
            this.Text = "Xiropht - Search result";
            this.Load += new System.EventHandler(this.SearchWalletExplorer_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxResearchResult;
    }
}