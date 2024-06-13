using Microsoft.AspNetCore.Mvc;

namespace MovieDatabase.Controllers
{
    public class FavoritesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
