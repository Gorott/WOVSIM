using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WOVSIM.Persistence.Domain.Models;

namespace WOVSIM.Persistence.EntityFramework
{
    public class WovsimDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<GameSession> GameSessions { get; set; }
        public virtual DbSet<Player> Players { get; set; }

        private readonly IConfiguration _configuration;

        public WovsimDbContext() {}

        // public WovsimDbContext(IConfiguration configuration)
        // {
        //     _configuration = configuration;
        // }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseNpgsql(_configuration["Database:ConnectionString"]);

                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=test;Database=wovsim;Command Timeout=60;Timeout=60;Persist Security Info=True");

                optionsBuilder.UseSnakeCaseNamingConvention();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.HasIndex(i => i.UserId);

                entity.HasIndex(i => i.DiscordUserId);
            });
            modelBuilder.Entity<GameSession>(entity =>
            {
                entity.HasKey(e => e.GameSessionId);

                entity.HasMany(g => g.Players)
                    .WithOne(p => p.GameSession)
                    .HasForeignKey(p => p.GameSessionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.PlayerId);

                entity.HasIndex(i => i.PlayerId);

                entity.HasIndex(i => i.UserId);

                entity.HasOne(i => i.User)
                    .WithOne()
                    .HasForeignKey<Player>(i => i.UserId);
            });
        }
    }
}