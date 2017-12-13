using System.Collections.Generic;
using DevelopmentMetrics.Builds;

namespace DevelopmentMetrics.Website.Models
{
    public class PartialViewModel
    {
        public List<BuildFailureRate> BuildRates { get; }
        public string CssStyle { get; }
        public string DivId { get; }

        public PartialViewModel(List<BuildFailureRate> buildRates, string cssStyle, string divId)
        {
            BuildRates = buildRates;
            CssStyle = cssStyle;
            DivId = divId;
        }
    }
}