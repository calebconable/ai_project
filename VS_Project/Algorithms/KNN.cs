using AI_Masters_Project;
using Deedle;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VS_Project.Extentions;
using VS_Project.Singletone;

namespace VS_Project.Algorithms
{
    public class KNN
    {
        private KNN() { }
        public static KNN New() => new KNN();

        public static void TestDifferentKValues(int start, int end)
        {
            Task.Run(async () =>
            {
                Console.WriteLine("Running kNN classification rate");
                for (int i = start; i < Math.Max(start, end); i++)
                {
                    int k = i;
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    var classifications = await KNN.New().ClassifyAsync(k);
                    stopwatch.Stop();
                    Console.WriteLine($"K: {k} Time: {stopwatch.ElapsedMilliseconds}ms \n" + classifications.ConfusionMatrixAndAccuracy());
                }
            });
        }

        public IList<Classification> Classify(int k)
        {
            List<Classification> classifications = new List<Classification>();  
            foreach (var testSample in Classification.TestSamples.Values)
            {
                classifications.Add(ComputeClassification(testSample, k));
            }
            return classifications;
        }

        public async Task<IList<Classification>> ClassifyAsync(int k)
        {
            List<Task<Classification>> tasks = new List<Task<Classification>>();

            foreach (var testSample in Classification.TestSamples.Values)
            {
                tasks.Add(Task.Run(() => ComputeClassification(testSample, k)));
            }

            Classification[] results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        private Classification ComputeClassification(ObjectSeries<string> testSample, int k) // Replace SampleType with the actual type
        {
            List<KNNPoint> classDistances = new List<KNNPoint>();
            foreach (var trainingSample in Classification.TrainingSet.Rows.Values)
            {
                classDistances.Add(new KNNPoint(trainingSample, testSample));
            }

            // Order with closest distance first
            List<KNNPoint> ordersKClassDistance = classDistances.OrderBy(x => x.Distance).Take(k).ToList();

            /*int predictedClass = ordersKClassDistance.GroupBy(x => x.PredifinedClassValue)
                                   .OrderByDescending(g => g.Count())
                                   .First()
                                   .Key;*/

            // Get class value with the heighest sum of weigths
            int predictedClass = ordersKClassDistance.GroupBy(x => x.PredifinedClassValue)
                                                 .Select(g => new { ClassValue = g.Key, TotalWeights = g.Sum(item => item.Weight) })
                                                 .OrderByDescending(g => g.TotalWeights)
                                                 .First()
                                                 .ClassValue;

            return new Classification(testSample.GetPredefinedClass(), predictedClass);
        }

    }

    public class KNNPoint
    {
        private int? predefinedClass;
        public int PredifinedClassValue
        {
            get { 
                if(predefinedClass == null)
                    predefinedClass = TrainingSample.GetPredefinedClass();
                return (int)predefinedClass; 
            }

        }
        public double Distance
        {
            get => SampleExtention.EuclideanDistance(TrainingSample, TestSample);
        }
        public double Weight
        {
            get => 1.0d.SafeDivide(Math.Sqrt(Distance));
        }
        public ObjectSeries<string> TrainingSample { get; private set; }
        public ObjectSeries<string> TestSample { get; private set; }

        public KNNPoint(ObjectSeries<string> trainingSample, ObjectSeries<string> testSample)
        {
            TrainingSample = trainingSample;
            TestSample = testSample;
        }
    }
}
