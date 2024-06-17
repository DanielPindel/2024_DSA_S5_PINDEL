using MovieDatabase.Data;
using System.Reflection.Metadata;
namespace MovieDatabase.Models
{
    public class Movie
    {
        public int id { get; set; }
        public string title { get; set; }
        public int year { get; set; }
        public int director_id { get; set; }
        public string posterImagePath { get; set; }
        public string? description { get; set; }
        public string? trailer_link {  get; set; }
        public float? rate { get; set; }

        public ICollection<Actor> actors { get; set;  } = [];
        public ICollection<UserMovie> usermovies { get; set; } = [];
        public ICollection<User> users { get; set; } = [];
        public ICollection<Genre> genres { get; set; } = [];
        public ICollection<Comment> comments { get; set; } = [];
        public ICollection<Rating> ratings { get; set; } = [];


        public Movie()
        {
            this.users = new HashSet<User>();
            this.usermovies = new HashSet<UserMovie>();
            this.actors = new HashSet<Actor>();
            this.genres = new HashSet<Genre>();
            this.comments = new HashSet<Comment>();
            this.ratings = new HashSet<Rating>();
        }
    }
}
