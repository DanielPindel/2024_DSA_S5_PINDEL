using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using MovieDatabase.Data;
using MovieDatabase.Models;
using SQLitePCL;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

/**
 * A Controller namespace for MovieDatabase controllers.
 */
namespace MovieDatabase.Controllers
{
    /**
     * A Home Controller class controlling the Home Page
     */
    public class HomeController : Controller
    {
        /**
         * An ILogger object - a generic interface for logging where the category name is derived from the specified HomeController type name.
         */
        private readonly ILogger<HomeController> _logger;

        /**
         * A IMovieService object - interface for accessing movie data from the database
         */
        private readonly IMovieService _movieService;

        /**
         * A MovieDatabase context object encapsulating all information about an individual HTTP request and response. 
         */
        private readonly MovieDatabaseContext _context;

        /**
         * A HomeController constructor.
         * @param logger - an ILogger object.
         * @param IMovieService object
         * @param context - MovieDatabaseContext object
         */
        public HomeController(ILogger<HomeController> logger, IMovieService movieService, MovieDatabaseContext context)
        {
            _logger = logger;
            _movieService = movieService;
            _context = context;
        }


        /**
         * An Index GET action passing all movies from database to the view along with other related information for display.
         * @param searchString - string object passed from the view for filtering the movies.
         * @param searchType - string object passed from the view for the type of filtering.
         * @return view with all movies.
         */
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


        /**
         * A member method for directing to a view displaying Privacy.
         * @return view.
         */
        public IActionResult Privacy()
        {
            return View();
        }

        /**
         * A member method for directing to an error view.
         * @return view with error.
         */
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
