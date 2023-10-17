using AI_Masters_Project;
using Deedle;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using VS_Project.Algorithms;
using VS_Project.Extentions;
using VS_Project.Forms;
using VS_Project.Singletone;

namespace VS_Project
{
    internal static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            RunConsoleTestApp(() =>
            {
                KNN.TestDifferentKValues(10, 20);
            });
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        
        static void RunConsoleTestApp(Action customActionToRun = null)
        {
            bool consoleAllocated = AllocConsole();
            if (!consoleAllocated)
            {
                MessageBox.Show("Failed to allocate console!");
                return;
            }

            customActionToRun?.Invoke();
            Console.ReadKey(true);
        }
        

        static void DetermineDistributionOfDataset()
        {
            var frame = DataExtentions.ReadTraningSet();

            Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();
            foreach (var classValue in frame.GetColumnAsList("fetal_health"))
            {
                if (keyValuePairs.TryGetValue(classValue, out var value))
                {
                    keyValuePairs[classValue]++;
                    continue;
                }
                keyValuePairs.Add(classValue, 1);
            }
            foreach (var classKeyValuePair in keyValuePairs)
            {
                Console.WriteLine($"Class:{classKeyValuePair.Key} Total:{classKeyValuePair.Value}");
            }
        }

        static void PrintRowsClassification()
        {
            int rowId = 0;
            foreach (var sample in Classification.TestSamples.Values)
            {
                var cellValue = sample.GetPredefinedClass();
                Console.WriteLine($"Classification Row ID: {rowId++} => {cellValue}");

            }
        }

        /*
        static List<KMeans.Point> ExtractDataPoints()
        {
            var frame = DataExtentions.ReadTraningSet();
            var feature1Values = frame.GetColumn<double>("fetal_movement").Values.ToArray();
            var feature2Values = frame.GetColumn<double>("abnormal_short_term_variability").Values.ToArray();


            var dataPoints = new List<KMeans.Point>();
            for (int i = 0; i < feature1Values.Length; i++)
            {
                dataPoints.Add(new KMeans.Point(feature1Values[i], feature2Values[i]));
            }
            return dataPoints;
        }

        static void ClusterDataPoints()
        {
            var dataPoints = ExtractDataPoints();

            var kMeans = new KMeans(3, dataPoints); 
            kMeans.Run();
            kMeans.PlotClusters();
        }

        static void RunConsoleTestApp()
        {
            bool consoleAllocated = AllocConsole();
            if (!consoleAllocated)
            {
                MessageBox.Show("Failed to allocate console!");
                return;
            }

            ClusterDataPoints();  // Add this line
            PrintRowsClassification();
            Console.ReadKey(true);
        }
        */

    }
}
