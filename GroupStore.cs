using System;
using System.Collections.Generic;
using System.Linq;

namespace Drawer
{
    internal static class GroupStore
    {
        public const string DefaultGroupName = "默认";
        private const string SamplePool = "OMpOcezBBlbG3U4oQTaooNuDCgXUQzz74B7FN6IAzE8=";

        private static readonly KeyValueStore store = new KeyValueStore();
        private static readonly EncryptString es = new EncryptString();

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

        public static List<string> GetInit(string id)
        {
            return Split(es.Decrypt(store.Get("initPool@" + id) ?? ""));
        }

        // writes the name list and rebuilds the pool
        public static void SetInit(string id, List<string> names)
        {
            List<string> clean = new List<string>(names);
            List<string> pool = StringPool.BuildSyncedPool(clean, GetPool(id));
            store.Update("initPool@" + id, es.Encrypt(string.Join(",", clean)));
            store.Update("pool@" + id, es.Encrypt(string.Join(",", pool)));
        }

        public static List<string> GetPool(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new List<string>();
            }
            return Split(es.Decrypt(store.Get("pool@" + id) ?? ""));
        }

        public static void SetPool(string id, List<string> pool)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }
            store.Update("pool@" + id, es.Encrypt(string.Join(",", pool)));
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
            return id;
        }

        public static void RenameGroup(string id, string name)
        {
            if (GetGroupIds().Contains(id))
            {
                store.Update("name@" + id, name);
            }
        }

        // removes a group, refuses to remove the last remaining one, returns true when removed
        public static bool DeleteGroup(string id)
        {
            List<string> ids = GetGroupIds();
            if (ids.Count <= 1 || !ids.Remove(id))
            {
                return false;
            }

            string active = GetActiveGroupId();

            store.Update("Groups", string.Join(",", ids));
            store.Delete("name@" + id);
            store.Delete("initPool@" + id);
            store.Delete("pool@" + id);
            if (id == active)
            {
                store.Update("CurrentGroup", ids[0]);
            }
            return true;
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