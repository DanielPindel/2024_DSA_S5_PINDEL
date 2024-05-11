using MovieDatabase.Data;
using System.Reflection.Metadata;
namespace MovieDatabase.Models
{
    public class Movie
    {
        private MovieDatabaseContext context;

        public int id { get; set; }

        public string title { get; set; }

        public int year { get; set; }

        public int director_id { get; set; }
        public Director director { get; set; }
    }
}
