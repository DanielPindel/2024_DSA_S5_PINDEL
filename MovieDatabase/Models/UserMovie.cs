﻿namespace MovieDatabase.Models
{
    public class UserMovie
    {
        public int id { get; set; }
        public int movie_id { get; set; }
        public string user_id { get; set; }
        public int context_id { get; set; }
    }
}
