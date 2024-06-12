namespace MovieDatabase.Models
{
    public class Subcomment
    {
        public int id { get; set; }

        public int comment_id { get; set; }

        public string user_id { get; set; }

        public string content { get; set; }

        public DateTime time { get; set; }

        public bool is_blocked { get; set; }

    }
}
