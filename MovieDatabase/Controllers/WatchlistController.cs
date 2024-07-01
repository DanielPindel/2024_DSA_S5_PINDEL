using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.Data;
using MovieDatabase.Models;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.Sig;
using System.Security.Claims;

/**
 * A Controller namespace for MovieDatabase controllers.
 */
namespace MovieDatabase.Controllers
{
    /**
     * A Watchlist Controller class controlling all actions executed on favourites list.
     */
    public class WatchlistController : Controller
    {
        /**
         * A MovieDatabase context object encapsulating all information about an individual HTTP request and response. 
         */
        private readonly MovieDatabaseContext _context;

        /**
         * A Watchlist Controller constructor. 
         * @param context of the database application.
         */
        public WatchlistController(MovieDatabaseContext context)
        {
            _context = context;
        }

        /**
         * An Index GET action passing all movies and user information from database to the view displaying watchlist of the user.
         * @return view with the user.
         */
        public async Task<IActionResult> Index()
        {
            // Some stuff to get the user
            string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var userMovies = _context.UserMovie
                       .Where(um => um.user_id == id)
                       .ToList();

            List<Movie> movies = new List<Movie>();

            foreach( var userMovie in userMovies )
            {
                if(userMovie.context_id == 1)
                {
                    var movie = _context.Movie.FirstOrDefault(m => m.id == userMovie.movie_id);
                    movies.Add(movie);
                }
            }

            ViewBag.moviesVB = movies;

            return View(user);
        }

        /**
         * A Remove POST action deleting the userMovie object (user-movie connection) from the database.
         * @param id of the movie from user-movie connection to be deleted.
         * @return OkResult object.
         */
        public async Task<IActionResult> Remove(int id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }

            var userMovie = await _context.UserMovie
                .FirstOrDefaultAsync(um => um.user_id == userId && um.movie_id == id && um.context_id == 1);

            if (userMovie == null)
            {
                return NotFound();
            }

            _context.UserMovie.Remove(userMovie);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
