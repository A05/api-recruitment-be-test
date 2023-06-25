using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ApiApplication.Database
{
    public class CinemaDbContext : DbContext
    {
        public DbSet<AuditoriumEntity> Auditoriums { get; set; }
        public DbSet<ShowtimeEntity> Showtimes { get; set; }
        public DbSet<MovieEntity> Movies { get; set; }

        public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditoriumEntity>(build =>
            {
                build.HasKey(entry => entry.Id);
                build.Property(entry => entry.Id).ValueGeneratedOnAdd();
                build.HasMany(entry => entry.Showtimes).WithOne().HasForeignKey(entity => entity.AuditoriumId);
            });

            modelBuilder.Entity<ShowtimeEntity>(build =>
            {
                build.HasKey(entry => entry.Id);
                build.Property(entry => entry.Id).ValueGeneratedOnAdd();                
                build.Property(entry => entry.Schedule).HasConversion(x => string.Join(",", x), y => y.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList());
                build.HasOne(entry => entry.Movie).WithOne().HasForeignKey<MovieEntity>(entry => entry.ShowtimeId);
            });

            modelBuilder.Entity<MovieEntity>(build =>
            {
                build.HasKey(entry => entry.Id);
                build.Property(entry => entry.Id).ValueGeneratedOnAdd();
            });            
        }
    }
}
