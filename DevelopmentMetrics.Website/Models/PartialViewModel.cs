using System.Collections.Generic;
using DevelopmentMetrics.Builds;

namespace DevelopmentMetrics.Website.Models
{
    public class PartialViewModel
    {
        public List<BuildRate> BuildRates { get; }
        public string CssStyle { get; }
        public string DivId { get; }

        public PartialViewModel(List<BuildRate> buildRates, string cssStyle, string divId)
        {
            BuildRates = buildRates;
            CssStyle = cssStyle;
            DivId = divId;
        }
    }
}