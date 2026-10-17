using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Drawer
{
    internal class KeyValueStore
    {
        private static readonly string dataFile = Path.Combine(Application.StartupPath, "Drawer.config");

        // the whole config is read once and kept in memory
        private static readonly object sync = new object();
        private static List<string> lines;

        // returns null when the file cannot be read, so a bad read never turns into an empty config
        private static List<string> Lines()
        {
            if (lines == null)
            {
                try
                {
                    lines = File.Exists(dataFile) ? File.ReadAllLines(dataFile).ToList() : new List<string>();
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
            return lines;
        }

        public void Update(string key, string value)
        {
            lock (sync)
            {
                List<string> all = Lines();
                if (all == null)
                {
                    return;
                }
                bool found = false;
                for (int i = 0; i < all.Count; i++)
                {
                    if (MatchesKey(all[i], key))
                    {
                        all[i] = $"{key}:{value}";
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    // append missing keys so an incomplete config can self-heal
                    all.Add($"{key}:{value}");
                }
                Save();
            }
        }

        public void Delete(string key)
        {
            lock (sync)
            {
                List<string> all = Lines();
                if (all != null && all.RemoveAll(line => MatchesKey(line, key)) > 0)
                {
                    Save();
                }
            }
        }

        public string Get(string key)
        {
            lock (sync)
            {
                List<string> all = Lines();
                if (all == null)
                {
                    return null;
                }
                foreach (string line in all)
                {
                    if (MatchesKey(line, key))
                    {
                        return line.Substring(key.Length + 1);
                    }
                }
            }
            return null;
        }

        private static void Save()
        {
            try
            {
                File.WriteAllLines(dataFile, lines);
            }
            catch (IOException)
            {
                // config file unavailable (deleted, locked or read-only): degrade silently instead of crashing
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        private static bool MatchesKey(string line, string key)
        {
            return line.StartsWith(key + ":");
        }
    }
}
