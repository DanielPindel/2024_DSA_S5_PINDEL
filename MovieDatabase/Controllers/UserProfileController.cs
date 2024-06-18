using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.Data;
using MovieDatabase.Models;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.Sig;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;


namespace MovieDatabase.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly MovieDatabaseContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Watchlist constructor that assigns our context to the class
        public UserProfileController(MovieDatabaseContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task<IActionResult> Index()
        {
            string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }


            var userMovies = _context.UserMovie
                       .Where(um => um.user_id == id)
                       .ToList();
            var ratings = _context.Rating
                       .Where(r => r.user_id == id)
                       .ToList(); ;

            int watchlistNo = 0;
            int favNo = 0;
            int ratedNo = 0;

            foreach (var userMovie in userMovies)
            {
                if (userMovie.context_id == 1)
                {
                    watchlistNo++;
                }
                if (userMovie.context_id == 2)
                {
                    favNo++;
                }
            }
            foreach(var rating in ratings)
            {
                ratedNo++;
            }

            ViewBag.watchlistNoVB = watchlistNo;
            ViewBag.favNoVB = favNo;
            ViewBag.ratedNoVB = ratedNo;

            ViewBag.avatarPathVB = user.avatar_path;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile avatarImagePath)
        {
            string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }



            if (avatarImagePath != null && avatarImagePath.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(avatarImagePath.FileName);
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarImagePath.CopyToAsync(fileStream);
                }

                user.avatar_path = fileName;
                ModelState.Remove("avatarImagePath");
            }


            _context.Update(user);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

    }
}
