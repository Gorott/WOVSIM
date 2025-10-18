using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WOVSIM.Persistence.Domain.Models;

namespace WOVSIM.Persistence.EntityFramework
{
    public class WovsimDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Guild> Guilds { get; set; }
        public virtual DbSet<GameSession> GameSessions { get; set; }
        public virtual DbSet<Player> Players { get; set; }

        private readonly IConfiguration _configuration;

        //public WovsimDbContext() {}

        public WovsimDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_configuration["Database:ConnectionString"]);

                //optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=test;Database=wovsim;Command Timeout=60;Timeout=60;Persist Security Info=True");


                optionsBuilder.UseSnakeCaseNamingConvention();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
                entity.HasOne(p => p.User)
                    .WithOne(u => u.Player)
                    .HasForeignKey<Player>(p => p.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            modelBuilder.Entity<Guild>()
                .HasOne(g => g.GameGuild)
                .WithOne(g => g.MainGuild)
                .HasForeignKey<Guild>(g => g.MainGuildId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Guild>()
                .HasIndex(g => g.MainGuildId)
                .IsUnique()
                .HasFilter("[MainGuildId] IS NOT NULL");
        }
    }
}