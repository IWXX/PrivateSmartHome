using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnJiaWebServer_V1.Models;

using Pomelo.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AnJiaWebServer_V1.Data
{
    public class AnJiaContext : DbContext
    {
        public AnJiaContext(DbContextOptions<AnJiaContext> options) : base(options)
        {
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<SubServers> Subservers { get; set; }
        public DbSet<Groups> Groups { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<UserToSubserver> UserToSubserver { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<SubServers>().ToTable("Subservers");
            modelBuilder.Entity<UserToSubserver>().ToTable("UserToSubserver");
        }


    }
}
