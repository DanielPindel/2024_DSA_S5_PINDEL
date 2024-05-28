using Microsoft.AspNetCore.Mvc;

namespace MovieDatabase.Controllers
{
    public class MovieSceneController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
