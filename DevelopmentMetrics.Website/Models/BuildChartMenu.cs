using System.Collections.Generic;

namespace DevelopmentMetrics.Website.Models
{
    public static class BuildChartMenu
    {
        public static Dictionary<string, string> GetBuildWeeksList()
        {
            var results = new Dictionary<string, string>
            {
                {"6 weeks (default)", "6"},
                {"7 weeks", "7"},
                {"8 weeks", "8"},
                {"9 weeks", "9"},
                {"10 weeks", "10"},
                {"15 weeks", "15"},
                {"20 weeks", "20"},
                {"All", "-2"}
            };

            return results;
        }

    }
}