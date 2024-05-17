using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MovieDatabase.Models;

namespace MovieDatabase.Data
{
    public class MovieDatabaseContext : DbContext
    {
        public MovieDatabaseContext (DbContextOptions<MovieDatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<MovieDatabase.Models.Movie> Movie { get; set; } = default!;
		public DbSet<MovieDatabase.Models.Actor> Actor { get; set; } = default!;
        public DbSet<MovieDatabase.Models.Director> Director { get; set; } = default!;
        public DbSet<MovieDatabase.Models.User> User { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Movie>()
                .HasOne<Director>()
                .WithMany(e => e.movies)
                .HasForeignKey(e => e.director_id)
                .IsRequired();
        }
    }
}
