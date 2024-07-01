using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.Data;
using MovieDatabase.Models;
using Newtonsoft.Json.Linq;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

/**
 * A Controller namespace for MovieDatabase controllers.
 */
namespace MovieDatabase.Controllers
{
    /**
     * A RatingsController class controlling all actions executed on ratings.
     */
    public class RatingsController : Controller
    {
        /**
         * A MovieDatabase context object encapsulating all information about an individual HTTP request and response. 
         */
        private readonly MovieDatabaseContext _context;

        /**
         * A Movie object for storing the last opened movie information. 
         */
        private static Movie currentMovie = new Movie();

        /**
         * A Ratings Controller constructor.
         * @param context of the database application.
         */
        public RatingsController(MovieDatabaseContext context)
        {
            _context = context;
        }


        /**
         * An Index GET action passing all ratings from database to the view displaying ratings in a table.
         * @return view with all ratings.
         */
        public async Task<IActionResult> Index()
        {
            var ratings = await _context.Rating.ToListAsync();

            ViewBag.moviesVB = _context.Movie.ToListAsync();
            ViewBag.usersVB = _context.User.ToListAsync();

            return View(ratings);
        }

        /**
         * A Details GET action passing a given rating and all related information for display to the view.
         * The view displays the details of the rating in a table.
         * @param id of the rating the details will be displayed of.
         * @return view with the Rating object.
         */
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Rating
                .FirstOrDefaultAsync(m => m.id == id);
            if (rating == null)
            {
                return NotFound();
            }

            ViewBag.movieVB = _context.Movie
                        .Where(m => m.id == rating.movie_id)
                        .ToList();
            ViewBag.userVB = _context.User
                       .Where(u => u.Id == rating.user_id)
                       .ToList();

            return View(rating);
        }

        /**
        * A Create GET action for rating creation.
        * @param id of the movie the rating will be related to.
        * @return view for creating the rating.
        */
        public async Task<IActionResult> Create(int? movie_id)
        {
            if (movie_id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.id == movie_id);
            if (movie == null)
            {
                return NotFound();
            }

            currentMovie = movie;
            ViewBag.movieVB = movie;
            return View();
        }

        /**
         * A Create POST action adding the rating to the database if the model is valid.
         * @param Rating class object passed from the view.
         * @return view with the rating if model not valid, redirect to Index view from MovieScene if valid.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,movie_id,user_id,rate,review,time")] Rating rating)
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

            rating.time = DateTime.Now;
            rating.movie_id = currentMovie.id;
            rating.user_id = user.Id;

            var context = new ValidationContext(rating, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(rating, context, validationResults, true))
            {
                return NotFound();
            }

            _context.Add(rating);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "MovieScene", new { id = currentMovie.id });
        }


        /**
         * An Edit GET action passing rating and related movie necessary for the editing view of the rating.
         * @param id of the rating to edit.
         * @param id of the movie the rating is related to.
         * @return view for editing the rating.
         */
        public async Task<IActionResult> Edit(int? id, int? movie_id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Rating.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }

            if (movie_id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.id == movie_id);
            if (movie == null)
            {
                return NotFound();
            }

            currentMovie = movie;
            ViewBag.movieVB = movie;

            return View(rating);
        }


        /**
         * An Edit POST action updating the rating in the database if the model is valid.
         * @param Rating class object passed from the view.
         * @return editing view with the rating if model not valid, redirect to Index view from MovieScene if valid.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,movie_id,user_id,rate,review,time")] Rating rating)
        {
            if (id != rating.id)
            {
                return NotFound();
            }

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

            rating.time = DateTime.Now;
            rating.movie_id = currentMovie.id;
            rating.user_id = user.Id;

            var context = new ValidationContext(rating, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(rating, context, validationResults, true))
            {
                return NotFound();
            }

            try
            {
                _context.Update(rating);
                await _context.SaveChangesAsync();

                // Calculating movie average rating
                var movie = await _context.Movie.FirstOrDefaultAsync(m => m.id == rating.movie_id);
                var ratings = _context.Rating
                    .Where(r => r.movie_id == movie.id)
                    .ToList();
                var count = movie.ratings.Count();
                var rating_sum = 0;
                foreach (Rating r in ratings)
                {
                    rating_sum += r.rate;
                }
                movie.rate = (rating_sum) / (count);
                _context.Update(movie);
                // ---

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RatingExists(rating.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            return RedirectToAction(nameof(Index), "MovieScene", new { id = currentMovie.id });
            
        }


        /**
         * A Delete GET action passing the rating to the Delete view.
         * @param id of the rating to delete.
         * @param id of the movie the rating is related to.
         * @return view for deleting the rating.
         */
        public async Task<IActionResult> Delete(int? id, int? movie_id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Rating
                .FirstOrDefaultAsync(m => m.id == id);
            if (rating == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.id == movie_id);
            if (movie == null)
            {
                return NotFound();
            }

            currentMovie = movie;
            ViewBag.movieVB = movie;

            return View(rating);
        }


        /**
         * A Delete POST action deleting the rating from the database.
         * @param id of the rating to be deleted.
         * @return redirect to Index action from MovieScene.
         */
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rating = await _context.Rating.FindAsync(id);
            if (rating != null)
            {
                // Calculating movie average rating
                var movie = await _context.Movie.FirstOrDefaultAsync(m => m.id == rating.movie_id);

                var ratings = _context.Rating
                    .Where(r => r.movie_id == movie.id)
                    .ToList();

                var count = movie.ratings.Count();
                if(count == 1)
                {
                    movie.rate = 0;
                }
                else
                {
                    movie.rate = (count * movie.rate - rating.rate) / (count - 1);
                }

                _context.Update(movie);
                // ---

                _context.Rating.Remove(rating);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), "MovieScene", new { id = currentMovie.id });
        }

        /**
         * A member method for checking whether the given rating exists.
         * @param id of the rating to be searched for.
         * @return bool value of whether the rating was found.
         */
        private bool RatingExists(int id)
        {
            return _context.Rating.Any(e => e.id == id);
        }


        /**
         * A member method for directing to an instruction view for not logged in user.
         * @return view with instruction.
         */
        public IActionResult Nope() { return View(); }


    }
}
