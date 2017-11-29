using System;
using DevelopmentMetrics.Helpers;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class TellTheTimeTests
    {
        [Test]
        public void Return_UK_date_from_US_date()
        {
            var expected = new DateTime(2017, 11, 21);

            var actual = new TellTheTime().ParseDateToUkFormat("11/21/2017");

            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public void Return_UK_date_from_UK_date_and_time_string()
        {
            var expected = new DateTime(2017, 11, 21);

            var actual = new TellTheTime().ParseDateToUkFormat("21/11/2017");

            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public void Return_UK_date_from_US_date_and_time_string()
        {
            var expected = new DateTime(2017, 11, 21, 9, 16, 21);

            var actual = new TellTheTime().ParseDateToUkFormat("11/21/2017 9:16:21 AM");

            Assert.That(expected, Is.EqualTo(actual));
        }
    }
}
