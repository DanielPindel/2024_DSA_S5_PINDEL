﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MovieDatabase.Data;

#nullable disable

namespace MovieDatabase.Migrations
{
    [DbContext(typeof(MovieDatabaseContext))]
    [Migration("20240529132309_AddedUserMovieConntectionTableContext")]
    partial class AddedUserMovieConntectionTableContext
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("ActorMovie", b =>
                {
                    b.Property<int>("actorsid")
                        .HasColumnType("int");

                    b.Property<int>("moviesid")
                        .HasColumnType("int");

                    b.HasKey("actorsid", "moviesid");

                    b.HasIndex("moviesid");

                    b.ToTable("ActorMovie");
                });

            modelBuilder.Entity("MovieDatabase.Models.Actor", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("id"));

                    b.Property<DateOnly>("date_of_birth")
                        .HasColumnType("date");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("surname")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("Actor");
                });

            modelBuilder.Entity("MovieDatabase.Models.Director", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("id"));

                    b.Property<DateOnly>("date_of_birth")
                        .HasColumnType("date");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("surname")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("Director");
                });

            modelBuilder.Entity("MovieDatabase.Models.Movie", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("director_id")
                        .HasColumnType("int");

                    b.Property<string>("posterImagePath")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("year")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("director_id");

                    b.ToTable("Movie");
                });

            modelBuilder.Entity("MovieDatabase.Models.User", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("related_account_id")
                        .HasColumnType("int");

                    b.Property<string>("username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("MovieDatabase.Models.UserMovie", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("context_id")
                        .HasColumnType("int");

                    b.Property<int>("movie_id")
                        .HasColumnType("int");

                    b.Property<int>("user_id")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("movie_id");

                    b.HasIndex("user_id");

                    b.ToTable("UserMovie");
                });

            modelBuilder.Entity("ActorMovie", b =>
                {
                    b.HasOne("MovieDatabase.Models.Actor", null)
                        .WithMany()
                        .HasForeignKey("actorsid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieDatabase.Models.Movie", null)
                        .WithMany()
                        .HasForeignKey("moviesid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MovieDatabase.Models.Movie", b =>
                {
                    b.HasOne("MovieDatabase.Models.Director", null)
                        .WithMany("movies")
                        .HasForeignKey("director_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MovieDatabase.Models.UserMovie", b =>
                {
                    b.HasOne("MovieDatabase.Models.Movie", null)
                        .WithMany("usermovies")
                        .HasForeignKey("movie_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieDatabase.Models.User", null)
                        .WithMany("usermovies")
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MovieDatabase.Models.Director", b =>
                {
                    b.Navigation("movies");
                });

            modelBuilder.Entity("MovieDatabase.Models.Movie", b =>
                {
                    b.Navigation("usermovies");
                });

            modelBuilder.Entity("MovieDatabase.Models.User", b =>
                {
                    b.Navigation("usermovies");
                });
#pragma warning restore 612, 618
        }
    }
}
