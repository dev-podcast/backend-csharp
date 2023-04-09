using Newtonsoft.Json;

namespace devpodcasts.common.JsonObjects
{
    public class PodcastResult
    {
        [JsonProperty("trackName")] public string TrackName { get; set; }
        [JsonProperty("artworkUrl600")] public string ImageUrl600 { get; set; }
        [JsonProperty("artworkUrl30")] public string ImageUrl30 { get; set; }
        [JsonProperty("artworkUrl60")] public string ImageUrl60 { get; set; }
        [JsonProperty("artworkUrl100")] public string ImageUrl100 { get; set; }
        [JsonProperty("feedUrl")] public string FeedUrl { get; set; }
        [JsonProperty("trackCount")] public int TrackCount { get; set; }
        [JsonProperty("country")] public string Country { get; set; }
        [JsonProperty("artistName")] public string Artists { get; set; }
        [JsonProperty("releaseDate")] public DateTime ReleaseDate { get; set; }
        [JsonProperty("genres")] public List<string> Genres { get; set; }
    }
}