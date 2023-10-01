﻿using AI_Masters_Project;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VS_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //KMeans.Test().Draw(kMeansPlot);
            int pointCount = 500;
            Random rand = new Random(0);
            double[] xs1 = ScottPlot.DataGen.RandomWalk(rand, pointCount);
            double[] ys1 = ScottPlot.DataGen.RandomWalk(rand, pointCount);
            double[] xs2 = ScottPlot.DataGen.RandomWalk(rand, pointCount);
            double[] ys2 = ScottPlot.DataGen.RandomWalk(rand, pointCount);

            // plot the data
            kMeansPlot.Plot.PlotScatter(xs1, ys1);
            kMeansPlot.Plot.PlotScatter(xs2, ys2);
            kMeansPlot.Refresh();
        }
    }
}
