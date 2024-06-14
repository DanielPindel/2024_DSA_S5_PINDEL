namespace MovieDatabase.Models
{
    public class Rating
    {
        public int id { get; set; }

        public int movie_id { get; set; }

        public string user_id { get; set; }

        public int rate { get; set; }

        public string? review { get; set; }

        public DateTime time { get; set; }

        public Rating()
        {
        }
    }
}
