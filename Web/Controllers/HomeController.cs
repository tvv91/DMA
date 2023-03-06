using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Db;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private IDbRepository _repository;
        public HomeController(IDbRepository repository)
        {
            _repository = repository;
        }
        public IActionResult Index() => View(_repository.Albums);
    }
}
