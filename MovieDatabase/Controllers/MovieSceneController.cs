using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualBasic;
using MovieDatabase.Data;
using MovieDatabase.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

/**
 * A Controller namespace for MovieDatabase controllers.
 */
namespace MovieDatabase.Controllers
{
    /**
     * A MovieScene Controller class controlling actions executed on the movie page.
     */
    public class MovieSceneController : Controller
    {
        /**
         * A MovieDatabase context object encapsulating all information about an individual HTTP request and response. 
         */
        private readonly MovieDatabaseContext _context;

        /**
         * A IMovieService object - interface for accessing movie data from the database
         */
        private readonly IMovieService _movieService;

        /**
         * A Movie object for storing the currenly opened movie information. 
         */
        private static Movie currentMovie = new Movie();

        /**
         * A MovieSceneController constructor.
         * @param IMovieService object
         * @param context - MovieDatabaseContext object
         */
        public MovieSceneController(IMovieService movieService, MovieDatabaseContext context)
        {
            _context = context;
            _movieService = movieService;
        }

        /**
         * An Index GET action passing a movie and all related information from database to the view.
         * @param id of the movie to be displayed.
         * @return view with the movie.
         */
        public async Task<IActionResult> Index(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.id == id);
            if (movie == null)
            {
                return NotFound();
            }

            // Calculating movie average rating
            var ratings = _context.Rating
                    .Where(r => r.movie_id == movie.id)
                    .ToList();
            var count = ratings.Count();
            float sum = 0;
            foreach (Rating rating in ratings)
            {
                sum = sum + rating.rate;
            }

            if (count != 0)
            {
                if (movie.rate != sum / count)
                {
                    movie.rate = sum / count;
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                movie.rate = sum;
            }
            // ---

            currentMovie = movie;

            ViewBag.directorVB = _context.Director
                        .Where(d => d.id == movie.director_id)
                        .ToList();

            ViewBag.actorsVB = _context.Actor
                        .Include(a => a.movies.Where(m => m.id == movie.id))
                        .ToList();

            ViewBag.genresVB = _context.Genre
                        .Include(g => g.movies.Where(m => m.id == movie.id))
                        .ToList();

            var reviews = _context.Rating
                       .Where(c => c.movie_id == id)
                       .ToList();

            ViewBag.reviewsVB = reviews;

            var comments = _context.Comment
                        .Where(c => c.movie_id == id)
                        .ToList();

            ViewBag.commentsVB = comments;

            var all_subcomments = await _context.Subcomment.ToListAsync();
            List<Subcomment> subcomments = new List<Subcomment>();


            foreach(Comment comment in comments)
            {
                var subcoms = _context.Subcomment.Where(sc => sc.comment_id == comment.id).ToList();
                subcomments.AddRange(subcoms);
            }

            ViewBag.subcommentsVB = subcomments;

            ViewBag.usersVB = await _context.Users.ToListAsync();

            string? u_id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (u_id == null)
            {
                ViewBag.currentUserVB = null;
            }
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == u_id);
            if (user == null)
            {
                ViewBag.currentUserVB = null;
            }

            ViewBag.currentUserVB = user;
            ViewBag.actorsVB = movie.actors;
            ViewBag.genresVB = movie.genres;
          
            return View(movie);
        }


        /**
         * An AddToFavorites POST action creating a new userMovie object with the correct context id (user-movie connection) in the database.
         * @return redirection to the Index view.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToFavorites()
        {
            string? u_id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (u_id == null)
            {
                return NotFound();
            }
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == u_id);
            if (user == null)
            {
                return NotFound();
            }

            UserMovie userMovie = new UserMovie();
            userMovie.context_id = 2;
            userMovie.user_id = user.Id;
            userMovie.movie_id = currentMovie.id;

            var userMovies = _context.UserMovie.Where(um => um.user_id == userMovie.user_id).ToList();
            userMovies = userMovies.Where(um => um.context_id == 2).ToList();
            bool userMovieExists = userMovies.Any(um => um.movie_id == currentMovie.id);
            if(userMovieExists)
            {
                return RedirectToAction(nameof(Index), new { id = currentMovie.id });
            }

            var context = new ValidationContext(userMovie, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(userMovie, context, validationResults, true))
            {
                return NotFound();
            }


            _context.Add(userMovie);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { id = currentMovie.id });
        }


        /**
         * An AddToWatchlist POST action creating a new userMovie object with the correct context id (user-movie connection) in the database.
         * @return redirection to the Index view.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToWatchlist()
        {
            string? u_id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (u_id == null)
            {
                return NotFound();
            }
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == u_id);
            if (user == null)
            {
                return NotFound();
            }

            UserMovie userMovie = new UserMovie();
            userMovie.context_id = 1;
            userMovie.user_id = user.Id;
            userMovie.movie_id = currentMovie.id;

            var userMovies = _context.UserMovie.Where(um => um.user_id == userMovie.user_id).ToList();
            userMovies = userMovies.Where(um => um.context_id == 2).ToList();
            bool userMovieExists = userMovies.Any(um => um.movie_id == currentMovie.id);
            if (userMovieExists)
            {
                return RedirectToAction(nameof(Index), new { id = currentMovie.id });
            }

            var context = new ValidationContext(userMovie, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(userMovie, context, validationResults, true))
            {
                return NotFound();
            }


            _context.Add(userMovie);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { id = currentMovie.id });
        }


        /**
        * A member method for directing to an instruction view for not logged in user.
        * @return view with instruction.
        */
        public IActionResult Nope() { return View(); }

    }
}
