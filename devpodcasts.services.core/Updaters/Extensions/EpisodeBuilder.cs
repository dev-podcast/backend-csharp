using System;
using System.Linq;
using System.Xml.Linq;
using devpodcasts.Domain.Entities;
using devpodcasts.Services.Core.Utils;

namespace devpodcasts.Services.Core.Updaters.Extensions
{
    public class EpisodeBuilder
    {
        private string _title;
        private int _podcastId;
        private string _author;
        private string _audioUrl;
        private string _audioType;
        private string _audioDuration;
        private DateTime? _publishedDate;
        private string _description;
        private string _imageUrl;
        private string _sourceUrl;

        public EpisodeBuilder AddTitle(string title)
        {
            if (title == null) return this;
            if (title.Length > 250)
                title = title.Substring(0, 249);

            _title = title;

            return this;
        }

        public EpisodeBuilder AddImageUrl(string imageUrl)
        {
            _imageUrl = imageUrl;
            return this;
        }

        public EpisodeBuilder AddAudioTypeAndAudioUrl(XElement enclosure)
        {
            if (enclosure == null) return this;
            if (enclosure.HasAttributes)
            {
                var audioStreamObject = enclosure.Attributes().ToList();
                var audioType = audioStreamObject.FirstOrDefault(x => x.Name == EpisodeConstants.TypeElementName);
                var audioUrl = audioStreamObject.FirstOrDefault(x => x.Name == EpisodeConstants.UrlElementName);

                _audioType = audioType?.Value;

                if (_audioType != null && _audioType.Length > 10)
                    _audioType = _audioType.Substring(0, 10); 

                _audioUrl = audioUrl?.Value;
            }
            return this;
        }

        public EpisodeBuilder AddSourceUrl(XElement link)
        {
            if (link != null) _sourceUrl = link.Value;
            return this;
        }

        public EpisodeBuilder AddPublishedDate(XElement publishedDate)
        {
            if (DateTime.TryParse(publishedDate?.Value, out var newDate)) _publishedDate = newDate;
            return this;
        }

        public EpisodeBuilder AddAuthor(XElement itunesAuthor, XElement author)
        {
            if (itunesAuthor != null)
                _author = itunesAuthor.Value;
            else if (author != null) _author = author.Value;

            if (_author != null && _author.Length > 250)
                _author = _author.Substring(0, 250);
            return this;
        }

        public EpisodeBuilder AddDescription(XElement description, XElement itunesSummary, XElement summary)
        {

            if (description != null)
                _ = description.Value;
            else if (itunesSummary != null)
               _description = itunesSummary.Value;
            else if (summary != null) _description = summary.Value;

            if (_description != null)
                _description = StringCleaner.CleanHtml(_description);

            return this;
        }

        public EpisodeBuilder AddAudioDuration(XElement itunesDuration, XElement duration)
        {
            if (itunesDuration != null)
               _audioDuration = itunesDuration.Value;
            else if (duration != null) _audioDuration = duration.Value;

            if (_audioDuration != null && _audioDuration.Length > 10)
                _audioDuration = _audioDuration.Substring(0, 10);

            return this;
        }
    
        public EpisodeBuilder AddPodcast(Podcast podcast)
        {
            if (podcast == null) return this;
            _podcastId = podcast.Id;
            return this;
        }

        public Episode Build()
        {
            return new Episode()
            {
                Title =  _title,
                ImageUrl = _imageUrl,
                SourceUrl =  _sourceUrl,
                AudioType =  _audioType,
                AudioUrl = _audioUrl,
                AudioDuration = _audioDuration,
                Description = _description,
                Author = _author,
                PublishedDate = _publishedDate,
                PodcastId =  _podcastId,
                CreatedDate = DateTime.Now
            };
        }
    }
}