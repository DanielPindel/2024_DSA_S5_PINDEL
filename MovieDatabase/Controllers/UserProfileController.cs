using Microsoft.AspNetCore.Mvc;

namespace MovieDatabase.Controllers
{
    public class UserProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
