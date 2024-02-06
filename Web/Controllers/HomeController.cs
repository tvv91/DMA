using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Db;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {

        }
        public IActionResult Index() => View();
    }
}
