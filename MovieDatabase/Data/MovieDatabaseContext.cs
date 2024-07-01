using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using MovieDatabase.Models;

/**
 * A Data namespace for MovieDatabase data.
 */
namespace MovieDatabase.Data
{
    /**
     * A Movie Database Context class inheriting after the base class of the Entity Framework database context used for identity.
     */
    public class MovieDatabaseContext : IdentityDbContext<User>
    {
        /**
         * MovieDatabaseContext constructor.
         * @param options to be used by a DbContext.
         */
        public MovieDatabaseContext (DbContextOptions<MovieDatabaseContext> options)
            : base(options)
        {
        }
        /**
         * A DbSet for Movies to be used to query and save instances of Movie, 
         * where LINQ queries against this set will be translated into queries against the database.
         */
        public DbSet<MovieDatabase.Models.Movie> Movie { get; set; } = default!;

        /**
         * A DbSet for Actors to be used to query and save instances of Actor,
         * where LINQ queries against this set will be translated into queries against the database.
         */
        public DbSet<MovieDatabase.Models.Actor> Actor { get; set; } = default!;

        /**
         * A DbSet for Directors to be used to query and save instances of Director, 
         * where LINQ queries against this set will be translated into queries against the database.
         */
        public DbSet<MovieDatabase.Models.Director> Director { get; set; } = default!;

        /**
         * A DbSet for Users to be used to query and save instances of User, 
         * where LINQ queries against this set will be translated into queries against the database.
         */
        public DbSet<MovieDatabase.Models.User> User { get; set; } = default!;

        /**
         * A DbSet for UserMovies to be used to query and save instances of UserMovie, 
         * where LINQ queries against this set will be translated into queries against the database.
         */
        public DbSet<MovieDatabase.Models.UserMovie> UserMovie { get; set; } = default!;

        /**
         * A DbSet for Genres to be used to query and save instances of Genre, 
         * where LINQ queries against this set will be translated into queries against the database.
         */
        public DbSet<MovieDatabase.Models.Genre> Genre { get; set; } = default!;

        /**
         * A DbSet for GenreMovies to be used to query and save instances of GenreMovie, 
         * where LINQ queries against this set will be translated into queries against the database.
         */
        public DbSet<MovieDatabase.Models.GenreMovie> GenreMovie { get; set; } = default!;

        /**
         * A DbSet for Comments to be used to query and save instances of Comment, 
         * where LINQ queries against this set will be translated into queries against the database.
         */
        public DbSet<MovieDatabase.Models.Comment> Comment { get; set; } = default!;

        /**
         * A DbSet for Subcomments to be used to query and save instances of Subcomment, 
         * where LINQ queries against this set will be translated into queries against the database.
         */
        public DbSet<MovieDatabase.Models.Subcomment> Subcomment { get; set; } = default!;

        /**
         * A DbSet for Ratings to be used to query and save instances of Rating, 
         * where LINQ queries against this set will be translated into queries against the database.
         */
        public DbSet<MovieDatabase.Models.Rating> Rating { get; set; } = default!;

        /**
         * An override of the method OnModelCreating for using the API to configure the model. 
         */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>()
                .HasOne<Director>()
                .WithMany(e => e.movies)
                .HasForeignKey(e => e.director_id)
                .IsRequired();

            modelBuilder.Entity<Movie>()
                .HasMany(m => m.actors)
                .WithMany(a => a.movies);

            modelBuilder.Entity<Movie>()
                .HasMany(m => m.users)
                .WithMany(u => u.movies)
                .UsingEntity<UserMovie>(
                    l => l.HasOne<User>().WithMany(u => u.usermovies).HasForeignKey(um => um.user_id),
                    r => r.HasOne<Movie>().WithMany(m => m.usermovies).HasForeignKey(um => um.movie_id));

            modelBuilder.Entity<Movie>()
                .HasMany(m => m.genres)
                .WithMany(g => g.movies)
                .UsingEntity<GenreMovie>(
                    l => l.HasOne<Genre>().WithMany(g => g.genreMovies).HasForeignKey(um => um.genresid),
                    r => r.HasOne<Movie>().WithMany(m => m.genreMovies).HasForeignKey(um => um.moviesid));

            modelBuilder.Entity<Comment>()
                .HasOne<Movie>()
                .WithMany(m => m.comments)
                .HasForeignKey(c => c.movie_id)
                .IsRequired();

            modelBuilder.Entity<Comment>()
                .HasOne<User>()
                .WithMany(u => u.comments)
                .HasForeignKey(c => c.user_id)
                .IsRequired();

            modelBuilder.Entity<Subcomment>()
                .HasOne<User>()
                .WithMany(u => u.subcomments)
                .HasForeignKey(s => s.user_id)
                .IsRequired();

            modelBuilder.Entity<Subcomment>()
                .HasOne<Comment>()
                .WithMany(c => c.subcomments)
                .HasForeignKey(s => s.comment_id)
                .IsRequired();

            modelBuilder.Entity<Rating>()
               .HasOne<Movie>()
               .WithMany(m => m.ratings)
               .HasForeignKey(r => r.movie_id)
               .IsRequired();

            modelBuilder.Entity<Rating>()
              .HasOne<User>()
              .WithMany(u => u.ratings)
              .HasForeignKey(r => r.user_id)
              .IsRequired();

        }
    }
}
