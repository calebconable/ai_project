using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VS_Project.Model;
using VS_Project.Singletone;

namespace VS_Project.Extentions
{
    public static class ClassificationExtentions
    {

        public static Matrix DetermineConfusionMatrix(this IList<Classification> classifications)
        {
            // Assuming maximum class number as the matrix dimension
            int dimension = classifications.Max(c => Math.Max(c.PredefinedClass, c.PredictedClass));
            Matrix matrix = new Matrix(dimension, dimension);

            foreach (var c in classifications)
            {
                matrix.Increment(c.PredefinedClass - 1, c.PredictedClass - 1); // Assuming 0-indexed matrix
            }

            return matrix;
        }

        public static IEnumerable<(int className, float accuracy)> DetermineAccuracy(this IList<Classification> classifications)
        {
            var matrix = DetermineConfusionMatrix(classifications);

            for (int i = 0; i < matrix.RowsCount; i++)
            {
                float correct = matrix[i, i];
                float total = matrix.RowSum(i);

                yield return (i + 1, correct / total); // Assuming 1-indexed class names
            }
        }

        public static void PrintConfusionMatrixAndAccuracy(this IList<Classification> classifications)
        {
            var matrix = classifications.DetermineConfusionMatrix();
            // Print the matrix (assuming a Print method in Matrix class)
            // matrix.Print();

            var accuracies = DetermineAccuracy(classifications);
            Console.WriteLine("\nClass Accuracies:");
            foreach (var (className, accuracy) in accuracies)
            {
                Console.WriteLine($"Class {className}: {accuracy:P2}");
            }
        }

        public static string ConfusionMatrixAndAccuracy(this IList<Classification> classifications)
        {
            var matrix = classifications.DetermineConfusionMatrix();
            // Assuming the Matrix class has a ToString method to get its string representation
            // string matrixString = matrix.ToString();
            // If not, you will need to implement a method to convert your matrix to string

            var accuracies = DetermineAccuracy(classifications);

            // Calculate overall accuracy
            int totalCorrect = classifications.Count(c => c.IsCorrectlyClassified);
            double overallAccuracy = (double)totalCorrect / classifications.Count;

            StringBuilder resultBuilder = new StringBuilder();

            // Add the matrix string to the result
            // resultBuilder.AppendLine(matrixString);

            foreach (var (className, accuracy) in accuracies)
            {
                resultBuilder.AppendLine($"Class {className}: {accuracy:P2}");
            }

            // Append the overall accuracy
            resultBuilder.AppendLine($"Overall Accuracy: {overallAccuracy:P2}");

            return resultBuilder.ToString();
        }


    }
}
