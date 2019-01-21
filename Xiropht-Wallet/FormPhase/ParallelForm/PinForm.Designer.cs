namespace Xiropht_Wallet.FormPhase.ParallelForm
{
    partial class PinForm
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
            this.textBoxPinCode = new System.Windows.Forms.TextBox();
            this.buttonSendPinCode = new System.Windows.Forms.Button();
            this.labelNoticePinCode = new System.Windows.Forms.Label();
            this.buttonNumberZero = new System.Windows.Forms.Button();
            this.buttonNumberOne = new System.Windows.Forms.Button();
            this.buttonNumberTwo = new System.Windows.Forms.Button();
            this.buttonNumberThree = new System.Windows.Forms.Button();
            this.buttonNumberFour = new System.Windows.Forms.Button();
            this.buttonNumberFive = new System.Windows.Forms.Button();
            this.buttonNumberSix = new System.Windows.Forms.Button();
            this.buttonNumberSeven = new System.Windows.Forms.Button();
            this.buttonNumberEight = new System.Windows.Forms.Button();
            this.buttonNumberNine = new System.Windows.Forms.Button();
            this.buttonNumberEleven = new System.Windows.Forms.Button();
            this.buttonNumberTen = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxPinCode
            // 
            this.textBoxPinCode.Location = new System.Drawing.Point(76, 93);
            this.textBoxPinCode.Name = "textBoxPinCode";
            this.textBoxPinCode.PasswordChar = '*';
            this.textBoxPinCode.ReadOnly = true;
            this.textBoxPinCode.Size = new System.Drawing.Size(234, 20);
            this.textBoxPinCode.TabIndex = 0;
            this.textBoxPinCode.TextChanged += new System.EventHandler(this.textBoxPinCode_TextChangedAsync);
            // 
            // buttonSendPinCode
            // 
            this.buttonSendPinCode.Location = new System.Drawing.Point(136, 287);
            this.buttonSendPinCode.Name = "buttonSendPinCode";
            this.buttonSendPinCode.Size = new System.Drawing.Size(114, 25);
            this.buttonSendPinCode.TabIndex = 1;
            this.buttonSendPinCode.Text = "OK";
            this.buttonSendPinCode.Click += new System.EventHandler(this.ButtonSendPinCode_ClickAsync);
            // 
            // labelNoticePinCode
            // 
            this.labelNoticePinCode.AutoSize = true;
            this.labelNoticePinCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNoticePinCode.Location = new System.Drawing.Point(25, 9);
            this.labelNoticePinCode.Name = "labelNoticePinCode";
            this.labelNoticePinCode.Size = new System.Drawing.Size(330, 30);
            this.labelNoticePinCode.TabIndex = 2;
            this.labelNoticePinCode.Text = "The blockchain ask your pin code.\r\nYou need to write it for continue to use your " +
    "wallet:";
            this.labelNoticePinCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonNumberZero
            // 
            this.buttonNumberZero.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNumberZero.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNumberZero.Location = new System.Drawing.Point(76, 132);
            this.buttonNumberZero.Name = "buttonNumberZero";
            this.buttonNumberZero.Size = new System.Drawing.Size(54, 40);
            this.buttonNumberZero.TabIndex = 3;
            this.buttonNumberZero.UseVisualStyleBackColor = true;
            this.buttonNumberZero.Click += new System.EventHandler(this.buttonNumberZero_Click);
            // 
            // buttonNumberOne
            // 
            this.buttonNumberOne.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNumberOne.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNumberOne.Location = new System.Drawing.Point(136, 132);
            this.buttonNumberOne.Name = "buttonNumberOne";
            this.buttonNumberOne.Size = new System.Drawing.Size(54, 40);
            this.buttonNumberOne.TabIndex = 4;
            this.buttonNumberOne.UseVisualStyleBackColor = true;
            this.buttonNumberOne.Click += new System.EventHandler(this.buttonNumberOne_Click);
            // 
            // buttonNumberTwo
            // 
            this.buttonNumberTwo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNumberTwo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNumberTwo.Location = new System.Drawing.Point(196, 132);
            this.buttonNumberTwo.Name = "buttonNumberTwo";
            this.buttonNumberTwo.Size = new System.Drawing.Size(54, 40);
            this.buttonNumberTwo.TabIndex = 5;
            this.buttonNumberTwo.UseVisualStyleBackColor = true;
            this.buttonNumberTwo.Click += new System.EventHandler(this.buttonNumberTwo_Click);
            // 
            // buttonNumberThree
            // 
            this.buttonNumberThree.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNumberThree.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNumberThree.Location = new System.Drawing.Point(256, 132);
            this.buttonNumberThree.Name = "buttonNumberThree";
            this.buttonNumberThree.Size = new System.Drawing.Size(54, 40);
            this.buttonNumberThree.TabIndex = 6;
            this.buttonNumberThree.UseVisualStyleBackColor = true;
            this.buttonNumberThree.Click += new System.EventHandler(this.buttonNumberThree_Click);
            // 
            // buttonNumberFour
            // 
            this.buttonNumberFour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNumberFour.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNumberFour.Location = new System.Drawing.Point(76, 178);
            this.buttonNumberFour.Name = "buttonNumberFour";
            this.buttonNumberFour.Size = new System.Drawing.Size(54, 40);
            this.buttonNumberFour.TabIndex = 7;
            this.buttonNumberFour.UseVisualStyleBackColor = true;
            this.buttonNumberFour.Click += new System.EventHandler(this.buttonNumberFour_Click);
            // 
            // buttonNumberFive
            // 
            this.buttonNumberFive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNumberFive.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNumberFive.Location = new System.Drawing.Point(136, 178);
            this.buttonNumberFive.Name = "buttonNumberFive";
            this.buttonNumberFive.Size = new System.Drawing.Size(54, 40);
            this.buttonNumberFive.TabIndex = 8;
            this.buttonNumberFive.UseVisualStyleBackColor = true;
            this.buttonNumberFive.Click += new System.EventHandler(this.buttonNumberFive_Click);
            // 
            // buttonNumberSix
            // 
            this.buttonNumberSix.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNumberSix.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNumberSix.Location = new System.Drawing.Point(196, 178);
            this.buttonNumberSix.Name = "buttonNumberSix";
            this.buttonNumberSix.Size = new System.Drawing.Size(54, 40);
            this.buttonNumberSix.TabIndex = 9;
            this.buttonNumberSix.UseVisualStyleBackColor = true;
            this.buttonNumberSix.Click += new System.EventHandler(this.buttonNumberSix_Click);
            // 
            // buttonNumberSeven
            // 
            this.buttonNumberSeven.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNumberSeven.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNumberSeven.Location = new System.Drawing.Point(256, 178);
            this.buttonNumberSeven.Name = "buttonNumberSeven";
            this.buttonNumberSeven.Size = new System.Drawing.Size(54, 40);
            this.buttonNumberSeven.TabIndex = 10;
            this.buttonNumberSeven.UseVisualStyleBackColor = true;
            this.buttonNumberSeven.Click += new System.EventHandler(this.buttonNumberSeven_Click);
            // 
            // buttonNumberEight
            // 
            this.buttonNumberEight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNumberEight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNumberEight.Location = new System.Drawing.Point(76, 224);
            this.buttonNumberEight.Name = "buttonNumberEight";
            this.buttonNumberEight.Size = new System.Drawing.Size(54, 40);
            this.buttonNumberEight.TabIndex = 11;
            this.buttonNumberEight.UseVisualStyleBackColor = true;
            this.buttonNumberEight.Click += new System.EventHandler(this.buttonNumberEight_Click);
            // 
            // buttonNumberNine
            // 
            this.buttonNumberNine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNumberNine.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNumberNine.Location = new System.Drawing.Point(136, 224);
            this.buttonNumberNine.Name = "buttonNumberNine";
            this.buttonNumberNine.Size = new System.Drawing.Size(54, 40);
            this.buttonNumberNine.TabIndex = 12;
            this.buttonNumberNine.UseVisualStyleBackColor = true;
            this.buttonNumberNine.Click += new System.EventHandler(this.buttonNumberNine_Click);
            // 
            // buttonNumberEleven
            // 
            this.buttonNumberEleven.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNumberEleven.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNumberEleven.Location = new System.Drawing.Point(256, 224);
            this.buttonNumberEleven.Name = "buttonNumberEleven";
            this.buttonNumberEleven.Size = new System.Drawing.Size(54, 40);
            this.buttonNumberEleven.TabIndex = 13;
            this.buttonNumberEleven.UseVisualStyleBackColor = true;
            this.buttonNumberEleven.Click += new System.EventHandler(this.buttonNumberEleven_Click);
            // 
            // buttonNumberTen
            // 
            this.buttonNumberTen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNumberTen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNumberTen.Location = new System.Drawing.Point(196, 224);
            this.buttonNumberTen.Name = "buttonNumberTen";
            this.buttonNumberTen.Size = new System.Drawing.Size(54, 40);
            this.buttonNumberTen.TabIndex = 14;
            this.buttonNumberTen.UseVisualStyleBackColor = true;
            this.buttonNumberTen.Click += new System.EventHandler(this.buttonNumberTen_Click);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(316, 87);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(39, 30);
            this.button1.TabIndex = 15;
            this.button1.Text = "<";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // PinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(393, 321);
            this.ControlBox = false;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonNumberTen);
            this.Controls.Add(this.buttonNumberEleven);
            this.Controls.Add(this.buttonNumberNine);
            this.Controls.Add(this.buttonNumberEight);
            this.Controls.Add(this.buttonNumberSeven);
            this.Controls.Add(this.buttonNumberSix);
            this.Controls.Add(this.buttonNumberFive);
            this.Controls.Add(this.buttonNumberFour);
            this.Controls.Add(this.buttonNumberThree);
            this.Controls.Add(this.buttonNumberTwo);
            this.Controls.Add(this.buttonNumberOne);
            this.Controls.Add(this.buttonNumberZero);
            this.Controls.Add(this.labelNoticePinCode);
            this.Controls.Add(this.buttonSendPinCode);
            this.Controls.Add(this.textBoxPinCode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PinForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PinForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.PinForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxPinCode;
        private System.Windows.Forms.Button buttonSendPinCode;
        public System.Windows.Forms.Label labelNoticePinCode;
        private System.Windows.Forms.Button buttonNumberZero;
        private System.Windows.Forms.Button buttonNumberOne;
        private System.Windows.Forms.Button buttonNumberTwo;
        private System.Windows.Forms.Button buttonNumberThree;
        private System.Windows.Forms.Button buttonNumberFour;
        private System.Windows.Forms.Button buttonNumberFive;
        private System.Windows.Forms.Button buttonNumberSix;
        private System.Windows.Forms.Button buttonNumberSeven;
        private System.Windows.Forms.Button buttonNumberEight;
        private System.Windows.Forms.Button buttonNumberNine;
        private System.Windows.Forms.Button buttonNumberEleven;
        private System.Windows.Forms.Button buttonNumberTen;
        private System.Windows.Forms.Button button1;
    }
}