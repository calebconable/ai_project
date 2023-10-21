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

                /*Task.Run(async () =>
                {

                    while (true)
                    {
                        var randomForest = RandomForest.Open();
                        var classifications = await randomForest.EvaluateAsync(Classification.TestSamples.ToArray());
                        Console.WriteLine(classifications.ConfusionMatrixAndAccuracy());
                    }
                });*/

                var randomForest = RandomForest.New(3);
                Task.Run(async () =>
                {
                    await randomForest.PerformBootstrapSamplingAsync(Classification.TrainingSamples.ToArray());
                    randomForest.Build(15, 20, 10f, doneAction: () => { randomForest.Save(); });
                });

                /*var desicionTree = DesicionTree.New(Classification.TrainingSamples.ToArray());
                desicionTree.BuildTree(100, 20, 10f, doneAction: () =>
                {
                    desicionTree.Save();
                });*/

                /*var desisionTree = FileExtentions.LoadModelDynamic<DesicionTree>(nameof(DesicionTree));
                Console.WriteLine(desisionTree.RootNode);*/
                /*var kmeans = kMeans.New(3);
                Task.Run(async () =>
                {
                    await kmeans.TraingModel_UntilConversionAsync(0.10f);
                    kmeans.SaveModel();
                });*/

                // kMeans.TestRun(1, 30, n:.10f);
                // KNN.TestDifferentKValues(1, 2);
            });
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        static void Menu()
        {
            int algorithmIndex = -1;
            while (true) {
                Console.WriteLine("\nChoose a algorithm:");
                Console.WriteLine("0. kMeans");
                Console.WriteLine("1. Random Forest");
                if (int.TryParse(Console.ReadLine(), out algorithmIndex) && algorithmIndex <= 1)
                {
                    AlgorithmMenu();
                }
                else
                {
                    Console.WriteLine("Choose from 0 - 1");
                }

            }
        }

        static void AlgorithmMenu()
        {
            int choice = -1;
            while (true)
            {
                Console.WriteLine("\nWhat do you want to do?");
                Console.WriteLine("0. Get Models");
                Console.WriteLine("1. Train");
                Console.WriteLine("2. Evaluate");
                Console.WriteLine("3. Back");
                if (int.TryParse(Console.ReadLine(), out choice) && choice <= 2)
                {
                    switch(choice)
                    {
                        case 3:
                            Console.WriteLine("\n");
                            return;
                    }
                }
                else
                {
                    Console.WriteLine("Choose from 0 - 1");
                }

            }
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
            foreach (var sample in Classification.TestSamples)
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
