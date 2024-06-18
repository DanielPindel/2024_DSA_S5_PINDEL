using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MovieDatabase.Data;
using MovieDatabase.Models;
using SQLitePCL;
using System.Diagnostics;
using System.Security.Claims;

namespace MovieDatabase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService _movieService;
        private readonly MovieDatabaseContext _context;

        public HomeController(ILogger<HomeController> logger, IMovieService movieService, MovieDatabaseContext context)
        {
            _logger = logger;
            _movieService = movieService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _movieService.GetAllMoviesAsync();
            ViewBag.isAdminVB = false;

            string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != null)
            {
                var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    ViewBag.isAdminVB = user.is_admin;
                }
            }
            

            return View(movies);
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
