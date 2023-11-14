using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using System.Threading.Tasks;
using VS_Project.Algorithms;

namespace VS_Project.Model
{
    public class NNLayer
    {
        private static Random _random = new Random();
        public int BatchSize { get; set; }
        public int Size { get; set; }
        public NNLayer PreviousLayer { get; set; } = null;
        public NNLayer NextLayer { get; set; } = null;
        public Matrix<double> Input
        {
            get => PreviousLayer?.Output;
        }
        public Matrix<double> Output { get; set; }
        public Matrix<double> Bais { get; set; }
        public Matrix<double> Weights { get; set; }

        private NNLayer() { }

        public NNLayer(int batchSize, int layerSize, NNLayer previousLayer = null) {

            BatchSize = batchSize;
            Size = layerSize;
            if (previousLayer is NNLayer inputLayer)
            {
                inputLayer.NextLayer = this;
                PreviousLayer = inputLayer;
            }
            // Randomly create weights and baises
            Bais = Matrix<double>.Build.Dense(batchSize, layerSize, (i, j) => _random.NextDouble() * 100);
            Weights = Matrix<double>.Build.Dense(batchSize, layerSize, (i, j) => _random.NextDouble() * 100);
            // Initialize output with 0
            Output = Matrix<double>.Build.Dense(batchSize, layerSize, 0);
        }

        private Matrix<double> CalculateLinearTransformationZ()
        {
            return Weights * Input + Bais;
        }

        private static Matrix<double> CalculateActivationFunction(Matrix<double> Z)
        {
            return Z.Map(z => 1 / (1 + Math.Exp(-z)));
        }

        public static NNLayer NewInputLayer(int batchSize, int layerSize)
        {
            throw new NotImplementedException();
        }

        public static NNLayer NewHiddenLayer(int batchSize, int layerSize)
        {
            throw new NotImplementedException();
        }

        public static NNLayer NewOutputLayer(int batchSize, int layerSize)
        {
            throw new NotImplementedException();
        }
    }
}
