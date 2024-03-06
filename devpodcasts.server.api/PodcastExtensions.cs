using devpodcasts.data.mock;
namespace devpodcasts.server.api;

public static class PodcastExtensions
{
    public static void PodcastEndpoints(this WebApplication app)
    {
        
        app.MapGet("/v1/podcasts", (IPodcastGenerator podcastGenerator, string? title, DateTime? fromDate) =>
            {
                var podcasts = podcastGenerator.GenerateMockPodcasts(5);

                if (title != null)
                {
                    podcasts = podcasts.Where(p => p.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (fromDate != null)
                {
                    podcasts = podcasts.Where(p => p.LatestReleaseDate >= fromDate).ToList();
                }

                return Results.Ok(podcasts);
            })
            .WithName("GetPodcasts")
            .WithOpenApi();
        
        
        app.MapGet("v1/podcast/{id}", (IPodcastGenerator podcastGenerator, int id) =>
        {
            var podcast = podcastGenerator.GenerateMockPodcasts(1).FirstOrDefault(p => p.Id == id);
            return podcast != null ? Results.Ok(podcast) : Results.NotFound();
        }).WithName("GetPodcast").WithOpenApi();
    }
}