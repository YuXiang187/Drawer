using System;
using System.Collections.Generic;
using System.Linq;

namespace Drawer
{
    internal class StringPool
    {
        private readonly KeyValueStore store;
        public static List<string> initPool;
        public static List<string> pool;
        private readonly EncryptString es;
        private readonly Random random;

        public StringPool()
        {
            store = new KeyValueStore();
            es = new EncryptString();
            pool = new List<string>(es.Decrypt(store.Get("pool")).Split(','));
            initPool = new List<string>(es.Decrypt(store.Get("initPool")).Split(','));
            random = new Random();

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
            return pool[random.Next(pool.Count())];
        }

        public void Reset()
        {
            pool = new List<string>(initPool);
        }

        public void Remove(string value)
        {
            pool.Remove(value);
        }

        public void Save()
        {
            store.Update("pool", es.Encrypt(string.Join(",", pool)));
        }
    }
}