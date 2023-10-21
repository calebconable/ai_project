using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VS_Project.Model;
using VS_Project.Singletone;

namespace VS_Project.Algorithms
{
    internal interface IAlgorithm
    {
        Task<Classification[]> EvaluateAsync(Sample[] testSampels);
    }
}
