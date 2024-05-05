using MovieDatabase.Data;
namespace MovieDatabase.Models
{
    public class Movie
    {
        private MovieDatabaseContext context;

        public int id { get; set; }

        public string title { get; set; }

        public int year { get; set; }
    }
}
