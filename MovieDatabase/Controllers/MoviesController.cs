using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieDatabase.Data;
using MovieDatabase.Models;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;


/**
 * A Controller namespace for MovieDatabase controllers.
 */
namespace MovieDatabase.Controllers
{
    /**
     * A Movie Controller class controlling all actions executed on movies.
     */
    public class MoviesController : Controller
    {

        /**
         * A MovieDatabase context object encapsulating all information about an individual HTTP request and response. 
         */
        private readonly MovieDatabaseContext _context;

        /**
         * An IWebHostEnvironment object providing information about the web hosting environment an application is running in.
         */
        private readonly IWebHostEnvironment _webHostEnvironment;

        /**
         * A collection for storing actors chosen from the list during creation or editing of the movie.
         */
        private static IEnumerable<int> chosenActorsId = new HashSet<int>();

        /**
         * A nullable string object for storing current movie poster path
         */
        private static string? currentPoster = null;

        /**
         * An int array for storing current movie's genres IDs
         */
        private static int[] currentGenresId;

        /**
         * A Movie Controller constructor. 
         * @param context of the database application.
         * @param webHostEnvironment providing information about the web hosting environment.
         */
        public MoviesController(MovieDatabaseContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        /**
         * An Index GET action passing all movies from database to the view displaying movies in a table.
         * @return view with all movies.
         */
        public async Task<IActionResult> Index()
        {
            ViewBag.isAdminVB = false;
            string? user_id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (user_id != null)
            {
                var user = await _context.User.FirstOrDefaultAsync(u => u.Id == user_id);
                if (user != null)
                {
                    ViewBag.isAdminVB = user.is_admin;
                }
            }
            return View(await _context.Movie.ToListAsync());
        }

        /**
         * A Details GET action passing a given movie and all related information for display to the view.
         * The view displays the movie details in a table.
         * @param id of the movie the details will be displayed of.
         * @return view with the movie.
         */
        public async Task<IActionResult> Details(int? id)
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


            ViewBag.isAdminVB = false;
            string? user_id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (user_id != null)
            {
                var user = await _context.User.FirstOrDefaultAsync(u => u.Id == user_id);
                if (user != null)
                {
                    ViewBag.isAdminVB = user.is_admin;
                }
            }

            return View(movie);
        }

        /**
         * A Create GET action passing related information from database necessary for movie creation.
         * @return view for creating the movie.
         */
        public IActionResult Create()
        {
            ViewBag.directorVB = new SelectList(_context.Director, "id", "nameSurnameLabel");
            ViewBag.genres = new MultiSelectList(_context.Genre, "id", "tag");
            return View();
        }


        /**
         * A Create POST action adding the movie to the database if the model is valid.
         * @param Movie class object passed from the view.
         * @param IFormFile object representing a file sent with the HttpRequest for the poster image.
         * @param string of selected actors.
         * @param int array of selected genres.
         * @return view with the movie if model not valid, redirect to Index view if valid.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,title,year,director_id,description,trailer_link,rate")] Movie movie, IFormFile posterImagePath, string selectedActors, int[] genres)
        {
            if (posterImagePath != null && posterImagePath.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(posterImagePath.FileName);
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await posterImagePath.CopyToAsync(fileStream);
                }

                movie.posterImagePath = fileName;
                ModelState.Remove("posterImagePath");
            }

            if (ModelState.IsValid)
            {
                var actorIds = selectedActors?.Split(',').Select(int.Parse).ToList();

                if (actorIds != null)
                {
                    var selectedActorsList = _context.Actor.Where(a => actorIds.Contains(a.id)).ToList();
                    movie.actors = selectedActorsList;
                }

                if (genres != null)
                {
                    var selectedGenresList = _context.Genre.Where(g => genres.Contains(g.id)).ToList();
                    movie.genres = selectedGenresList;
                }

                try
                {
                    _context.Add(movie);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving movie: {ex.Message}");
                }
            }
            else
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Model error: {error.ErrorMessage}");
                    }
                }
            }

            ViewBag.directorVB = new SelectList(_context.Director, "id", "nameSurnameLabel", movie.director_id);
            ViewBag.genres = new MultiSelectList(_context.Genre, "id", "tag");
            return View(movie);
        }

        /**
         * A SearchActors GET action passing actors filtered with searchString to the view.
         * @param string searchString object passed from the view for filtering the actors.
         * @return a JsonResult object.
         */
        [HttpGet]
        public IActionResult SearchActors(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return Json(new List<Actor>());
            }

