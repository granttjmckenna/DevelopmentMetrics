using System;
using System.Text.RegularExpressions;

namespace DevelopmentMetrics.Helpers
{
    public static class Display
    {
        public static string PercentageAsString(double percentage)
        {
            return Math.Round(percentage * 100, 0) + "%";
        }

        public static string ConvertCamelCaseString(string input)
        {
            return Regex.Replace(input, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");
        }
    }
}
