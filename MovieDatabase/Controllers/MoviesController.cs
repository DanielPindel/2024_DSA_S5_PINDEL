using System;
using System.Collections.Generic;
using System.Linq;
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

        public MoviesController(MovieDatabaseContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
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

            //Not sure why, but after the previous query ViewBag has all the actors available, but movie.actors
            //has only the ones added to it, that's why this line has to be here.
            ViewBag.actorsVB = movie.actors;

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            ViewBag.directorVB = new SelectList(_context.Director, "id", "nameSurnameLabel");
            ViewBag.actors = new MultiSelectList(_context.Actor, "id", "nameSurnameLabel");

            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,title,year,director_id,description,trailer_link")] Movie movie, IFormFile posterImagePath, int[] actors)
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
                //int[] actors is the result of selected stuff from the list, and for some reason
                //it also has to have the same name as the list from the object (Movie.actors)
                if (actors != null)
                {
                    var a = new List<Actor>();
                    foreach (var actor in actors)
                    {
                        var item = _context.Actor.Find(actor);
                        a.Add(item);
                    }
                    movie.actors = a;
                }

                _context.Add(movie);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
            }

            return View(movie);
        }



        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);

            /*ViewBag.actorsVB = _context.Actor
                        .Include(a => a.movies.Where(m => m.id == movie.id))
                        .ToList();

            List<int> selectedActors = new List<int>();
            foreach (var actor in movie.actors)
            {
                selectedActors.Add(actor.id);
            }
            int[] selectedAs = selectedActors.ToArray();*/

            if (movie == null)
            {
                return NotFound();
            }
            ViewBag.directorVB = new SelectList(_context.Director, "id", "nameSurnameLabel", movie.director_id);
            //ViewBag.actors = new MultiSelectList(_context.Actor, "id", "nameSurnameLabel", selectedAs);
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,title,year,director_id,description,trailer_link")] Movie movie)
        {
            if (id != movie.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                /*ViewBag.actorsVB = _context.Actor
                        .Include(a => a.movies.Where(m => m.id == movie.id))
                        .ToList();

                _context.Entry(movie).State = EntityState.Modified;
                _context.Entry(movie).Collection(p => p.actors).Load();

                var newActors = _context.Actor.Where(x => actors.Contains(x.id)).ToList();
                movie.actors = newActors;*/


                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            ViewBag.directorVB = new SelectList(_context.Director, "id", "nameSurnameLabel", movie.director_id);

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
