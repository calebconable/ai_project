using Newtonsoft.Json;
using ScottPlot.Palettes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VS_Project.Extentions;
using VS_Project.Singletone;

namespace VS_Project.Model
{
    public class Node
    {
        public int Depth { get; set; }
        public int ParentSize { get; set; }
        public double ParentGiniImpurity { get; set; }
        public string ConditionFeature { get; set; }
        public double ConditionThreshold { get; set; }
        public int Size
        {
            get => Samples.Length;
        }
        public int? ClassValue
        {
            get
            {
                if (IsLeaf)
                    return (int)Math.Round(Samples.Average(x => x.PredifinedClassValue));
                return null;
            }
        }
        [JsonIgnore]
        public Sample[] Samples { get; set; } // Only the leaf nodes will have samples
        public Node LSplit { get; set; }
        public Node HSplit { get; set; }
        public bool IsLeaf { get => LSplit == null && null == HSplit && Samples.Any(); }
        public double WeightedGiniImpurity { get; set; }
        [JsonIgnore]
        public double GiniImpurity
        {
            get
            {
                double sum = 0;
                foreach (var classValue in Classification.PossibleClassValues)
                {
                    var classSize = SamplesWithPredefinedClass(classValue).Length;
                    if (Size == 0)
                        break;
                    sum += Math.Pow((double)classSize / (double)Size, 2);
                }
                return 1 - sum;
            }
        }

        public static double CalcWeightedGiniImpurity(int parentSize, Node[] nodes)
        {
            double sum = 0;
            if (nodes == null)
                return sum;

            foreach (var sample in nodes)
            {
                sum += (sample.Size * sample.GiniImpurity);
            }

            return sum / (double)parentSize;
        }

        private Node()
        {
            Samples = new Sample[0];
        }

        public static Node NewRoot(Sample[] samples)
        {
            Console.WriteLine("Created Root Node");
            return new Node()
            {
                Depth = 0,
                ParentSize = 0,
                ParentGiniImpurity = double.NaN,
                ConditionFeature = null, 
                ConditionThreshold = double.NaN,
                Samples = samples
            };
        }

        public static Node NewBranch(Node parentNode, Sample[] dataSamples, (string feature, double threshold) condition)
        {
            return new Node
            {
                Depth = parentNode.Depth + 1,
                Samples = dataSamples,
                ParentGiniImpurity = parentNode.WeightedGiniImpurity,
                ConditionFeature = condition.feature,
                ConditionThreshold = condition.threshold
            };
        }

        public Sample[] SamplesWithPredefinedClass(int predefinedClassValue)
        {
            return Samples.Where(sample => sample.GetFeatureValue(Classification.CLASS_ATTR_NAME) == predefinedClassValue).ToArray();   
        }

        private static (Sample[] left, Sample[] right) GenerateSplits(Node node, string feature, double threshold)
        {
            return (node.Samples.Where(sample => (double)sample.GetFeatureValue(feature) < threshold).ToArray(),
                node.Samples.Where(sample => (double)sample.GetFeatureValue(feature) >= threshold).ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="n">Max Possible Thresholds</param>
        /// <returns></returns>
        private double[] GetThresholds(string feature, int n = 10)
        {
            IEnumerable<float> possibleThresholds = Samples.Select(sample => sample.GetFeatureValue(feature)).Distinct();
            if(possibleThresholds.Count() > n)
            {
                return possibleThresholds.Min().ToRangeList(possibleThresholds.Max(),  n).Select(x => (double)x).ToArray();
            }
            return possibleThresholds.Select(x => (double)x).ToArray();
        }
        public static List<Node> ActiveNodes = new List<Node>();
        public async Task CreateSplits(string[] randomSelectedFeatures, Func<int, double, int, bool> stopCriteria)
        {
            try
            {
                ActiveNodes.Add(this);
                Console.WriteLine("Splitting the data...");
                // Prevent splitting if all the samples in node is only one class
                if (Samples.Select(sample => sample.GetFeatureValue(Classification.CLASS_ATTR_NAME)).Distinct().Count() <= 1)
                {
                    Console.WriteLine("Prevented splitting of node being that all samples are in same class");
                    return;
                }

                // Get splits
                List<Task<(Node[] splits, double giniImpurity)>> splitsTasks = new List<Task<(Node[] splits, double giniImpurity)>>();
                foreach (var feature in randomSelectedFeatures)
                {
                    double[] thresholds = GetThresholds(feature);
                    foreach (var threshold in thresholds)
                    {
                        splitsTasks.Add(Task.Run(() =>
                        {
                            // Generate splits
                            var splits = GenerateSplits(this, feature, threshold);
                            // Replace split
                            var nodeSplits = new Node[2]
                            {
                                NewBranch(this, splits.left, (feature, threshold)),
                                NewBranch(this, splits.right, (feature, threshold))
                            };
                            var giniImpurity = CalcWeightedGiniImpurity(Size, nodeSplits);
                            Console.WriteLine($"Node Splits ({Depth})\n Feature: {feature}\tThreshold: {threshold}\t: GINI: {giniImpurity}");
                            return (nodeSplits, giniImpurity);
                        }));
                    }
                }

                var result = await Task.WhenAll(splitsTasks.ToArray());
                (Node[] splits, double giniImpurity) bestSplit = result.FirstOrDefault(x => x.giniImpurity == result.Min(r => r.giniImpurity));
                LSplit = bestSplit.splits.FirstOrDefault();
                HSplit = bestSplit.splits.LastOrDefault();
                WeightedGiniImpurity = bestSplit.giniImpurity;
                LSplit.ParentSize = HSplit.ParentSize = Size;
                LSplit.ParentGiniImpurity = HSplit.ParentGiniImpurity = WeightedGiniImpurity;

                if (LSplit.Size <= 0)
                    LSplit = null;
                else
                    Console.WriteLine($"New Branches with < splits");
                if (HSplit.Size <= 0)
                    HSplit = null;
                else
                    Console.WriteLine($"New Branches with > splits");



                Console.WriteLine($"Lowest Gini Index Depth ({Depth})\tSplits:{bestSplit.splits.Length}\tGINI: {bestSplit.giniImpurity}");
                if (LSplit == HSplit)
                    return;


                // Check for stop criteria
                if (stopCriteria != null && ParentGiniImpurity != double.NaN && stopCriteria.Invoke(Depth, Math.Abs(GiniImpurity - ParentGiniImpurity), Size))
                {
                    Console.WriteLine($"Stop sploting Node ({Depth}). Stop Criteria succeeded.");
                    return;
                }
                // If stop criteria is not met, then continue to root nodes
                Thread thread = new Thread(() =>
                {
                    foreach (Node node in bestSplit.splits.Where(x => x.Size != 0))
                    {
                        node.ParentSize = Size;
                        node.ParentGiniImpurity = WeightedGiniImpurity;
                        node.CreateSplits(randomSelectedFeatures, stopCriteria);
                    }
                });
                thread.Start();
                
                // If adding child nodes CLEAR samples
            }
            finally
            {
                ActiveNodes.Remove(this);
            }
        }
    }
}
