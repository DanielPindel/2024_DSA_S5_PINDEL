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

/**
 * A Controller namespace for MovieDatabase controllers.
 */
namespace MovieDatabase.Controllers
{
    /**
     * A Comments Controller class controlling all actions executed on comments.
     */
    public class CommentsController : Controller
    {
        /**
         * A MovieDatabase context object encapsulating all information about an individual HTTP request and response. 
         */
        private readonly MovieDatabaseContext _context;

        /**
         * A Movie object for storing the last opened movie information. 
         */
        private static Movie currentMovie = new Movie();

        /**
         * A User object for storing the user that created the comment.
         */
        private static User commentUser = new User();

        /**
         * A Comments Controller constructor. 
         * @param context of the database application.
         */
        public CommentsController(MovieDatabaseContext context)
        {
            _context = context;
        }

        /**
         * An Index GET action passing all comments from database to the view displaying comments in a table.
         * @return view with all comments.
         */
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comment.ToListAsync());
        }

        /**
         * A Blocked GET action passing all blocked comments from database to the view.
         * @return view with all blocked comments.
         */
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


        /**
         * A Details GET action passing a given comment and all related information for display to the view.
         * The view displays the details of the comment in a table.
         * @param id of the comment the details will be displayed of.
         * @return view with the Comment object.
         */
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


        /**
        * A Create GET action for comment creation.
        * @param id of the movie the comment will be related to.
        * @return view for creating the comment.
        */
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

        /**
         * A Create POST action adding the comment to the database if the model is valid.
         * @param Comment class object passed from the view.
         * @return view with the comment if model not valid, redirect to Index view from MovieScene if valid.
         */
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

        /**
         * An Edit GET action passing comment and related movie necessary for the editing view of the comment.
         * @param id of the comment to edit.
         * @param id of the movie the comment is related to.
         * @return view for editing the comment.
         */
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

        /**
         * An Edit POST action updating the comment in the database if the model is valid.
         * @param Comment class object passed from the view.
         * @return editing view with the comment if model not valid, redirect to Index view from MovieScene if valid.
         */
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


        /**
         * A Delete GET action passing the comment to the Delete view.
         * @param id of the comment to delete.
         * @param id of the movie the comment is related to.
         * @return view for deleting the comment.
         */
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

        /**
         * A Delete POST action deleting the comment from the database.
         * @param id of the comment to be deleted.
         * @return redirect to Index action from MovieScene.
         */
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

        /**
         * A member method for checking whether the given comment exists.
         * @param id of the comment to be searched for.
         * @return bool value of whether the comment was found.
         */
        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.id == id);
        }

        /**
         * A member method for directing to an instruction view for not logged in user.
         * @return view with instruction.
         */
        public IActionResult Nope() { return View(); }


        /**
         * A Block GET action passing the comment to the Block view.
         * @param id of the comment to block.
         * @param id of the movie the comment is related to.
         * @param id of the user the comment is related to.
         * @return view for blocking the comment.
         */
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


        /**
         * A Block POST action updating the comment in the database if the model is valid.
         * @param Comment class object passed from the view.
         * @return block view with the comment if model not valid, redirect to Index view from MovieScene if valid.
         */
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


        /**
         * An Unblock GET action passing the comment to the Unblock view.
         * @param id of the comment to block.
         * @param id of the movie the comment is related to.
         * @param id of the user the comment is related to.
         * @return view for unblocking the comment.
         */
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


        /**
         * An Unblock POST action updating the comment in the database if the model is valid.
         * @param Comment class object passed from the view.
         * @return unblock view with the comment if model not valid, redirect to Blocked view if valid.
         */
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