            var actors = _context.Actor
                .Where(a => (a.name + " " + a.surname).Contains(searchString))
                .Select(a => new { a.id, nameSurnameLabel = a.name + " " + a.surname })
                .ToList();

            return Json(actors);
        }

        /**
         * An Edit GET action passing movie and related information from database necessary for editing the movie.
         * @param id of the movie to edit.
         * @return view for editing the movie.
         */
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.actors)
                .Include(m => m.genres)
                .FirstOrDefaultAsync(m => m.id == id);

            if (movie == null)
            {
                return NotFound();
            }

            ViewBag.directorVB = new SelectList(_context.Director, "id", "nameSurnameLabel", movie.director_id);
            ViewBag.genres = new MultiSelectList(_context.Genre, "id", "tag", movie.genres.Select(g => g.id));
            ViewBag.actorsVB = movie.actors.Select(a => new { a.id, nameSurnameLabel = a.name + " " + a.surname }).ToList();
            ViewBag.posterImagePathVB = movie.posterImagePath;
            currentPoster = movie.posterImagePath;
            return View(movie);
        }

        /**
         * An Edit POST action updating the movie in the database if the model is valid.
         * @param Movie class object passed from the view.
         * @param IFormFile object representing a file sent with the HttpRequest for the poster image.
         * @param string of selected actors.
         * @param int array of selected genres.
         * @return editing view with the movie if model not valid, redirect to Index view if valid.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,title,year,director_id,description,trailer_link,rate")] Movie movie, IFormFile posterImagePath, string selectedActors, int[] genres)
        {
            if (id != movie.id)
            {
                return NotFound();
            }

            if (posterImagePath != null && posterImagePath.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(posterImagePath.FileName);
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await posterImagePath.CopyToAsync(fileStream);
                }

                movie.posterImagePath = fileName;
                ModelState.Remove("posterImagePath");
            }

            if (posterImagePath == null)
            {
                if(currentPoster != null)
                {
                    movie.posterImagePath = currentPoster;
                    ModelState.Remove("posterImagePath");
                }
            }



            if (ModelState.IsValid)
            {
                try
                {
                    var existingMovie = await _context.Movie
                        .Include(m => m.actors)
                        .Include(m => m.genres)
                        .FirstOrDefaultAsync(m => m.id == id);

                    if (existingMovie == null)
                    {
                        return NotFound();
                    }

                    existingMovie.title = movie.title;
                    existingMovie.year = movie.year;
                    existingMovie.director_id = movie.director_id;
                    existingMovie.description = movie.description;
                    existingMovie.trailer_link = movie.trailer_link;
                    existingMovie.rate = movie.rate;
                    if (movie.posterImagePath != null)
                    {
                        existingMovie.posterImagePath = movie.posterImagePath;
                    }

                    var actorIds = selectedActors?.Split(',').Select(int.Parse).ToList();
                    if (actorIds != null)
                    {
                        existingMovie.actors = await _context.Actor.Where(a => actorIds.Contains(a.id)).ToListAsync();
                    }

                    if (genres != null)
                    {
                        existingMovie.genres = await _context.Genre.Where(g => genres.Contains(g.id)).ToListAsync();
                    }

                    _context.Update(existingMovie);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Model error: {error.ErrorMessage}");
                    }
                }
            }

            ViewBag.directorVB = new SelectList(_context.Director, "id", "nameSurnameLabel", movie.director_id);
            ViewBag.genres = new MultiSelectList(_context.Genre, "id", "tag", movie.genres.Select(g => g.id));
            return View(movie);
        }


        /**
         * A Delete GET action passing the movie to the Delete view.
         * @param id of the movie to delete.
         * @return view for deleting the movie.
         */
        public async Task<IActionResult> Delete(int? id)
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

            return View(movie);
        }

        /**
         * A Delete POST action deleting the movie from the database.
         * @param id of the movie to be deleted.
         * @return redirect to Index action.
         */
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                if (!string.IsNullOrEmpty(movie.posterImagePath))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", movie.posterImagePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /**
         * A member method for checking whether the given movie exists.
         * @param id of the movie to be searched for.
         * @return bool value of whether the movie was found.
         */
        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.id == id);
        }
    }
}
