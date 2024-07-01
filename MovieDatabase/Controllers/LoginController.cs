using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

/**
 * A Controller namespace for MovieDatabase controllers.
 */
namespace MovieDatabase.Controllers
{
    /**
     * A Login Controller class controlling all actions executed on actors.
     */
    public class LoginController : Controller
    {
        /**
         * An Index GET action for the Index view.
         * @return view.
         */
        public IActionResult Index()
        {
            return View();
        }

        /**
         * A Login action for logging in with a Google account.
         */
        public async Task Login()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties 
            { 
                RedirectUri = Url.Action("Index", "Home")
            });
        }

        /**
         * A GoogleResponse action for logging in with a Google account.
         * @return Json result with claims.
         */
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });
            return Json(claims);
        }

        /**
         * A Logout action for logging out.
         * @return redirect to the Home Page (Index action from Home Controller).
         */
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
