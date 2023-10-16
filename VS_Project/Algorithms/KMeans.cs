using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScottPlot;

namespace AI_Masters_Project
{
    public class KMeans
    {
        // Number of clusters
        public int K { get; private set; }
        // All data points
        public List<Point> Points { get; private set; }
        // All clusters
        public List<Cluster> Clusters { get; private set; }

        // Constructor initializing KMeans with number of clusters and data points
        public KMeans(int k, List<Point> points)
        {
            K = k;
            Points = points;
            Clusters = new List<Cluster>();

            // Initialize the starting centroids
            InitializeCentroids();
        }

        // Run KMeans clustering
        public void Run(int maxIterations = 100)
        {
            for (int i = 0; i < maxIterations; i++)
            {
                // Assign data points to the nearest cluster
                AssignPointsToClusters();
                // Update the centroid of each cluster based on the data points in the cluster
                UpdateCentroids();
            }
        }

        // Initialize centroids using random data points
        private void InitializeCentroids()
        {
            var random = new Random();
            var centroids = Points.OrderBy(x => random.Next()).Take(K).ToList();
            Clusters = centroids.Select(centroid => new Cluster(centroid)).ToList();
        }

        // Assign each data point to the closest cluster
        private void AssignPointsToClusters()
        {
            foreach (var point in Points)
            {
                var closestCluster = Clusters.OrderBy(cluster => Point.Distance(point, cluster.Centroid)).First();
                closestCluster.Points.Add(point);
            }
        }

        // Update centroids based on the mean of data points in each cluster
        private void UpdateCentroids()
        {
            foreach (var cluster in Clusters)
            {
                cluster.UpdateCentroid();
            }
        }

        // Plot clusters and centroids using ScottPlot
        public void PlotClusters()
        {
            var plt = new Plot(600, 400);

            // Define cluster colors
            var colors = new List<System.Drawing.Color>
                {
                System.Drawing.Color.Red,
                System.Drawing.Color.Blue,
                System.Drawing.Color.Green,
                System.Drawing.Color.Purple,
                System.Drawing.Color.Orange
                };

            for (int i = 0; i < Clusters.Count; i++)
            {
                var cluster = Clusters[i];
                Console.WriteLine("TEST TEST TEST");
                // Only plot clusters with points
                if (cluster.Points.Any())
                {
                    Console.WriteLine("TEST 2 TEST 2 TEST 2");
                    var clusterPointsX = cluster.Points.Select(p => p.X).ToArray();
                    var clusterPointsY = cluster.Points.Select(p => p.Y).ToArray();
                    Console.WriteLine("Cluster points" + clusterPointsY + "\n");
                    plt.AddScatter(clusterPointsX, clusterPointsY, color: colors[i], lineWidth: 0, markerSize: 5);

                    // Plot centroid of each cluster
                    plt.AddScatter(new double[] { cluster.Centroid.X }, new double[] { cluster.Centroid.Y }, color: colors[i], lineWidth: 0, markerSize: 10, markerShape: MarkerShape.openCircle);
                }
            }
        
        }


        public class Point
        {
            // X and Y coordinates of the point
            public double X { get; set; }
            public double Y { get; set; }

            public Point(double x, double y)
            {
                X = x;
                Y = y;
            }

            // Calculate Euclidean distance between two points
            public static double Distance(Point a, Point b)
            {
                return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
            }
        }

        public class Cluster
        {
            // Centroid of the cluster
            public Point Centroid { get; private set; }
            // Data points in the cluster
            public List<Point> Points { get; private set; }

            public Cluster(Point initialCentroid)
            {
                Centroid = initialCentroid;
                Points = new List<Point>();
            }

            // Update centroid to the mean of data points in the cluster
            public void UpdateCentroid()
            {
                if (!Points.Any()) return;

                double newX = Points.Average(p => p.X);
                double newY = Points.Average(p => p.Y);
                Centroid = new Point(newX, newY);
                Points.Clear();
            }
        }
    }
}
