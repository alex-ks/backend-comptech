using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            modelBuilder.Entity<DbUser>().ToTable("AspNetUsers");

            modelBuilder
                .Entity<DbPhoto>()
                .HasKey(p=>p.PhotoId);

            modelBuilder
                .Entity<DbPhoto>()
                .HasOne(p => p.Session)
                .WithMany(t=>t.Photos);

            modelBuilder
                .Entity<DbPulse>()
                .HasKey(p=>new {p.SessionId,p.timestamp });

            modelBuilder
                .Entity<DbPulse>().HasOne(p=>p.Session)
                .WithMany(t=>t.Pulses);

            modelBuilder
                .Entity<DbResult>()
                .HasKey(p => p.PhotoId);

            modelBuilder
                .Entity<DbResult>()
                .HasOne(p=>p.Photo)
                .WithOne(t=>t.Result);

            modelBuilder
                .Entity<DbSession>()
                .HasKey(p => p.SessionId);

            modelBuilder
                .Entity<DbSession>()
                .HasOne(p=>p.DbUser)
                .WithMany(t=>t.Sessions);

            base.OnModelCreating(modelBuilder);

        }
        
    }
}
