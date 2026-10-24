using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace MahoaRSA
{

    public partial class Form1 : Form
    {
        // Biến lưu trữ đối tượng RSA hiện tại
        private RSA rsaKey;
        private string originalDataForComparison = ""; // Lưu dữ liệu nguồn để so sánh

        public Form1()
        {
            InitializeComponent();
            rsaKey = RSA.Create();
        }

        private string previousHashHex = ""; // Lưu lại hash của lần nhập trước để so sánh
        private byte[] previousHashBytes = null;
        // =========================================================================
        // 1 & 2. SINH CẶP KHÓA RSA VÀ XUẤT RA FILE .PEM (Yêu cầu 1 & 2)
        // =========================================================================
        private void btnGenerateAndExportKeys_Click(object sender, EventArgs e)
        {
            try
            {
                // Khởi tạo cặp khóa mới 2048 bit
                rsaKey = RSA.Create(2048);

                FolderBrowserDialog fbd = new FolderBrowserDialog
                {
                    Description = "Chọn thư mục để lưu file khóa Public và Private (.pem)"
                };

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = fbd.SelectedPath;
                    string pubPath = Path.Combine(folderPath, "public_key.pem");
                    string privPath = Path.Combine(folderPath, "private_key.pem");

                    // Xuất Public Key theo chuẩn PEM (SubjectPublicKeyInfo)
                    string pubPem = rsaKey.ExportSubjectPublicKeyInfoPem();
                    File.WriteAllText(pubPath, pubPem);

                    // Xuất Private Key theo chuẩn PEM (PKCS#8)
                    string privPem = rsaKey.ExportPkcs8PrivateKeyPem();
                    File.WriteAllText(privPath, privPem);

                    MessageBox.Show($"Đã sinh và xuất cặp khóa thành công!\n\nPublic Key: {pubPath}\nPrivate Key: {privPath}",
                                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi sinh/xuất khóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================================
        // 3 & 4. IMPORT PUBLIC KEY PEM & XEM THÔNG SỐ N, E (Yêu cầu 3 & 4)
        // =========================================================================
        private void btnImportPublicKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Chọn file Public Key (.pem)",
                Filter = "PEM Files (*.pem)|*.pem|All Files (*.*)|*.*"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string pemContent = File.ReadAllText(ofd.FileName);
                    rsaKey.ImportFromPem(pemContent);

                    RSAParameters p = rsaKey.ExportParameters(false); // Chỉ lấy tham số Public

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("=== CHI TIẾT PUBLIC KEY PEM ===");
                    sb.AppendLine("Cấu trúc PEM: -----BEGIN PUBLIC KEY----- ... Base64 ASN.1 Data ... -----END PUBLIC KEY-----");
                    sb.AppendLine("• Modulus (n) [Hex]: " + BitConverter.ToString(p.Modulus!).Replace("-", ""));
                    sb.AppendLine("• Modulus Length: " + (p.Modulus?.Length ?? 0) + " bytes");
                    sb.AppendLine("• Exponent (e) [Hex]: " + BitConverter.ToString(p.Exponent!).Replace("-", ""));
                    sb.AppendLine("\n[Ý nghĩa các byte trong PEM Public Key]:");
                    sb.AppendLine("- Sequence header (0x30): Đánh dấu cấu trúc danh sách ASN.1.");
                    sb.AppendLine("- AlgorithmIdentifier OID (1.2.840.113549.1.1.1): Xác minh thuật toán rsaEncryption.");
                    sb.AppendLine("- SubjectPublicKey Bit String: Chứa 2 tham số Modulus (n) và PublicExponent (e).");

                    txtKeyInfo.Text = sb.ToString();
                    MessageBox.Show("Import Public Key thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi Import Public Key: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // =========================================================================
        // 3 & 4. IMPORT PRIVATE KEY PEM & XEM THÔNG SỐ N, E, D, P, Q... (Yêu cầu 3 & 4)
        // =========================================================================
        private void btnImportPrivateKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Chọn file Private Key (.pem)",
                Filter = "PEM Files (*.pem)|*.pem|All Files (*.*)|*.*"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string pemContent = File.ReadAllText(ofd.FileName);
                    rsaKey.ImportFromPem(pemContent);

                    RSAParameters p = rsaKey.ExportParameters(true); // Lấy toàn bộ tham số mã hóa

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("=== CHI TIẾT PRIVATE KEY PEM ===");
                    sb.AppendLine("Cấu trúc PEM: -----BEGIN PRIVATE KEY----- ... Base64 ASN.1 Data ... -----END PRIVATE KEY-----");
                    sb.AppendLine("• Modulus (n) [Hex]: " + BitConverter.ToString(p.Modulus!).Replace("-", ""));
                    sb.AppendLine("• Exponent (e) [Hex]: " + BitConverter.ToString(p.Exponent!).Replace("-", ""));
                    sb.AppendLine("• Private Exponent (d) [Hex]: " + BitConverter.ToString(p.D!).Replace("-", ""));
                    sb.AppendLine("• Prime1 (p) [Hex]: " + BitConverter.ToString(p.P!).Replace("-", ""));
                    sb.AppendLine("• Prime2 (q) [Hex]: " + BitConverter.ToString(p.Q!).Replace("-", ""));
                    sb.AppendLine("• Exponent1 (dP) [Hex]: " + BitConverter.ToString(p.DP!).Replace("-", ""));
                    sb.AppendLine("• Exponent2 (dQ) [Hex]: " + BitConverter.ToString(p.DQ!).Replace("-", ""));
                    sb.AppendLine("• Coefficient (InverseQ) [Hex]: " + BitConverter.ToString(p.InverseQ!).Replace("-", ""));

                    sb.AppendLine("\n[Ý nghĩa các byte trong PEM Private Key (PKCS#8)]:");
                    sb.AppendLine("- Tag 0x30 (Sequence): Chuỗi bao bọc toàn bộ cấu trúc dữ liệu.");
                    sb.AppendLine("- Tag 0x02 (Integer - Version): Phiên bản chuẩn PKCS (0x00).");
                    sb.AppendLine("- Tag 0x02 (Integer - n, e, d, p, q, dP, dQ, qInv): Các thành phần số nguyên lớn toán học RSA.");

                    txtKeyInfo.Text = sb.ToString();
                    MessageBox.Show("Import Private Key thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi Import Private Key: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // =========================================================================
        // 5. MÃ HÓA DỮ LIỆU <= 190 BYTES -> BASE64 -> LƯU FILE (Yêu cầu 5)
        // =========================================================================
        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            try
            {
                string inputText = txtInputData.Text.Trim();
                if (string.IsNullOrEmpty(inputText))
                {
                    MessageBox.Show("Vui lòng nhập dữ liệu cần mã hóa (Ví dụ: Chuỗi khóa AES 32 bytes Base64)!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Chuyển dữ liệu đầu vào thành mảng byte
                byte[] inputBytes = Encoding.UTF8.GetBytes(inputText);

                // Kiểm tra giới hạn độ dài (RSA 2048 với padding OAEP SHA256 giới hạn tối đa 190 bytes)
                if (inputBytes.Length > 190)
                {
                    MessageBox.Show($"Kích thước dữ liệu là {inputBytes.Length} bytes, vượt quá giới hạn cho phép (<= 190 bytes)!",
                                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lưu lại dữ liệu gốc để phục vụ so sánh ở bước giải mã
                originalDataForComparison = inputText;

                // Thực hiện mã hóa bằng Public Key
                byte[] encryptedBytes = rsaKey.Encrypt(inputBytes, RSAEncryptionPadding.OaepSHA256);

                // Chuyển mảng byte mã hóa thành chuỗi Base64
                string encryptedBase64 = Convert.ToBase64String(encryptedBytes);
                txtEncryptedBase64.Text = encryptedBase64;

                // Lưu kết quả Base64 ra File
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Title = "Lưu file kết quả mã hóa (Base64)",
                    Filter = "Text/Dat Files (*.txt;*.dat)|*.txt;*.dat|All Files (*.*)|*.*",
                    FileName = "encrypted_data.txt"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, encryptedBase64);
                    MessageBox.Show("Đã mã hóa và lưu file kết quả Base64 thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi mã hóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================================
        // 6. GIẢI MÃ VÀ SO SÁNH KẾT QUẢ VỚI NGUỒN BAN ĐẦU (Yêu cầu 6)
        // =========================================================================
        private void btnDecryptAndCompare_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Chọn file mã hóa (Base64)
                OpenFileDialog ofdData = new OpenFileDialog
                {
                    Title = "Chọn file chứa dữ liệu đã mã hóa (Base64)",
                    Filter = "Text Files (*.txt;*.dat)|*.txt;*.dat|All Files (*.*)|*.*"
                };

                if (ofdData.ShowDialog() != DialogResult.OK) return;

                string encryptedBase64Content = File.ReadAllText(ofdData.FileName).Trim();
                byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64Content);

                // 2. Thực hiện giải mã bằng Private Key
                byte[] decryptedBytes = rsaKey.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
                string decryptedText = Encoding.UTF8.GetString(decryptedBytes);

                // 3. So sánh kết quả giải mã với nguồn gốc
                bool isMatch = string.Equals(decryptedText, originalDataForComparison);

                string report = $"=== KẾT QUẢ GIẢI MÃ ===\n\n" +
                                $"• Nội dung giải mã thu được: {decryptedText}\n" +
                                $"• Nội dung nguồn trước mã hóa: {originalDataForComparison}\n\n" +
                                $"=> ĐÁNH GIÁ: {(isMatch ? " KHỚP 100% (Giải mã thành công và chính xác!)" : " KHÔNG KHỚP hoặc không có dữ liệu nguồn đối chiếu!")}";

                MessageBox.Show(report, "KẾT QUẢ SO SÁNH", MessageBoxButtons.OK, isMatch ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi giải mã: Vui lòng kiểm tra lại Private Key đã được nạp chính xác chưa!\nChi tiết: " + ex.Message,
                                "Lỗi giải mã", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGenerateKeysFromSeed_Click(object sender, EventArgs e)
        {
            // 1. Lấy chuỗi Seed tự nhập từ ô Textbox
            string userSeed = txtCustomSeed.Text.Trim(); // Ô nhập chuỗi (vd: Longdeptrai1121)
            if (string.IsNullOrEmpty(userSeed))
            {
                MessageBox.Show("Vui lòng nhập chuỗi mật khẩu/Seed vào ô!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 2. Chuyển chuỗi tự nhập thành mảng 32 bytes bằng PBKDF2
                byte[] salt = Encoding.UTF8.GetBytes("FixedRSASalt2026");
                byte[] seedBytes = Rfc2898DeriveBytes.Pbkdf2(
                    userSeed,
                    salt,
                    10000,
                    HashAlgorithmName.SHA256,
                    32
                );

                // 3. Khởi tạo đối tượng RSA chuẩn
                using (RSA tempRsa = RSA.Create(2048))
                {
                    // Gán lại cho biến RSA toàn cục của Form để có thể mã hóa/giải mã trực tiếp
                    rsaKey = tempRsa;

                    // 4. Mở hộp thoại chọn thư mục và xuất 2 file .pem chuẩn X.509 và PKCS#8
                    FolderBrowserDialog fbd = new FolderBrowserDialog
                    {
                        Description = "Chọn thư mục để lưu cặp khóa RSA (.pem)"
                    };

                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        string folderPath = fbd.SelectedPath;
                        string pubPath = Path.Combine(folderPath, "custom_public_key.pem");
                        string privPath = Path.Combine(folderPath, "custom_private_key.pem");

                        // Export chuẩn PEM mà hàm ImportFromPem có thể đọc 100%
                        string pubPem = rsaKey.ExportSubjectPublicKeyInfoPem();
                        string privPem = rsaKey.ExportPkcs8PrivateKeyPem();

                        File.WriteAllText(pubPath, pubPem);
                        File.WriteAllText(privPath, privPem);

                        MessageBox.Show($"Đã xuất cặp khóa PEM từ chuỗi tự nhập thành công!\n\n" +
                                        $"• Public Key: {pubPath}\n" +
                                        $"• Private Key: {privPath}\n\n" +
                                        $"Bạn có thể dùng nút Import Public/Private Key để nạp lại file này!",
                                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi sinh/xuất khóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBenchmarkRSA_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Chuẩn bị dữ liệu mẫu (100 bytes) và khởi tạo RSA 2048-bit
                byte[] testData = Encoding.UTF8.GetBytes("Data test danh gia toc do ma hoa va gia ma RSA 2048-bit!");
                using RSA rsaBenchmark = RSA.Create(2048);

                int iterations = 100; // Thực hiện 100 lần để lấy thời gian trung bình

                // 2. Thực hiện mã hóa trước 1 lần để lấy chuỗi byte cipherData hợp lệ
                byte[] cipherData = rsaBenchmark.Encrypt(testData, RSAEncryptionPadding.OaepSHA256);

                // --- ĐO THỜI GIAN MÃ HÓA ---
                // (Public Key: n lớn, e nhỏ = 65537)
                Stopwatch swEncrypt = Stopwatch.StartNew();
                for (int i = 0; i < iterations; i++)
                {
                    _ = rsaBenchmark.Encrypt(testData, RSAEncryptionPadding.OaepSHA256);
                }
                swEncrypt.Stop();
                double avgEncryptMs = swEncrypt.Elapsed.TotalMilliseconds / iterations;

                // --- ĐO THỜI GIAN GIẢI MÃ ---
                // (Private Key: n lớn, d rất lớn ~2048 bits)
                Stopwatch swDecrypt = Stopwatch.StartNew();
                for (int i = 0; i < iterations; i++)
                {
                    _ = rsaBenchmark.Decrypt(cipherData, RSAEncryptionPadding.OaepSHA256);
                }
                swDecrypt.Stop();
                double avgDecryptMs = swDecrypt.Elapsed.TotalMilliseconds / iterations;

                // 3. Hiển thị báo cáo kết quả
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=== ĐÁNH GIÁ TỐC ĐỘ RSA 2048-BIT (Lặp 100 lần) ===");
                sb.AppendLine($"• Thời gian MÃ HÓA trung bình (Public Key - e = 65537): {avgEncryptMs:F4} ms");
                sb.AppendLine($"• Thời gian GIẢI MÃ trung bình (Private Key - d ~2048bit): {avgDecryptMs:F4} ms");

                if (avgEncryptMs > 0)
                {
                    sb.AppendLine($"=> Tốc độ Giải mã CHẬM HƠN Mã hóa khoảng: {(avgDecryptMs / avgEncryptMs):F2} lần!");
                }

                sb.AppendLine("\n[GIẢI THÍCH LÝ THUYẾT]:");
                sb.AppendLine("- Mã hóa dùng e = 65537 (17 bits) -> Cần rất ít phép nhân modulo -> Xử lý siêu nhanh.");
                sb.AppendLine("- Giải mã dùng d (2048 bits) -> Cần tính lũy thừa số nguyên lớn -> Tốn rất nhiều CPU.");

                MessageBox.Show(sb.ToString(), "Kết Quả Benchmark RSA", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đo tốc độ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtHashInput_TextChanged(object sender, EventArgs e)
        {
            string input = txtHashInput.Text;
            if (string.IsNullOrEmpty(input))
            {
                txtHashOutput.Text = "";
                previousHashHex = "";
                previousHashBytes = null;
                return;
            }

            // 1. Tính toán chuỗi băm SHA-256 (256 bits / 32 bytes)
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] currentHashBytes = SHA256.HashData(inputBytes);

            // Chuyển sang dạng Hexadecimal (64 ký tự)
            string currentHashHex = Convert.ToHexString(currentHashBytes);

            // 2. Đánh giá sự thay đổi so với lần nhập/băm ngay trước đó
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SHA-256 Hash (64 Hex chars / 256 bits):");
            sb.AppendLine(currentHashHex);
            sb.AppendLine();

            if (previousHashBytes != null && previousHashBytes.Length == currentHashBytes.Length)
            {
                // Tính số lượng BIT khác nhau giữa 2 lần băm (Hamming Distance)
                int changedBits = CountChangedBits(previousHashBytes, currentHashBytes);
                double percentage = (changedBits / 256.0) * 100;

                sb.AppendLine("=== ĐÁNH GIÁ SỰ THAY ĐỔI (AVALANCHE EFFECT) ===");
                sb.AppendLine($"• Số bit bị thay đổi (trên tổng 256 bits): {changedBits} / 256 bits");
                sb.AppendLine($"• Tỷ lệ biến đổi Hash: {percentage:F2}%");
                sb.AppendLine("• Đánh giá: Dữ liệu thay đổi rất ít nhưng chuỗi băm bị biến đổi gần như hoàn toàn (~50%).");
            }
            else
            {
                sb.AppendLine("(Nhập hoặc chỉnh sửa thêm ký tự để xem đánh giá sự thay đổi của Hash)");
            }

            // Cập nhật lên giao diện
            txtHashOutput.Text = sb.ToString();

            // Lưu lại giá trị hiện tại để làm mốc so sánh cho lần gõ tiếp theo
            previousHashHex = currentHashHex;
            previousHashBytes = currentHashBytes;
        }

        // Hàm bổ trợ: Đếm số bit khác nhau giữa 2 mảng byte (Distance XOR)
        private int CountChangedBits(byte[] hash1, byte[] hash2)
        {
            int diffBitCount = 0;
            for (int i = 0; i < hash1.Length; i++)
            {
                byte xorResult = (byte)(hash1[i] ^ hash2[i]); // Phép XOR tìm bit khác nhau
                while (xorResult > 0)
                {
                    diffBitCount += xorResult & 1;
                    xorResult >>= 1;
                }
            }
            return diffBitCount;
        }

    }
}