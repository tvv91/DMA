using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Web.Db.Interfaces;
using Web.ViewModels;

namespace Web.Controllers
{
    public class StatisticController : Controller
    {
        private readonly IStatisticRepository _statisticRepository;

        public StatisticController(IStatisticRepository statisticRepository)
        {
            _statisticRepository = statisticRepository;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _statisticRepository.Process();
            var viewModel = JsonSerializer.Deserialize<StatisticViewModel>(result.Data);
            viewModel.LastUpdate = result.LastUpdate;
            return View("Index", viewModel);
        }
    }
}
