using devpodcasts.common.Extensions;
using devpodcasts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devpodcasts.common.Builders
{
    public class PodcastBuilder
    {

        private Guid _id;
        private string? _title;
        private string? _itunesId;
        private DateTime? _createdDate;
        private string? _description;
        private string? _imageUrl;
        private string? _showUrl;
        private string? _feedUrl;
        private string? _artists;
        private string? _country;
        private DateTime? _latestReleaseDate;
        private int _episodeCount;

        public PodcastBuilder AddTitle(string? title, BasePodcast basePodcast)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                if (basePodcast != null)
                {
                    if (title.Length > 100 && basePodcast.Title.Length < 100)
                    {
                        _title = basePodcast.Title;
                    }
                    else if (title.Length > 100)
                    {
                        _title = title.Substring(0, 99);
                    }else
                    {
                        _title = title;
                    }
                   
                }
                else
                {
                    _title = title;
                }
            }
            return this;
        }

        public PodcastBuilder WithId(Guid id)
        {
            _id = Guid.NewGuid();
            return this;
        }

        public PodcastBuilder AddImageUrl(string? imageUrl)
        {
            _imageUrl = imageUrl;
            return this;
        }

        public PodcastBuilder AddCreatedDate(DateTime createdDate)
        {
            _createdDate = createdDate;
            return this;
        }

        public PodcastBuilder AddItunesId(string? itunesId)
        {
            _itunesId = itunesId;
            return this;
        }

        public PodcastBuilder AddDescription(string? description)
        {
            if (!string.IsNullOrWhiteSpace(description))
                _description = description.CleanHtml();
            return this;
        }


        public PodcastBuilder AddShowUrl(string? showUrl)
        {
            if (!string.IsNullOrWhiteSpace(showUrl))
                _showUrl = showUrl;
            return this;
        }

        public PodcastBuilder AddFeedUrl(string? feedUrl)
        {
            _feedUrl = feedUrl;
            return this;
        }

        public PodcastBuilder AddCountry(string? country)
        {
            _country = country;
            return this;
        }

        public PodcastBuilder AddArtists(string? artists)
        {
            if (!string.IsNullOrWhiteSpace(artists))
            {
                if (artists.Length > 100)
                {
                    _artists = artists[..99];
                }
                else
                {
                    _artists = artists;
                }
            }

            return this;
        }

        public PodcastBuilder AddEpisodeCount(int episodeCount)
        {
            _episodeCount = episodeCount;
            return this;
        }

        public PodcastBuilder AddLatestReleaseDate(DateTime? latestReleaseDate)
        {
            _latestReleaseDate = latestReleaseDate;
            return this;
        }

        public Podcast Build()
        {
            return new Podcast
            {
                Id = _id,
                ItunesId = _itunesId,
                CreatedDate = _createdDate.HasValue ? _createdDate.Value : default,
                Title = _title,
                Description = _description,
                ShowUrl = _showUrl,
                ImageUrl = _imageUrl,
                FeedUrl = _feedUrl,
                Country = _country,
                Artists = _artists,
                EpisodeCount = _episodeCount,
                LatestReleaseDate = _latestReleaseDate.HasValue ? _latestReleaseDate.Value : default,
            };
        }
    }
}
