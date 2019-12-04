using System;
using Xunit;
using Moq;
using DevPodcast.Domain;
using DevPodcast.Domain.Entities;
using DevPodcast.Domain.Interfaces;
using DevPodcast.Server.ViewModels;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DevPodcast.Server.Controllers;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using SharpTestsEx;

namespace DevPodcast.Server.Core.Test
{
    public class PodcastControllerTest
    {

        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IMapper> _mapper;
        PodcastController podcastController;
        List<Podcast> podcasts;
        private List<PodcastViewModel> podcastViewModels;


        public PodcastControllerTest()
        {

            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();

            podcastController = new PodcastController(_unitOfWork.Object, _mapper.Object);
            podcasts = new List<Podcast>()
            {
                new Podcast()
                {
                    Artists = "John", PodcastCategories = Array.Empty<PodcastCategory>(), Country = "USA",
                    CreatedDate = DateTime.Now, Description = "Test1", EpisodeCount = 4, Episodes = new List<Episode>(),
                    FeedUrl = "", Id = 1, ImageUrl = ""
                },
                new Podcast()
                {
                    Artists = "Jack", PodcastCategories = Array.Empty<PodcastCategory>(), Country = "USA",
                    CreatedDate = DateTime.Now, Description = "Test2", EpisodeCount = 5, Episodes = new List<Episode>(),
                    FeedUrl = "", Id = 2, ImageUrl = ""
                },
                new Podcast()
                {
                    Artists = "Jingle", PodcastCategories = Array.Empty<PodcastCategory>(), Country = "USA",
                    CreatedDate = DateTime.Now, Description = "Test3", EpisodeCount = 7, Episodes = new List<Episode>(),
                    FeedUrl = "", Id = 3, ImageUrl = ""
                },
                new Podcast()
                {
                    Artists = "Heimer", PodcastCategories = Array.Empty<PodcastCategory>(), Country = "USA",
                    CreatedDate = DateTime.Now, Description = "Test4", EpisodeCount = 20,
                    Episodes = new List<Episode>(), FeedUrl = "", Id = 4, ImageUrl = ""
                },
                new Podcast()
                {
                    Artists = "Smith", PodcastCategories = Array.Empty<PodcastCategory>(), Country = "USA",
                    CreatedDate = DateTime.Now, Description = "Test5", EpisodeCount = 3, Episodes = new List<Episode>(),
                    FeedUrl = "", Id = 5, ImageUrl = ""
                },
            };


            podcastViewModels = new List<PodcastViewModel>()
            {
                new PodcastViewModel()
                {
                    Artists = "John", Categories = Array.Empty<PodcastCategoryViewModel>(), Country = "USA",
                    CreatedDate = DateTime.Now, Description = "Test1", EpisodeCount = 4,
                    Episodes = new List<EpisodeViewModel>(), FeedUrl = "", Id = 1, ImageUrl = ""
                },
                new PodcastViewModel()
                {
                    Artists = "Jack", Categories = Array.Empty<PodcastCategoryViewModel>(), Country = "USA",
                    CreatedDate = DateTime.Now, Description = "Test2", EpisodeCount = 5,
                    Episodes = new List<EpisodeViewModel>(), FeedUrl = "", Id = 2, ImageUrl = ""
                },
                new PodcastViewModel()
                {
                    Artists = "Jingle", Categories = Array.Empty<PodcastCategoryViewModel>(), Country = "USA",
                    CreatedDate = DateTime.Now, Description = "Test3", EpisodeCount = 7,
                    Episodes = new List<EpisodeViewModel>(), FeedUrl = "", Id = 3, ImageUrl = ""
                },
                new PodcastViewModel()
                {
                    Artists = "Heimer", Categories = Array.Empty<PodcastCategoryViewModel>(), Country = "USA",
                    CreatedDate = DateTime.Now, Description = "Test4", EpisodeCount = 20,
                    Episodes = new List<EpisodeViewModel>(), FeedUrl = "", Id = 4, ImageUrl = ""
                },
                new PodcastViewModel()
                {
                    Artists = "Smith", Categories = Array.Empty<PodcastCategoryViewModel>(), Country = "USA",
                    CreatedDate = DateTime.Now, Description = "Test5", EpisodeCount = 3,
                    Episodes = new List<EpisodeViewModel>(), FeedUrl = "", Id = 5, ImageUrl = ""
                },
            };


        }

        [Fact]
        public void GetAllPodcasts()
        {
            _unitOfWork.Setup(x => x.PodcastRepository.GetAllAsync(b => true)).ReturnsAsync(podcasts);
            _mapper.Setup(x => x.Map<List<Podcast>, List<PodcastViewModel>>(podcasts)).Returns(podcastViewModels);
            var result =  podcastController.Get().Result;

            var objectResult = result as OkObjectResult;
            var podcastResult = objectResult.Value as List<PodcastViewModel>;

            podcastResult.Should().Not.Be.Null();
            podcastResult.Should().Have.Count.GreaterThan(0);
            podcastResult.Should().Have.Count.EqualTo(5);

        }

        [Fact]
        public void Get_Podcast()
        {
            var podcast = podcasts.FirstOrDefault();
            _unitOfWork.Setup(p => p.PodcastRepository.GetAsync(x => x.Id == 1)).ReturnsAsync(podcast);
            _mapper.Setup(x => x.Map<List<Podcast>, List<PodcastViewModel>>(podcasts)).Returns(podcastViewModels);

            var result = podcastController.Get(1).Result;
            var objectResult = result as OkObjectResult;
            var podcastResult = objectResult.Value as List<PodcastViewModel>;

            podcastResult.Should().Not.Be.Null();
            podcastResult.Select(x => x?.Id).First().Should().Be.EqualTo(1);
        }
    }
}
