using Deedle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VS_Project.Singletone;

namespace AI_Masters_Project
{
    public static class SampleExtention
    {
        public static int GetPredefinedClass(this ObjectSeries<string> sample)
        {
            return sample.TryGetAs<int>("fetal_health").Value;
        }

        public static ObjectSeries<string> ToObjectSeries<T>(this Series<string, T> series)
        {
            return new ObjectSeries<string>(series.SelectValues(v => (object)v));
        }

        public static Series<string, T> ToSeries<T>(this ObjectSeries<string> objectSeries, Func<object, T> converter)
        {
            return objectSeries.SelectValues(v => converter(v));
        }

        public static Series<string, double> CalculateAverage(this List<ObjectSeries<string>> listOfObjectSeries)
        {
            // Convert the list of ObjectSeries to a Frame
            List<KeyValuePair<string, double>> series = new List<KeyValuePair<string, double>>();
            if(!listOfObjectSeries.Any())
                return new Series<string, double>(
                series
            );
            foreach (var attribute in listOfObjectSeries.FirstOrDefault().Keys)
            {
                double average = listOfObjectSeries.Average(x => Convert.ToDouble(x[attribute]));

                series.Add(new KeyValuePair<string, double>(attribute, average));
            }

            return new Series<string, double>(
                series
            );
        }

        /// <summary>
        /// Calculate the Euclidean distance between two samples
        /// </summary>
        /// <param name="sample1"></param>
        /// <param name="sample2"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static double EuclideanDistance(this ObjectSeries<string> sample1, ObjectSeries<string> sample2)
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

        /// <summary>
        /// Calculates the Euclidean distance between all samples in the sample space (training set) and a single sample from
        /// a test set.
        /// </summary>
        /// <param name="trainingSampleSet"></param>
        /// <param name="testSample"></param>
        /// <returns></returns>
        public static double EuclideanDistance(this Frame<int, string> trainingSampleSet, ObjectSeries<string> testSample)
        {
            double distance = 0.0;
            foreach(var trainingSample in trainingSampleSet.Rows.Values)
            {
                distance += EuclideanDistance(trainingSample, testSample);
            }
            return distance;
        }

    }
}
