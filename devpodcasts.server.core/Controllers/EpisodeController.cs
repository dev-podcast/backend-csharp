using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevPodcast.Domain;
using DevPodcast.Domain.Entities;
using DevPodcast.Server.Core.ViewModels;

namespace DevPodcast.Server.Core.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    public class EpisodeController : Controller
    {
        private IMapper _mapper { get; }
        private IUnitOfWork _unitOfWork { get; }

        public EpisodeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("v1/{showId}/{episodeId}")]
        public async Task<IActionResult> GetEpisode(int showId, int episodeId)
        {
            var episode = await _unitOfWork.EpisodeRepository.GetAsync(x => x.PodcastId == showId && x.Id == episodeId);
            var model = _mapper.Map<Episode, EpisodeViewModel>(episode);
            return Ok(model);
        }

        [HttpGet]
        [Route("v1/all/{showId}")]
        public async Task<IActionResult> GetAllEpisodes(int showId)
        {
            var episodes = await _unitOfWork.EpisodeRepository.GetAllAsync(x => x.PodcastId == showId);
            var model = _mapper.Map<List<Episode>, List<EpisodeViewModel>>(episodes);
            return Ok(model);
        }

        [HttpGet]
        [Route("v1/recent/{showId}")]
        public async Task<IActionResult> Recent(int showId)
        {
            var episodes = await _unitOfWork.EpisodeRepository.GetRecentAsync(showId, 15);
            var model = _mapper.Map<List<Episode>, List<EpisodeViewModel>>(episodes);
            return Ok(model);
        }

        [HttpGet]
        [Route("v1/recent/{showId}/{limit}")]
        public async Task<IActionResult> Recent(int showId, int limit)
        {
            var episodes = await _unitOfWork.EpisodeRepository.GetRecentAsync(showId, limit);
            var model = _mapper.Map<List<Episode>, List<EpisodeViewModel>>(episodes);
            return Ok(model);
        }
    }
}