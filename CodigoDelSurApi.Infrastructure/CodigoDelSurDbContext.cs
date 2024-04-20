using CodigoDelSurApi.Infrastructure.DataEntities;
using Microsoft.EntityFrameworkCore;
using System;

namespace CodigoDelSurApi.Infrastructure
{
    public class CodigoDelSurDbContext : DbContext, ICodigoDelSurDbContext
    {
        public CodigoDelSurDbContext() { }

        public CodigoDelSurDbContext(DbContextOptions<CodigoDelSurDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<UserPreferences> UserPreferences { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=CodigoDelSurDb.db;"); // Change to match your connection string and provider
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<UserPreferences>()
                .HasKey(p => p.UserId);  // Confirming UserPreferences has a primary key

            modelBuilder.Entity<User>()
                .HasOne(u => u.Preferences)
                .WithOne(p => p.User)
                .HasForeignKey<UserPreferences>(p => p.UserId);


        }
    }
}