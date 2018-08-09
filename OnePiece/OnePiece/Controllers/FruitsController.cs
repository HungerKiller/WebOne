using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnePiece.Data;
using OnePiece.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePiece.Controllers
{
    public class FruitsController : Controller
    {
        private readonly OnePieceContext _context;

        public FruitsController(OnePieceContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Create([Bind("Id,Name,Type,Description")] Fruit fruit)
        {
            if (ModelState.IsValid)
            {
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
