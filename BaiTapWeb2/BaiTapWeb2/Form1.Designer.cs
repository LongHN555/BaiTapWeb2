namespace AESWinFormApp
{
    partial class Form1
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
            this.txtCustomKey = new System.Windows.Forms.TextBox();
            this.btnSaveKey = new System.Windows.Forms.Button();
            this.btnLoadKey = new System.Windows.Forms.Button();
            this.txtInputText = new System.Windows.Forms.TextBox();
            this.btnEncryptText = new System.Windows.Forms.Button();
            this.txtOutputText = new System.Windows.Forms.TextBox();
            this.btnEncryptFile = new System.Windows.Forms.Button();
            this.btnDecryptFile = new System.Windows.Forms.Button();
            this.btnDecryptText = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtCustomKey
            // 
            this.txtCustomKey.Location = new System.Drawing.Point(142, 60);
            this.txtCustomKey.Name = "txtCustomKey";
            this.txtCustomKey.Size = new System.Drawing.Size(290, 26);
            this.txtCustomKey.TabIndex = 0;
            // 
            // btnSaveKey
            // 
            this.btnSaveKey.Location = new System.Drawing.Point(448, 53);
            this.btnSaveKey.Name = "btnSaveKey";
            this.btnSaveKey.Size = new System.Drawing.Size(141, 41);
            this.btnSaveKey.TabIndex = 1;
            this.btnSaveKey.Text = "Tạo và lưu khóa";
            this.btnSaveKey.UseVisualStyleBackColor = true;
            this.btnSaveKey.Click += new System.EventHandler(this.btnSaveKey_Click);
            // 
            // btnLoadKey
            // 
            this.btnLoadKey.Location = new System.Drawing.Point(265, 112);
            this.btnLoadKey.Name = "btnLoadKey";
            this.btnLoadKey.Size = new System.Drawing.Size(166, 37);
            this.btnLoadKey.TabIndex = 2;
            this.btnLoadKey.Text = "Import khóa";
            this.btnLoadKey.UseVisualStyleBackColor = true;
            this.btnLoadKey.Click += new System.EventHandler(this.btnLoadKey_Click);
            // 
            // txtInputText
            // 
            this.txtInputText.Location = new System.Drawing.Point(142, 175);
            this.txtInputText.Name = "txtInputText";
            this.txtInputText.Size = new System.Drawing.Size(289, 26);
            this.txtInputText.TabIndex = 3;
            // 
            // btnEncryptText
            // 
            this.btnEncryptText.Location = new System.Drawing.Point(448, 227);
            this.btnEncryptText.Name = "btnEncryptText";
            this.btnEncryptText.Size = new System.Drawing.Size(140, 35);
            this.btnEncryptText.TabIndex = 4;
            this.btnEncryptText.Text = "Mã hóa văn bản";
            this.btnEncryptText.UseVisualStyleBackColor = true;
            this.btnEncryptText.Click += new System.EventHandler(this.btnEncryptText_Click);
            // 
            // txtOutputText
            // 
            this.txtOutputText.Location = new System.Drawing.Point(142, 231);
            this.txtOutputText.Name = "txtOutputText";
            this.txtOutputText.Size = new System.Drawing.Size(290, 26);
            this.txtOutputText.TabIndex = 5;
            // 
            // btnEncryptFile
            // 
            this.btnEncryptFile.Location = new System.Drawing.Point(142, 281);
            this.btnEncryptFile.Name = "btnEncryptFile";
            this.btnEncryptFile.Size = new System.Drawing.Size(177, 51);
            this.btnEncryptFile.TabIndex = 6;
            this.btnEncryptFile.Text = "Chọn và mã hóa file";
            this.btnEncryptFile.UseVisualStyleBackColor = true;
            this.btnEncryptFile.Click += new System.EventHandler(this.btnEncryptFile_Click);
            // 
            // btnDecryptFile
            // 
            this.btnDecryptFile.Location = new System.Drawing.Point(380, 281);
            this.btnDecryptFile.Name = "btnDecryptFile";
            this.btnDecryptFile.Size = new System.Drawing.Size(192, 51);
            this.btnDecryptFile.TabIndex = 7;
            this.btnDecryptFile.Text = "Giải mã và so sánh file";
            this.btnDecryptFile.UseVisualStyleBackColor = true;
            this.btnDecryptFile.Click += new System.EventHandler(this.btnDecryptFile_Click);
            // 
            // btnDecryptText
            // 
            this.btnDecryptText.Location = new System.Drawing.Point(610, 227);
            this.btnDecryptText.Name = "btnDecryptText";
            this.btnDecryptText.Size = new System.Drawing.Size(139, 34);
            this.btnDecryptText.TabIndex = 8;
            this.btnDecryptText.Text = "Giải mã văn bản";
            this.btnDecryptText.UseVisualStyleBackColor = true;
            this.btnDecryptText.Click += new System.EventHandler(this.btnDecryptText_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnDecryptText);
            this.Controls.Add(this.btnDecryptFile);
            this.Controls.Add(this.btnEncryptFile);
            this.Controls.Add(this.txtOutputText);
            this.Controls.Add(this.btnEncryptText);
            this.Controls.Add(this.txtInputText);
            this.Controls.Add(this.btnLoadKey);
            this.Controls.Add(this.btnSaveKey);
            this.Controls.Add(this.txtCustomKey);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCustomKey;
        private System.Windows.Forms.Button btnSaveKey;
        private System.Windows.Forms.Button btnLoadKey;
        private System.Windows.Forms.TextBox txtInputText;
        private System.Windows.Forms.Button btnEncryptText;
        private System.Windows.Forms.TextBox txtOutputText;
        private System.Windows.Forms.Button btnEncryptFile;
        private System.Windows.Forms.Button btnDecryptFile;
        private System.Windows.Forms.Button btnDecryptText;
    }
}

