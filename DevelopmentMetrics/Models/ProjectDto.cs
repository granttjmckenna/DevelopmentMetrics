namespace DevelopmentMetrics.Models
{
    public class ProjectDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParentProjectId { get; set; }
        public string Href { get; set; }
        public string WebUrl { get; set; }
    }
}