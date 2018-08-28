using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using OnePiece.Data;
using OnePiece.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnePiece.Controllers
{
    public class WeaponsController : Controller
    {
        private readonly OnePieceContext _context;
        private readonly IHostingEnvironment _environment;
        private readonly IStringLocalizer _localizer;

        public WeaponsController(OnePieceContext context, IHostingEnvironment IHostingEnvironment, IStringLocalizer<WeaponsController> localizer)
        {
            _context = context;
            _environment = IHostingEnvironment;
            _localizer = localizer;
        }

        // GET: Weapons
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            searchString = searchString ?? currentFilter;
            ViewData["CurrentFilter"] = searchString;
            
            var weapons = from w in _context.Weapons select w;
            // Search
            if (!String.IsNullOrEmpty(searchString))
            {
                weapons = weapons.Where(w => w.Name.Contains(searchString));
            }
            // Order
            switch (sortOrder)
            {
                case "name_desc":
                    weapons = weapons.OrderByDescending(w => w.Name);
                    break;
                default:
                    weapons = weapons.OrderBy(w => w.Name);
                    break;
            }
            int pageSize = _context.Settings.FirstOrDefault().WeaponCountPerPage; ;
            return View(await PaginatedList<Weapon>.CreateAsync(weapons.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: Weapons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weapon = await _context.Weapons.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
            if (weapon == null)
            {
                return NotFound();
            }

            return View(weapon);
        }

        // GET: Weapons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Weapons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,ImagePath")] Weapon weapon)
        {
            if (_context.Weapons.Any(w => w.Name == weapon.Name))
            {
                ViewBag.NameExists = _localizer["Name '{0}' already exists.", weapon.Name];
                return View(weapon);
            }
            if (ModelState.IsValid)
            {
                // Try to upload file
                if (!(await TryUploadFile(weapon)))
                    return View(weapon);
                // Save DB
                _context.Add(weapon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(weapon);
        }

        // GET: Weapons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weapon = await _context.Weapons.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
            if (weapon == null)
            {
                return NotFound();
            }
            return View(weapon);
        }

        // POST: Weapons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImagePath")] Weapon weapon)
        {
            if (id != weapon.Id)
            {
                return NotFound();
            }

            if (_context.Weapons.Any(w => w.Name == weapon.Name && w.Id != weapon.Id))
            {
                ViewBag.NameExists = _localizer["Name '{0}' already exists.", weapon.Name];
                return View(weapon);
            }

            if (ModelState.IsValid)
            {
                // Try to upload file
                if (!(await TryUploadFile(weapon)))
                    return View(weapon);
                // Update DB
                try
                {
                    _context.Update(weapon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeaponExists(weapon.Id))
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
            return View(weapon);
        }

        // GET: Weapons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weapon = await _context.Weapons.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
            if (weapon == null)
            {
                return NotFound();
            }

            return View(weapon);
        }

        // POST: Weapons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var weapon = await _context.Weapons.SingleOrDefaultAsync(m => m.Id == id);
            // Remove file at first
            if (weapon.ImagePath != null)
            {
                string filePath = Path.Combine(_environment.WebRootPath, weapon.ImagePath);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            // Then remove entity from DB
            _context.Weapons.Remove(weapon);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WeaponExists(int id)
        {
            return _context.Weapons.Any(e => e.Id == id);
        }

        #region Helper

        private async Task<string> UploadFile(IFormFile file)
        {
            // New file name
            string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePathDB = Path.Combine("clientimages", newFileName);
            string filePath = Path.Combine(_environment.WebRootPath, filePathDB);
            using (FileStream fs = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(fs);
                fs.Flush();
            }
            return filePathDB;
        }

        private string CheckExtension(IFormFile file)
        {
            List<string> validExtensions = new List<string>() { ".png", ".jpg", ".jpeg", ".gif" };
            string extension = Path.GetExtension(file.FileName);
            if (validExtensions.Contains(extension))
                return null;
            else
                return _localizer["File '{0}' was removed because of wrong extension.", file.Name];
        }

        /// <summary>
        /// Try to upload file and update entity object
        /// </summary>
        /// <param name="weapon"></param>
        /// <returns>true=No error; false=Has error</returns>
        private async Task<bool> TryUploadFile(Weapon weapon)
        {
            // If no file to upload, return true
            if (HttpContext.Request.Form.Files == null)
                return true;
            var file = HttpContext.Request.Form.Files.FirstOrDefault();
            if (file == null || file.Length == 0)
                return true;
            // Check extension
            string extensionMsg = CheckExtension(file);
            if (!string.IsNullOrEmpty(extensionMsg))
            {
                ViewBag.WrongExtension = extensionMsg;
                return false;
            }
            // Remove old image
            if (weapon.ImagePath != null)
            {
                string filePath = Path.Combine(_environment.WebRootPath, weapon.ImagePath);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            // Upload new image
            weapon.ImagePath = await UploadFile(file);
            return true;
        }

        #endregion Helper
    }
}
