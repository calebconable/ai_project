using AI_Masters_Project;
using Deedle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VS_Project.Algorithms;
using VS_Project.Extentions;

namespace VS_Project.Model
{
    public class Sample : ObjectSeries<string>
    {
        public int PredifinedClassValue { get => this.GetPredefinedClass(); }


        public Sample(ObjectSeries<string> trainingSample) : base(trainingSample) { }

        public double GetDistance(Sample sample) => SampleExtention.EuclideanDistance(this, sample);
        public double GetWeight(Sample sample) => 1.0d.SafeDivide(Math.Sqrt(GetDistance(sample)));
        public float GetFeatureValue(string featureName) => (float)System.Convert.ToDouble(this[featureName]);
        public string[] GetFeatures() => this.Keys.ToArray();

        public double GetDistanceToCentroid(Centroid cluster)
        {
            return SampleExtention.EuclideanDistance(this, (Sample)cluster.Point.ToObjectSeries());
        }

        public Dictionary<string, object> ToJson()
        {
            var json = new Dictionary<string, object>();

            foreach (var feature in GetFeatures())
            {
                json[feature] = GetFeatureValue(feature);
            }

            return json;
        }

        public static Sample FromJson(Dictionary<string, object> json)
        {
            var features = json.Keys.ToArray();
            var series = new List<KeyValuePair<string, double>>();

            foreach (var feature in features)
            {
                if (json.TryGetValue(feature, out var featureValueObj) && featureValueObj is double featureValue)
                {
                    series.Add(new KeyValuePair<string, double>(feature, featureValue));
                }
            }

            return new Sample(new Series<string, double>(series).ToObjectSeries());
        }
    }
}
