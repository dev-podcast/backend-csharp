using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DevPodcast.Domain;

namespace DevPodcast.Server.Controllers
{
    [Produces("application/json")]
    //[Route("api/Search")]
    public class SearchController : Controller
    {
        private IMapper _mapper { get; }
        private IUnitOfWork _unitOfWork { get; }

        public SearchController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [Route("api/search/{searchString}")]
        public async Task<IActionResult> Get(string searchString)
        {
            var result = await _unitOfWork.SearchRepository.GetSearchResultAsync(_unitOfWork, searchString);
            return Ok(result);
        }
    }
}