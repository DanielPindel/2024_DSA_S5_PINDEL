using MovieDatabase.Data;
using Microsoft.AspNetCore.Identity;

namespace MovieDatabase.Models
{
    public class User
    {
        private MovieDatabaseContext context;

        public string id { get; set; }

        public string username { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public int related_account_id { get; set; }

        public ICollection<UserMovie> usermovies { get; set; } = [];
        public ICollection<Movie> movies { get; set; } = [];

        public User()
        {
            this.movies = new HashSet<Movie>();
            this.usermovies = new HashSet<UserMovie>();
        }

    }
}
