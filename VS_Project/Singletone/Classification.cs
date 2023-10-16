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
        public Frame<int, string> TrainingSet => DataExtentions.ReadTraningSet();
        public Frame<int, string> TestSet => DataExtentions.ReadTestSet();
        public RowSeries<int, string> TrainingSamples => TrainingSet.Rows;
        public RowSeries<int, string> TestSamples => TestSet.Rows;
        private static Classification classification;

        public Classification()
        {

        }

        public static Classification Instance
        {
            get {
                if (classification == null)
                {
                    classification = new Classification();
                }
                return classification;
            }
        }

    }
}
