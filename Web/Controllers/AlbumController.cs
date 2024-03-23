using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Models;
using Web.ViewModels;

namespace Web.Controllers
{
    public class AlbumController : Controller
    {
        private const int ALBUMS_PER_PAGE = 15;
        private readonly IAlbumRepository _repository;

        public AlbumController(IAlbumRepository albumRepository)
        {
            _repository = albumRepository;
        }

        [HttpGet]
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

        [HttpGet("album/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id == 0 || id < 0)
            {
                return BadRequest();
            }
            
            Album? album = await _repository.Albums
                .Include(a => a.Artist)
                .Include(a => a.Country)
                .Include(a => a.Genre)
                .Include(a => a.Label)
                .Include(a => a.Reissue)
                .Include(a => a.Year)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            if (album == null)
            {
                return NotFound();
            }

            AlbumDetailsViewModel albumDetails = new AlbumDetailsViewModel
            {
                Album = album,
            };
            return View("AlbumDetails", albumDetails);
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
