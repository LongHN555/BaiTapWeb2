using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics; 

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

        // 4. Nhập vào 1 file -> mã hóa file đó + đo thời gian -> lưu ra file
        private void btnEncryptFile_Click(object sender, EventArgs e)
        {
            if (currentKey == null || currentIV == null)
            {
                MessageBox.Show("Vui lòng sinh khóa hoặc import khóa trước!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenFileDialog openFile = new OpenFileDialog { Title = "Chọn file lớn cần mã hóa" };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                SaveFileDialog saveFile = new SaveFileDialog
                {
                    Title = "Lưu file kết quả mã hóa",
                    Filter = "Encrypted Files (*.aes)|*.aes",
                    FileName = Path.GetFileName(openFile.FileName) + ".aes"
                };

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        this.Cursor = Cursors.WaitCursor;

                        // --- BẮT ĐẦU ĐO THỜI GIAN MÃ HÓA ---
                        Stopwatch sw = Stopwatch.StartNew();

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

                        sw.Stop(); // --- KẾT THÚC ĐO THỜI GIAN ---
                        this.Cursor = Cursors.Default;

                        double seconds = sw.Elapsed.TotalSeconds;
                        long fileSizeBytes = new FileInfo(openFile.FileName).Length;
                        double speedMBps = (fileSizeBytes / (1024.0 * 1024.0)) / seconds;

                        string resultMsg = $"Mã hóa file thành công!\n\n" +
                                           $"• File nguồn: {Path.GetFileName(openFile.FileName)}\n" +
                                           $"• Dung lượng: {fileSizeBytes:N0} Bytes ({(fileSizeBytes / (1024.0 * 1024.0)):N2} MB)\n" +
                                           $"• Thời gian mã hóa: {seconds:F2} giây\n" +
                                           $"• Tốc độ mã hóa: {speedMBps:F2} MB/s";

                        MessageBox.Show(resultMsg, "KẾT QUẢ MÃ HÓA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("Lỗi mã hóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        static void CreateLargeFile(string filePath, long sizeInBytes)
        {
            byte[] buffer = new byte[1024 * 1024]; // Buffer 1 MB
            Random rnd = new Random();

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                long bytesWritten = 0;
                while (bytesWritten < sizeInBytes)
                {
                    rnd.NextBytes(buffer);
                    int bytesToWrite = (int)Math.Min(buffer.Length, sizeInBytes - bytesWritten);
                    fs.Write(buffer, 0, bytesToWrite);
                    bytesWritten += bytesToWrite;
                }
            }
        }
        // Gọi hàm để tạo file 1.5 GB
        // CreateLargeFile(@"D:\large_test.dat", 1500L * 1024 * 1024);

        private void btnDecryptText_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra khóa đã được nạp hay chưa
            if (currentKey == null || currentIV == null)
            {
                MessageBox.Show("Vui lòng sinh khóa hoặc import khóa trước!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy chuỗi Base64 từ ô txtInputText (hoặc txtOutputText nếu người dùng để nguyên kết quả vừa mã hóa)
            string cipherTextBase64 = txtInputText.Text.Trim();
            if (string.IsNullOrEmpty(cipherTextBase64))
            {
                // Nếu ô trên trống, thử lấy từ ô kết quả bên dưới
                cipherTextBase64 = txtOutputText.Text.Trim();
            }

            if (string.IsNullOrEmpty(cipherTextBase64))
            {
                MessageBox.Show("Vui lòng nhập/dán chuỗi Base64 cần giải mã!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 2. Chuyển chuỗi Base64 thành mảng Byte
                byte[] cipherBytes = Convert.FromBase64String(cipherTextBase64);

                // 3. Thực hiện giải mã AES
                using (Aes aes = Aes.Create())
                {
                    aes.Key = currentKey;
                    aes.IV = currentIV;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

                    // 4. In kết quả đã giải mã thành công ra ô Output
                    txtOutputText.Text = Encoding.UTF8.GetString(plainBytes);
                    MessageBox.Show("Giải mã văn bản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Giải mã thất bại! Vui lòng kiểm tra lại đúng Khóa/IV hoặc định dạng chuỗi mã hóa.\nChi tiết lỗi: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDecryptFile_Click(object sender, EventArgs e)
        {
            if (currentKey == null || currentIV == null)
            {
                MessageBox.Show("Vui lòng nạp khóa AES trước khi giải mã!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Chọn file đã mã hóa (.aes)
            OpenFileDialog openEncryptedFile = new OpenFileDialog
            {
                Title = "1. Chọn file đã mã hóa (.aes)",
                Filter = "Encrypted Files (*.aes)|*.aes|All Files (*.*)|*.*"
            };

            if (openEncryptedFile.ShowDialog() == DialogResult.OK)
            {
                // Chọn file Nguồn ban đầu để tiến hành so sánh Kích thước Byte
                OpenFileDialog openOriginalFile = new OpenFileDialog
                {
                    Title = "2. Chọn file Nguồn gốc (chưa mã hóa) để so sánh Byte"
                };

                if (openOriginalFile.ShowDialog() == DialogResult.OK)
                {
                    SaveFileDialog saveDecryptedFile = new SaveFileDialog
                    {
                        Title = "3. Chọn nơi lưu file sau khi giải mã",
                        FileName = "GiaiMa_" + Path.GetFileNameWithoutExtension(openEncryptedFile.FileName)
                    };

                    if (saveDecryptedFile.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            this.Cursor = Cursors.WaitCursor;

                            // --- BẮT ĐẦU ĐO THỜI GIAN GIẢI MÃ ---
                            Stopwatch sw = Stopwatch.StartNew();

                            using (Aes aes = Aes.Create())
                            {
                                aes.Key = currentKey;
                                aes.IV = currentIV;

                                using (FileStream fsInput = new FileStream(openEncryptedFile.FileName, FileMode.Open, FileAccess.Read))
                                using (CryptoStream cs = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read))
                                using (FileStream fsOutput = new FileStream(saveDecryptedFile.FileName, FileMode.Create, FileAccess.Write))
                                {
                                    cs.CopyTo(fsOutput);
                                }
                            }

                            sw.Stop(); // --- KẾT THÚC ĐO THỜI GIAN GIẢI MÃ ---
                            this.Cursor = Cursors.Default;

                            // --- THỰC HIỆN SO SÁNH BYTES ---
                            long originalSizeBytes = new FileInfo(openOriginalFile.FileName).Length;
                            long decryptedSizeBytes = new FileInfo(saveDecryptedFile.FileName).Length;
                            bool isMatch = (originalSizeBytes == decryptedSizeBytes);

                            double seconds = sw.Elapsed.TotalSeconds;
                            double speedMBps = (decryptedSizeBytes / (1024.0 * 1024.0)) / seconds;

                            string resultReport = $"=== KẾT QUẢ GIẢI MÃ & SO SÁNH ===\n\n" +
                                                  $"• Thời gian giải mã: {seconds:F2} giây\n" +
                                                  $"• Tốc độ giải mã: {speedMBps:F2} MB/s\n\n" +
                                                  $"--- BẢNG SO SÁNH KÍCH THƯỚC FILE ---\n" +
                                                  $"• Dung lượng File Nguồn: {originalSizeBytes:N0} Bytes\n" +
                                                  $"• Dung lượng File Giải Mã: {decryptedSizeBytes:N0} Bytes\n\n" +
                                                  $"=> KẾT LUẬN: {(isMatch ? " KHỚP 100% BYTES (Giải mã chính xác hoàn toàn)" : " KHÔNG KHỚP (Có lỗi xảy ra)")}";

                            MessageBox.Show(resultReport, "BÁO CÁO KẾT QUẢ THỬ NGHIỆM",
                                            MessageBoxButtons.OK, isMatch ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            this.Cursor = Cursors.Default;
                            MessageBox.Show("Lỗi giải mã: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
    }
}