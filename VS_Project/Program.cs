using AI_Masters_Project;
using Deedle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            RunConsoleTestApp();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        static void RunConsoleTestApp()
        {
            bool consoleAllocated = AllocConsole();
            if (!consoleAllocated)
            {
                MessageBox.Show("Failed to allocate console!");
                return;
            }

            PrintRowsClassification();
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
            foreach (var sample in Classification.Instance.TestSamples.Values)
            {
                var cellValue = sample.GetPredefinedClass();
                Console.WriteLine($"Classification Row ID: {rowId++} => {cellValue}");

            }
        }
    }
}
