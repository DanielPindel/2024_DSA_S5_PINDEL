using MovieDatabase.Data;
namespace MovieDatabase.Models
{
	public class Actor
	{
		private MovieDatabaseContext context;

		public int id { get; set; }

		public string name { get; set; }

		public string surname { get; set; }

		public int date_of_birth { get; set; }
	}
}
