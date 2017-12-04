using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;

namespace DevelopmentMetrics.Helpers
{
    public interface ITellTheTime
    {
        DateTime Now();
        DateTime Today();
        DateTime UtcNow();
        DateTime ParseDateToUkFormat(string input);
        DateTime ParseBuildDetailDateTimes(string input);
    }

    public class TellTheTime : ITellTheTime
    {
        [DebuggerStepThrough]
        public DateTime Now()
        {
            return DateTime.Now;
        }

        [DebuggerStepThrough]
        public DateTime Today()
        {
            return DateTime.Today;
        }

        [DebuggerStepThrough]
        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }

        public DateTime ParseDateToUkFormat(string input)
        {
            DateTime result;
            var dateFormats = new[] { "dd/MM/yyyy", "MM/dd/yyyy", "MM/dd/yyyy h:mm:ss tt", "MM/d/yyyy h:mm:ss tt", "M/dd/yyyy h:mm:ss tt", "M/d/yyyy h:mm:ss tt", "yyyyMMddHHmmss" };

            if (!DateTime.TryParseExact(input, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out result))
            {
                result = DateTime.ParseExact(input, dateFormats, CultureInfo.InvariantCulture,
                    DateTimeStyles.NoCurrentDateDefault);
            }

            return result;
        }

        public DateTime ParseBuildDetailDateTimes(string input)
        {
            var newDateTimeString = input
                .Replace("T", "")
                .Substring(0, input.IndexOf("+", StringComparison.InvariantCultureIgnoreCase) - 1);

            return ParseDateToUkFormat(newDateTimeString);
        }
    }
}
