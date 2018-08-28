using OnePiece.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
                var fruits = GetFruitsFromFile("Data/Files/Fruit.txt");
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
                //pirateGroups[0].Persons = new List<Person>();
                //pirateGroups[0].Persons.Add(context.Persons.First());
                context.SaveChanges();
            }
            // Person
            if (!context.Persons.Any())
            {
                var persons = new Person[]
                {
                    new Person{Name="Person1",Description="des1",ImagePath="path1"},
                    new Person{Name="Person2",Description="des1",ImagePath="path1"},
                    new Person{Name="Person3",Description="des1",ImagePath="path1"},
                };
                foreach (Person p in persons)
                {
                    context.Persons.Add(p);
                    context.SaveChanges();
                }
                //context.SaveChanges();
                //// Add FruitPossessions
                //var person = context.Persons.FirstOrDefault();
                //var fruit = context.Fruits.FirstOrDefault();
                //person.FruitPossessions = new List<FruitPossession>();
                //person.FruitPossessions.Add(new FruitPossession { PersonID = person.Id, FruitID = fruit.Id });
                //// Add WeaponPossession
                //var weapon = context.Weapons.FirstOrDefault();
                //person.WeaponPossessions = new List<WeaponPossession>();
                //person.WeaponPossessions.Add(new WeaponPossession { PersonID = person.Id, WeaponID = weapon.Id });
                //context.SaveChanges();
            }

            // Add FruitPossessions
            if (!context.FruitPossessions.Any())
            {
                var person = context.Persons.FirstOrDefault();
                var fruit = context.Fruits.FirstOrDefault();
                person.FruitPossessions = new List<FruitPossession>();
                person.FruitPossessions.Add(new FruitPossession { PersonID = person.Id, FruitID = fruit.Id });
                context.SaveChanges();
            }
            // Add WeaponPossessions
            if (!context.WeaponPossessions.Any())
            {
                var person = context.Persons.FirstOrDefault();
                var weapon = context.Weapons.FirstOrDefault();
                person.WeaponPossessions = new List<WeaponPossession>();
                person.WeaponPossessions.Add(new WeaponPossession { PersonID = person.Id, WeaponID = weapon.Id });
                context.SaveChanges();
            }
            // Add Setting
            if (!context.Settings.Any())
            {
                context.Settings.Add(new Setting { FruitCountPerPage = 10, WeaponCountPerPage = 10, PirateGroupCountPerPage = 10, PersonCountPerPage = 10 });
                context.SaveChanges();
            }
        }

        private static List<Fruit> GetFruitsFromFile(string filePath)
        {
            List<Fruit> fruits = new List<Fruit>();
            if (!File.Exists(filePath))
                return fruits;
            // Read file
            using (StreamReader sr = new StreamReader(filePath))
            {
                // Read header
                sr.ReadLine();
                // Read content
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var array = line.Split('&');
                    Enum.TryParse((array[1]), out FruitType fruitType);
                    fruits.Add(new Fruit { Name = array[0], Type = fruitType, Ability = array[3]});
                }
            }
            return fruits;
        }
    }
}
