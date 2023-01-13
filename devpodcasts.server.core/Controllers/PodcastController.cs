using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using devpodcasts.Domain;
using devpodcasts.Domain.Entities;
using devpodcasts.Server.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace devpodcasts.Server.Core.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    public class PodcastController : Controller
    {
        private IMapper _mapper { get; }
        private IUnitOfWork _unitOfWork { get; }

        public PodcastController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("v1/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var podcast = await _unitOfWork.PodcastRepository.GetAsync(x => x.Id == id);
            var model = _mapper.Map<Podcast, PodcastViewModel>(podcast);
            return Ok(model);
        }

        [HttpGet]
        [Route("v1/podcast/all")]
        public async Task<IActionResult> GetAll()
        {
            var podcasts = await _unitOfWork.PodcastRepository.GetAllAsync();
            var model = _mapper.Map<ICollection<Podcast>, List<PodcastViewModel>>(podcasts);
            return Ok(model);
        }

        [HttpGet]
        [Route("v1/podcast/recent")]
        public async Task<IActionResult> Recent()
        {
            var podcasts = await _unitOfWork.PodcastRepository.GetRecentAsync(15);
            var model = _mapper.Map<ICollection<Podcast>, List<PodcastViewModel>>(podcasts);
            return Ok(model);
        }

        [HttpGet]
        [Route("v1/podcast/recent/{limit}")]
        public async Task<IActionResult> Recent(int limit)
        {
            var podcasts = await _unitOfWork.PodcastRepository.GetRecentAsync(limit);
            var model = _mapper.Map<ICollection<Podcast>, List<PodcastViewModel>>(podcasts);

            return Ok(model);
        }

        [HttpGet]
        [Route("v1/podcast/recent/{podcastLimit}/{episodeLimit}")]
        public async Task<IActionResult> Recent(int podcastLimit, int episodeLimit)
        {
            var podcasts = await _unitOfWork.PodcastRepository.GetRecentAsync(podcastLimit, episodeLimit);
            var model = _mapper.Map<ICollection<Podcast>, List<PodcastViewModel>>(podcasts);
            return Ok(model);
        }

        [HttpGet]
        [Route("v1/podcast/tag/{id}")]
        public async Task<IActionResult> Tag(int Id)
        {
            var tags = await _unitOfWork.TagRepository.GetAsync(x => x.Id == Id);
            var model = _mapper.Map<ICollection<Podcast>, List<PodcastViewModel>>(tags.Podcasts);
            return Ok(model);
        }
    }
}