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
        private int k;
        private List<CustomPoint> data;
        private List<Cluster> clusters = new List<Cluster>();

        public KMeans(int k, List<CustomPoint> data)
        {
            this.k = k;
            this.data = data;
        }

        public static KMeans Test()
        {
            List<CustomPoint> data = new List<CustomPoint>
            {
                new CustomPoint { X = 1, Y = 2 },
                new CustomPoint { X = 5, Y = 6 },
                new CustomPoint { X = 2, Y = 2 },
                // ... add more points here
            };

            int numberOfClusters = 2;
            KMeans kMeans = new KMeans(numberOfClusters, data);
            var clusters = kMeans.Fit();

            for (int i = 0; i < clusters.Count; i++)
            {
                Console.WriteLine($"Cluster {i + 1} Centroid: {clusters[i].Centroid.X}, {clusters[i].Centroid.Y}");
            }
            return kMeans;
        }

        public List<Cluster> Fit(int maxIterations = 100)
        {
            List<Cluster> clusters = InitializeClusters();

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                AssignPointsToClusters(clusters);
                UpdateClusterCentroids(clusters);
            }
            this.clusters = clusters;
            return clusters;
        }

        private List<Cluster> InitializeClusters()
        {
            Random rnd = new Random();
            return data.OrderBy(x => rnd.Next()).Take(k).Select(point => new Cluster { Centroid = point }).ToList();
        }

        private void AssignPointsToClusters(List<Cluster> clusters)
        {
            foreach (var cluster in clusters)
            {
                cluster.Points.Clear();
            }

            foreach (var point in data)
            {
                var closestCluster = clusters.OrderBy(cluster => point.Distance(cluster.Centroid)).First();
                closestCluster.Points.Add(point);
            }
        }

        private void UpdateClusterCentroids(List<Cluster> clusters)
        {
            foreach (var cluster in clusters)
            {
                double averageX = cluster.Points.Average(point => point.X);
                double averageY = cluster.Points.Average(point => point.Y);
                cluster.Centroid = new CustomPoint { X = averageX, Y = averageY };
            }
        }

        public void Draw(FormsPlot formsPlot, string imagePath = null)
        {
            foreach (var cluster in this.clusters)
            {
                double[] xs = cluster.Points.Select(p => p.X).ToArray();
                double[] ys = cluster.Points.Select(p => p.Y).ToArray();
                formsPlot.Plot.AddScatter(xs, ys, markerSize: 5);
            }

            if (imagePath != null)
                formsPlot.Plot.SaveFig(imagePath);

            formsPlot.Refresh();
        }

    }

    public class Cluster
    {
        public CustomPoint Centroid { get; set; }
        public List<CustomPoint> Points { get; set; } = new List<CustomPoint>();

    }
}
