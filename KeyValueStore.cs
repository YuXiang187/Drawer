using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Drawer
{
    internal class KeyValueStore
    {
        private readonly string dataFile = Path.Combine(Application.StartupPath, "Drawer.config");

        public void Add(string key, string value)
        {
            try
            {
                File.AppendAllText(dataFile, $"{key}:{value}\n");
            }
            catch (IOException)
            {
                // config file unavailable (deleted, locked or read-only): degrade silently instead of crashing
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        public void Update(string key, string value)
        {
            try
            {
                List<string> lines = File.Exists(dataFile) ? File.ReadAllLines(dataFile).ToList() : new List<string>();
                bool found = false;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (MatchesKey(lines[i], key))
                    {
                        lines[i] = $"{key}:{value}";
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    // append missing keys so an incomplete config can self-heal
                    lines.Add($"{key}:{value}");
                }
                File.WriteAllLines(dataFile, lines);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        public void Delete(string key)
        {
            try
            {
                if (!File.Exists(dataFile))
                {
                    return;
                }
                List<string> lines = File.ReadAllLines(dataFile).ToList();
                lines.RemoveAll(line => MatchesKey(line, key));
                File.WriteAllLines(dataFile, lines);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        private static bool MatchesKey(string line, string key)
        {
            if (!line.StartsWith(key + ":"))
            {
                return false;
            }
            int separatorIndex = line.IndexOf(':');
            return separatorIndex == key.Length;
        }

        public string Get(string key)
        {
            try
            {
                string[] lines = File.ReadAllLines(dataFile);
                foreach (string line in lines)
                {
                    if (MatchesKey(line, key))
                    {
                        return line.Substring(key.Length + 1);
                    }
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            return null;
        }
    }
}
