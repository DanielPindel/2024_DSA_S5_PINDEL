﻿using System;
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

namespace MovieDatabase.Data
{
    public class MovieDatabaseContext : IdentityDbContext<User>
    {
        public MovieDatabaseContext (DbContextOptions<MovieDatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<MovieDatabase.Models.Movie> Movie { get; set; } = default!;
		public DbSet<MovieDatabase.Models.Actor> Actor { get; set; } = default!;
        public DbSet<MovieDatabase.Models.Director> Director { get; set; } = default!;
        public DbSet<MovieDatabase.Models.User> User { get; set; } = default!;
        public DbSet<MovieDatabase.Models.UserMovie> UserMovie { get; set; } = default!;
        public DbSet<MovieDatabase.Models.Genre> Genre { get; set; } = default!;

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

            /*modelBuilder.Entity<UserMovie>()
                .HasNoKey()
                .HasOne<Movie>()
                .WithMany(m => m.usermovies)
                .HasForeignKey(um => um.movie_id);


            modelBuilder.Entity<UserMovie>()
                .HasNoKey()
                .HasOne<User>()
                .WithMany(m => m.usermovies)
                .HasForeignKey(um => um.user_id);*/

        }
    }
}
