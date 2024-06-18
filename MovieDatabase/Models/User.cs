using MovieDatabase.Data;
using Microsoft.AspNetCore.Identity;

namespace MovieDatabase.Models
{
    public class User : IdentityUser
    {
        private MovieDatabaseContext context;
        public int related_account_id { get; set; }
        public string? avatar_path { get; set; }
        public bool is_admin { get; set; }

        public ICollection<UserMovie> usermovies { get; set; } = [];
        public ICollection<Movie> movies { get; set; } = [];
        public ICollection<Comment> comments { get; set; }
        public ICollection<Subcomment> subcomments { get; set; }
        public ICollection<Rating> ratings { get; set; }

        public User()
        {
            this.movies = new HashSet<Movie>();
            this.usermovies = new HashSet<UserMovie>();
            this.comments = new HashSet<Comment>();
            this.subcomments = new HashSet<Subcomment>();
            this.ratings = new HashSet<Rating>();
        }

    }
}
