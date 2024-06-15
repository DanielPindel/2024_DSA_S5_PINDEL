using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.Data;
using MovieDatabase.Models;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.Sig;
using System.Security.Claims;

namespace MovieDatabase.Controllers
{
    public class WatchlistController : Controller
    {
        // Declaration of context as a class member
        private readonly MovieDatabaseContext _context;
        private readonly ILogger<WatchlistController> _logger;

        // Watchlist constructor that assigns our context to the class
        public WatchlistController(MovieDatabaseContext context)
        {
            _context = context;
        }


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
    }
}
