using MovieDatabase.Data;

namespace MovieDatabase.Models
{
    public class User
    {
        private MovieDatabaseContext context;

        public int id { get; set; }

        public string username { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public ICollection<Movie> watchlist { get; set; } = [];

        public User()
        {
            this.watchlist = new HashSet<Movie>();
        }

    }
}
