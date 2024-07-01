using Microsoft.EntityFrameworkCore;
using MovieDatabase.Data;
using MovieDatabase.Models;

/**
 * MovieDatabase namespace.
 */
namespace MovieDatabase
{
    /**
     * Interface for accessing movie data from the database
     */
    public interface IMovieService
    {
        Task<List<Movie>> GetAllMoviesAsync();
    }

    /**
     * MovieService class inheriting after IMovieService interface for accessing movie data from the database
     */
    public class MovieService : IMovieService
    {
        /**
         * A MovieDatabase context object encapsulating all information about an individual HTTP request and response.
         */
        private readonly MovieDatabaseContext _context;

        /**
         * A MovieSrvice constructor.
         * @param context of the database application.
         */
        public MovieService(MovieDatabaseContext context)
        {
            _context = context;
        }

        /**
         * A GetAllMoviesAsync GET action getting all movies from the database.
         * @return a list with all movies from the database.
         */
        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            return await _context.Movie.ToListAsync();
        }
    }
}
