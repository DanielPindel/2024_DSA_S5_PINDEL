using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.Data;
using MovieDatabase.Models;

namespace MovieDatabase.Controllers
{
    public class SubcommentsController : Controller
    {
        private readonly MovieDatabaseContext _context;
        private static Movie currentMovie = new Movie();
        private static Comment currentComment = new Comment();

        public SubcommentsController(MovieDatabaseContext context)
        {
            _context = context;
        }

        // GET: Subcomments
        public async Task<IActionResult> Index()
        {

            return View(await _context.Subcomment.ToListAsync());
        }

        // GET: Subcomments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcomment = await _context.Subcomment
                .FirstOrDefaultAsync(m => m.id == id);
            if (subcomment == null)
            {
                return NotFound();
            }

            return View(subcomment);
        }

        public async Task<IActionResult> Create(int? com_id, int? movie_id)
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

            if (movie_id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.id == movie_id);
            if (movie == null)
            {
                return NotFound();
            }

            currentMovie = movie;

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
        public async Task<IActionResult> Create([Bind("id,comment_id,user_id,content,time,is_blocked")] Subcomment subcomment)
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
            return RedirectToAction(nameof(Index), "MovieScene", new { id = currentMovie.id });

        }


        // GET: Subcomments/Edit/5
        public async Task<IActionResult> Edit(int? id, int? com_id, int? movie_id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcomment = await _context.Subcomment.FindAsync(id);
            if (subcomment == null)
            {
                return NotFound();
            }

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

            if (movie_id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.id == movie_id);
            if (movie == null)
            {
                return NotFound();
            }

            currentMovie = movie;

            ViewBag.replyToUserVB = user;
            ViewBag.replyToCommentVB = comment;
            ViewBag.movieVB = movie;


            return View(subcomment);
        }

        // POST: Subcomments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,comment_id,user_id,content,time,is_blocked")] Subcomment subcomment)
        {
            if (id != subcomment.id)
            {
                return NotFound();
            }

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

            subcomment.time = DateTime.Now;
            subcomment.comment_id = currentComment.id;
            subcomment.user_id = user.Id;

            var context = new ValidationContext(subcomment, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(subcomment, context, validationResults, true))
            {
                return NotFound();
            }


            try
            {

                _context.Update(subcomment);

                await _context.SaveChangesAsync();


            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubcommentExists(subcomment.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


            return RedirectToAction(nameof(Index), "MovieScene", new { id = currentMovie.id });
        }



        // GET: Subcomments/Delete/5
        public async Task<IActionResult> Delete(int? id, int? com_id, int? movie_id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcomment = await _context.Subcomment
                .FirstOrDefaultAsync(m => m.id == id);
            if (subcomment == null)
            {
                return NotFound();
            }

            if (movie_id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.id == movie_id);
            if (movie == null)
            {
                return NotFound();
            }

            currentMovie = movie;
            ViewBag.movieVB = movie;

            return View(subcomment);
        }

        // POST: Subcomments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subcomment = await _context.Subcomment.FindAsync(id);
            if (subcomment != null)
            {
                _context.Subcomment.Remove(subcomment);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "MovieScene", new { id = currentMovie.id });
        }

        private bool SubcommentExists(int id)
        {
            return _context.Subcomment.Any(e => e.id == id);
        }

        public IActionResult Nope() { return View(); }
    }
}
