using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Controllers
{
    public class StatisticController(IStatisticService statisticService) : Controller
    {
        private readonly IStatisticService _statisticService = statisticService;

        [HttpGet("statistic")]
        public async Task<IActionResult> Index()
        {
            var result = await _statisticService.ProcessAsync();
            var viewModel = JsonSerializer.Deserialize<StatisticViewModel>(result.Name);
            if (viewModel is null)
                return Problem("Failed to deserialize StatisticViewModel.");
            viewModel.LastUpdate = result.LastUpdate;
            return View("Index", viewModel);
        }
    }
}
