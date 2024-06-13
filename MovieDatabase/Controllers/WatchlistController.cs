using Microsoft.AspNetCore.Mvc;

namespace MovieDatabase.Controllers
{
    public class WatchlistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
