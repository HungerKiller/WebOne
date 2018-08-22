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
    public class PirateGroupsController : Controller
    {
        private readonly OnePieceContext _context;
        private readonly IHostingEnvironment _environment;
        private readonly IStringLocalizer _localizer;

        public PirateGroupsController(OnePieceContext context, IHostingEnvironment IHostingEnvironment, IStringLocalizer<PirateGroupsController> localizer)
        {
            _context = context;
            _environment = IHostingEnvironment;
            _localizer = localizer;
        }

        // GET: PirateGroups
        public async Task<IActionResult> Index()
        {
            return View(await _context.PirateGroups.ToListAsync());
        }

        // GET: PirateGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pirateGroup = await _context.PirateGroups
                .SingleOrDefaultAsync(m => m.Id == id);
            if (pirateGroup == null)
            {
                return NotFound();
            }

            return View(pirateGroup);
        }

        // GET: PirateGroups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PirateGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,ImagePath")] PirateGroup pirateGroup)
        {
            if (_context.PirateGroups.Any(group => group.Name == pirateGroup.Name))
            {
                ViewBag.NameExists = _localizer["Name '{0}' already exists.", pirateGroup.Name];
                return View(pirateGroup);
            }
            if (ModelState.IsValid)
            {
                // Upload file
                if (HttpContext.Request.Form.Files != null)
                {
                    var file = HttpContext.Request.Form.Files.FirstOrDefault();
                    if (file != null && file.Length > 0)
                    {
                        // Check extension
                        string extensionMsg = CheckExtension(file);
                        if (!string.IsNullOrEmpty(extensionMsg))
                        {
                            ViewBag.WrongExtension = extensionMsg;
                            return View(pirateGroup);
                        }
                        // Upload file
                        pirateGroup.ImagePath = await UploadFile(file);
                    }
                }
                // Save DB
                _context.Add(pirateGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pirateGroup);
        }

        // GET: PirateGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pirateGroup = await _context.PirateGroups.SingleOrDefaultAsync(m => m.Id == id);
            if (pirateGroup == null)
            {
                return NotFound();
            }
            return View(pirateGroup);
        }

        // POST: PirateGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImagePath")] PirateGroup pirateGroup)
        {
            if (id != pirateGroup.Id)
            {
                return NotFound();
            }

            if (_context.PirateGroups.Any(group => group.Name == pirateGroup.Name && group.Id != pirateGroup.Id))
            {
                ViewBag.NameExists = _localizer["Name '{0}' already exists.", pirateGroup.Name];
                return View(pirateGroup);
            }

            if (ModelState.IsValid)
            {
                // Upload file
                if (HttpContext.Request.Form.Files != null)
                {
                    var file = HttpContext.Request.Form.Files.FirstOrDefault();
                    if (file != null && file.Length > 0)
                    {
                        // Check extension
                        string extensionMsg = CheckExtension(file);
                        if (!string.IsNullOrEmpty(extensionMsg))
                        {
                            ViewBag.WrongExtension = extensionMsg;
                            return View(pirateGroup);
                        }
                        // Remove old image
                        if (pirateGroup.ImagePath != null)
                        {
                            string filePath = Path.Combine(_environment.WebRootPath, pirateGroup.ImagePath);
                            if (System.IO.File.Exists(filePath))
                                System.IO.File.Delete(filePath);
                        }
                        // Upload new image
                        pirateGroup.ImagePath = await UploadFile(file);
                    }
                }
                // Update DB
                try
                {
                    _context.Update(pirateGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PirateGroupExists(pirateGroup.Id))
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
            return View(pirateGroup);
        }

        // GET: PirateGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pirateGroup = await _context.PirateGroups
                .SingleOrDefaultAsync(m => m.Id == id);
            if (pirateGroup == null)
            {
                return NotFound();
            }

            return View(pirateGroup);
        }

        // POST: PirateGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pirateGroup = await _context.PirateGroups.SingleOrDefaultAsync(m => m.Id == id);
            _context.PirateGroups.Remove(pirateGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PirateGroupExists(int id)
        {
            return _context.PirateGroups.Any(e => e.Id == id);
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

        #endregion Helper
    }
}
