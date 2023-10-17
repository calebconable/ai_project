using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VS_Project.Model
{
    public class Matrix
    {
        private int[,] _data;

        public Matrix(int rows, int columns)
        {
            _data = new int[rows, columns];
        }

        public int this[int row, int col]
        {
            get
            {
                return _data[row, col];
            }
            set
            {
                _data[row, col] = value;
            }
        }

        public int RowsCount => _data.GetLength(0);
        public int ColumnsCount => _data.GetLength(1);

        public int RowSum(int rowIndex)
        {
            int sum = 0;
            for (int i = 0; i < ColumnsCount; i++)
            {
                sum += _data[rowIndex, i];
            }
            return sum;
        }

        public void Increment(int row, int col)
        {
            _data[row, col]++;
        }
    }
}
