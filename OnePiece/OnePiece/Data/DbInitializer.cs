using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnePiece.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            //WriteJson("PersonOwn.json", GetPersonOwnObjects(context).Cast<object>().ToList());

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
            // Add FruitPossessions, WeaponPossessions, PersonPirateGroup
            if (!context.FruitPossessions.Any())
            {
                var fruits = context.Fruits.ToList();
                var weapons = context.Weapons.ToList();
                var pirateGroups = context.PirateGroups.ToList();

                var personOwns = ReadPersonOwnFromJson("Data/Files/PersonOwn.json");
                foreach (PersonOwn po in personOwns)
                {
                    var person = context.Persons.SingleOrDefault(p => p.Name == po.PersonName);
                    foreach (var fruitName in po.OwnedFruitNames)
                    {
                        var fruit = fruits.SingleOrDefault(f => f.Name == fruitName);
                        context.FruitPossessions.Add(new FruitPossession { PersonID = person.Id, FruitID = fruit.Id });
                    }
                    foreach (var weaponName in po.OwnedWeaponNames)
                    {
                        var weapon = weapons.SingleOrDefault(w => w.Name == weaponName);
                        context.WeaponPossessions.Add(new WeaponPossession { PersonID = person.Id, WeaponID = weapon.Id });
                    }
                    var group = context.PirateGroups.SingleOrDefault(pg => pg.Name == po.PirateGroupName);
                    person.PirateGroupID = group?.Id;
                }
                context.SaveChanges();
            }
        }

        #region Helper DB to Json

        public static void WriteJson(string jsonFilePath, List<object> items)
        {
            using (StreamWriter sw = new StreamWriter(jsonFilePath))
            {
                JArray jArray = (JArray)JToken.FromObject(items);
                sw.Write(jArray.ToString());
            }
        }

        public static List<PersonOwn> GetPersonOwnObjects(OnePieceContext context)
        {
            var persons = context.Persons.AsNoTracking()
                .Include(p => p.FruitPossessions).ThenInclude(fp => fp.Fruit)
                .Include(p => p.WeaponPossessions).ThenInclude(wp => wp.Weapon)
                .Include(p => p.PirateGroup);
            List<PersonOwn> personOwns = new List<PersonOwn>();
            foreach (var person in persons)
            {
                PersonOwn personOwn = new PersonOwn();
                personOwn.PersonName = person.Name;
                personOwn.OwnedFruitNames = person.FruitPossessions.Select(fp => fp.Fruit.Name).ToList();
                personOwn.OwnedWeaponNames = person.WeaponPossessions.Select(wp => wp.Weapon.Name).ToList();
                personOwn.PirateGroupName = person.PirateGroup?.Name;
                personOwns.Add(personOwn);
            }
            return personOwns;
        }

        #endregion Helper DB to Json

        #region Helper Json to DB

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

        public static List<PersonOwn> ReadPersonOwnFromJson(string jsonFilePath)
        {
            string jsonPersonOwns = null;
            using (StreamReader sr = new StreamReader(jsonFilePath))
            {
                jsonPersonOwns = sr.ReadToEnd();
            }
            List<PersonOwn> personOwns = JsonConvert.DeserializeObject<List<PersonOwn>>(jsonPersonOwns);
            return personOwns;
        }

        #endregion Helper Json to DB
    }
}
