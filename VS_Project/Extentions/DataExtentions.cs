using Deedle;
using ScottPlot.Palettes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VS_Project.Extentions
{
    public static class DataExtentions
    {
        public static string ReadEmbeddedResource(string folderName, string fileName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Dynamically determine the namespace
            string defaultNamespace = assembly.GetName().Name;

            // Construct the full resource name
            string resourceName = $"{defaultNamespace}.{folderName}.{fileName}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new ArgumentException($"Cannot find resource named {resourceName}.", nameof(resourceName));
                }

                using (StreamReader reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }

        public static List<string> GetColumnAsList(this Frame<int, string> df, string columnName)
        {
            // Extract the column from the dataframe
            Series<int, string> column = df.GetColumn<string>(columnName);

            // Convert the series to a list and return
            return new List<string>(column.Values);
        }

        public static List<float> GetColumnValues(this Frame<int, string> df, string columnName)
        {
            // Extract the column from the dataframe
            Series<int, float> column = df.GetColumn<float>(columnName);

            // Convert the series to a list and return
            return new List<float>(column.Values);
        }

        public static Frame<int, string> ReadTraningSet(Action<Frame<int, string>> action = null)
        {
            var csvContent = ReadEmbeddedResource("Data", "training_set.csv");
            // Convert the CSV string into a MemoryStream
            using (MemoryStream memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvContent)))
            {
                // Use Deedle to read the CSV content from the MemoryStream
                var df = Frame.ReadCsv(memoryStream, true, separators: ",");
                action?.Invoke(df);
                return df;
            }
        }
        public static Frame<int, string> ReadTestSet(Action<Frame<int, string>> action = null)
        {
            var csvContent = ReadEmbeddedResource("Data", "test_set.csv");
            // Convert the CSV string into a MemoryStream
            using (MemoryStream memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvContent)))
            {
                // Use Deedle to read the CSV content from the MemoryStream
                var df = Frame.ReadCsv(memoryStream, true, separators: ",");
                action?.Invoke(df);
                return df;
            }
        }


        public static void ExcludeAttributes(this Frame<int, string> df, string[] attributesToExclude)
        {

            // Drop specified columns
            foreach (var attr in attributesToExclude)
            {
                if (df.ColumnKeys.Contains(attr))
                {
                    // Drop column with the specified attribute
                    df.DropColumn(attr);
                }
            }
        }

        public static void IncludeAttributes(this Frame<int, string> df, string[] attributesToInclude)
        {
            // List of columns to drop
            List<string> columnsToDrop = new List<string>();

            // Identify columns that are not in the attributesToInclude list
            foreach (var columnKey in df.ColumnKeys)
            {
                if (!attributesToInclude.Contains(columnKey))
                {
                    columnsToDrop.Add(columnKey);
                }
            }

            // Drop identified columns
            foreach (var attr in columnsToDrop)
            {
                df.DropColumn(attr);
            }
        }


        private static Random rng = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {

            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        #if DEBUG
        public static void SaveStratifiedSetsAsCSV(double trainFraction = 0.8)
        {
            var csvContent = ReadEmbeddedResource("Files", "fetal_health.csv");
            var rows = csvContent.Split('\r','\n');
            var header = rows[0];
            var columns = header.Split(',');
            Dictionary<double, List<string>> groupedClasses = new Dictionary<double, List<string>>();
            for(int i = 1;  i < rows.Length - 1; i++)
            {
                double classValue = double.Parse(rows[i].Split(',').Last(), NumberStyles.Any, CultureInfo.InvariantCulture);
                if(groupedClasses.TryGetValue(classValue, out var tuples))
                {
                    tuples.Add(rows[i]);
                }
                else
                    groupedClasses.Add(classValue, new List<string>() { rows[i] });
            }

            List<string> trainingSet = new List<string>();
            List<string> testSet = new List<string>();
            trainingSet.Insert(0, header);
            testSet.Insert(0, header);
            // Shuffle
            foreach (var group in groupedClasses.Values)
            {
                group.Shuffle();
                var trainingSlice = group.Take((int)(group.Count * trainFraction));
                var testSlice = group.Skip((int)(group.Count * trainFraction));
                trainingSet.AddRange(trainingSlice);
                testSet.AddRange(testSlice);
            }

            File.WriteAllLines("E:\\NewDocuments\\1. Test Folder\\ai_project\\VS_Project\\Files\\training_set.csv", trainingSet.ToArray());
            File.WriteAllLines("E:\\NewDocuments\\1. Test Folder\\ai_project\\VS_Project\\Files\\test_set.csv", testSet.ToArray());
        }
        #endif

    }
}
