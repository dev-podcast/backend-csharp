using devpodcasts.server.api.Models;

namespace devpodcasts.server.api.Models;

public class SearchResultDto
{
    public List<EpisodeDto> Episodes { get; set; } = new();
    public List<PodcastDto> Podcasts { get; set; } = new();
}
