using System.Collections.Generic;
using DevelopmentMetrics.Builds;

namespace DevelopmentMetrics.Website.Models
{
    public class PartialViewModel
    {
        public List<FailureRate> FailureRates { get; }
        public string CssStyle { get; }
        public string DivId { get; }

        public PartialViewModel(List<FailureRate> failureRates, string cssStyle, string divId)
        {
            FailureRates = failureRates;
            CssStyle = cssStyle;
            DivId = divId;
        }
    }
}