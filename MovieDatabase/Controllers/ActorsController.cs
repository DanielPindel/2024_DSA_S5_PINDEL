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
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

/**
 * A Controller namespace for MovieDatabase controllers.
 */
namespace MovieDatabase.Controllers
{
    /**
     * An Actor Controller class controlling all actions executed on actors.
     */
    public class ActorsController : Controller
    {
        /**
         * A MovieDatabase context object encapsulating all information about an individual HTTP request and response. 
         */
        private readonly MovieDatabaseContext _context;

        /**
         * An Actors Controller constructor. 
         * @param context of the database application.
         */
        public ActorsController(MovieDatabaseContext context)
        {
            _context = context;
        }


        /**
         * An Index GET action passing all actors from database to the view displaying actors in a table.
         * @return view with all actors.
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

            return View(await _context.Actor.ToListAsync());
        }


        /**
         * A Details GET action passing a given actor and all related information for display to the view.
         * The view displays the details of the actor in a table.
         * @param id of the actor the details will be displayed of.
         * @return view with the Actor object.
         */
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.id == id);
            if (actor == null)
            {
                return NotFound();
            }


            ViewBag.moviesVB = _context.Movie
                       .Include(m => m.actors.Where(a => a.id == actor.id))
                       .ToList();

            //Not sure why, but after the previous query ViewBag has all the movies available, but actor.movies
            //has only the ones added to it, that's why this line has to be here.
            ViewBag.moviesVB = actor.movies;


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

            return View(actor);
        }


        /**
        * A Create GET action for actor creation.
        * @return view for creating the actor.
        */
        public IActionResult Create()
        {
            return View();
        }

        /**
         * A Create POST action adding the actor to the database if the model is valid.
         * @param Actor class object passed from the view.
         * @return view with the actor if model not valid, redirect to Index view if valid.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,surname,date_of_birth")] Actor actor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }


        /**
         * An Edit GET action passing actor and related information from database necessary for editing the actor.
         * @param id of the actor to edit.
         * @return view for editing the actor.
         */
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        /**
         * An Edit POST action updating the actor in the database if the model is valid.
         * @param Actor class object passed from the view.
         * @return editing view with the actor if model not valid, redirect to Index view if valid.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,surname,date_of_birth")] Actor actor)
        {
            if (id != actor.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.id))
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
            return View(actor);
        }

        /**
         * A Delete GET action passing the actor to the Delete view.
         * @param id of the actor to delete.
         * @return view for deleting the actor.
         */
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }


        /**
         * A Delete POST action deleting the actor from the database.
         * @param id of the actor to be deleted.
         * @return redirect to Index action.
         */
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actor.FindAsync(id);
            if (actor != null)
            {
                _context.Actor.Remove(actor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        /**
         * A member method for checking whether the given actor exists.
         * @param id of the actor to be searched for.
         * @return bool value of whether the actor was found.
         */
        private bool ActorExists(int id)
        {
            return _context.Actor.Any(e => e.id == id);
        }
    }
}
