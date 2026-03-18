using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Controllers
{
    public class StatisticController : Controller
    {
        private readonly IStatisticService _statisticService;

        public StatisticController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        [HttpGet("statistic")]
        public async Task<IActionResult> Index()
        {
            var result = await _statisticService.ProcessAsync();
            var viewModel = JsonSerializer.Deserialize<StatisticViewModel>(result.Name);
            viewModel.LastUpdate = result.LastUpdate;
            return View("Index", viewModel);
        }
    }
}
