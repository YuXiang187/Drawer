using System;
using System.Collections.Generic;

namespace Drawer
{
    internal class StringPool
    {
        private const double StddevScalingFactor = 3.0;

        // width estimates measure this many names instead of the whole list
        internal const int SampleCount = 64;

        private readonly Random random = new Random();

        // draws one name from the active group using the gaussian model
        public string Draw()
        {
            string id = GroupStore.GetActiveGroupId();
            List<string> pool = GroupStore.GetPoolForUpdate(id);
            if (pool.Count == 0)
            {
                return string.Empty;
            }

            // move the drawn name to the end by index instead of searching for its value
            int index = PickIndex(pool);
            return GroupStore.MoveDrawnToEnd(id, pool, index);
        }

        // used by the rolling animation
        public string Get()
        {
            string id = GroupStore.GetActiveGroupId();
            List<string> pool = GroupStore.GetPoolReference(id);
            if (pool.Count == 0)
            {
                pool = GroupStore.GetPoolForUpdate(id);
            }
            if (pool.Count == 0)
            {
                return string.Empty;
            }
            return pool[random.Next(pool.Count)];
        }

        // up to maxNames entries spread evenly over the active pool, for size estimates
        public List<string> Sample(int maxNames)
        {
            string id = GroupStore.GetActiveGroupId();
            List<string> pool = GroupStore.GetPoolReference(id);
            if (pool.Count == 0)
            {
                pool = GroupStore.GetInitReference(id);
            }

            int count = Math.Min(pool.Count, maxNames);
            List<string> result = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                result.Add(pool[(int)((long)i * pool.Count / count)]);
            }
            return result;
        }

        private int PickIndex(List<string> pool)
        {
            while (true)
            {
                double stddev = pool.Count / StddevScalingFactor;
                double sample = Math.Abs(NextGaussian(0.0, stddev));

                if (sample < pool.Count)
                {
                    return (int)sample;
                }
            }
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