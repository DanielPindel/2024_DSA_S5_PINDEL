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
    public class HelpController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService _movieService;
        private readonly MovieDatabaseContext _context;

        public HelpController(ILogger<HomeController> logger, IMovieService movieService, MovieDatabaseContext context)
        {

        }

        public async Task<IActionResult> Index()
        {

            return View();
        }


    }
}
