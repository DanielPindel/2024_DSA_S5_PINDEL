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

        public ICollection<Actor> actors { get; set;  } = [];

        public Movie()
        {
            this.actors = new HashSet<Actor>();
        }
    }
}
