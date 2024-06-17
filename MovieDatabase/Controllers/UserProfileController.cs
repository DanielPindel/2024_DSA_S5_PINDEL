using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.Data;
using MovieDatabase.Models;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.Sig;
using System.Security.Claims;


namespace MovieDatabase.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly MovieDatabaseContext _context;

        // Watchlist constructor that assigns our context to the class
        public UserProfileController(MovieDatabaseContext context)
        {
            _context = context;
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

            return View();
        }
    }
}
