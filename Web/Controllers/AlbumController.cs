using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Models;
using Web.SignalRHubs;
using Web.ViewModels;

namespace Web.Controllers
{
    public class AlbumController : Controller
    {
        private const int ALBUMS_PER_PAGE = 15;
        private readonly IAlbumRepository _repository;
        private readonly IHubContext<DefaultHub> _hub;

        public AlbumController(IAlbumRepository albumRepository, IHubContext<DefaultHub> hubContext)
        {
            _repository = albumRepository;
            _hub = hubContext;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            AlbumViewModel albumViewModel = new AlbumViewModel
            {
                 Albums = await _repository.Albums
                 .Skip((page - 1) * ALBUMS_PER_PAGE)
                 .Take(ALBUMS_PER_PAGE)
                 .Include(a => a.Artist)
                 .ToListAsync()
            };

            return View("Index", albumViewModel);
        }

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
