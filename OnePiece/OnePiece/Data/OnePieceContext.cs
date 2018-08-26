using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnePiece.Models;

namespace OnePiece.Data
{
    public class OnePieceContext : DbContext
    {
        public OnePieceContext (DbContextOptions<OnePieceContext> options)
            : base(options)
        {
        }

        public DbSet<Fruit> Fruits { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<PirateGroup> PirateGroups { get; set; }
        public DbSet<FruitPossession> FruitPossessions { get; set; }
        public DbSet<WeaponPossession> WeaponPossessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fruit>().ToTable("Fruit");
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<Weapon>().ToTable("Weapon");
            modelBuilder.Entity<PirateGroup>().ToTable("PirateGroup");
            modelBuilder.Entity<FruitPossession>().ToTable("FruitPossession");
            modelBuilder.Entity<WeaponPossession>().ToTable("WeaponPossession");

            // Fluent API 方式制定主键
            modelBuilder.Entity<FruitPossession>()
                .HasKey(fp => new { fp.PersonID, fp.FruitID });
            modelBuilder.Entity<WeaponPossession>()
                .HasKey(fp => new { fp.PersonID, fp.WeaponID });
        }
    }
}
