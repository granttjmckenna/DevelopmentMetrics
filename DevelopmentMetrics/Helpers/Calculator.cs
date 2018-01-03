using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentMetrics.Helpers
{
    public static class Calculator
    {
        public static double Percentage(int numerator, int denominator)
        {
            return (denominator == 0)
                ? 0
                : Math.Round((double)numerator / denominator, 2);
        }


        public static double CalculateStandardDeviation(List<double> values)
        {
            if (!values.Any() || values.Count == 1)
                return 0;

            var average = values.Average();

            var sumOf = values.Sum(d => Math.Pow(d - average, 2));

            return Math.Sqrt(sumOf / (values.Count - 1));
        }

        public static int ConvertMillisecondsToDays(double milliseconds)
        {
            return (int)(Math.Round(TimeSpan.FromMilliseconds(milliseconds).TotalDays, 0));
        }

        public static int ConvertMillisecondsToHours(double milliseconds)
        {
            return (int)(Math.Round(TimeSpan.FromMilliseconds(milliseconds).TotalHours, 0));
        }

        public static int ConvertMillisecondsToMinutes(double milliseconds)
        {
            return (int)(Math.Round(TimeSpan.FromMilliseconds(milliseconds).TotalMinutes, 0));
        }
    }
}
