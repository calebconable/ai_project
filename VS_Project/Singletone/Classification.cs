using AI_Masters_Project;
using Deedle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VS_Project.Extentions;
using VS_Project.Model;

namespace VS_Project.Singletone
{
    public class Classification
    {
        public readonly static string CLASS_ATTR_NAME = "fetal_health";
        public readonly static string[] ATTREBUTES_TO_INCLUDE = new string[] { "prolongued_decelerations", "abnormal_short_term_variability", "percentage_of_time_with_abnormal_long_term_variability", "accelerations", "fetal_health" };
        public static Frame<int, string> TrainingSet => DataExtentions.ReadTraningSet((df) =>
        {
            DataExtentions.IncludeAttributes(df, ATTREBUTES_TO_INCLUDE);
        });
        public static Frame<int, string> TestSet => DataExtentions.ReadTestSet((df) =>
        {
            DataExtentions.IncludeAttributes(df, ATTREBUTES_TO_INCLUDE);
        });
        public static List<Sample> TrainingSamples => TrainingSet.ToSampleList();
        public static List<Sample> TestSamples => TestSet.ToSampleList();
        public static IEnumerable<int> PossibleClassValues => TrainingSet.GetColumnAsList(CLASS_ATTR_NAME).Select(x => (int)double.Parse(x));
        public int PredefinedClass { get; private set; }
        public int PredictedClass { get; private set; }
        public bool IsCorrectlyClassified => PredictedClass == PredefinedClass;
        public Classification(int predefined, int classified)
        {
            PredefinedClass = predefined;
            PredictedClass = classified;
        }

        public bool TruePositive(int predictedClass) => predictedClass == PredefinedClass;
        public bool TrueNegative(int predictedClass) => predictedClass == PredefinedClass;
        public bool FalsePositive() => PredictedClass != PredefinedClass;
        public bool FalseNegative() => PredictedClass != PredefinedClass;

        public override string ToString()
        {
            return $"Classification of {CLASS_ATTR_NAME} is {IsCorrectlyClassified}";
        } 
        
        public string ToString(string additionalString)
        {
            return $"Classification of {CLASS_ATTR_NAME} ({additionalString}) is {IsCorrectlyClassified}";
        }

        public static Dictionary<int, T> NewClassDictionary<T>()
        {
            var dict = new Dictionary<int, T>(3);
            dict.Add(1, default(T));
            dict.Add(2, default(T));
            dict.Add(3, default(T));
            return dict;
        }



    }
}
