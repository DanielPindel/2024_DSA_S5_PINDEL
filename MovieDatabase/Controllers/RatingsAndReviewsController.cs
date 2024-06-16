using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.Data;
using MovieDatabase.Models;
using System.Security.Claims;

namespace MovieDatabase.Controllers
{
    public class RatingsAndReviewsController : Controller
    {
        // Declaration of context as a class member
        private readonly MovieDatabaseContext _context;

        // Watchlist constructor that assigns our context to the class
        public RatingsAndReviewsController(MovieDatabaseContext context)
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

            var ratings = _context.Rating
                       .Where(r => r.user_id == id)
                       .ToList(); ;

            ViewBag.ratingsVB = ratings;

            List<Movie> movies = new List<Movie>();

            foreach (var rating in ratings)
            {
                var movie = _context.Movie.FirstOrDefault(m => m.id == rating.movie_id);
                movies.Add(movie);
            }

            ViewBag.moviesVB = movies;


            return View(user);
        }
    }
}
