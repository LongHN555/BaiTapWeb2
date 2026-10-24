using System.Windows.Forms;

namespace MahoaRSA
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnGenerateAndExportKeys = new Button();
            btnImportPublicKey = new Button();
            btnImportPrivateKey = new Button();
            txtKeyInfo = new TextBox();
            txtInputData = new TextBox();
            btnEncrypt = new Button();
            txtEncryptedBase64 = new TextBox();
            btnDecryptAndCompare = new Button();
            btnGenerateKeysFromSeed = new Button();
            txtCustomSeed = new TextBox();
            btnBenchmarkRSA = new Button();
            txtHashInput = new TextBox();
            txtHashOutput = new TextBox();
            SuspendLayout();
            // 
            // btnGenerateAndExportKeys
            // 
            btnGenerateAndExportKeys.Location = new Point(70, 79);
            btnGenerateAndExportKeys.Name = "btnGenerateAndExportKeys";
            btnGenerateAndExportKeys.Size = new Size(204, 68);
            btnGenerateAndExportKeys.TabIndex = 0;
            btnGenerateAndExportKeys.Text = "Sinh và xuất cặp khóa AUTO";
            btnGenerateAndExportKeys.UseVisualStyleBackColor = true;
            btnGenerateAndExportKeys.Click += btnGenerateAndExportKeys_Click;
            // 
            // btnImportPublicKey
            // 
            btnImportPublicKey.Location = new Point(100, 153);
            btnImportPublicKey.Name = "btnImportPublicKey";
            btnImportPublicKey.Size = new Size(174, 38);
            btnImportPublicKey.TabIndex = 1;
            btnImportPublicKey.Text = "Import public key";
            btnImportPublicKey.UseVisualStyleBackColor = true;
            btnImportPublicKey.Click += btnImportPublicKey_Click;
            // 
            // btnImportPrivateKey
            // 
            btnImportPrivateKey.Location = new Point(103, 207);
            btnImportPrivateKey.Name = "btnImportPrivateKey";
            btnImportPrivateKey.Size = new Size(174, 38);
            btnImportPrivateKey.TabIndex = 2;
            btnImportPrivateKey.Text = "Import private key";
            btnImportPrivateKey.UseVisualStyleBackColor = true;
            btnImportPrivateKey.Click += btnImportPrivateKey_Click;
            // 
            // txtKeyInfo
            // 
            txtKeyInfo.Location = new Point(348, 80);
            txtKeyInfo.Multiline = true;
            txtKeyInfo.Name = "txtKeyInfo";
            txtKeyInfo.ScrollBars = ScrollBars.Both;
            txtKeyInfo.Size = new Size(263, 156);
            txtKeyInfo.TabIndex = 3;
            // 
            // txtInputData
            // 
            txtInputData.Location = new Point(100, 266);
            txtInputData.Name = "txtInputData";
            txtInputData.Size = new Size(533, 31);
            txtInputData.TabIndex = 4;
            // 
            // btnEncrypt
            // 
            btnEncrypt.Location = new Point(320, 320);
            btnEncrypt.Name = "btnEncrypt";
            btnEncrypt.Size = new Size(146, 41);
            btnEncrypt.TabIndex = 5;
            btnEncrypt.Text = "Mã hóa và lưu";
            btnEncrypt.UseVisualStyleBackColor = true;
            btnEncrypt.Click += btnEncrypt_Click;
            // 
            // txtEncryptedBase64
            // 
            txtEncryptedBase64.Location = new Point(90, 320);
            txtEncryptedBase64.Multiline = true;
            txtEncryptedBase64.Name = "txtEncryptedBase64";
            txtEncryptedBase64.Size = new Size(174, 126);
            txtEncryptedBase64.TabIndex = 6;
            // 
            // btnDecryptAndCompare
            // 
            btnDecryptAndCompare.Location = new Point(320, 390);
            btnDecryptAndCompare.Name = "btnDecryptAndCompare";
            btnDecryptAndCompare.Size = new Size(234, 41);
            btnDecryptAndCompare.TabIndex = 7;
            btnDecryptAndCompare.Text = "Giải mã và so sánh nguồn";
            btnDecryptAndCompare.UseVisualStyleBackColor = true;
            btnDecryptAndCompare.Click += btnDecryptAndCompare_Click;
            // 
            // btnGenerateKeysFromSeed
            // 
            btnGenerateKeysFromSeed.Location = new Point(103, 24);
            btnGenerateKeysFromSeed.Name = "btnGenerateKeysFromSeed";
            btnGenerateKeysFromSeed.Size = new Size(171, 36);
            btnGenerateKeysFromSeed.TabIndex = 8;
            btnGenerateKeysFromSeed.Text = "Xuất khóa tự nhập";
            btnGenerateKeysFromSeed.UseVisualStyleBackColor = true;
            btnGenerateKeysFromSeed.Click += btnGenerateKeysFromSeed_Click;
            // 
            // txtCustomSeed
            // 
            txtCustomSeed.Location = new Point(348, 29);
            txtCustomSeed.Name = "txtCustomSeed";
            txtCustomSeed.Size = new Size(234, 31);
            txtCustomSeed.TabIndex = 9;
            // 
            // btnBenchmarkRSA
            // 
            btnBenchmarkRSA.Location = new Point(582, 341);
            btnBenchmarkRSA.Name = "btnBenchmarkRSA";
            btnBenchmarkRSA.Size = new Size(176, 48);
            btnBenchmarkRSA.TabIndex = 10;
            btnBenchmarkRSA.Text = "Đánh giá tốc độ";
            btnBenchmarkRSA.UseVisualStyleBackColor = true;
            btnBenchmarkRSA.Click += btnBenchmarkRSA_Click;
            // 
            // txtHashInput
            // 
            txtHashInput.Location = new Point(143, 472);
            txtHashInput.Name = "txtHashInput";
            txtHashInput.Size = new Size(211, 31);
            txtHashInput.TabIndex = 11;
            txtHashInput.TextChanged += txtHashInput_TextChanged;
            // 
            // txtHashOutput
            // 
            txtHashOutput.Location = new Point(422, 470);
            txtHashOutput.Multiline = true;
            txtHashOutput.Name = "txtHashOutput";
            txtHashOutput.ScrollBars = ScrollBars.Both;
            txtHashOutput.Size = new Size(223, 112);
            txtHashOutput.TabIndex = 12;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(816, 637);
            Controls.Add(txtHashOutput);
            Controls.Add(txtHashInput);
            Controls.Add(btnBenchmarkRSA);
            Controls.Add(txtCustomSeed);
            Controls.Add(btnGenerateKeysFromSeed);
            Controls.Add(btnDecryptAndCompare);
            Controls.Add(txtEncryptedBase64);
            Controls.Add(btnEncrypt);
            Controls.Add(txtInputData);
            Controls.Add(txtKeyInfo);
            Controls.Add(btnImportPrivateKey);
            Controls.Add(btnImportPublicKey);
            Controls.Add(btnGenerateAndExportKeys);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnGenerateAndExportKeys;
        private Button btnImportPublicKey;
        private Button btnImportPrivateKey;
        private TextBox txtKeyInfo;
        private TextBox txtInputData;
        private Button btnEncrypt;
        private TextBox txtEncryptedBase64;
        private Button btnDecryptAndCompare;
        private Button btnGenerateKeysFromSeed;
        private TextBox txtCustomSeed;
        private Button btnBenchmarkRSA;
        private TextBox txtHashInput;
        private TextBox txtHashOutput;
    }
}
