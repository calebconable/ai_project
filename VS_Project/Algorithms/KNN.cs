using AI_Masters_Project;
using Deedle;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VS_Project.Extentions;
using VS_Project.Model;
using VS_Project.Singletone;

namespace VS_Project.Algorithms
{
    public class KNN : IAlgorithm
    {
        private int K;
        private KNN() { }
        public static KNN New() => new KNN();

        public void SaveModel()
        {
         
        }

        public static void TestDifferentKValues(int start, int end)
        {
            Task.Run(async () =>
            {
                Console.WriteLine("Running kNN classification rate");
                for (int i = start; i < Math.Max(start, end); i++)
                {
                    int k = i;
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    var classifications = await KNN.New().EvaluateAsync(Classification.TestSamples.ToArray());
                    stopwatch.Stop();
                    Console.WriteLine($"K: {k} Time: {stopwatch.ElapsedMilliseconds}ms \n" + classifications.ConfusionMatrixAndAccuracy());
                }
            });
        }

        public IList<Classification> Classify(int k)
        {
            List<Classification> classifications = new List<Classification>();  
            foreach (Sample testSample in Classification.TestSamples)
            {
                classifications.Add(ComputeClassification(testSample, k));
            }
            return classifications;
        }

        public async Task<Classification[]> EvaluateAsync(Sample[] testSamples)
        {
            Console.Write("Provide k-NN k value = ");
            K = int.Parse(Console.ReadLine());  
            List<Task<Classification>> tasks = new List<Task<Classification>>();

            foreach (var testSample in testSamples)
            {
                tasks.Add(Task.Run(() => ComputeClassification(testSample, K)));
            }

            Classification[] results = await Task.WhenAll(tasks);

            return results.ToArray();
        }

        private Classification ComputeClassification(Sample testSample, int k) // Replace SampleType with the actual type
        {
            Sample[] classDistances = Classification.TrainingSamples.ToArray();


            // Order with closest distance first
            List<Sample> ordersKClassDistance = classDistances.OrderBy(x => x.GetDistance(testSample)).Take(k).ToList();

            /*int predictedClass = ordersKClassDistance.GroupBy(x => x.PredifinedClassValue)
                                   .OrderByDescending(g => g.Count())
                                   .First()
                                   .Key;*/

            // Get class value with the heighest sum of weigths
            int predictedClass = ordersKClassDistance.GroupBy(x => x.PredifinedClassValue)
                                                 .Select(g => new { ClassValue = g.Key, TotalWeights = g.Sum(item => item.GetWeight(testSample)) })
                                                 .OrderByDescending(g => g.TotalWeights)
                                                 .First()
                                                 .ClassValue;

            return new Classification(testSample.GetPredefinedClass(), predictedClass);
        }

    }
}
