using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VS_Project.Extentions
{
    public static class ListExtentions
    {
        public static List<string> ToRandomStrings(this List<string> strings, int numToChoose)
        {
            Random random = new Random();
            List<string> chosenStrings = new List<string>();

            // Shuffle the input list to randomize the order
            for (int i = strings.Count - 1; i >= 1; i--)
            {
                int j = random.Next(i + 1);
                string temp = strings[i];
                strings[i] = strings[j];
                strings[j] = temp;
            }

            // Choose the first 'numToChoose' strings
            for (int i = 0; i < numToChoose; i++)
            {
                chosenStrings.Add(strings[i]);
            }

            return chosenStrings;
        }
    }
}
