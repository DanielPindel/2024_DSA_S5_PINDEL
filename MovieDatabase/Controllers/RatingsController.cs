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
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace MovieDatabase.Controllers
{
    public class RatingsController : Controller
    {
        private readonly MovieDatabaseContext _context;
        private static Movie currentMovie = new Movie();

        public RatingsController(MovieDatabaseContext context)
        {
            _context = context;
        }

        // GET: Ratings
        public async Task<IActionResult> Index()
        {
            var ratings = await _context.Rating.ToListAsync();

            ViewBag.moviesVB = _context.Movie.ToListAsync();
            ViewBag.usersVB = _context.User.ToListAsync();

            return View(ratings);
        }

        // GET: Ratings/Details/5
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

        // GET: Ratings/Create
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
            return View();
        }

        // POST: Ratings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
    

        // GET: Ratings/Edit/5
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

        // POST: Ratings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Ratings/Delete/5
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

        // POST: Ratings/Delete/5
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

        private bool RatingExists(int id)
        {
            return _context.Rating.Any(e => e.id == id);
        }


        public IActionResult Nope() { return View(); }


    }
}
