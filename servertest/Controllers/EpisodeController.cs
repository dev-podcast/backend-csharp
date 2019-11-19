using devpodcasts.domain.entities;
using devpodcasts.domain;
using devpodcasts.server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;

namespace devpodcasts.server.Controllers
{
    [Produces("application/json")]
    //[Route("api/Episode")]
    public class EpisodeController : Controller
    {
        private IMapper _mapper { get; }
        private IUnitOfWork _unitOfWork { get; }
        public EpisodeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
    
     
        [Route("api/episode/{episodeId}")]
        public async Task<IActionResult> Get(int showId, int episodeId)
        {
            var episode = await _unitOfWork.EpisodeRepository.GetAsync(x => x.PodcastId == showId && x.Id == episodeId);
            var model = _mapper.Map<Episode, EpisodeViewModel>(episode);
            return Ok(model);
        }

        [Route("api/episode/{showId}")]
        public async Task<IActionResult> Get(int showId)
        {
            var episodes = await _unitOfWork.EpisodeRepository.GetAllAsync(x => x.PodcastId == showId);
            var model = _mapper.Map<List<Episode>, List<EpisodeViewModel>>(episodes);
            return Ok(model); 
        }

        [HttpGet]
        [Route("api/episode/recent/{showId}")]
        public async Task<IActionResult> Recent(int showId)
        {
            var episodes = await _unitOfWork.EpisodeRepository.GetRecentAsync(showId, 15);
            var model = _mapper.Map<List<Episode>, List<EpisodeViewModel>>(episodes);
            return Ok(model);         
        }


        [HttpGet]
        [Route("api/episode/recent/{showId}/{limit}")]
        public async Task<IActionResult> Recent(int showId, int limit)
        {
            var episodes = await _unitOfWork.EpisodeRepository.GetRecentAsync(showId, limit);
            var model = _mapper.Map<List<Episode>, List<EpisodeViewModel>>(episodes);
            return Ok(model);
        }




    }
}