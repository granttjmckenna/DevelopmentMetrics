using System;
using System.Diagnostics;
using System.Globalization;

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
            var result = DateTime.ParseExact(input, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            return result;
        }
    }
}
