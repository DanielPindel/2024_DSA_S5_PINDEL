﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieDatabase.Data;
using MovieDatabase.Models;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

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

<<<<<<< Updated upstream
            ViewData["director"] = _context.Director
                        .Where(d => d.id == movie.director_id)
                        .ToList();

=======
            ViewBag.directorVB = _context.Director
                        .Where(d => d.id == movie.director_id)
                        .ToList();

            /*ViewBag.actorsVB = _context.Actor
                        .Include(a => a.movies).Where(m => m.id == movie.id)
                        .ToList();*/

            
            //ViewBag.actorsVB = movie.actors;

            /*Console.WriteLine("==========================================");
            foreach (var a in movie.actors)
            {
                Console.WriteLine("actor found");
            }
            Console.WriteLine("==========================================");*/

>>>>>>> Stashed changes
            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
<<<<<<< Updated upstream
            ViewData["director"] = new SelectList(_context.Director, "id", "nameSurnameLabel");
            ViewData["actors"] = new MultiSelectList(_context.Actor, "id", "nameSurnameLabel");
=======
            ViewBag.directorVB = new SelectList(_context.Director, "id", "nameSurnameLabel");
            ViewBag.actorsVB = new MultiSelectList(_context.Actor, "id", "nameSurnameLabel");
           
>>>>>>> Stashed changes
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,title,year,director_id")] Movie movie, IFormFile posterImagePath, int[] actorsVB)
        {
            ////to delete later
            Console.WriteLine("############################## " + actorsVB.Length);
            ////

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
                if(actorsVB != null)
                {
                    var a = new List<Actor>();
                    foreach(var actor in actorsVB)
                    {
                        var item = _context.Actor.Find(actor);
                        a.Add(item);
                    }
                    movie.actors = a;
                }

                ////to delete later
                Console.WriteLine("Actors:");
                foreach (Actor a in movie.actors)
                {
                    Console.WriteLine(a.nameSurnameLabel);
                }
                ////

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
<<<<<<< Updated upstream
            ViewData["director"] = new SelectList(_context.Director, "id", "nameSurnameLabel", movie.director_id);
            ViewData["actors"] = new MultiSelectList(_context.Actor, "id", "nameSurnameLabel", movie.actors);
=======

            

>>>>>>> Stashed changes
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
            if (movie == null)
            {
                return NotFound();
            }
<<<<<<< Updated upstream
            ViewData["director"] = new SelectList(_context.Director, "id", "nameSurnameLabel", movie.director_id);
=======
            ViewBag.directorVB = new SelectList(_context.Director, "id", "nameSurnameLabel", movie.director_id);
>>>>>>> Stashed changes
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,title,year,director_id")] Movie movie)
        {
            if (id != movie.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
<<<<<<< Updated upstream
            ViewData["director"] = new SelectList(_context.Director, "id", "nameSurnameLabel", movie.director_id);
=======
            ViewBag.directorVB = new SelectList(_context.Director, "id", "nameSurnameLabel", movie.director_id);
>>>>>>> Stashed changes
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
