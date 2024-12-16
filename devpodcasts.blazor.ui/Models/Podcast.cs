namespace devpodcasts.blazor.ui.Models;

public record Podcast
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? ShowUrl { get; set; }
    public DateTime? LatestReleaseDate { get; set; }
}