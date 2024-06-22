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

            ViewBag.commentsVB = _context.Comment
                        .Where(c => c.movie_id == id)
                        .ToList();

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

        public async Task<IActionResult> Create()
        {
            ViewBag.movieVB = currentMovie;
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,movie_id,user_id,content,time,is_blocked")] Comment comment)
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
        

        public IActionResult Nope() { return View(); }

    }
}
