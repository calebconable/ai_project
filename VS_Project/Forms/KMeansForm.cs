using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace VS_Project.Forms
{
    public partial class KMeansForm : Form
    {
        public KMeansForm()
        {
            InitializeComponent(); 
            int pointCount = 500;
            Random rand = new Random(0);
            double[] xs1 = ScottPlot.DataGen.RandomWalk(rand, pointCount);
            double[] ys1 = ScottPlot.DataGen.RandomWalk(rand, pointCount);
            double[] xs2 = ScottPlot.DataGen.RandomWalk(rand, pointCount);
            double[] ys2 = ScottPlot.DataGen.RandomWalk(rand, pointCount);

            // plot the data
            scottPlot.Plot.PlotScatter(xs1, ys1);
            scottPlot.Plot.PlotScatter(xs2, ys2);
            scottPlot.Refresh();
        }
    }
}
