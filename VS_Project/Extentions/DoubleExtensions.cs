using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VS_Project.Extentions
{
    public static class DoubleExtensions
    {
        public static double SafeDivide(this double numerator, double denominator)
        {
            if (denominator == 0)
            {
                return double.PositiveInfinity;  // or whatever value you want
            }
            return numerator / denominator;
        }
    }

}
