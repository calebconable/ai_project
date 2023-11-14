using AI_Masters_Project;
using Deedle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VS_Project.Singletone;
using VS_Project.Extentions;
using static Microsoft.FSharp.Core.ByRefKinds;

namespace VS_Project.Model
{
    public class CPoint : Sample
    {
        public static Random rnd = new Random();
        protected new int PredifinedClassValue;
        private CPoint(ObjectSeries<string> trainingSample) : base(trainingSample) { }
        static Dictionary<string, double> attributeMax = new Dictionary<string, double>();
        static Dictionary<string, double> attributeMin = new Dictionary<string, double>();

        public static CPoint Random(string[] features)
        {
            List<KeyValuePair<string, double>> series = new List<KeyValuePair<string, double>>();
            foreach (var attribute in features)
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

            return new CPoint(new Series<string, double>(series).ToObjectSeries());
        }

        public static CPoint Empty(string[] features)
        {
            List<KeyValuePair<string, double>> series = new List<KeyValuePair<string, double>>();
            foreach (var attribute in features)
            {
                series.Add(new KeyValuePair<string, double>(attribute, 0));
            }
            return new CPoint(new Series<string, double>(series).ToObjectSeries());
        }
        public CPoint ModifyFeatureValue(string modifyFeature, double value)
        {
            List<KeyValuePair<string, double>> series = new List<KeyValuePair<string, double>>();
            foreach (var feature in GetFeatures())
            {
                if(feature.Equals(modifyFeature))
                    series.Add(new KeyValuePair<string, double>(feature, value));
                else
                    series.Add(new KeyValuePair<string, double>(feature, GetFeatureValue(feature)));
            }

            return new CPoint(new Series<string, double>(series).ToObjectSeries());
        } 
        public double Sum() => Values.Sum(x => System.Convert.ToDouble(x));
        public double Average() => Values.Average(x => System.Convert.ToDouble(x));
    }
}
