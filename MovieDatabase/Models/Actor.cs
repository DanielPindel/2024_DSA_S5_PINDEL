﻿using MovieDatabase.Data;
namespace MovieDatabase.Models
{
	public class Actor
	{
		private MovieDatabaseContext context;

		public int id { get; set; }

		public string name { get; set; }

		public string surname { get; set; }

		public DateOnly date_of_birth { get; set; }

        public ICollection<Movie> movies { get; set; } = [];

        public Actor()
        {
            this.movies = new HashSet<Movie>();
        }

        //for displaying name and surname at once
        public string nameSurnameLabel
        {
            get
            {
                return string.Format("{0} {1}", name, surname);
            }
        }
    }
}
