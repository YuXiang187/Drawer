using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Drawer
{
    internal class EncryptString
    {
        private readonly string pinKey = "cifosrxqj5430986";
        private string key;
        private readonly KeyValueStore store;
        public EncryptString()
        {
            key = pinKey;
            store = new KeyValueStore(Path.Combine(Application.StartupPath, "Drawer.config"));
            string configKey = Decrypt(store.Get("Key"));
            key = configKey;
        }
        public void ChangeKey(string key)
        {
            this.key = pinKey;
            store.Update("Key", Encrypt(key));
            this.key = key;
        }
        public string Encrypt(string text)
        {
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.Mode = CipherMode.ECB;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.Mode = CipherMode.ECB;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        try
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                        catch (CryptographicException)
                        {
                            _ = MessageBox.Show("列表文件读取失败。\n\n如果单击“确定”按钮后：\n- 程序退出，请再次启动程序。\n- 程序弹出密钥输入窗口，请输入正确的密钥。\n", "YuXiang Drawer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        }
                    }
                }
            }
        }
    }
}