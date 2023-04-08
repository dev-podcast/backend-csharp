using devpodcasts.common.Extensions;
using devpodcasts.Domain.Entities;

namespace devpodcasts.common.JsonObjects
{
    public static class BasePodcastJsonObjectExtensions
    {
        public  static BasePodcast CreateBasePodcast(this BasePodcastJsonObject basePodcastJsonObject)
        {
            return new BasePodcast()
            {
                Title = basePodcastJsonObject.Title.RemovePodcastFromName(),
                Description = basePodcastJsonObject.Description,
                ItunesId = basePodcastJsonObject.GetItunesIdFromQueryString(),
                ItunesSubscriberUrl =basePodcastJsonObject.ItunesSubscriberUrl,
                PodcastSite = basePodcastJsonObject.PodcastSite
            };
        }

        public static string GetItunesIdFromQueryString(this BasePodcastJsonObject basePodcastJsonObject)
        {
            var queryString = basePodcastJsonObject.ItunesSubscriberUrl.Split('/');
            return queryString[queryString.Length - 1].Split('?')[0];
        }
    }
}