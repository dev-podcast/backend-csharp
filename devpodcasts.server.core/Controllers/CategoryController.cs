using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using devpodcasts.Domain;
using devpodcasts.Domain.Entities;
using devpodcasts.Domain.Entities.Dtos;
using devpodcasts.Server.Core.ViewModels;

namespace devpodcasts.Server.Core.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Route("v1/[controller]")]
    public class CategoryController : Controller
    {
        private IMapper _mapper { get; }
        private IUnitOfWork _unitOfWork { get; }

        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("v1/category/all")]
        public async Task<IActionResult> GetAll()
        {
            var category = await _unitOfWork.CategoryRepository.GetAllAsync();

            var model = _mapper.Map<List<Category>, List<CategoryViewModel>>(category);
            return Ok(model);
        }

        [HttpGet]
        [Route("v1/category/{categoryId}")]
        public async Task<IActionResult> GetById(int categoryId)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(t => t.Id == categoryId);

            var model = _mapper.Map<Category, CategoryViewModel>(category);
            return Ok(model);
        }

        [HttpGet]
        [Route("v1/category{categoryName}")]
        public async Task<IActionResult> GetByName(string categoryName)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(t => t.Description == categoryName);

            var model = _mapper.Map<Category, CategoryViewModel>(category);
            return Ok(model);
        }

        [HttpGet]
        [Route("v1/category/search/{categoryId}/{type}")]
        public async Task<IActionResult> GetByIdAndType(int categoryId, int type)
        {
            SearchResult searchResult = new SearchResult();
            var category = await _unitOfWork.CategoryRepository.GetAsync(c => c.Id == categoryId);
            if (category != null)
            {
                searchResult.Category.Description = category.Description;
                searchResult.Category.Id = category.Id;
            }

            switch (type)
            {
                case (int)SearchType.All:                  
                    searchResult.Podcasts = category.Podcasts;
                    searchResult.Episodes = category.Episodes;
                    return Ok(searchResult);
                case (int)SearchType.Episode:
                    searchResult.Episodes = category.Episodes;
                    return Ok(searchResult); 
                case (int)SearchType.Podcast:
                    searchResult.Podcasts = category.Podcasts;
                    return Ok(searchResult);

                default:
                    return NotFound();
            }
        }

        [HttpGet]
        [Route("v1/category/search/{categoryName}/{type}")]
        public async Task<IActionResult> GetByNameAndType(string categoryName, int type)
        {
            SearchResult searchResult = new SearchResult();
            var category = await _unitOfWork.CategoryRepository.GetAsync(c => c.Description == categoryName);
            if (category != null)
            {
                searchResult.Category.Description = category.Description;
                searchResult.Category.Id = category.Id;
            }
            switch (type)
            {
                case (int)SearchType.All:
                    searchResult.Podcasts = category.Podcasts;
                    searchResult.Episodes = category.Episodes;
                    return Ok(searchResult);
                case (int)SearchType.Episode:
                    searchResult.Episodes = category.Episodes;
                    return Ok(searchResult);
                case (int)SearchType.Podcast:
                    searchResult.Podcasts = category.Podcasts;
                    return Ok(searchResult);

                default:
                    return NotFound();
            }
        }
    }
}