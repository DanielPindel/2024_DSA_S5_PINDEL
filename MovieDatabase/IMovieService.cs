using Microsoft.EntityFrameworkCore;
using MovieDatabase.Data;
using MovieDatabase.Models;

namespace MovieDatabase
{
    public interface IMovieService
    {
        Task<List<Movie>> GetAllMoviesAsync();
    }
    public class MovieService : IMovieService
    {
        private readonly MovieDatabaseContext _context;

        public MovieService(MovieDatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            return await _context.Movie.ToListAsync();
        }
    }
}
