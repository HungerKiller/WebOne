﻿using System;
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
    }
}