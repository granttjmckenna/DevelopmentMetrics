using System.Collections.Generic;

namespace DevelopmentMetrics.Website.Models
{
    public class ChartFilterViewModel
    {
        public string Id { get; }
        public string Title { get; }
        public string MenuClass { get; }
        public Dictionary<string, string> MenuItems { get; }
        public Dictionary<string, string> ExtraItems { get; }

        public ChartFilterViewModel(
            string id,
            string title,
            string menuClass,
            Dictionary<string, string> menuItems,
            Dictionary<string, string> extraItems)
        {
            Id = id;
            Title = title;
            MenuClass = menuClass;
            MenuItems = menuItems;
            ExtraItems = extraItems;
        }
    }
}