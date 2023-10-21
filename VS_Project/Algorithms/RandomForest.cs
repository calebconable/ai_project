using Deedle;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VS_Project.Extentions;
using VS_Project.Model;
using VS_Project.Singletone;

namespace VS_Project.Algorithms
{
    public class RandomForest : IAlgorithm
    {
        private static Random _random = new Random();
        public DesicionTree[] BootstrapDecisionTreees { get; set; }
        public int BootstrapSampleSize { get; set; }

        private RandomForest() { }

        private RandomForest(int bootstrapSampleSize)
        {
            BootstrapSampleSize = bootstrapSampleSize;
            BootstrapDecisionTreees = new DesicionTree[bootstrapSampleSize];
        }

        public static RandomForest Open() => FileExtentions.LoadModelDynamic<RandomForest>(nameof(RandomForest));
    
        public void Save()
        {
            var formattedDateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"); // date and time with safe characters
            var fileName = $"{formattedDateTime} (Size = {BootstrapSampleSize}).json"; // using .json extension
            string jsonContent = JsonConvert.SerializeObject(this, JsonSerializerSignletone.SETTINGS);
            FileExtentions.SaveFile(nameof(RandomForest), fileName, jsonContent);
        }

        public static RandomForest New(int bootstrapSampleSize) => new RandomForest(bootstrapSampleSize);

        public Task PerformBootstrapSamplingAsync(Sample[] trainingSample)
        {
            return Task.Run(() =>
            {
                Parallel.For(0, BootstrapDecisionTreees.Length, index =>
                {
                    BootstrapDecisionTreees[index] = DesicionTree.New(CreateBootstrapSample(trainingSample));
                    Console.WriteLine($"Created ({index}) Decision Tree");
                });
            });
        }


        public static Sample[] CreateBootstrapSample(Sample[] samples)
        {
            Sample[] bootstrapSamples = new Sample[samples.Length];
            for(int i = 0; i < bootstrapSamples.Length; i++)
            {
                bootstrapSamples[i] = samples[_random.Next(samples.Length - 1)];
            }
            return bootstrapSamples;
        }

        public int ActiveDecisionTreeCount = 0;
        public void Build(int maxTreeDepth, int minSampleSizePerLeaf, double giniImpuritySignificantDescreaseThreshold = double.NaN, Action doneAction = null)
        {
            List<Thread> treeThreads = new List<Thread>();
            foreach (var decisionTree in BootstrapDecisionTreees)
            {
                Thread thread = new Thread(() =>
                {
                    Console.WriteLine($"Bootstrap sample ({decisionTree.ID}) - START BUILDING");
                    ActiveDecisionTreeCount++;
                    decisionTree.BuildTree(maxTreeDepth, minSampleSizePerLeaf, giniImpuritySignificantDescreaseThreshold, () =>
                    {
                        Console.WriteLine($"Bootstrap sample ({decisionTree.ID}) - FINISHED BUILDING");
                        ActiveDecisionTreeCount--;
                    });
                });
                thread.Start();
                treeThreads.Add(thread);
            }

            Thread backgroundThread = new Thread(() =>
            {
                Thread.Sleep(500);
                while (ActiveDecisionTreeCount > 0)
                {
                    Console.WriteLine($"Random Forest has {ActiveDecisionTreeCount} active nodes...");
                    Thread.Sleep(1000);
                }
                doneAction?.Invoke();
            });
            backgroundThread.Start();

            foreach (var treeThread in treeThreads)
            {
                treeThread.Join();
            }
        }

        public async Task<Classification[]> EvaluateAsync(Sample[] testSamples)
        {
            List<Task<Classification>> tasks = new List<Task<Classification>>();
            foreach (Sample testSample in testSamples)
            {
                tasks.Add(Task.Run(() =>
                {
                    int[] predictions = new int[BootstrapDecisionTreees.Length];
                    int i = 0;
                    foreach (var bootstrapSample in BootstrapDecisionTreees)
                    {
                        var predictedClass = bootstrapSample.RootNode.Evaluate(testSample);
                        predictions[i++] = predictedClass;
                    }
                    int majorityVotes = predictions.GroupBy(n => n)
                          .OrderByDescending(g => g.Count())
                          .Select(g => g.Key)
                          .First();
                    return new Classification(testSample.PredifinedClassValue, majorityVotes);
                }));
            }
            return await Task.WhenAll(tasks);
        }
    }
}
