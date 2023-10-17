using AI_Masters_Project;
using Deedle;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VS_Project.Extentions;
using VS_Project.Singletone;
using static AI_Masters_Project.KMeans;
using static Microsoft.FSharp.Core.ByRefKinds;

namespace VS_Project.Algorithms
{
    public class kMeansCaleb
    {
        private Centroid[] Centroids;
        private kMeansCalebPoint[] sampleSpace;
        private kMeansCaleb(int k)
        {
            // Creates k new clusters
            InitClusters(k);
            InitSampleSpace();
        }

        public static void TestRun(int kStart, int kEnd, float n)
        {
            Task.Run(async () =>
            {
                (int k, float accuracy, IList<Classification> classifications) choice = (0, 0, null);
                kMeansCaleb kkMeans;
                for (int k = kStart; k <= kEnd; k++)
                {
                    // Console.WriteLine($"Test kMeans Alogirthm, Classification, k = {k}");
                    kkMeans = kMeansCaleb.New(k);
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    await kkMeans.TraingModel_UntilConversion(n);
                    var classifications = await kkMeans.ClassifyAsync(Classification.TestSet);
                    stopwatch.Stop();
                    // Console.WriteLine(new string('=', 100));
                    // Console.WriteLine($"K: {k} Time: {stopwatch.ElapsedMilliseconds}ms \n" + classifications.ConfusionMatrixAndAccuracy());
                    if(choice.accuracy < classifications.DetermineAccuracy().Average(x => x.accuracy))
                    {
                        choice = (k, classifications.DetermineAccuracy().Average(x => x.accuracy), classifications);
                    }
                    Console.WriteLine($"Done with k = {k}, Time: {stopwatch.ElapsedMilliseconds} ms,\nLeader: {choice.k}\n{choice.classifications.ConfusionMatrixAndAccuracy()}");
                    // Console.WriteLine(new string('=', 100));
                }
                Console.WriteLine($"\nBest k = {choice.k} " + choice.classifications.ConfusionMatrixAndAccuracy());
            });
        }

        public static kMeansCaleb New(int k) => new kMeansCaleb(k);

        private void InitClusters(int k)
        {
            // Console.WriteLine("Initializing centroids...");
            Centroids = new Centroid[k];
            for (int i = 0; i < k; i++)
            {
                Centroids[i] = new Centroid();
            }
        }

        private void InitSampleSpace()
        {
            // Console.WriteLine("Initializing sample space...");
            var trainingSamples = Classification.TrainingSamples.Values.ToArray();
            sampleSpace = new kMeansCalebPoint[trainingSamples.Length];
            for (int i = 0;i < trainingSamples.Length; i++)
            {
                sampleSpace[i] = new kMeansCalebPoint(trainingSamples[i]);
            }
        }

        public async Task TraingModel_UntilConversion(double conversionValue)
        {
            // Assing each centroid to nearby datapoints
            if (conversionValue == 0)
                conversionValue = 0.1f;
            double previousValue = double.MaxValue;
            while (Math.Abs(conversionValue) < previousValue) {
                double newValue = await RecomputeCentroidAsync();
                if (newValue > previousValue)
                    return;
                previousValue = newValue;
                // await Task.Delay(100);  
            }
        }

        public async Task TraingModel_nAmoutOfTimes(int n = 10)
        {
            // Assing each centroid to nearby datapoints
            while (n-- > 0){
                // Console.WriteLine($"N = {n + 1}");
                await RecomputeCentroidAsync();
            }
        }

        public async Task<double> RecomputeCentroidAsync()
        {
            Centroids.ToList().ForEach(centroid =>
            {
                centroid.AssignedDataPoints.Clear();
            });
            // Console.WriteLine("Create tasks for assigning points to centroids...");
            List<Task> assignPointsToCentroidTasks = new List<Task>(sampleSpace.Length);
            foreach (var point in sampleSpace) 
            {
                assignPointsToCentroidTasks.Add(Task.Run(() =>
                {
                    Centroid closestCentroid = ClosestCluster(point).centroid;
                    // Assign that centroid to point
                    try
                    {
                        closestCentroid.AssignPoint(point);
                    }
                    catch(Exception ex) { Console.WriteLine(ex.Message); }
                }));
            }

            // Console.WriteLine("Assigning Points To Centroids....");
            await Task.WhenAll(assignPointsToCentroidTasks);
            assignPointsToCentroidTasks.Clear();
            // Console.WriteLine("Done");

            var unAssignedCentroids = Centroids.Where(x => !x.AssignedDataPoints.Any()).ToList();

            if (unAssignedCentroids.Any())
            {
                // Console.WriteLine($"Have to reInit {unAssignedCentroids.Count} centroids");
                unAssignedCentroids.ForEach(centroid =>
                {
                    // // Console.WriteLine($"Init call on {centroid.ID}");
                    centroid.Init();

                });
                return await RecomputeCentroidAsync();
            }

            // Console.WriteLine("Recompute Centroids....");
            List<Task<double>> recomputeCentroidsTasks = new List<Task<double>>(Centroids.Length);
            foreach (var centroid in Centroids)
            {
                recomputeCentroidsTasks.Add(centroid.RecomputeValue());
            }
            double[] results = await Task.WhenAll(recomputeCentroidsTasks);
            // Console.WriteLine("Done");

            for (int i = 0; i < results.Length; i++)
            {
                // // Console.WriteLine($"Cluster {i + 1}, abs differece = {results[i]}");
            }
            recomputeCentroidsTasks.Clear();
            // // Console.WriteLine($"Clusters abs SUM differece = {results.Sum()}");
            return results.Sum();
        }

