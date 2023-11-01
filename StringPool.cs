using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Drawer
{
    internal class StringPool
    {
        private List<string> pool;
        private readonly EncryptString es;
        private readonly List<string> initPool;
        private readonly Random random;

        public StringPool()
        {
            es = new EncryptString("yuxiang118712023");
            random = new Random();
            initPool = new List<string>(Read("list.txt").Split(','));

            if (File.Exists(Path.Combine(Application.StartupPath, "pool.txt")))
            {
                pool = new List<string>(Read("pool.txt").Split(','));
            }
            else
            {
                Reset();
            }
            if (pool[0].Equals(""))
            {
                Reset();
            }
        }

        public string Get()
        {
            if (pool.Count == 0)
            {
                Reset();
            }
            return pool[random.Next(0, pool.Count() - 1)];
        }

        public void Reset()
        {
            pool = new List<string>(initPool);
        }

        public void Remove(string value)
        {
            _ = pool.Remove(value);
        }

        public string Read(string fileName)
        {
            string filePath = Path.Combine(Application.StartupPath, fileName);
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line = es.Decrypt(reader.ReadLine());
                    return line;
                }
            }
            else
            {
                _ = MessageBox.Show(fileName + " 文件读取错误，无法启动本软件。", "YuXiang Drawer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return null;
            }
        }

        public void Save()
        {
            string filePath = Path.Combine(Application.StartupPath, "pool.txt");
            File.WriteAllText(filePath, es.Encrypt(string.Join(",", pool)));
        }
    }
}