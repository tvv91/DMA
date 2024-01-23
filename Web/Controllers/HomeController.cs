using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Db;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private IAlbumRepository _repository;
        public int PageSize = 4;

        public HomeController(IAlbumRepository repository)
        {
            _repository = repository;
        }
        public IActionResult Index(int page = 1) => View(_repository.Albums
            .OrderBy(a => a.Id)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Include(a => a.Artist)
            .Include(a => a.Genre)
            .Include(a => a.Year)
            .Include(a => a.Reissue)
            .Include(a => a.Country)
            .Include(a => a.Label));
    }
}
