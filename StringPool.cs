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
        private static List<string> initPool;
        private readonly Random random;

        public StringPool()
        {
            es = new EncryptString();
            random = new Random();

            string listString;
            do
            {
                listString = Read("list.txt");
                if (listString == null)
                {
                    string key = InputDialog.Show("YuXiang Drawer", "请输入密钥：", 290);
                    if (key != null)
                    {
                        if (key.Length == 16 || key.Length == 24 || key.Length == 32)
                        {
                            es.ChangeKey(key);
                            if (File.Exists(Path.Combine(Application.StartupPath, "pool.txt")))
                            {
                                File.Delete(Path.Combine(Application.StartupPath, "pool.txt"));
                            }
                        }
                    }
                    else
                    {
                        Environment.Exit(0);
                        return;
                    }
                }
            }
            while (listString == null);

            initPool = new List<string>(Read("list.txt").Split(','));

            if (File.Exists(Path.Combine(Application.StartupPath, "pool.txt")))
            {
                string poolString = Read("pool.txt");
                if (poolString != null)
                {
                    pool = new List<string>(poolString.Split(','));
                }
                else
                {
                    File.Delete(Path.Combine(Application.StartupPath, "pool.txt"));
                    Environment.Exit(0);
                }
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
            foreach (string key in pool)
            {
                Console.Write(key + "|");
            }
            Console.WriteLine("Count:" + pool.Count());
            return pool[random.Next(pool.Count())];
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
                return null;
            }
        }

        public void Save()
        {
            string filePath = Path.Combine(Application.StartupPath, "pool.txt");
            File.WriteAllText(filePath, es.Encrypt(string.Join(",", pool)));
        }

        public static List<string> GetInitList()
        {
            return initPool;
        }
    }
}