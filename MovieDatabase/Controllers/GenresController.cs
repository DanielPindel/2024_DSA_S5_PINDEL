using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.Data;
using MovieDatabase.Models;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

/**
 * A Controller namespace for MovieDatabase controllers.
 */
namespace MovieDatabase.Controllers
{
    /**
     * A Genres Controller class controlling all actions executed on genres.
     */
    public class GenresController : Controller
    {
        /**
         * A MovieDatabase context object encapsulating all information about an individual HTTP request and response. 
         */
        private readonly MovieDatabaseContext _context;

        /**
         * A Genres Controller constructor. 
         * @param context of the database application.
         */
        public GenresController(MovieDatabaseContext context)
        {
            _context = context;
        }

        /**
         * An Index GET action passing all genres from database to the view displaying genres in a table.
         * @return view with all genres.
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

            return View(await _context.Genre.ToListAsync());
        }

        /**
          * A Details GET action passing a given genre and all related information for display to the view.
          * The view displays the details of the genre in a table.
          * @param id of the genre the details will be displayed of.
          * @return view with the Genre object.
          */
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genre
                .FirstOrDefaultAsync(m => m.id == id);
            if (genre == null)
            {
                return NotFound();
            }

            ViewBag.moviesVB = _context.Movie
                       .Include(m => m.genres.Where(g => g.id == genre.id))
                       .ToList();

            //Not sure why, but after the previous query ViewBag has all the movies available, but genre.movies
            //has only the ones added to it, that's why this line has to be here.
            ViewBag.moviesVB = genre.movies;


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
            return View(genre);
        }


        /**
        * A Create GET action for genre creation.
        * @return view for creating the genre.
        */
        public IActionResult Create()
        {
            return View();
        }


        /**
         * A Create POST action adding the genre to the database if the model is valid.
         * @param Genre class object passed from the view.
         * @return view with the genre if model not valid, redirect to Index view if valid.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,tag")] Genre genre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(genre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        /**
         * An Edit GET action passing genre to the editing view.
         * @param id of the genre to edit.
         * @return view for editing the genre.
         */
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genre.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }

        /**
         * An Edit POST action updating the genre in the database if the model is valid.
         * @param Genre class object passed from the view.
         * @return editing view with the genre if model not valid, redirect to Index view if valid.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,tag")] Genre genre)
        {
            if (id != genre.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenreExists(genre.id))
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
            return View(genre);
        }


        /**
         * A Delete GET action passing the genre to the Delete view.
         * @param id of the genre to delete.
         * @return view for deleting the genre.
         */
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genre
                .FirstOrDefaultAsync(m => m.id == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }


        /**
         * A Delete POST action deleting the genre from the database.
         * @param id of the genre to be deleted.
         * @return redirect to Index action.
         */
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _context.Genre.FindAsync(id);
            if (genre != null)
            {
                _context.Genre.Remove(genre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        /**
         * A member method for checking whether the given genre exists.
         * @param id of the genre to be searched for.
         * @return bool value of whether the genre was found.
         */
        private bool GenreExists(int id)
        {
            return _context.Genre.Any(e => e.id == id);
        }
    }
}
