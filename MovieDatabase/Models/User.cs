using MovieDatabase.Data;
using Microsoft.AspNetCore.Identity;

namespace MovieDatabase.Models
{
    public class User : IdentityUser
    {
        private MovieDatabaseContext context;
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
