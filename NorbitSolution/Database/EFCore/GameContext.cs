using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.EFCore
{
    internal class GameContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Platform> Platforms { get; set; }

        private readonly string _connectionString;

        public GameContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameGenre>()
                .HasKey(gg => new { gg.GameId, gg.GenreId });

            modelBuilder.Entity<GamePlatform>()
                .HasKey(gp => new { gp.GameId, gp.PlatformId });

            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.GameId, r.UserId })
                .IsUnique();
        }
    }
}
