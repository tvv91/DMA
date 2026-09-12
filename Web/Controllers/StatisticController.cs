using Microsoft.AspNetCore.Mvc;
using MediatR;
using Web.Features.Supporting;

namespace Web.Controllers
{
    public class StatisticController(ISender sender) : Controller
    {
        private readonly ISender _sender = sender;

        [HttpGet("statistic")]
        public async Task<IActionResult> Index()
        {
            var viewModel = await _sender.Send(new StatisticQuery());
            return viewModel is null ? Problem("Failed to deserialize StatisticViewModel.") : View("Index", viewModel);
        }
    }
}
