using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.Data;
using MovieDatabase.Models;
using System.Security.Claims;

/**
 * A Controller namespace for MovieDatabase controllers.
 */
namespace MovieDatabase.Controllers
{
    /**
     * A Ratings And Reviews Controller class controlling all actions executed on ratings and reviews list.
     */
    public class RatingsAndReviewsController : Controller
    {
        /**
         * A MovieDatabase context object encapsulating all information about an individual HTTP request and response. 
         */
        private readonly MovieDatabaseContext _context;

        /**
         * A Ratings And Reviews Controller constructor. 
         * @param context of the database application.
         */
        public RatingsAndReviewsController(MovieDatabaseContext context)
        {
            _context = context;
        }

        /**
         * An Index GET action passing all rated movies and user information from database to the view displaying ratings of the user.
         * @return view with the user.
         */
        public async Task<IActionResult> Index()
        {
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
