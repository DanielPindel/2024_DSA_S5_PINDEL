namespace MovieDatabase.Models
{
    public class Comment
    {
        public int id { get; set; }

        public int movie_id { get; set; }

        public string user_id { get; set; }

        public string content { get; set; }

        public DateTime time { get; set; }

        public bool is_blocked { get; set; }

    }
}
