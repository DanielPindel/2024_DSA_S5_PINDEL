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
     * A Subcomments Controller class controlling all actions executed on actors.
     */
    public class SubcommentsController : Controller
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
         * A Comment object for storing the comment the subcomment is related to.
         */
        private static Comment currentComment = new Comment();

        /**
         * A User object for storing the user that created the subcomment.
         */
        private static User subcommentUser = new User();

        /**
         * A Subcomments Controller constructor. 
         * @param context of the database application.
         */
        public SubcommentsController(MovieDatabaseContext context)
        {
            _context = context;
        }

        /**
         * An Index GET action passing all subcomments from database to the view displaying subcomments in a table.
         * @return view with all subcomments.
         */
        public async Task<IActionResult> Index()
        {

            return View(await _context.Subcomment.ToListAsync());
        }


        /**
        * A Blocked GET action passing all blocked subcomments from database to the view.
        * @return view with all blocked subcomments.
        */
        public async Task<IActionResult> Blocked()
        {

            var blockedSubcomments = await _context.Subcomment.ToListAsync();
            blockedSubcomments = blockedSubcomments.Where(sc => sc.is_blocked == true).ToList();

            var movies = await _context.Movie.ToListAsync();
            var users = await _context.User.ToListAsync();
            var comments = await _context.Comment.ToListAsync();

            ViewBag.usersVB = users;
            ViewBag.moviesVB = movies;
            ViewBag.commentsVB = comments;


            return View(blockedSubcomments);
        }


        /**
         * A Details GET action passing a given subcomment and all related information for display to the view.
         * The view displays the details of the subcomment in a table.
         * @param id of the subcomment the details will be displayed of.
         * @return view with the Subcomment object.
         */
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

        /**
        * A Create GET action for subcomment creation.
        * @param id of the comment the subcomment will be related to.
        * @param id of the movie the subcomment will be related to.
        * @return view for creating the subcomment.
        */
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


        /**
         * A Create POST action adding the subcomment to the database if the model is valid.
         * @param Subcomment class object passed from the view.
         * @return view with the subcomment if model not valid, redirect to Index view from MovieScene if valid.
         */
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


        /**
         * An Edit GET action passing subcomment and related movie necessary for the editing view of the comment.
         * @param id of the comment to edit.
         * @param id of the comment the subcomment is related to.
         * @param id of the movie the comment is related to.
         * @return view for editing the actor.
         */
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


        /**
         * An Edit POST action updating the subcomment in the database if the model is valid.
         * @param Subcomment class object passed from the view.
         * @return editing view with the subcomment if model not valid, redirect to Index view from MovieScene if valid.
         */
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



        /**
         * A Delete GET action passing the subcomment to the Delete view.
         * @param id of the subcomment to delete.
         * @param id of the comment the subcomment is related to.
         * @param id of the movie the subcomment is related to.
         * @return view for deleting the subcomment.
         */
        public async Task<IActionResult> Delete(int? id, int? com_id, int? movie_id)
        {
            if (id == null) { return NotFound(); }

            var subcomment = await _context.Subcomment
                .FirstOrDefaultAsync(m => m.id == id);
            if (subcomment == null) { return NotFound(); }

            if (movie_id == null){ return NotFound(); }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.id == movie_id);
            if (movie == null){ return NotFound(); }

            currentMovie = movie;
            ViewBag.movieVB = movie;

            return View(subcomment);
        }


        /**
         * A Delete POST action deleting the subcomment from the database.
         * @param id of the subcomment to be deleted.
         * @return redirect to Index action from MovieScene.
         */
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

        /**
         * A member method for checking whether the given subcomment exists.
         * @param id of the subcomment to be searched for.
         * @return bool value of whether the subcomment was found.
         */
        private bool SubcommentExists(int id)
        {
            return _context.Subcomment.Any(e => e.id == id);
        }

        /**
         * A member method for directing to an instruction view for not logged in user.
         * @return view with instruction.
         */
        public IActionResult Nope() { return View(); }


        /**
         * A Block GET action passing the subcomment to the Block view.
         * @param id of the subcomment to block.
         * @return view for blocking the subcomment.
         */
        public async Task<IActionResult> Block(int? id)
        {
            if (id == null) { return NotFound(); }
            var subcomment = await _context.Subcomment.FindAsync(id);
            if (subcomment == null) { return NotFound(); }

            var comment = await _context.Comment.FirstOrDefaultAsync(c => c.id == subcomment.comment_id);
            if (comment == null) { return NotFound(); }

            currentComment = comment;
            ViewBag.commentVB = comment;

            var movie = await _context.Movie.FirstOrDefaultAsync(m => m.id == comment.movie_id);
            if (movie == null) { return NotFound(); }

            currentMovie = movie;
            ViewBag.movieVB = movie;


            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == subcomment.user_id);
            if (user == null) { return NotFound(); }

            subcommentUser = user;
            ViewBag.userVB = user;

           

            return View(subcomment);
        }


        /**
         * A Block POST action updating the subcomment in the database if the model is valid.
         * @param Subcomment class object passed from the view.
         * @return block view with the subcomment if model not valid, redirect to Index view from MovieScene if valid.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block(int id, [Bind("id,movie_id,user_id,content,time,is_blocked")] Subcomment subcomment)
        {
            if (id != subcomment.id)
            {
                return NotFound();
            }

            subcomment.time = DateTime.Now;
            subcomment.user_id = subcommentUser.Id;
            subcomment.comment_id = currentComment.id;
            subcomment.is_blocked = true;

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


        /**
        * An Unblock GET action passing the subcomment to the Unblock view.
        * @param id of the subcomment to block.
        * @return view for unblocking the subcomment.
        */
        public async Task<IActionResult> Unblock(int? id)
        {
            if (id == null){ return NotFound(); }

            var subcomment = await _context.Subcomment.FindAsync(id);
            if (subcomment == null){ return NotFound(); }

            var comment = await _context.Comment.FirstOrDefaultAsync(c => c.id == subcomment.comment_id);
            if (comment == null) { return NotFound(); }

            currentComment = comment;
            ViewBag.commentVB = comment;

            var movie = await _context.Movie.FirstOrDefaultAsync(m => m.id == comment.movie_id);
            if (movie == null) {  return NotFound(); }

            currentMovie = movie;
            ViewBag.movieVB = movie;

            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == subcomment.user_id);
            if (user == null) { return NotFound(); }

            subcommentUser = user;
            ViewBag.userVB = user;

            return View(subcomment);
        }


        /**
         * An Unblock POST action updating the subcomment in the database if the model is valid.
         * @param Subcomment class object passed from the view.
         * @return unblock view with the subcomment if model not valid, redirect to Blocked view if valid.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unblock(int id, [Bind("id,movie_id,user_id,content,time,is_blocked")] Subcomment subcomment)
        {
            if (id != subcomment.id)
            {
                return NotFound();
            }

            subcomment.time = DateTime.Now;
            subcomment.comment_id = currentComment.id;
            subcomment.user_id = subcommentUser.Id;
            subcomment.is_blocked = false;

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
            return RedirectToAction(nameof(Blocked), "Subcomments");
        }



    }
}