        private (Centroid centroid, double distance) ClosestCluster(kMeansCalebPoint point)
        {
            (Centroid centroid, double distance) closestCentroid = (null, float.MaxValue);
            // Check which centroid is closest to the point
            foreach (var centroid in Centroids)
            {
                double distanceToCentroid = point.GetDistance(centroid);
                if (distanceToCentroid < closestCentroid.distance)
                {
                    closestCentroid = (centroid, distanceToCentroid);
                }
            }

            return closestCentroid;
        }
        private (Centroid centroid, double distance) ClosestCluster(ObjectSeries<string> point) => ClosestCluster(new kMeansCalebPoint(point)); 

        public async Task<IList<Classification>> ClassifyAsync(Frame<int, string> testData)
        {
            List<Task<Classification>> tasks = new List<Task<Classification>>();

            foreach (var testSample in testData.Rows.Values)
            {
                tasks.Add(Task.Run(() => ClassifyTestSample(testSample)));
            }

            Classification[] results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        private Classification ClassifyTestSample(ObjectSeries<string> testSample) // Replace SampleType with the actual type
        {
            var predefinedClass = testSample.GetPredefinedClass();
            var predictedClass = ClosestCluster(testSample).centroid.GetClustersClass();
            return new Classification(predefinedClass, predictedClass);
        }
    }

    public class Centroid
    {
        public Guid ID = Guid.NewGuid();
        public int ClustersClass { get; private set; }
        public static Random rnd = new Random();
        public Series<string, double> CentroidValue { get; private set; } // <attribute_name, distance>
        public List<kMeansCalebPoint> AssignedDataPoints { get; set; } 
        public Centroid() {
            // Creates a new cluster with random values for each feature
            AssignedDataPoints = new List<kMeansCalebPoint>();
            Init();
        }

        static Dictionary<string, double> attributeMax = new Dictionary<string, double>();
        static Dictionary<string, double> attributeMin = new Dictionary<string, double>();
        public void Init()
        {
            List<KeyValuePair<string, double>> series = new List<KeyValuePair<string, double>>();
            foreach (var attribute in Classification.ATTREBUTES_TO_INCLUDE)
            {
                bool minExist = attributeMin.TryGetValue(attribute, out double min);
                bool maxExist = attributeMax.TryGetValue(attribute, out double max);
                if (!minExist && !maxExist)
                {
                    var columnValues = Classification.TrainingSet.GetColumnValues(attribute);
                    min = columnValues.Max();
                    max = columnValues.Min();
                    attributeMax.Add(attribute, max);
                    attributeMin.Add(attribute, min);
                }
                var randomValue = min + (rnd.NextDouble() * (max - min));
                series.Add(new KeyValuePair<string, double>(attribute, randomValue));
            }

            CentroidValue = new Series<string, double>(
                series
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The absolute difference between previous centroid value and new centroid value</returns>
        public Task<double> RecomputeValue()
        {
            var previousCentroidSum = CentroidValue.Values.Sum(x => x);
            CentroidValue = GetCentroidValueByMeans();
            var currentCentroidSum = CentroidValue.Values.Sum(x => x);
            ClustersClass = GetClustersClass();
            return Task.FromResult(Math.Abs(currentCentroidSum - previousCentroidSum));
        }

        public Series<string, double> GetCentroidValueByMeans()
        {
            for (int i = 0; i < AssignedDataPoints.Count; i++)
            {
                if (AssignedDataPoints[i] == null)
                    AssignedDataPoints.RemoveAt(i);
            }
            return AssignedDataPoints.Where(x => x.TrainingSample != null).Select(x => x.TrainingSample).ToList().CalculateAverage();
        }

        public Series<string, double> GetCentroidValueByMedoids()
        {
            List<ObjectSeries<string>> dataPoints = AssignedDataPoints.Select(x => x.TrainingSample).ToList();
            double minDistanceSum = double.MaxValue;
            ObjectSeries<string> medoid = null;

            foreach (var candidate in dataPoints)
            {
                double distanceSum = 0;

                foreach (var otherPoint in dataPoints)
                {
                    if (!ReferenceEquals(candidate, otherPoint))
                    {
                        distanceSum += candidate.EuclideanDistance(otherPoint);
                    }
                }

                if (distanceSum < minDistanceSum)
                {
                    minDistanceSum = distanceSum;
                    medoid = candidate;
                }
            }

            // Convert ObjectSeries<string> to Series<string, double>
            var medoidSeries = medoid.ToSeries(x =>
            {
                return Convert.ToDouble(x);
            });
            return medoidSeries;
        }

        public int GetClustersClass()
        {
            var @class = AssignedDataPoints
            .GroupBy(point => point.PredifinedClassValue)
            .OrderByDescending(group => group.Count())
            .First()
            .Key;
            return @class;
            /*return AssignedDataPoints
            .OrderBy(x => x.GetDistance(this))
            .FirstOrDefault()
            .PredifinedClassValue;*/
        }
        private readonly object lockObject = new object();

        internal void AssignPoint(kMeansCalebPoint point)
        {
            lock (lockObject)
            {
                AssignedDataPoints.Add(point);
            }
        }
 
    }

    public class kMeansCalebPoint
    {
        public int PredifinedClassValue { get => TrainingSample.GetPredefinedClass(); }

        public ObjectSeries<string> TrainingSample { get; private set; }

        public kMeansCalebPoint(ObjectSeries<string> trainingSample)
        {
            TrainingSample = trainingSample;
        }

        public double GetDistance(Centroid cluster)
        {
            return SampleExtention.EuclideanDistance(TrainingSample, cluster.CentroidValue.ToObjectSeries());
        }
    }
}
