using Microsoft.AspNetCore.Mvc;

namespace Intelly_Api.Controllers
{
    public class ChartsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
