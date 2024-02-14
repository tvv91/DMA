using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index() => View();
    }
}
