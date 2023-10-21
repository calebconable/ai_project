using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VS_Project.Extentions;
using VS_Project.Model;
using VS_Project.Singletone;

namespace VS_Project.Algorithms
{
    public class DesicionTree : IAlgorithm
    {
        public Guid ID = Guid.NewGuid();
        public double GiniImpuritySignificantDescreaseThreshold { get; set; }
        public int MaxTreeDepth { get; set; }
        public int MinSampleSizePerLeaf { get; set; }
        public Node RootNode { get; set; }
        [JsonIgnore]
        private Sample[] BootstrapSample;
        private DesicionTree() { }  
        public static DesicionTree New(Sample[] bootstampSamples) {
            return new DesicionTree()
            {
                BootstrapSample = bootstampSamples
            };
        }  

        public static DesicionTree Open() => FileExtentions.LoadModelDynamic<DesicionTree>(nameof(DesicionTree));
        public void Save()
        {
            var formattedDateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"); // date and time with safe characters
            var fileName = $"{formattedDateTime} (GISDT = {GiniImpuritySignificantDescreaseThreshold}, MaxTD = {MaxTreeDepth}, MinSSPL = {MinSampleSizePerLeaf}).json"; // using .json extension
            string jsonContent = JsonConvert.SerializeObject(this, JsonSerializerSignletone.SETTINGS);
            FileExtentions.SaveFile(nameof(DesicionTree), fileName, jsonContent);
        }

        private string[] RandomFeatures()
        {
            var features = Classification.ATTREBUTES_TO_INCLUDE.ToList();
            features.Remove(Classification.CLASS_ATTR_NAME);
            var randFeatureSize = (int)Math.Sqrt(features.Count);
            return features.ToRandomStrings(randFeatureSize).ToArray();
        }

        public void BuildTree(int maxTreeDepth, int minSampleSizePerLeaf, double giniImpuritySignificantDescreaseThreshold = double.NaN, Action doneAction = null)
        {
            GiniImpuritySignificantDescreaseThreshold = giniImpuritySignificantDescreaseThreshold;
            MaxTreeDepth = maxTreeDepth;
            MinSampleSizePerLeaf = minSampleSizePerLeaf;
            
            RootNode = Node.NewRoot(BootstrapSample);
            string[] selectedFeatures = RandomFeatures();
            RootNode.CreateSplits(selectedFeatures, StopCriteria);

            Thread thed = new Thread(() =>
            {
                while (Node.ActiveNodesCount == 0) ;
                while (Node.ActiveNodesCount > 0)
                {
                    // Console.WriteLine($"Decision Tree ({ID}) has {Node.ActiveNodesCount} active nodes...");
                    Thread.Sleep(1000);
                }
                doneAction?.Invoke();
            });
            thed.Start();
        }

        private bool StopCriteria(int depth, double giniImpurityDescrese, int nodeSize)
        {
            if (GiniImpuritySignificantDescreaseThreshold != double.NaN && giniImpurityDescrese <= GiniImpuritySignificantDescreaseThreshold)
                return true;
            else if (depth >= MaxTreeDepth)
                return true;
            else if(nodeSize <= MinSampleSizePerLeaf)
                return true;
            return false;
        }

        public async Task<Classification[]> EvaluateAsync(Sample[] testSamples)
        {
            List<Task<Classification>> tasks = new List<Task<Classification>>();
            foreach(Sample testSample in testSamples)
            {
                tasks.Add(Task.Run(() =>
                {
                    var predictedClass = RootNode.Evaluate(testSample);
                    return new Classification(testSample.PredifinedClassValue, predictedClass);
                }));
            }
            return await Task.WhenAll(tasks);
        }
    }
}
