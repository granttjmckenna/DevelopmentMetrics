using DevelopmentMetrics.Helpers;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class DisplayTests
    {
        [Test]
        public void Return_percentage_as_string()
        {
            var displayPercentage = Display.PercentageAsString(0.25d);

            Assert.That(displayPercentage, Is.EqualTo("25%"));
        }

        [Test]
        public void Return_camel_case_string_with_spaces()
        {
            var convertedString = Display.ConvertCamelCaseString("ThisIsTheString");

            Assert.That(convertedString,Is.EqualTo("This Is The String"));
        }
    }
}
