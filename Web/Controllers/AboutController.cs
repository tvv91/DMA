using Microsoft.AspNetCore.Mvc;
using MediatR;
using Web.Features.Supporting;

namespace Web.Controllers
{
    public class AboutController(ISender sender) : Controller
    {
        private readonly ISender _sender = sender;

        public async Task<IActionResult> Index()
        {
            await _sender.Send(new AboutQuery());
            return View();
        }
    }
}
