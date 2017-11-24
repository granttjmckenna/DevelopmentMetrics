using System;
using System.Diagnostics;

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
    }
}
