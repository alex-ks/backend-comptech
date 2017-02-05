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


            //DbPhoto entity
            modelBuilder
                .Entity<DbPhoto>()
                .HasKey(p=>p.PhotoId);

            modelBuilder
                .Entity<DbPhoto>()
                .HasOne(p => p.Session)
                .WithMany(t => t.Photos)
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


            //DbPulse entity
            modelBuilder
                .Entity<DbPulse>()
                .HasKey(p=>new {p.SessionId,p.timestamp });

            modelBuilder
                .Entity<DbPulse>()
                .HasOne(p=>p.Session)
                .WithMany(t=>t.Pulses)
                .HasForeignKey(p=>p.SessionId)
                .HasPrincipalKey(p=>p.SessionId);

            modelBuilder
                .Entity<DbPulse>()
                .Property(p => p.SessionId).IsRequired();

            modelBuilder
                .Entity<DbPulse>()
                .Property(p => p.Bpm).IsRequired();

            modelBuilder
                .Entity<DbPulse>()
                .Property(p => p.timestamp).IsRequired();

            //DbResult entity
            modelBuilder
                .Entity<DbResult>()
                .HasKey(p => p.PhotoId);

            modelBuilder
                .Entity<DbResult>()
                .HasOne(p => p.Photo)
                .WithMany(p=>p.Results)
                .HasForeignKey(p => p.PhotoId)
                .HasPrincipalKey(p=>p.PhotoId);

            modelBuilder
                .Entity<DbResult>()
                .Property(p => p.IsValid).IsRequired();

            modelBuilder
                .Entity<DbResult>()
                .Property(p => p.X1).IsRequired();

            modelBuilder
                .Entity<DbResult>()
                .Property(p => p.X2).IsRequired();

            modelBuilder
                .Entity<DbResult>()
                .Property(p => p.Y1).IsRequired();

            modelBuilder
                .Entity<DbResult>()
                .Property(p => p.Y1).IsRequired();

            modelBuilder
                .Entity<DbResult>()
                .Property(p => p.PhotoId).IsRequired();

            //DbSession entity
            modelBuilder
                .Entity<DbSession>()
                .HasKey(p => p.SessionId);

            modelBuilder
                .Entity<DbSession>()
                .HasOne(p=>p.User)
                .WithMany(t=>t.Sessions)
                .HasForeignKey(p=>p.UserId)
                .HasPrincipalKey(p=>p.UserId);

            modelBuilder
                .Entity<DbSession>()
                .Property(p => p.UserId).IsRequired();

            modelBuilder
                .Entity<DbSession>()
                .Property(p => p.Start).IsRequired();

            modelBuilder
                .Entity<DbSession>()
                .Property(p => p.Status).IsRequired();

            base.OnModelCreating(modelBuilder);

        }
        
    }
}
