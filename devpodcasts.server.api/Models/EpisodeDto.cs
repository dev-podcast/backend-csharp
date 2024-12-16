namespace devpodcasts.server.api.Models;

public class EpisodeDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; }
    public string AudioUrl { get; set; }
    public string AudioType { get; set; }
    public string AudioDuration { get; set; }
    public DateTime? PublishedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public string ImageUrl { get; set; }
    public string SourceUrl { get; set; }
    public Guid PodcastId { get; set; }
    public string PodcastTitle { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Categories { get; set; } = new();
}