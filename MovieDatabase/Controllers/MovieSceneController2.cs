using Microsoft.AspNetCore.Mvc;

namespace MovieDatabase.Controllers
{
    public class MovieSceneController2 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
