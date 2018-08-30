using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnePiece.Data;
using OnePiece.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;

namespace OnePiece.Controllers
{
    public class SettingsController : Controller
    {
        private readonly OnePieceContext _context;

        public SettingsController(OnePieceContext context)
        {
            _context = context;
        }

        // GET: Settings
        public async Task<IActionResult> Index()
        {
            var settings = await _context.Settings.ToListAsync();
            var setting = settings.FirstOrDefault();
            return View(setting);
        }

        // GET: Settings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings.SingleOrDefaultAsync(m => m.Id == id);
            if (setting == null)
            {
                return NotFound();
            }
            return View(setting);
        }

        // POST: Settings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FruitCountPerPage,WeaponCountPerPage,PirateGroupCountPerPage,PersonCountPerPage")] Setting setting)
        {
            if (id != setting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(setting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SettingExists(setting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(setting);
        }

        public async Task<IActionResult> Export()
        {
            WriteJson("Fruits.json", _context.Fruits.ToList().Cast<object>().ToList());
            WriteJson("Weapons.json", _context.Weapons.ToList().Cast<object>().ToList());
            WriteJson("PirateGroups.json", _context.PirateGroups.ToList().Cast<object>().ToList());
            WriteJson("Persons.json", _context.Persons.ToList().Cast<object>().ToList());
            ViewData["Export"] = "Export Finished!";
            return RedirectToAction(nameof(Index));
        }

        public static void WriteJson(string jsonFilePath, List<object> items)
        {
            using (StreamWriter sw = new StreamWriter(jsonFilePath))
            {
                JArray jArray = (JArray)JToken.FromObject(items);
                sw.Write(jArray.ToString());
            }
        }

        private bool SettingExists(int id)
        {
            return _context.Settings.Any(e => e.Id == id);
        }

    }
}
