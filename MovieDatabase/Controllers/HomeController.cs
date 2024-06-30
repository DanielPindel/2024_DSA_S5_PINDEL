using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using MovieDatabase.Data;
using MovieDatabase.Models;
using SQLitePCL;
using System.Diagnostics;
using System.Security.Claims;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

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

        /*[HttpGet]
        public IActionResult SearchMovies(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return Json(new List<Movie>());
            }

            var movies = _context.Movie
                .Where(m => (m.title).Contains(searchString))
                .Select(m => new { m.id, title = m.title })
                .ToList();

            return Json(movies);
        }*/

        public async Task<IActionResult> Index(string searchString, string searchType)
        {
            var movies = await _movieService.GetAllMoviesAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                if(searchType=="Title")
                    movies = movies.Where(m => m.title!.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();

                if(searchType == "Genre")
                {
                    foreach (Movie mo in movies)
                    {
                        var genreMovies = _context.GenreMovie
                               .Where(gm => gm.moviesid == mo.id)
                               .ToList();
                    }
                    var genres = _context.Genre.Where(g => g.tag.Contains(searchString)).ToList();
                    movies = movies.Where(m => m.genres.Intersect(genres).Any()).ToList();
                }

                if (searchType == "Actor")
                {

                    var actors = await _context.Actor.ToListAsync();

                    actors = actors.Where(g => g.nameSurnameLabel.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();

                    //loop needed for updating movie.actors, otherwise it's empty for some reason
                    foreach (Actor actor in actors)
                    {
                        var movie = _context.Movie
                        .Include(m => m.actors.Where(m => m.id == actor.id))
                        .ToList();
                    }

                    movies = movies.Where(m => m.actors.Intersect(actors).Any()).ToList();
                }
            }

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

            ViewBag.genresVB = await _context.Genre.ToListAsync();

            foreach (Movie mo in movies)
            {
                var genreMovies = _context.GenreMovie
                       .Where(gm => gm.moviesid == mo.id)
                       .ToList();
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
