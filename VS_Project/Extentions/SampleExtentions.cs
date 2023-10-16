using Deedle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Masters_Project
{
    public static class SampleExtention
    {
        public static int GetPredefinedClass(this ObjectSeries<string> sample)
        {
            return sample.TryGetAs<int>("fetal_health").Value;
        }
        public static double Distance(this ObjectSeries<string> sample1, ObjectSeries<string> sample2)
        {
            if (sample1 == null || sample2 == null)
                throw new ArgumentNullException("Samples cannot be null.");

            if (sample1.KeyCount != sample2.KeyCount)
                throw new ArgumentException("Samples must have the same number of attributes.");

            double sum = 0.0;

            foreach (var key in sample1.Keys)
            {
                if (sample2.ContainsKey(key))
                {
                    double val1 = Convert.ToDouble(sample1[key]);
                    double val2 = Convert.ToDouble(sample2[key]);
                    sum += Math.Pow(val1 - val2, 2);
                }
                else
                {
                    throw new ArgumentException($"Key {key} is not present in both samples.");
                }
            }

            return Math.Sqrt(sum);
        }

    }
}
