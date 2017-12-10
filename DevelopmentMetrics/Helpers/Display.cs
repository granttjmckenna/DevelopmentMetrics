using System;

namespace DevelopmentMetrics.Helpers
{
    public static class Display
    {
        public static string PercentageAsString(double percentage)
        {
            return Math.Round(percentage * 100, 0) + "%";
        }
    }
}
