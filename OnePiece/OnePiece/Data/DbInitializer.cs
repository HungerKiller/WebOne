using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            // Write DB object in json file
            //WriteJson("Fruits.json", context.Fruits.ToList().Cast<object>().ToList());
            //WriteJson("Weapons.json", context.Weapons.ToList().Cast<object>().ToList());
            //WriteJson("PirateGroups.json", context.PirateGroups.ToList().Cast<object>().ToList());
            //WriteJson("Persons.json", context.Persons.ToList().Cast<object>().ToList());

            // Initialize DB by reading json file
            context.Database.EnsureCreated();
            // Add Setting
            if (!context.Settings.Any())
            {
                context.Settings.Add(new Setting { FruitCountPerPage = 10, WeaponCountPerPage = 10, PirateGroupCountPerPage = 10, PersonCountPerPage = 10 });
                context.SaveChanges();
            }
            // Fruits
            if (!context.Fruits.Any())
            {
                var fruits = ReadFruitFromJson("Data/Files/Fruits.json");
                foreach (Fruit f in fruits)
                {
                    context.Fruits.Add(f);
                }
                context.SaveChanges();
            }
            // Weapons
            if (!context.Weapons.Any())
            {
                var weapons = ReadWeaponFromJson("Data/Files/Weapons.json");
                foreach (Weapon w in weapons)
                {
                    context.Weapons.Add(w);
                }
                context.SaveChanges();
            }
            // PirateGroups
            if (!context.PirateGroups.Any())
            {
                var pirateGroups = ReadPirateGroupFromJson("Data/Files/PirateGroups.json");
                foreach (PirateGroup group in pirateGroups)
                {
                    context.PirateGroups.Add(group);
                }
                context.SaveChanges();
            }
            // Person
            if (!context.Persons.Any())
            {
                var persons = ReadPersonFromJson("Data/Files/Persons.json");
                foreach (Person p in persons)
                {
                    context.Persons.Add(p);
                }
                context.SaveChanges();
            }
            //// TODO Add FruitPossessions
            //if (!context.FruitPossessions.Any())
            //{
            //    var person = context.Persons.FirstOrDefault();
            //    var fruit = context.Fruits.FirstOrDefault();
            //    person.FruitPossessions = new List<FruitPossession>();
            //    person.FruitPossessions.Add(new FruitPossession { PersonID = person.Id, FruitID = fruit.Id });
            //    context.SaveChanges();
            //}
            //// TODO Add WeaponPossessions
            //if (!context.WeaponPossessions.Any())
            //{
            //    var person = context.Persons.FirstOrDefault();
            //    var weapon = context.Weapons.FirstOrDefault();
            //    person.WeaponPossessions = new List<WeaponPossession>();
            //    person.WeaponPossessions.Add(new WeaponPossession { PersonID = person.Id, WeaponID = weapon.Id });
            //    context.SaveChanges();
            //}
        }

        #region Helper

        public static void WriteJson(string jsonFilePath, List<object> items)
        {
            using (StreamWriter sw = new StreamWriter(jsonFilePath))
            {
                JArray jArray = (JArray)JToken.FromObject(items);
                sw.Write(jArray.ToString());
            }
        }

        public static List<PirateGroup> ReadPirateGroupFromJson(string jsonFilePath)
        {
            string jsonPirateGroups = null;
            using (StreamReader sr = new StreamReader(jsonFilePath))
            {
                jsonPirateGroups = sr.ReadToEnd();
            }
            List<PirateGroup> groups = JsonConvert.DeserializeObject<List<PirateGroup>>(jsonPirateGroups);
            return groups;
        }

        public static List<Weapon> ReadWeaponFromJson(string jsonFilePath)
        {
            string jsonWeapons = null;
            using (StreamReader sr = new StreamReader(jsonFilePath))
            {
                jsonWeapons = sr.ReadToEnd();
            }
            List<Weapon> weapons = JsonConvert.DeserializeObject<List<Weapon>>(jsonWeapons);
            return weapons;
        }

        public static List<Fruit> ReadFruitFromJson(string jsonFilePath)
        {
            string jsonFruits = null;
            using (StreamReader sr = new StreamReader(jsonFilePath))
            {
                jsonFruits = sr.ReadToEnd();
            }
            List<Fruit> fruits = JsonConvert.DeserializeObject<List<Fruit>>(jsonFruits);
            return fruits;
        }

        public static List<Person> ReadPersonFromJson(string jsonFilePath)
        {
            string jsonPersons = null;
            using (StreamReader sr = new StreamReader(jsonFilePath))
            {
                jsonPersons = sr.ReadToEnd();
            }
            List<Person> persons = JsonConvert.DeserializeObject<List<Person>>(jsonPersons);
            return persons;
        }

        #endregion Helper
    }
}
