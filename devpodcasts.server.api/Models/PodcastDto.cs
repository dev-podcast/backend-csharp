namespace devpodcasts.server.api.Models;

public class PodcastDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? ShowUrl { get; set; }
    public string? FeedUrl { get; set; }
    public DateTime? LatestReleaseDate { get; set; }
    public int EpisodeCount { get; set; }
    public string? Country { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? Artists { get; set; }
    public string? ItunesId { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Categories { get; set; } = new();
}