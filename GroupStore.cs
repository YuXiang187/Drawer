using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Drawer
{
    internal static class GroupStore
    {
        public const string DefaultGroupName = "默认";
        private const string SamplePool = "OMpOcezBBlbG3U4oQTaooNuDCgXUQzz74B7FN6IAzE8=";
        private const int PoolFlushDelayMs = 2000;

        private static readonly KeyValueStore store = new KeyValueStore();
        private static readonly EncryptString es = new EncryptString();

        private static readonly Dictionary<string, List<string>> initCache = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, List<string>> poolCache = new Dictionary<string, List<string>>();
        private static readonly HashSet<string> dirtyPools = new HashSet<string>();
        private static readonly object cacheLock = new object();
        private static Timer flushTimer;

        static GroupStore()
        {
            MigrateLegacyListIfNeeded();
        }

        private static void MigrateLegacyListIfNeeded()
        {
            if (GetGroupIds().Count > 0)
            {
                return;
            }

            string legacyInitPool = store.Get("initPool");
            string legacyPool = store.Get("pool");

            store.Update("Groups", "g0");
            store.Update("CurrentGroup", "g0");
            store.Update("NextId", "1");
            store.Update("name@g0", DefaultGroupName);
            store.Update("initPool@g0", legacyInitPool ?? SamplePool);
            store.Update("pool@g0", legacyPool ?? legacyInitPool ?? SamplePool);

            // remove legacy keys once migrated
            if (legacyInitPool != null)
            {
                store.Delete("initPool");
            }
            if (legacyPool != null)
            {
                store.Delete("pool");
            }
        }

        // reads a group into the cache
        public static void Preload(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }
            GetInitReference(id);
            GetPoolForUpdate(id);
        }

        // the ids in display order
        public static List<string> GetGroupIds()
        {
            return Split(store.Get("Groups"));
        }

        public static string GetGroupName(string id)
        {
            return store.Get("name@" + id) ?? string.Empty;
        }

        public static string GetActiveGroupId()
        {
            List<string> ids = GetGroupIds();
            if (ids.Count == 0)
            {
                return string.Empty;
            }
            string active = store.Get("CurrentGroup");
            return ids.Contains(active) ? active : ids[0];
        }

        public static void SetActiveGroup(string id)
        {
            if (GetGroupIds().Contains(id))
            {
                store.Update("CurrentGroup", id);
            }
        }

        // shared name list, must not be modified by the caller
        public static List<string> GetInitReference(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new List<string>();
            }
            lock (cacheLock)
            {
                List<string> init;
                if (!initCache.TryGetValue(id, out init))
                {
                    init = Split(es.Decrypt(store.Get("initPool@" + id) ?? ""));
                    initCache[id] = init;
                }
                return init;
            }
        }

        // writes the name list and rebuilds the draw history
        public static void SetInit(string id, List<string> names)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            List<string> clean = new List<string>(names);
            List<string> pool;
            lock (cacheLock)
            {
                pool = SyncPool(clean, LoadPool(id));
                initCache[id] = clean;
                poolCache[id] = pool;
                dirtyPools.Remove(id);
            }
            store.Update("initPool@" + id, es.Encrypt(string.Join(",", clean)));
            store.Update("pool@" + id, es.Encrypt(string.Join(",", pool)));
        }

        // shared draw history, must not be modified by the caller
        public static List<string> GetPoolReference(string id)
        {
            lock (cacheLock)
            {
                return LoadPool(id);
            }
        }

        // shared draw history, mutated in place by StringPool
        public static List<string> GetPoolForUpdate(string id)
        {
            lock (cacheLock)
            {
                List<string> pool = LoadPool(id);
                if (pool.Count == 0)
                {
                    List<string> init = GetInitReference(id);
                    if (init.Count > 0)
                    {
                        pool = new List<string>(init);
                        poolCache[id] = pool;
                    }
                }
                return pool;
            }
        }

        private static List<string> LoadPool(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new List<string>();
            }
            List<string> pool;
            if (!poolCache.TryGetValue(id, out pool))
            {
                pool = Split(es.Decrypt(store.Get("pool@" + id) ?? ""));
                poolCache[id] = pool;
            }
            return pool;
        }

        // moves the drawn entry to the end and marks the history dirty, the pool must come from GetPoolForUpdate
        internal static string MoveDrawnToEnd(string id, List<string> pool, int index)
        {
            lock (cacheLock)
            {
                string value = pool[index];
                pool.RemoveAt(index);
                pool.Add(value);
                dirtyPools.Add(id);
                ScheduleFlush();
                return value;
            }
        }

        // the history is written back in batches, at most one write per interval
        private static void ScheduleFlush()
        {
            if (flushTimer == null)
            {
                flushTimer = new Timer(state => FlushPools(), null, PoolFlushDelayMs, Timeout.Infinite);
            }
            else
            {
                flushTimer.Change(PoolFlushDelayMs, Timeout.Infinite);
            }
        }

        // writes every changed history back in one pass
        public static void FlushPools()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            lock (cacheLock)
            {
                foreach (string id in dirtyPools)
                {
                    List<string> pool;
                    values["pool@" + id] = poolCache.TryGetValue(id, out pool) ? string.Join(",", pool) : "";
                }
                dirtyPools.Clear();
            }
            foreach (KeyValuePair<string, string> entry in values)
            {
                store.Update(entry.Key, es.Encrypt(entry.Value));
            }
        }

        // drops the cached lists of a group that is gone, so they are never written back
        private static void DiscardGroup(string id)
        {
            lock (cacheLock)
            {
                initCache.Remove(id);
                poolCache.Remove(id);
                dirtyPools.Remove(id);
            }
        }

        public static int GetGroupCount()
        {
            return GetGroupIds().Count;
        }

        // creates a group seeded with the sample names, returns its id
        public static string AddGroup(string name)
        {
            int next = int.TryParse(store.Get("NextId"), out int n) ? n : 1;
            string id = "g" + next;

            List<string> ids = GetGroupIds();
            ids.Add(id);

            store.Update("Groups", string.Join(",", ids));
            store.Update("NextId", (next + 1).ToString());
            store.Update("name@" + id, name);
            store.Update("initPool@" + id, SamplePool);
            store.Update("pool@" + id, SamplePool);

            List<string> sample = Split(es.Decrypt(SamplePool));
            lock (cacheLock)
            {
                initCache[id] = sample;
                poolCache[id] = new List<string>(sample);
                dirtyPools.Remove(id);
            }
            return id;
        }

        public static void RenameGroup(string id, string name)
        {
            if (GetGroupIds().Contains(id))
            {
                store.Update("name@" + id, name);
            }
        }

        // removes a group, refuses to remove the last remaining one
        public static void DeleteGroup(string id)
        {
            List<string> ids = GetGroupIds();
            if (ids.Count <= 1 || !ids.Remove(id))
            {
                return;
            }

            string active = GetActiveGroupId();
            DiscardGroup(id);

            store.Update("Groups", string.Join(",", ids));
            store.Delete("name@" + id);
            store.Delete("initPool@" + id);
            store.Delete("pool@" + id);
            if (id == active)
            {
                store.Update("CurrentGroup", ids[0]);
            }
        }

        public static string FindGroupIdByName(string name)
        {
            string wanted = (name ?? string.Empty).Trim();
            foreach (string id in GetGroupIds())
            {
                if (string.Equals(GetGroupName(id).Trim(), wanted, StringComparison.OrdinalIgnoreCase))
                {
                    return id;
                }
            }
            return null;
        }

        // keeps the draw history order of the names that are still listed, appends new names first
        private static List<string> SyncPool(List<string> init, List<string> history)
        {
            Dictionary<string, int> required = new Dictionary<string, int>();
            foreach (string name in init)
            {
                required[name] = required.TryGetValue(name, out int count) ? count + 1 : 1;
            }

            List<string> keptHistory = new List<string>();
            Dictionary<string, int> kept = new Dictionary<string, int>();
            foreach (string name in history)
            {
                int keptCount = kept.TryGetValue(name, out int k) ? k : 0;
                int requiredCount = required.TryGetValue(name, out int r) ? r : 0;
                if (keptCount < requiredCount)
                {
                    keptHistory.Add(name);
                    kept[name] = keptCount + 1;
                }
            }

            List<string> fresh = new List<string>();
            Dictionary<string, int> added = new Dictionary<string, int>();
            foreach (string name in init)
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

            List<string> merged = new List<string>(fresh);
            merged.AddRange(keptHistory);
            return merged;
        }

        private static List<string> Split(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new List<string>();
            }
            return value.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToList();
        }
    }
}
