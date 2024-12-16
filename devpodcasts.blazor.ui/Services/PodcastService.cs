using devpodcasts.blazor.ui.Interfaces;
using devpodcasts.blazor.ui.Models;
using devpodcasts.common.Services;

namespace devpodcasts.blazor.ui.Services;

public class PodcastService : IPodcastService
{
    private readonly ILogger<PodcastService> _logger;
    private readonly CommonHttpClient _commonHttpClient;

    public PodcastService(ILogger<PodcastService> logger, CommonHttpClient commonHttpClient)
    {
        _logger = logger;
        _commonHttpClient = commonHttpClient;
    }
    
     /// <summary>
    /// Retrieves all podcasts with optional filtering by title and date.
    /// </summary>
    /// <param name="title">Optional filter for the podcast title.</param>
    /// <param name="fromDate">Optional filter for the release date.</param>
    /// <returns>A list of podcasts.</returns>
    public async Task<List<Podcast>> GetPodcastsAsync(string? title = null, DateTime? fromDate = null)
    {
        try
        {
            var url = "/v1/podcasts";

            if (!string.IsNullOrEmpty(title) || fromDate != null)
            {
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(title))
                    queryParams.Add($"title={Uri.EscapeDataString(title)}");
                if (fromDate != null)
                    queryParams.Add($"fromDate={fromDate:O}"); // Use ISO 8601 format for dates

                url += "?" + string.Join("&", queryParams);
            }

            var response = await _commonHttpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var jsonContent =  await response.Content.ReadFromJsonAsync<List<Podcast>>() ?? new List<Podcast>();
            
    
            return jsonContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving podcasts.");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a podcast by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the podcast.</param>
    /// <returns>The podcast, or null if not found.</returns>
    public async Task<Podcast?> GetPodcastByIdAsync(Guid id)
    {
        try
        {
            var response = await _commonHttpClient.GetAsync($"/v1/podcast/{id}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Podcast>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving podcast by ID.");
            throw;
        }
    }

    /// <summary>
    /// Retrieves the most recent podcasts.
    /// </summary>
    /// <returns>A list of recent podcasts.</returns>
    public async Task<List<Podcast>> GetRecentPodcastsAsync()
    {
        try
        {
            var response = await _commonHttpClient.GetAsync("/v1/podcasts/recent");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Podcast>>() ?? new List<Podcast>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent podcasts.");
            throw;
        }
    }

    /// <summary>
    /// Searches for podcasts based on a search term.
    /// </summary>
    /// <param name="searchTerm">The term to search for in podcast titles and descriptions.</param>
    /// <returns>A list of matching podcasts.</returns>
    public async Task<List<Podcast>> SearchPodcastsAsync(string? searchTerm)
    {
        try
        {
            var url = string.IsNullOrEmpty(searchTerm) ? "/v1/podcasts" : $"/v1/podcasts/search?searchTerm={Uri.EscapeDataString(searchTerm)}";

            var response = await _commonHttpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Podcast>>() ?? new List<Podcast>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for podcasts.");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all episodes for a specific podcast.
    /// </summary>
    /// <param name="podcastId">The unique identifier of the podcast.</param>
    /// <returns>A list of episodes for the podcast.</returns>
    public async Task<List<Episode>> GetPodcastEpisodesAsync(Guid podcastId)
    {
        try
        {
            var response = await _commonHttpClient.GetAsync($"/v1/podcasts/{podcastId}/episodes");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Episode>>() ?? new List<Episode>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving episodes for podcast.");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all categories for a specific podcast.
    /// </summary>
    /// <param name="podcastId">The unique identifier of the podcast.</param>
    /// <returns>A list of categories for the podcast.</returns>
    public async Task<List<Category>> GetPodcastCategoriesAsync(Guid podcastId)
    {
        try
        {
            var response = await _commonHttpClient.GetAsync($"/v1/podcasts/{podcastId}/categories");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Category>>() ?? new List<Category>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories for podcast.");
            throw;
        }
    }
}