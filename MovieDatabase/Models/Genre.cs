namespace MovieDatabase.Models
{
    public class Genre
    {
        public int id { get; set; }

        public string tag { get; set; }

        public ICollection<Movie> movies { get; set; } = [];
        public ICollection<GenreMovie> genreMovies { get; set; } = [];

        public Genre()
        {
            this.movies = new HashSet<Movie>();
            this.genreMovies = new HashSet<GenreMovie>();
        }
    }
}
