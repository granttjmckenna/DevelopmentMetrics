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

            if (!DateTime.TryParseExact(input, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out result))
            {
                result = DateTime.ParseExact(input, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            }

            return result;
        }
    }
}
