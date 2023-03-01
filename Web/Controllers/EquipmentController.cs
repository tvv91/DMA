using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Response;

namespace Web.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly ITechInfoRepository _repository;

        public EquipmentController(ITechInfoRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Category (string category)
        {
            return Ok();
        }

        [HttpGet("search/state")]
        public async Task<IActionResult> SearchVinylState(string term)
        {
            return Ok(await _repository.VinylStates.Where(x => x.Data.Contains(term)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
        }

        [HttpGet("search/codec")]
        public async Task<IActionResult> SearchCodec(string term)
        {
            return Ok(await _repository.DigitalFormats.Where(x => x.Data.Contains(term)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
        }

        [HttpGet("search/bitness")]
        public async Task<IActionResult> SearchBitness(string term)
        {
            return Ok(await _repository.Bitnesses.Where(x => x.Data.ToString().Contains(term)).Select(x => new AutocompleteResponse { Label = x.Data.ToString() }).ToArrayAsync());
        }

        [HttpGet("search/sampling")]
        public async Task<IActionResult> SearchSampling(string term)
        {
            return Ok(await _repository.Samplings.Where(x => x.Data.ToString().Contains(term)).Select(x => new AutocompleteResponse { Label = x.Data.ToString() }).ToArrayAsync());
        }

        [HttpGet("search/format")]
        public async Task<IActionResult> SearchFormat(string term)
        {
            return Ok(await _repository.SourceFormats.Where(x => x.Data.Contains(term)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
        }

        [HttpGet("search/device")]
        public async Task<IActionResult> SearchDevice(string term)
        {
            return Ok(await _repository.Players.Where(x => x.Data.Contains(term)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
        }

        [HttpGet("search/cartridge")]
        public async Task<IActionResult> SearchCartridge(string term)
        {
            return Ok(await _repository.Cartriges.Where(x => x.Data.Contains(term)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
        }

        [HttpGet("search/amp")]
        public async Task<IActionResult> SearchAmp(string term)
        {
            return Ok(await _repository.Amplifiers.Where(x => x.Data.Contains(term)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
        }

        [HttpGet("search/adc")]
        public async Task<IActionResult> SearchAdc(string term)
        {
            return Ok(await _repository.Adcs.Where(x => x.Data.Contains(term)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
        }
    }
}
