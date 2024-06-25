using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.Data;
using MovieDatabase.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MovieDatabase.Controllers
{
    public class MovieSceneController : Controller
    {
        private readonly MovieDatabaseContext _context;
        private readonly ILogger<MovieSceneController> _logger;
        private readonly IMovieService _movieService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static Movie currentMovie = new Movie();
        private static Comment currentComment = new Comment();

        public MovieSceneController(ILogger<MovieSceneController> logger, IMovieService movieService, MovieDatabaseContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _logger = logger;
            _movieService = movieService;
            _webHostEnvironment = webHostEnvironment;
        }
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



            // Some stuff to get the user
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


            //Not sure why, but after the previous query ViewBag has all the actors available, but movie.actors
            //has only the ones added to it, that's why this line has to be here.
            ViewBag.actorsVB = movie.actors;
            ViewBag.genresVB = movie.genres;
           
          
            return View(movie);
        }

        public async Task<IActionResult> CreateComment()
        {
            ViewBag.movieVB = currentMovie;
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment([Bind("id,movie_id,user_id,content,time,is_blocked")] Comment comment)
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

            ViewBag.currentUserVB = user;
            comment.user_id = user.Id;

            DateTime datetime = DateTime.Now;
            comment.time = datetime;

            comment.movie_id = currentMovie.id;

            Console.WriteLine("--------> comment.movie_id:" + comment.movie_id);



            var context = new ValidationContext(comment, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            ModelState.ClearValidationState(nameof(comment));
            if (!Validator.TryValidateObject(comment, context, validationResults, true))
            {
                return NotFound();
            }


            _context.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = comment.movie_id });

        }


        public async Task<IActionResult> CreateSubcomment(int? com_id)
        {
            if (com_id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.FirstOrDefaultAsync(c => c.id == com_id);

            currentComment = comment;

            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == comment.user_id);
            if (user == null || comment == null)
            {
                return NotFound();
            }

            ViewBag.replyToUserVB = user;
            ViewBag.replyToCommentVB = comment;
            ViewBag.movieVB = currentMovie;


            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSubcomment([Bind("id,comment_id,user_id,content,time,is_blocked")] Subcomment subcomment)
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

            subcomment.user_id = user.Id;

            subcomment.comment_id = currentComment.id;

            DateTime datetime = DateTime.Now;
            subcomment.time = datetime;



            var context = new ValidationContext(subcomment, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            ModelState.ClearValidationState(nameof(subcomment));
            if (!Validator.TryValidateObject(subcomment, context, validationResults, true))
            {
                return NotFound();
            }


            _context.Add(subcomment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = currentMovie.id});

        }



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


        public IActionResult AddRating()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRating([Bind("id,movie_id,user_id,rate,review,time")] Rating rating)
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



            return RedirectToAction(nameof(Index), new { id = currentMovie.id });
        }


        public IActionResult Nope() { return View(); }

    }
}
