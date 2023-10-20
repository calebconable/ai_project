using AI_Masters_Project;
using Deedle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VS_Project.Model;

namespace VS_Project.Extentions
{
    public static class CPointExtentions
    {
        public static CPoint ToCPoint<T>(this Series<string, T> series)
        {
            return (CPoint)series.ToObjectSeries();
        }
    }
}
