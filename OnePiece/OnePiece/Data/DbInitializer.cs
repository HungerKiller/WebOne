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
                var weapons = GetWeaponsFromFile("Data/Files/Weapon.txt");
                foreach (Weapon w in weapons)
                {
                    context.Weapons.Add(w);
                }
                context.SaveChanges();
            }
            // PirateGroups
            if (!context.PirateGroups.Any())
            {
                var pirateGroups = GetPirateGroupsFromFile("Data/Files/PirateGroup.txt");
                foreach (PirateGroup group in pirateGroups)
                {
                    context.PirateGroups.Add(group);
                }
                context.SaveChanges();
            }
            // TODO Person
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
            // TODO Add FruitPossessions
            if (!context.FruitPossessions.Any())
            {
                var person = context.Persons.FirstOrDefault();
                var fruit = context.Fruits.FirstOrDefault();
                person.FruitPossessions = new List<FruitPossession>();
                person.FruitPossessions.Add(new FruitPossession { PersonID = person.Id, FruitID = fruit.Id });
                context.SaveChanges();
            }
            // TODO Add WeaponPossessions
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
                    if (line.StartsWith("#"))
                        continue;
                    var array = line.Split('&');
                    Enum.TryParse((array[1]), out FruitType fruitType);
                    fruits.Add(new Fruit { Name = array[0], Type = fruitType, Ability = array[3] });
                }
            }
            return fruits;
        }

        private static List<Weapon> GetWeaponsFromFile(string filePath)
        {
            List<Weapon> weapons = new List<Weapon>();
            if (!File.Exists(filePath))
                return weapons;
            // Read file
            using (StreamReader sr = new StreamReader(filePath))
            {
                // Read header
                sr.ReadLine();
                // Read content
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("#"))
                        continue;
                    var array = line.Split('&');
                    weapons.Add(new Weapon { Name = array[0], Description = array[1] });
                }
            }
            return weapons;
        }

        private static List<PirateGroup> GetPirateGroupsFromFile(string filePath)
        {
            List<PirateGroup> pirateGroups = new List<PirateGroup>();
            if (!File.Exists(filePath))
                return pirateGroups;
            // Read file
            using (StreamReader sr = new StreamReader(filePath))
            {
                // Read header
                sr.ReadLine();
                // Read content
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("#"))
                        continue;
                    var array = line.Split('&');
                    pirateGroups.Add(new PirateGroup { Name = array[0], Description = array[1] });
                }
            }
            return pirateGroups;
        }
    }
}
