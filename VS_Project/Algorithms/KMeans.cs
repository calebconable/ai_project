using AI_Masters_Project;
using Deedle;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VS_Project.Extentions;
using VS_Project.Model;
using VS_Project.Singletone;

namespace VS_Project.Algorithms
{
    public class kMeans
    {
        public Centroid[] Centroids { get; set; }
        [JsonIgnore]
        private Sample[] SampleSpace { get; set; }
        [JsonIgnore]
        private int K { get; set; }
        private kMeans() { }

        public static kMeans LoadModel(string name)
        {
            var folderPath = Path.GetDirectoryName("../../AI_Models/kMeans");
            var filePath = Path.Combine(folderPath, $"{name}.json");
            var jsonContent = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<kMeans>(jsonContent);
        }

        public void SaveModel()
        {
            var formattedDateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"); // date and time with safe characters
            var fileName = $"{formattedDateTime} (Model k = {K}).json"; // using .json extension
            string jsonContent = JsonConvert.SerializeObject(this, JsonSerializerSignletone.SETTINGS);
            FileExtentions.SaveFile("kMeans", fileName, jsonContent);
        }

        public static void TestRun(int kStart, int kEnd, float n)
        {
            Task.Run(async () =>
            {
                (int k, float accuracy, IList<Classification> classifications) choice = (0, 0, null);
                kMeans kkMeans;
                for (int k = kStart; k <= kEnd; k++)
                {
                    // Console.WriteLine($"Test kMeans Alogirthm, Classification, k = {k}");
                    kkMeans = kMeans.New(k);
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    await kkMeans.TraingModel_UntilConversionAsync(n);
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

        public static kMeans New(int k)
        {
            kMeans kMeans = new kMeans()
            {
                SampleSpace = Classification.TrainingSamples.ToArray(),
                K = k
            };
            kMeans.InitClusters(k);
            return kMeans;
        }

        private void InitClusters(int k)
        {
            // Console.WriteLine("Initializing centroids...");
            Centroids = new Centroid[k];
            for (int i = 0; i < k; i++)
            {
                Centroids[i] = new Centroid();
            }
        }

        public kMeans TraingModel_UntilConversion(double conversionValue)
        {
            Task.Run(async () =>
            {
                await TraingModel_UntilConversionAsync(conversionValue);
            }).GetAwaiter();
            return this;
        }
        public async Task TraingModel_UntilConversionAsync(double conversionValue)
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
            List<Task> assignPointsToCentroidTasks = new List<Task>(SampleSpace.Length);
            foreach (var point in SampleSpace) 
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

        private (Centroid centroid, double distance) ClosestCluster(Sample point)
        {
            (Centroid centroid, double distance) closestCentroid = (null, float.MaxValue);
            // Check which centroid is closest to the point
            foreach (var centroid in Centroids)
            {
                double distanceToCentroid = point.GetDistance(centroid.Point);
                if (distanceToCentroid < closestCentroid.distance)
                {
                    closestCentroid = (centroid, distanceToCentroid);
                }
            }

            return closestCentroid;
        }
        private (Centroid centroid, double distance) ClosestCluster(ObjectSeries<string> point) => ClosestCluster(new Sample(point)); 

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
        public CPoint Point { get; private set; } // <attribute_name, distance>
        [JsonIgnore]
        public List<Sample> AssignedDataPoints { get; set; } 
        public Centroid() {
            // Creates a new cluster with random values for each feature
            AssignedDataPoints = new List<Sample>();
            Point = CPoint.Random(Classification.ATTREBUTES_TO_INCLUDE);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The absolute difference between previous centroid value and new centroid value</returns>
        public Task<double> RecomputeValue()
        {
            var previousCentroidSum = Point.Sum();
            Point = GetCentroidValueByMeans();
            var currentCentroidSum = Point.Sum();
            ClustersClass = GetClustersClass();
            return Task.FromResult(Math.Abs(currentCentroidSum - previousCentroidSum));
        }

        public CPoint GetCentroidValueByMeans()
        {
            for (int i = 0; i < AssignedDataPoints.Count; i++)
            {
                if (AssignedDataPoints[i] == null)
                    AssignedDataPoints.RemoveAt(i);
            }
            return AssignedDataPoints.Where(x => x != null).ToList().CalculateAverage().ToCPoint();
        }

        public CPoint GetCentroidValueByMedoids()
        {
            List<Sample> dataPoints = AssignedDataPoints.ToList();
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
            return medoidSeries.ToCPoint();
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

        internal void AssignPoint(Sample point)
        {
            lock (lockObject)
            {
                AssignedDataPoints.Add(point);
            }
        }
 
    }
}
