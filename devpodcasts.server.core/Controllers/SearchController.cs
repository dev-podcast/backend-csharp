using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DevPodcast.Domain;

namespace DevPodcast.Server.Core.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Route("v1/[controller]")]
    public class SearchController : Controller
    {
        private IMapper _mapper { get; }
        private IUnitOfWork _unitOfWork { get; }

        public SearchController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("search/{searchString}")]
        public async Task<IActionResult> Get(string searchString)
        {
            var result = await _unitOfWork.SearchRepository.GetSearchResultAsync(_unitOfWork, searchString);
            return Ok(result);
        }
    }
}