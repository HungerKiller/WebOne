using OnePiece.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePiece.Data
{
    public class DbInitializer
    {
        public static void Initialize(OnePieceContext context)
        {
            context.Database.EnsureCreated();
            // Fruits
            if (!context.Fruits.Any())
            {
                var fruits = new Fruit[]
                {
                    new Fruit{Name="111",Type=FruitType.动物系,Description="des1",ImagePath="path1"},
                    new Fruit{Name="222",Type=FruitType.动物系,Description="des1",ImagePath="path1"},
                    new Fruit{Name="333",Type=FruitType.动物系,Description="des1",ImagePath="path1"},
                    new Fruit{Name="444",Type=FruitType.动物系,Description="des1",ImagePath="path1"},
                    new Fruit{Name="555",Type=FruitType.动物系,Description="des1",ImagePath="path1"}
                };
                foreach (Fruit f in fruits)
                {
                    context.Fruits.Add(f);
                }
                context.SaveChanges();
            }
            // Weapons
            if (!context.Weapons.Any())
            {
                var weapons = new Weapon[]
                {
                    new Weapon{Name="11",Description="des1",ImagePath="path1"},
                    new Weapon{Name="22",Description="des1",ImagePath="path1"},
                    new Weapon{Name="33",Description="des1",ImagePath="path1"},
                    new Weapon{Name="44",Description="des1",ImagePath="path1"}
                };
                foreach (Weapon w in weapons)
                {
                    context.Weapons.Add(w);
                }
                context.SaveChanges();
            }
            // PirateGroup
            if (!context.PirateGroups.Any())
            {
                var pirateGroups = new PirateGroup[]
                {
                    new PirateGroup{Name="1",Description="des1",ImagePath="path1"},
                    new PirateGroup{Name="2",Description="des1",ImagePath="path1"},
                    new PirateGroup{Name="3",Description="des1",ImagePath="path1"},
                };
                foreach (PirateGroup group in pirateGroups)
                {
                    context.PirateGroups.Add(group);
                }
                context.SaveChanges();
            }
            // Person
            if (!context.Persons.Any())
            {
                var persons = new Person[]
                {
                    new Person{Name="1",Description="des1",ImagePath="path1"},
                    new Person{Name="2",Description="des1",ImagePath="path1"},
                    new Person{Name="3",Description="des1",ImagePath="path1"},
                };
                foreach (Person p in persons)
                {
                    context.Persons.Add(p);
                }
                context.SaveChanges();
            }
        }
    }
}
