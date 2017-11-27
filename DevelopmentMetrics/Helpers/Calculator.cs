﻿using System;

namespace DevelopmentMetrics.Helpers
{
    public static class Calculator
    {
        public static double Percentage(int nominator, int denominator)
        {
            return (denominator == 0)
                ? 0
                : Math.Round((double)nominator / denominator, 2);
        }
    }
}