using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Graph_Manager.ViewModel
{
    static class Validation
    {
        static Regex _expression = new Regex("[1-9]+([,]{1}[1-9])");
        public static bool IsEven(object obj) // Parzysta
        {
            int sum;
            string sequenceString = (string)obj;

            if (!string.IsNullOrEmpty(sequenceString))
            {
                if (sequenceString.Last() != ',' && _expression.IsMatch(sequenceString))
                {
                    List<int> degreeSequence = SplitSequence(sequenceString);
                    degreeSequence = degreeSequence.OrderByDescending(x => x).ToList();
                    sum = degreeSequence.Sum();
                    if (degreeSequence[0] < degreeSequence.Count)
                    {
                        return sum % 2 == 0;
                    }
                }
                return false;
            }

            return false;
        }

        public static bool IsPruferCode(object obj)
        {
            string sequenceString = (string)obj;
            List<int> pruferCodeList;

            if (!string.IsNullOrEmpty(sequenceString))
            {
                if (sequenceString.Last() != ',' && _expression.IsMatch(sequenceString))
                {
                    pruferCodeList = SplitSequence(sequenceString);
                    return pruferCodeList.All(x => x < pruferCodeList.Count);
                }
            }

            return false;
        }

        public static List<int> SplitSequence(string str)
        {
            return str.Split(',').Select(int.Parse).ToList();
        }

    }

}
