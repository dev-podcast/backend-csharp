﻿using System;
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
    public class TagController : Controller
    {
        private IMapper _mapper { get; }
        private IUnitOfWork _unitOfWork { get; }

        public TagController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("v1/tags/all")]
        public async Task<IActionResult> Get()
        {
            var tags = await _unitOfWork.TagRepository.GetAllAsync();
            var model = _mapper.Map<List<Tag>, List<TagViewModel>>(tags);
            return Ok(model);
        }

        [HttpGet]
        [Route("v1/tags/{tagId}")]
        public async Task<IActionResult> Get(Guid tagId)
        {
            var tag = await _unitOfWork.TagRepository.GetAsync(t => t.Id == tagId);
            var model = _mapper.Map<Tag, TagViewModel>(tag);
            return Ok(model);
        }

        [HttpGet]
        [Route("v1/tags/{tagName}")]
        public async Task<IActionResult> Get(string tagName)
        {
            var tag = await _unitOfWork.TagRepository.GetAsync(t => t.Description == tagName);
            var model = _mapper.Map<Tag, TagViewModel>(tag);
            return Ok(model);
        }

        //[HttpGet]
        //[Route("id/search/{tagId}/{type}")]
        //public async Task<IActionResult> Get(int tagId, int type)
        //{
        //    //switch (type)
        //    //{
        //    //    case (int)SearchType.All:
        //    //        SearchResult searchResult = new SearchResult();
        //    //        searchResult.Episodes = await _unitOfWork.EpisodeTagRepository.GetByTagIdAsync(tagId);
        //    //        searchResult.Podcasts = await _unitOfWork.PodcastTagRepository.GetByTagIdAsync(tagId);
        //    //        return Ok(searchResult);

        //    //    case (int)SearchType.Episode:
        //    //        var episode = await _unitOfWork.EpisodeTagRepository.GetByTagIdAsync(tagId);
        //    //        var episodeModel = _mapper.Map<List<Episode>, List<EpisodeViewModel>>(episode);
        //    //        return Ok(episodeModel);

        //    //    case (int)SearchType.Podcast:
        //    //        var podcast = await _unitOfWork.PodcastTagRepository.GetByTagIdAsync(tagId);
        //    //        var podcastModel = _mapper.Map<List<Podcast>, List<PodcastViewModel>>(podcast);
        //    //        return Ok(podcastModel);

        //    //    default:
        //    //        return NotFound();
        //    //}
        //    return Ok();
        //}

        //[HttpGet]
        //[Route("name/search/{tagName}/{type}")]
        //public async Task<IActionResult> Get(string tagName, int type)
        //{
        //    //switch (type)
        //    //{
        //    //    case (int)SearchType.All:
        //    //        SearchResult searchResult = new SearchResult();
        //    //        searchResult.Episodes = await _unitOfWork.EpisodeTagRepository.GetByTagNameAsync(tagName);
        //    //        searchResult.Podcasts = await _unitOfWork.PodcastTagRepository.GetByTagNameAsync(tagName);
        //    //        return Ok(searchResult);

        //    //    case (int)SearchType.Episode:
        //    //        var episode = await _unitOfWork.EpisodeTagRepository.GetByTagNameAsync(tagName);
        //    //        var episodeModel = _mapper.Map<List<Episode>, List<EpisodeViewModel>>(episode);
        //    //        return Ok(episodeModel);

        //    //    case (int)SearchType.Podcast:
        //    //        var podcast = await _unitOfWork.PodcastTagRepository.GetByTagNameAsync(tagName);
        //    //        var podcastModel = _mapper.Map<List<Podcast>, List<PodcastViewModel>>(podcast);
        //    //        return Ok(podcastModel);

        //    //    default:
        //    //        return NotFound();
        //    //}
        //    return Ok();
        //}
    }
}