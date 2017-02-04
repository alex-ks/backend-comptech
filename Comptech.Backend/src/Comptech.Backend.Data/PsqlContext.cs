using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Comptech.Backend.Data.DbEntities;


namespace Comptech.Backend.Data
{
     class PsqlContext:DbContext
    {
        public DbSet<DbPhoto> Photos { get; set; }
        public DbSet<DbPulse> Pulse { get; set; }
        public DbSet<DbResult> Results { get; set; }
        public DbSet<DbSession> Sessions { get; set; }
        public DbSet<DbUser> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            
        }
         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbPhoto>().HasKey(p=>p.PhotoId);
            modelBuilder.Entity<DbPhoto>().HasOne(p => p.DbSession);
            modelBuilder.Entity<DbPulse>().



            modelBuilder.Entity<DbSession>().HasOne(p => p.DbUser);
        }
        
    }
}
