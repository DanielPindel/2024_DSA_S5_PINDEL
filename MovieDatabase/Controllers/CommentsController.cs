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
    public class CommentsController : Controller
    {
        private readonly MovieDatabaseContext _context;
        private static Movie currentMovie = new Movie();
        private static User commentUser = new User();

        public CommentsController(MovieDatabaseContext context)
        {
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comment.ToListAsync());
        }


        public async Task<IActionResult> Blocked()
        {

            var blockedComments = await _context.Comment.ToListAsync();
            blockedComments = blockedComments.Where(c => c.is_blocked == true).ToList();

            var movies = await _context.Movie.ToListAsync();
            var users = await _context.User.ToListAsync();
            
            ViewBag.usersVB = users;
            ViewBag.moviesVB = movies;


            return View(blockedComments);
        }


        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .FirstOrDefaultAsync(m => m.id == id);
            if (comment == null)
            {
                return NotFound();
            }

            ViewBag.subcommentsVB = _context.Subcomment
                        .Where(s => s.comment_id == id)
                        .ToList();

            return View(comment);
        }



        public async Task<IActionResult> Create(int? movie_id)
        {
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
            return RedirectToAction(nameof(Index), "MovieScene", new { id = currentMovie.id });

        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id, int? movie_id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
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

            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,movie_id,user_id,content,time,is_blocked")] Comment comment)
        {
            if (id != comment.id)
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

            comment.time = DateTime.Now;
            comment.movie_id = currentMovie.id;
            comment.user_id = user.Id;

            var context = new ValidationContext(comment, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(comment, context, validationResults, true))
            {
                return NotFound();
            }


            try
            {

                _context.Update(comment);

                await _context.SaveChangesAsync();


            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(comment.id))
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

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id, int? movie_id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .FirstOrDefaultAsync(m => m.id == id);
            if (comment == null)
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


            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comment.FindAsync(id);
            if (comment != null)
            {
                _context.Comment.Remove(comment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), "MovieScene", new { id = currentMovie.id });
        }

        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.id == id);
        }

        public IActionResult Nope() { return View(); }


        public async Task<IActionResult> Block(int? id, int? movie_id, string? user_id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
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


            if (user_id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(u => u.Id == user_id);
            if (user == null)
            {
                return NotFound();
            }

            commentUser = user;
            ViewBag.userVB = user;


            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block(int id, [Bind("id,movie_id,user_id,content,time,is_blocked")] Comment comment)
        {
            if (id != comment.id)
            {
                return NotFound();
            }

            comment.time = DateTime.Now;
            comment.movie_id = currentMovie.id;
            comment.user_id = commentUser.Id;
            comment.is_blocked = true;

            var context = new ValidationContext(comment, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(comment, context, validationResults, true))
            {
                return NotFound();
            }

            try
            {
                _context.Update(comment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(comment.id))
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


        public async Task<IActionResult> Unblock(int? id, int? movie_id, string? user_id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
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


            if (user_id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(u => u.Id == user_id);
            if (user == null)
            {
                return NotFound();
            }

            commentUser = user;
            ViewBag.userVB = user;


            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unblock(int id, [Bind("id,movie_id,user_id,content,time,is_blocked")] Comment comment)
        {
            if (id != comment.id)
            {
                return NotFound();
            }

            comment.time = DateTime.Now;
            comment.movie_id = currentMovie.id;
            comment.user_id = commentUser.Id;
            comment.is_blocked = false;

            var context = new ValidationContext(comment, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(comment, context, validationResults, true))
            {
                return NotFound();
            }

            try
            {
                _context.Update(comment);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(comment.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Blocked), "Comments");
        }

    }
}
