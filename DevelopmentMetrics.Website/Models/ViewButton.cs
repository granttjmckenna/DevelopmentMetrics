namespace DevelopmentMetrics.Website.Models
{
    public class ViewButton
    {
        public string Type;
        public string Label { get; }
        public string Action { get; }

        public ViewButton(string type, string label, string action)
        {
            Type = type;
            Label = label;
            Action = action;
        }
    }
}