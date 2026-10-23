using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace AESWinFormApp
{
    public partial class Form1 : Form
    {
        private byte[] currentKey;
        private byte[] currentIV;

        public Form1()
        {
            InitializeComponent();
        }

        // 1. Sinh khóa AES (tự nhập chuỗi) rồi lưu ra file
        private void btnSaveKey_Click(object sender, EventArgs e)
        {
            string userSecret = txtCustomKey.Text;
            if (string.IsNullOrWhiteSpace(userSecret))
            {
                MessageBox.Show("Vui lòng nhập chuỗi khóa/mật khẩu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfdKey = new SaveFileDialog
            {
                Title = "Lưu File Khóa AES",
                Filter = "Key Files (*.key)|*.key|All Files (*.*)|*.*"
            };

            if (sfdKey.ShowDialog() == DialogResult.OK)
            {
                using (Aes aes = Aes.Create())
                {
                    // Băm chuỗi tự nhập thành 32 bytes chuẩn AES-256
                    using (SHA256 sha256 = SHA256.Create())
                    {
                        currentKey = sha256.ComputeHash(Encoding.UTF8.GetBytes(userSecret));
                    }

                    // Tự động sinh IV 16 bytes
                    aes.GenerateIV();
                    currentIV = aes.IV;

                    // Gộp 32 bytes Key + 16 bytes IV = 48 bytes lưu vào file
                    byte[] keyAndIv = new byte[currentKey.Length + currentIV.Length];
                    Array.Copy(currentKey, 0, keyAndIv, 0, currentKey.Length);
                    Array.Copy(currentIV, 0, keyAndIv, currentKey.Length, currentIV.Length);

                    File.WriteAllBytes(sfdKey.FileName, keyAndIv);
                    MessageBox.Show("Đã lưu khóa tự nhập ra file thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // 2. Import khóa từ file
        private void btnLoadKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Chọn File Khóa AES",
                Filter = "Key Files (*.key)|*.key|All Files (*.*)|*.*"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                byte[] fileBytes = File.ReadAllBytes(ofd.FileName);
                if (fileBytes.Length == 48) // 32 bytes Key + 16 bytes IV
                {
                    currentKey = new byte[32];
                    currentIV = new byte[16];
                    Array.Copy(fileBytes, 0, currentKey, 0, 32);
                    Array.Copy(fileBytes, 32, currentIV, 0, 16);

                    MessageBox.Show("Import khóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Cấu trúc file khóa không hợp lệ (cần đúng 48 bytes)!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 3. Mã hóa 1 đoạn văn bản -> kq mã hóa ra màn hình
        private void btnEncryptText_Click(object sender, EventArgs e)
        {
            if (currentKey == null || currentIV == null)
            {
                MessageBox.Show("Vui lòng sinh khóa hoặc import khóa trước!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string plainText = txtInputText.Text;
            if (string.IsNullOrEmpty(plainText))
            {
                MessageBox.Show("Vui lòng nhập văn bản cần mã hóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = currentKey;
                aes.IV = currentIV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                txtOutputText.Text = Convert.ToBase64String(cipherBytes);
            }
        }

        // 4. Nhập vào 1 file -> mã hóa file đó -> lưu ra file
        private void btnEncryptFile_Click(object sender, EventArgs e)
        {
            if (currentKey == null || currentIV == null)
            {
                MessageBox.Show("Vui lòng sinh khóa hoặc import khóa trước!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenFileDialog openFile = new OpenFileDialog { Title = "Chọn file nguồn cần mã hóa" };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                SaveFileDialog saveFile = new SaveFileDialog
                {
                    Title = "Lưu file kết quả mã hóa",
                    Filter = "Encrypted Files (*.aes)|*.aes|All Files (*.*)|*.*"
                };

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = currentKey;
                        aes.IV = currentIV;

                        using (FileStream fsInput = new FileStream(openFile.FileName, FileMode.Open, FileAccess.Read))
                        using (FileStream fsOutput = new FileStream(saveFile.FileName, FileMode.Create, FileAccess.Write))
                        using (CryptoStream cs = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            fsInput.CopyTo(cs);
                        }
                    }
                    MessageBox.Show("Mã hóa file và lưu ra file thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}