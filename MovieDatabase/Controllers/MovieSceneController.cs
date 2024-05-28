using Microsoft.AspNetCore.Mvc;

namespace MovieDatabase.Controllers
{
    public class MovieSceneController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LaLaLand()
        {
            return View();
        }

        public IActionResult Oppenheimer()
        {
            return View();
        }

        public IActionResult BladeRunner2049()
        {
            return View();
        }
    }
}
