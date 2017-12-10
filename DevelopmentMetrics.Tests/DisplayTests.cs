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
    }
}
