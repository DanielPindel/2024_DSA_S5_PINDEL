using Microsoft.Extensions.Hosting;
using MovieDatabase.Data;

namespace MovieDatabase.Models
{
    public class Director
    {
        private MovieDatabaseContext context;

        public int id { get; set; }

        public string name { get; set; }

        public string surname { get; set; }

        public DateOnly date_of_birth { get; set; }

        public ICollection<Movie> movies { get; } = new List<Movie>();
    }
}
