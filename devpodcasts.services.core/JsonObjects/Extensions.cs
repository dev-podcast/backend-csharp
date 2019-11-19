using DevPodcast.Domain.Entities;

namespace DevPodcast.Services.Core.JsonObjects
{
    public static class Extensions
    {
        public  static BasePodcast CreateBasePodcast(this BasePodcastJsonObject jsonObject)
        {
            return new BasePodcast()
            {
                Description = jsonObject.Description,
                ItunesId = jsonObject.ItunesId,
                ItunesSubscriberUrl = jsonObject.ItunesSubscriberUrl,
                PodcastSite = jsonObject.PodcastSite,
                Title = jsonObject.Title
            };
        }   
    }
}