using System.IO;
using System.Linq;

namespace Drawer
{
    internal class KeyValueStore
    {
        private readonly string dataFile;

        public KeyValueStore(string filePath)
        {
            dataFile = filePath;
        }

        public void Add(string key, string value)
        {
            File.AppendAllText(dataFile, $"{key}={value}\n");
        }

        public void Remove(string key)
        {
            System.Collections.Generic.List<string> lines = File.ReadAllLines(dataFile).ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith(key + "="))
                {
                    lines.RemoveAt(i);
                    break;
                }
            }
            File.WriteAllLines(dataFile, lines);
        }

        public void Update(string key, string value)
        {
            System.Collections.Generic.List<string> lines = File.ReadAllLines(dataFile).ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith(key + "="))
                {
                    lines[i] = $"{key}={value}";
                    break;
                }
            }
            File.WriteAllLines(dataFile, lines);
        }

        public string Get(string key)
        {
            string[] lines = File.ReadAllLines(dataFile);
            foreach (string line in lines)
            {
                if (line.StartsWith(key + "="))
                {
                    return line.Substring(key.Length + 1);
                }
            }
            return null;
        }
    }
}