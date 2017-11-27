using System;

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
    }
}
