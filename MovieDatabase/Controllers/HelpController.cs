using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MovieDatabase.Data;
using MovieDatabase.Models;
using SQLitePCL;
using System.Diagnostics;
using System.Security.Claims;

/**
 * A Controller namespace for MovieDatabase controllers.
 */
namespace MovieDatabase.Controllers
{
    /**
     * A Help Controller class controlling the help tab.
     */
    public class HelpController : Controller
    {
        /**
         * An Index GET action for the Index view.
         * @return view.
         */
        public async Task<IActionResult> Index()
        {
            return View();
        }

    }
}
