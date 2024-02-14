using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class EquipmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
