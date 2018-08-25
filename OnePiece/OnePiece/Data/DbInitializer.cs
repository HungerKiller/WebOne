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
                    new Fruit{Name="Fruit1",Type=FruitType.动物系,Description="des1",ImagePath="path1"},
                    new Fruit{Name="Fruit2",Type=FruitType.动物系,Description="des1",ImagePath="path1"},
                    new Fruit{Name="Fruit3",Type=FruitType.动物系,Description="des1",ImagePath="path1"},
                    new Fruit{Name="Fruit4",Type=FruitType.动物系,Description="des1",ImagePath="path1"},
                    new Fruit{Name="Fruit5",Type=FruitType.动物系,Description="des1",ImagePath="path1"}
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
                    new Weapon{Name="Weapon1",Description="des1",ImagePath="path1"},
                    new Weapon{Name="Weapon2",Description="des1",ImagePath="path1"},
                    new Weapon{Name="Weapon3",Description="des1",ImagePath="path1"},
                    new Weapon{Name="Weapon4",Description="des1",ImagePath="path1"}
                };
                foreach (Weapon w in weapons)
                {
                    context.Weapons.Add(w);
                }
                context.SaveChanges();
            }
            // Person
            if (!context.Persons.Any())
            {
                var persons = new Person[]
                {
                    new Person{Name="Person1",Description="des1",ImagePath="path1",Fruits=new List<Fruit>(), Weapons=new List<Weapon>()},
                    new Person{Name="Person2",Description="des1",ImagePath="path1",Fruits=new List<Fruit>(), Weapons=new List<Weapon>()},
                    new Person{Name="Person3",Description="des1",ImagePath="path1",Fruits=new List<Fruit>(), Weapons=new List<Weapon>()},
                };
                foreach (Person p in persons)
                {
                    context.Persons.Add(p);
                }
                context.SaveChanges();
            }
            // PirateGroup
            if (!context.PirateGroups.Any())
            {
                var pirateGroups = new PirateGroup[]
                {
                    new PirateGroup{Name="PirateGroup1",Description="des1",ImagePath="path1"},
                    new PirateGroup{Name="PirateGroup2",Description="des1",ImagePath="path1"},
                    new PirateGroup{Name="PirateGroup3",Description="des1",ImagePath="path1"},
                };
                foreach (PirateGroup group in pirateGroups)
                {
                    context.PirateGroups.Add(group);
                }
                pirateGroups[0].Persons = new List<Person>();
                pirateGroups[0].Persons.Add(context.Persons.First());
                context.SaveChanges();
            }
        }
    }
}
