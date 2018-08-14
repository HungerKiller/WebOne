using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnePiece.Data;
using OnePiece.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OnePiece.Controllers
{
    public class FruitsController : Controller
    {
        private readonly OnePieceContext _context;
        private readonly IHostingEnvironment _environment;

        public FruitsController(OnePieceContext context, IHostingEnvironment IHostingEnvironment)
        {
            _context = context;
            _environment = IHostingEnvironment;
        }

        // GET: Fruits
        public async Task<IActionResult> Index()
        {
            return View(await _context.Fruits.ToListAsync());
        }

        // GET: Fruits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fruit = await _context.Fruits.AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id);
            if (fruit == null)
            {
                return NotFound();
            }
            // TODO Image?
            return View(fruit);
        }

        // GET: Fruits/Create
        public IActionResult Create()
        {
            var types = Enum.GetValues(typeof(FruitType)).Cast<string>();
            ViewData["Type"] = types;
            ViewBag.Type = types;
            return View();
        }

        // POST: Fruits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Type,Description")] Fruit fruit)
        {
            if (ModelState.IsValid)
            {
                // Upload file
                if (HttpContext.Request.Form.Files != null)
                {                   
                    var files = HttpContext.Request.Form.Files;
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            //Getting FileName
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            //fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            // New file name
                            string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
                            string filePathDB = Path.Combine("images", newFileName);
                            string filePath = Path.Combine(_environment.WebRootPath, filePathDB);
                            using (FileStream fs = System.IO.File.Create(filePath))
                            {
                                await file.CopyToAsync(fs);
                                fs.Flush();
                                fruit.ImagePath = filePathDB;
                            }
                        }
                    }
                }

                _context.Add(fruit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fruit);
        }

        // GET: Fruits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fruit = await _context.Fruits.SingleOrDefaultAsync(m => m.Id == id);
            if (fruit == null)
            {
                return NotFound();
            }
            return View(fruit);
        }

        // POST: Fruits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type,Description")] Fruit fruit)
        {
            if (id != fruit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //TODO delete old image
                    //var f = await _context.Fruits.AsNoTracking().SingleOrDefaultAsync(m => m.Id == fruit.Id);

                    _context.Update(fruit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FruitExists(fruit.Id))
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
            return View(fruit);
        }

        // GET: Fruits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fruit = await _context.Fruits
                .SingleOrDefaultAsync(m => m.Id == id);
            if (fruit == null)
            {
                return NotFound();
            }

            return View(fruit);
        }

        // POST: Fruits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fruit = await _context.Fruits.SingleOrDefaultAsync(m => m.Id == id);
            _context.Fruits.Remove(fruit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FruitExists(int id)
        {
            return _context.Fruits.Any(e => e.Id == id);
        }
    }
}
