using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevPodcast.Domain;
using DevPodcast.Domain.Entities;
using DevPodcast.Domain.Interfaces;
using DevPodcast.Server.ViewModels;

namespace DevPodcast.Server.Controllers
{
    [Produces("application/json")]
    // [Route("api/Podcast")]
    public class PodcastController : Controller
    {
        private IMapper _mapper { get; }
        private IUnitOfWork _unitOfWork { get; }
        private IPodcastRepository _podcastRepository { get; }
        private IPodcastTagRepository _podcastTagRepository { get; }

        public PodcastController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [Route("api/podcast/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var podcast = await _unitOfWork.PodcastRepository.GetAsync(x => x.Id == id);
            var model = _mapper.Map<Podcast, PodcastViewModel>(podcast);
            return Ok(model);
        }

        [Route("api/podcast")]
        public async Task<IActionResult> Get()
        {
            var podcasts = await _unitOfWork.PodcastRepository.GetAllAsync(_ => true);
            var model = _mapper.Map<List<Podcast>, List<PodcastViewModel>>(podcasts);
            return Ok(model);
        }

        [Route("api/podcast/recent")]
        public async Task<IActionResult> Recent()
        {
            var podcasts = await _unitOfWork.PodcastRepository.GetRecentAsync(15);
            var model = _mapper.Map<List<Podcast>, List<PodcastViewModel>>(podcasts);
            return Ok(model);
        }

        [Route("api/podcast/recent/{limit}")]
        public async Task<IActionResult> Recent(int limit)
        {
            var podcasts = await _unitOfWork.PodcastRepository.GetRecentAsync(limit);
            var model = _mapper.Map<List<Podcast>, List<PodcastViewModel>>(podcasts);

            return Ok(model);
        }

        [Route("api/podcast/recent/{podcastLimit}/{episodeLimit}")]
        public async Task<IActionResult> Recent(int podcastLimit, int episodeLimit)
        {
            var podcasts = await _unitOfWork.PodcastRepository.GetRecentAsync(podcastLimit, episodeLimit);
            var model = _mapper.Map<List<Podcast>, List<PodcastViewModel>>(podcasts);
            return Ok(model);
        }

        [Route("api/podcast/tag/{id}")]
        public async Task<IActionResult> Tag(int Id)
        {
            var podcasts = await _unitOfWork.PodcastTagRepository.GetByTagIdAsync(Id);
            var model = _mapper.Map<List<Podcast>, List<PodcastViewModel>>(podcasts);
            return Ok(model);
        }
    }
}