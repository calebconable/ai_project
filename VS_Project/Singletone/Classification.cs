using Deedle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VS_Project.Extentions;

namespace VS_Project.Singletone
{
    public class Classification
    {
        public readonly static string CLASS_ATTR_NAME = "fetal_health";
        public static Frame<int, string> TrainingSet
        {
            get => DataExtentions.ReadTraningSet();
        }
        public static Frame<int, string> TestSet
        {
            get => DataExtentions.ReadTestSet();
        }
        public static RowSeries<int, string> TrainingSamples
        {
            get => TrainingSet.Rows;
        }
        public static RowSeries<int, string> TestSamples
        {
            get => TestSet.Rows;
        }
        public int PredefinedClass { get; private set; }
        public int ClassifiedAs { get; private set; }
        public bool IsCorrectClassification => ClassifiedAs == PredefinedClass;
        public Classification(int predefined, int classified)
        {
            PredefinedClass = predefined;
            ClassifiedAs = classified;
        }

        public override string ToString()
        {
            return $"Classification of {CLASS_ATTR_NAME} is {IsCorrectClassification}";
        } 
        
        public string ToString(string additionalString)
        {
            return $"Classification of {CLASS_ATTR_NAME} ({additionalString}) is {IsCorrectClassification}";
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
