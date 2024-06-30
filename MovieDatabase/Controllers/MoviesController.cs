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

//there are some parts regarding editing of the actors to be uncommented and tested once editing works

namespace MovieDatabase.Controllers
{
    public class MoviesController : Controller
    {

        private readonly MovieDatabaseContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static IEnumerable<int> chosenActorsId = new HashSet<int>();

        public MoviesController(MovieDatabaseContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Movies
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

        // GET: Movies/Details/5
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

        // GET: Movies/Create
        public IActionResult Create()
        {
            ViewBag.directorVB = new SelectList(_context.Director, "id", "nameSurnameLabel");
            ViewBag.genres = new MultiSelectList(_context.Genre, "id", "tag");
            return View();
        }


        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Movies/Edit/5
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
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,title,year,director_id,description,trailer_link,rate")] Movie movie, IFormFile posterImagePath, string selectedActors, int[] genres)
        {
            Console.WriteLine($"Movie ID: {movie.id}");
            Console.WriteLine($"Title: {movie.title}");
            Console.WriteLine($"Year: {movie.year}");
            Console.WriteLine($"Director ID: {movie.director_id}");
            Console.WriteLine($"Selected Actors: {selectedActors}");
            Console.WriteLine($"Genres: {string.Join(",", genres)}");

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

        // GET: Movies/Delete/5
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

        // POST: Movies/Delete/5
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

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.id == id);
        }
    }
}
