using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VS_Project.Extentions
{
    public static class FloatExtentions
    {
        public static List<float> ToRangeList(this float min, float max, int length)
        {
            List<float> result = new List<float>();

            if (length <= 0)
            {
                throw new ArgumentException("Length must be greater than 0.");
            }

            float interval = (max - min) / (length - 1);

            for (int i = 0; i < length; i++)
            {
                float value = min + i * interval;
                result.Add(value);
            }

            return result;
        }
    }
}
