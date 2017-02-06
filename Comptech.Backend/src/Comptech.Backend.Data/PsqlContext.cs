using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Comptech.Backend.Data.DbEntities;


namespace Comptech.Backend.Data
{
    public class PsqlContext:DbContext
    {
        internal DbSet<DbPhoto> Photos { get; set; }
        internal DbSet<DbPulse> Pulse { get; set; }
        internal DbSet<DbResult> Results { get; set; }
        internal DbSet<DbSession> Sessions { get; set; }
        internal DbSet<DbUser> Users { get; set; }

        public static string DefaultConnection = "";

        private string _connectionString;

        public PsqlContext()
        {
            _connectionString = DefaultConnection;
        }

        public PsqlContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseNpgsql(_connectionString);
        }
         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            InitializeDbUser(modelBuilder);
            InitializeDbPhoto(modelBuilder);
            InitializeDbPulse(modelBuilder);
            InitializeDbResult(modelBuilder);
            InitializeDbSession(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
        
        private void InitializeDbUser(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<DbUser>()
                .ToTable("AspNetUsers");
        }

        private void InitializeDbPhoto(ModelBuilder modelBuilder)
        {
            modelBuilder
               .Entity<DbPhoto>()
               .HasKey(p => p.PhotoId);

            modelBuilder
                .Entity<DbPhoto>()
                .Property<int>("PhotoId")
                .ValueGeneratedOnAdd();

            modelBuilder
                .Entity<DbPhoto>()
                .HasOne(p => p.Session)
                .WithMany()
                .HasForeignKey(p => p.SessionId)
                .HasPrincipalKey(p => p.SessionId);

            modelBuilder
                .Entity<DbPhoto>()
                .Property(p => p.SessionId).IsRequired();

            modelBuilder
                .Entity<DbPhoto>()
                .Property(p => p.Image).IsRequired();

            modelBuilder
                .Entity<DbPhoto>()
                .Property(p => p.Timestamp).IsRequired();
        }

        private void InitializeDbPulse(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<DbPulse>()
                .HasKey(p => new { p.SessionId, p.timestamp });

            modelBuilder
                .Entity<DbPulse>()
                .HasOne(p => p.Session)
                .WithMany()
                .HasForeignKey(p => p.SessionId)
                .HasPrincipalKey(p => p.SessionId);

            modelBuilder
                .Entity<DbPulse>()
                .Property(p => p.Bpm).IsRequired();
        }

        private void InitializeDbResult(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<DbResult>()
                .HasKey(p => p.PhotoId);

            modelBuilder
                .Entity<DbResult>()
                .HasOne(p => p.Photo)
                .WithOne()
                .HasForeignKey<DbPhoto>(p => p.PhotoId);

            modelBuilder
                .Entity<DbResult>()
                .Property(p => p.IsValid).IsRequired();

            modelBuilder
                .Entity<DbResult>()
                .Property(p => p.PhotoId).IsRequired();
        }

        private void InitializeDbSession(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<DbSession>()
                .HasKey(p => p.SessionId);

            modelBuilder
                .Entity<DbSession>()
                .Property<int>("SessionId")
                .ValueGeneratedOnAdd();

            modelBuilder
                .Entity<DbSession>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .HasPrincipalKey(p => p.UserId);

            modelBuilder
                .Entity<DbSession>()
                .Property(p => p.UserId).IsRequired();

            modelBuilder
                .Entity<DbSession>()
                .Property(p => p.Start).IsRequired();

            modelBuilder
                .Entity<DbSession>()
                .Property(p => p.Status).IsRequired();
        }
    }
}
