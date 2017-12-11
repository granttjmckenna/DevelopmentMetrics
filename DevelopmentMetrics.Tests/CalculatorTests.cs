using System.Collections.Generic;
using DevelopmentMetrics.Helpers;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    public class CalculatorTests
    {
        [TestCase(1, 4, ExpectedResult = 0.25d)]
        [TestCase(1, 3, ExpectedResult = 0.33d)]
        public double Return_percentage(int nominator, int denominator)
        {
            return Calculator.Percentage(nominator, denominator);
        }

        [Test]
        public void Return_for_standard_deviation_when_list_is_empty()
        {
            var values = new List<double>();

            var standardDeviation = Calculator.CalculateStandardDeviation(values);

            Assert.That(standardDeviation, Is.EqualTo(0));
        }

        [Test]
        public void Return_standard_deviation_when_list_is_not_empty()
        {
            var values = new List<double> { 1d, 2d, 3d, 2d, 1d };

            var standardDeviation = Calculator.CalculateStandardDeviation(values);

            Assert.That(standardDeviation, Is.GreaterThan(0.83d));
            Assert.That(standardDeviation, Is.LessThan(0.84d));
        }

        [Test]
        public void Return_standard_deviation_of_recovery_time_in_hours()
        {
            var doubles = new List<double> { 2100000d, 2400000d, 2760000d };

            var standardDeviationInHours =
                Calculator.ConvertMillisecondsToHours(
                    Calculator.CalculateStandardDeviation(doubles));

            Assert.That(standardDeviationInHours, Is.EqualTo(0));
        }

    }
}
