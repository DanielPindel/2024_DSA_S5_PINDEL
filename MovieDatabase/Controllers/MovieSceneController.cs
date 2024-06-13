using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.Data;
using MovieDatabase.Models;

namespace MovieDatabase.Controllers
{
    public class MovieSceneController : Controller
    {
        private readonly MovieDatabaseContext _context;
        private readonly ILogger<MovieSceneController> _logger;
        private readonly IMovieService _movieService;
        private readonly IWebHostEnvironment _webHostEnvironment;

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

            //Not sure why, but after the previous query ViewBag has all the actors available, but movie.actors
            //has only the ones added to it, that's why this line has to be here.
            ViewBag.actorsVB = movie.actors;
            ViewBag.genresVB = movie.genres;
           
          
            return View(movie);
        }

        


    }
}
