using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.Data;
using MovieDatabase.Models;

/**
 * A Controller namespace for MovieDatabase controllers.
 */
namespace MovieDatabase.Controllers
{
    /**
     * A Director Controller class controlling all actions executed on actors.
     */
    public class DirectorsController : Controller
    {
        /**
         * A MovieDatabase context object encapsulating all information about an individual HTTP request and response. 
         */
        private readonly MovieDatabaseContext _context;

        /**
         * A Directors Controller constructor. 
         * @param context of the database application.
         */
        public DirectorsController(MovieDatabaseContext context)
        {
            _context = context;
        }

        /**
         * An Index GET action passing all directors from database to the view displaying directors in a table.
         * @return view with all directors.
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

            return View(await _context.Director.ToListAsync());
        }


        /**
         * A Details GET action passing a given director and all related information for display to the view.
         * The view displays the details of the director in a table.
         * @param id of the director the details will be displayed of.
         * @return view with the Director object.
         */
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = await _context.Director
                .FirstOrDefaultAsync(m => m.id == id);
            if (director == null)
            {
                return NotFound();
            }

            ViewBag.moviesVB = _context.Movie
                        .Where(m => m.director_id == id)
                        .ToList();


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

            return View(director);
        }


        /**
        * A Create GET action for director creation.
        * @return view for creating the director.
        */
        public IActionResult Create()
        {
            return View();
        }


        /**
         * A Create POST action adding the director to the database if the model is valid.
         * @param Director class object passed from the view.
         * @return view with the director if model not valid, redirect to Index view if valid.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,surname,date_of_birth")] Director director)
        {
            if (ModelState.IsValid)
            {
                _context.Add(director);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(director);
        }


        /**
         * An Edit GET action passing director and related information from database necessary for editing the director.
         * @param id of the director to edit.
         * @return view for editing the director.
         */
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = await _context.Director.FindAsync(id);
            if (director == null)
            {
                return NotFound();
            }
            return View(director);
        }


        /**
         * An Edit POST action updating the director in the database if the model is valid.
         * @param Director class object passed from the view.
         * @return editing view with the director if model not valid, redirect to Index view if valid.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,surname,date_of_birth")] Director director)
        {
            if (id != director.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(director);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DirectorExists(director.id))
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
            return View(director);
        }


        /**
         * A Delete GET action passing the director to the Delete view.
         * @param id of the director to delete.
         * @return view for deleting the director.
         */
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = await _context.Director
                .FirstOrDefaultAsync(m => m.id == id);
            if (director == null)
            {
                return NotFound();
            }

            return View(director);
        }


        /**
         * A Delete POST action deleting the director from the database.
         * @param id of the director to be deleted.
         * @return redirect to Index action.
         */
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var director = await _context.Director.FindAsync(id);
            if (director != null)
            {
                _context.Director.Remove(director);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        /**
         * A member method for checking whether the given director exists.
         * @param id of the director to be searched for.
         * @return bool value of whether the director was found.
         */
        private bool DirectorExists(int id)
        {
            return _context.Director.Any(e => e.id == id);
        }
    }
}
