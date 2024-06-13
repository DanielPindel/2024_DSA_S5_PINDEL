using Microsoft.AspNetCore.Mvc;

namespace MovieDatabase.Controllers
{
    public class RatingsAndReviewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
