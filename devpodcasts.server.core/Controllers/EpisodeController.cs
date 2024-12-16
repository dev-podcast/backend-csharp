using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using devpodcasts.Domain;
using devpodcasts.Domain.Entities;
using devpodcasts.Server.Core.ViewModels;

namespace devpodcasts.Server.Core.Controllers
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
        public async Task<IActionResult> GetEpisode(Guid showId, Guid episodeId)
        {
            var episode = await _unitOfWork.EpisodeRepository.GetAsync(x => x.PodcastId == showId && x.Id == episodeId);
            var model = _mapper.Map<Episode, EpisodeViewModel>(episode);
            return Ok(model);
        }

        [HttpGet]
        [Route("v1/all/{showId}")]
        public async Task<IActionResult> GetAllEpisodes(Guid showId)
        {
            var episodes = await _unitOfWork.EpisodeRepository.GetByShowIdAsync(showId);
            var model = _mapper.Map<List<Episode>, List<EpisodeViewModel>>(episodes);
            return Ok(model);
        }

        [HttpGet]
        [Route("v1/recent/{showId}")]
        public async Task<IActionResult> Recent(Guid showId)
        {
            var episodes = await _unitOfWork.EpisodeRepository.GetRecentAsync(showId, 15);
            var model = _mapper.Map<List<Episode>, List<EpisodeViewModel>>(episodes);
            return Ok(model);
        }

        [HttpGet]
        [Route("v1/recent/{showId}/{limit}")]
        public async Task<IActionResult> Recent(Guid showId, int limit)
        {
            var episodes = await _unitOfWork.EpisodeRepository.GetRecentAsync(showId, limit);
            var model = _mapper.Map<List<Episode>, List<EpisodeViewModel>>(episodes);
            return Ok(model);
        }
    }
}