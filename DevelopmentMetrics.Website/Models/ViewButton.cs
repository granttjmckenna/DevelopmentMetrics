namespace DevelopmentMetrics.Website.Models
{
    public class ViewButton
    {
        public string Label { get; }
        public string Action { get; }

        public ViewButton(string label, string action)
        {
            Label = label;
            Action = action;
        }
    }
}