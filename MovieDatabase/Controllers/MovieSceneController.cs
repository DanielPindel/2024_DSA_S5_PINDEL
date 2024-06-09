using Microsoft.AspNetCore.Mvc;

namespace MovieDatabase.Controllers
{
    public class MovieSceneController : Controller
    {
        private readonly ILogger<MovieSceneController> _logger;
        private readonly IMovieService _movieService;

        public MovieSceneController(ILogger<MovieSceneController> logger, IMovieService movieService)
        {
            _logger = logger;
            _movieService = movieService;
        }
        public async Task<IActionResult> Index()
        {
            var movies = await _movieService.GetAllMoviesAsync();
            return View(movies);
        }

    }
}
