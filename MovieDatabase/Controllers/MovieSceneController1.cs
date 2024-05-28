using Microsoft.AspNetCore.Mvc;

namespace MovieDatabase.Controllers
{
    public class MovieSceneController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
