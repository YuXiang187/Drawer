using System;
using System.Collections.Generic;
using System.Linq;

namespace Drawer
{
    internal class StringPool
    {
        private const double StddevScalingFactor = 3.0;

        private readonly KeyValueStore store;
        private readonly EncryptString es;
        private readonly Random random;

        public static List<string> initPool = new List<string>();

        public static List<string> pool = new List<string>();

        public StringPool()
        {
            store = new KeyValueStore();
            es = new EncryptString();
            random = new Random();

            List<string> loadedInitPool = SplitNames(es.Decrypt(store.Get("initPool") ?? ""));
            List<string> loadedPool = SplitNames(es.Decrypt(store.Get("pool") ?? ""));
            SetState(loadedInitPool, loadedPool);
        }

        private static List<string> SplitNames(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new List<string>();
            }
            return value.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToList();
        }

        public static void SetNames(List<string> names)
        {
            initPool = new List<string>(names);
            SyncPool();
        }

        public static void SetState(List<string> init, List<string> history)
        {
            initPool = new List<string>(init);
            pool = new List<string>(history);
            SyncPool();
        }

        public string Get()
        {
            if (pool.Count == 0)
            {
                return string.Empty;
            }
            return pool[random.Next(pool.Count)];
        }

        public string Draw()
        {
            if (initPool.Count == 0)
            {
                return string.Empty;
            }

            SyncPool();

            int index;
            while (true)
            {
                double stddev = pool.Count / StddevScalingFactor;
                double sample = Math.Abs(NextGaussian(0.0, stddev));

                if (sample < pool.Count)
                {
                    index = (int)sample;
                    break;
                }
            }

            string value = pool[index];

            int first = pool.IndexOf(value);
            pool.RemoveAt(first);
            pool.Add(value);

            return value;
        }

        public void Reset()
        {
            pool = new List<string>(initPool);
        }

        public void Save()
        {
            store.Update("pool", es.Encrypt(string.Join(",", pool)));
        }

        private static void SyncPool()
        {
            Dictionary<string, int> required = new Dictionary<string, int>();
            foreach (string name in initPool)
            {
                required[name] = required.TryGetValue(name, out int count) ? count + 1 : 1;
            }

            List<string> history = new List<string>();
            Dictionary<string, int> kept = new Dictionary<string, int>();
            foreach (string name in pool)
            {
                int keptCount = kept.TryGetValue(name, out int k) ? k : 0;
                int requiredCount = required.TryGetValue(name, out int r) ? r : 0;
                if (keptCount < requiredCount)
                {
                    history.Add(name);
                    kept[name] = keptCount + 1;
                }
            }

            List<string> fresh = new List<string>();
            Dictionary<string, int> added = new Dictionary<string, int>();
            foreach (string name in initPool)
            {
                int keptCount = kept.TryGetValue(name, out int k) ? k : 0;
                int addedCount = added.TryGetValue(name, out int a) ? a : 0;
                int requiredCount = required.TryGetValue(name, out int r) ? r : 0;
                if (keptCount + addedCount < requiredCount)
                {
                    fresh.Add(name);
                    added[name] = addedCount + 1;
                }
            }

            pool = new List<string>(fresh);
            pool.AddRange(history);
        }

        private double NextGaussian(double mean, double stddev)
        {
            if (stddev <= 0.0)
            {
                return mean;
            }

            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double standardNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stddev * standardNormal;
        }
    }
}