using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Models;

namespace Web.Controllers
{
    public class AlbumController : Controller
    {
        private readonly IAlbumRepository _repository;
        public int PageSize = 100;

        public AlbumController(IAlbumRepository albumRepository)
        {
            _repository = albumRepository;
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

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Album album)
        {
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok();
        }
    }
}
