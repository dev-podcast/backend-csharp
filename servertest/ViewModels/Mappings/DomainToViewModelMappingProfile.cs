using AutoMapper;
using devpodcasts.domain.entities;


namespace devpodcasts.server.ViewModels.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Category, CategoryViewModel>()
                .ForMember(vm => vm.Id,
                    map => map.MapFrom(c => c.Id))
                .ForMember(vm => vm.Name, map =>
                    map.MapFrom(c => c.Description))
                .ForMember(vm => vm.PodcastCategories, map => 
                    map.MapFrom(c => c.PodcastCategories));

            CreateMap<EpisodeTag, EpisodeTagViewModel>()
                .ForMember(vm => vm.Episode,
                    map => map.MapFrom(e => e.Episode))
                .ForMember(vm => vm.Tag,
                    map => map.MapFrom(e => e.Tag));

            CreateMap<Episode, EpisodeViewModel>()
                .ForMember(vm => vm.Id,
                    map => map.MapFrom(e => e.Id))
                .ForMember(vm => vm.Description,
                    map => map.MapFrom(e => e.Description))
                .ForMember(vm => vm.CreatedDate,
                    map => map.MapFrom(e => e.CreatedDate))
                .ForMember(vm => vm.Author,
                    map => map.MapFrom(e => e.Author))
                .ForMember(vm => vm.ImageUrl,
                    map => map.MapFrom(e => e.ImageUrl))
                .ForMember(vm => vm.AudioDuration,
                    map => map.MapFrom(e => e.AudioDuration))
                .ForMember(vm => vm.AudioType,
                    map => map.MapFrom(e => e.AudioType))
                .ForMember(vm => vm.AudioUrl,
                    map => map.MapFrom(e => e.AudioUrl))
                .ForMember(vm => vm.Podcast,
                    map => map.MapFrom(e => e.Podcast))
                .ForMember(vm => vm.PublishedDate,
                    map => map.MapFrom(e => e.PublishedDate))
                .ForMember(vm => vm.SourceUrl,
                    map => map.MapFrom(e => e.SourceUrl))
                .ForMember(vm => vm.Title,
                    map => map.MapFrom(e => e.Title))
                .ForMember(vm => vm.Tags,
                    map => map.MapFrom(e => e.EpisodeTags));


            CreateMap<PodcastCategory, PodcastCategoryViewModel>()
                .ForMember(vm => vm.Podcast,
                    map => map.MapFrom(pc => pc.Podcast))
                .ForMember(vm => vm.Category,
                    map => map.MapFrom(pc => pc.Category));

            CreateMap<PodcastTag, PodcastTagViewModel>()
                .ForMember(vm => vm.Podcast,
                    map => map.MapFrom(pt => pt.Podcast))
                .ForMember(vm => vm.Tag,
                    map => map.MapFrom(pt => pt.Tag));

            CreateMap<Podcast, PodcastViewModel>()
               .ForMember(vm => vm.Id,
                   map => map.MapFrom(e => e.Id))
               .ForMember(vm => vm.Description,
                   map => map.MapFrom(e => e.Description))
               .ForMember(vm => vm.CreatedDate,
                   map => map.MapFrom(e => e.CreatedDate))
               .ForMember(vm => vm.Artists,
                   map => map.MapFrom(e => e.Artists))
               .ForMember(vm => vm.ImageUrl,
                   map => map.MapFrom(e => e.ImageUrl))
               .ForMember(vm => vm.EpisodeCount,
                   map => map.MapFrom(e => e.EpisodeCount))
               .ForMember(vm => vm.Categories,
                   map => map.MapFrom(e => e.PodcastCategories))
               .ForMember(vm => vm.Country,
                   map => map.MapFrom(e => e.Country))
               .ForMember(vm => vm.Episodes,
                   map => map.MapFrom(e => e.Episodes))
               .ForMember(vm => vm.FeedUrl,
                   map => map.MapFrom(e => e.FeedUrl))
               .ForMember(vm => vm.ItunesId,
                   map => map.MapFrom(e => e.ItunesId))
               .ForMember(vm => vm.Title,
                   map => map.MapFrom(e => e.Title))
               .ForMember(vm => vm.ShowUrl,
                   map => map.MapFrom(e => e.ShowUrl))
               .ForMember(vm => vm.LatestReleaseDate,
                   map => map.MapFrom(e => e.LatestReleaseDate))
               .ForMember(vm => vm.Tags,
                   map => map.MapFrom(e => e.PodcastTags)).PreserveReferences();

            CreateMap<Tag, TagViewModel>()
                .ForMember(vm => vm.Id,
                    map => map.MapFrom(t => t.Id))
                .ForMember(vm => vm.Description,
                    map => map.MapFrom(t => t.Description))
                .ForMember(vm => vm.EpisodeTags,
                    map => map.MapFrom(t => t.EpisodeTags))
                .ForMember(vm => vm.PodcastTags,
                    map => map.MapFrom(t => t.PodcastTags));

            
        }
    }
}
