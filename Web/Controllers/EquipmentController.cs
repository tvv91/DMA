using Microsoft.AspNetCore.Mvc;
using Web.Db;

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
    }
}
