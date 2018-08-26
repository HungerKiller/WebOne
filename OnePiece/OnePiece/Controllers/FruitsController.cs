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
    public class FruitsController : Controller
    {
        private readonly OnePieceContext _context;
        private readonly IHostingEnvironment _environment;
        private readonly IStringLocalizer _localizer;

        public FruitsController(OnePieceContext context, IHostingEnvironment IHostingEnvironment, IStringLocalizer<FruitsController> localizer)
        {
            _context = context;
            _environment = IHostingEnvironment;
            _localizer = localizer;
        }

        // GET: Fruits
        public async Task<IActionResult> Index()
        {
            return View(await _context.Fruits.AsNoTracking().ToListAsync());
        }

        // GET: Fruits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fruit = await _context.Fruits.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
            if (fruit == null)
            {
                return NotFound();
            }

            return View(fruit);
        }

        // GET: Fruits/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fruits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Type,Description")] Fruit fruit)
        {
            // Check if name already exists
            if (_context.Fruits.Any(f => f.Name == fruit.Name))
            {
                ViewBag.NameExists = _localizer["Name '{0}' already exists.", fruit.Name];
                return View(fruit);
            }

            if (ModelState.IsValid)
            {
                // Try to upload file
                if (!(await TryUploadFile(fruit)))
                    return View(fruit);
                // Save DB
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

            var fruit = await _context.Fruits.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type,Description,ImagePath")] Fruit fruit)
        {
            if (id != fruit.Id)
            {
                return NotFound();
            }
            // Check if name already exists
            if (_context.Fruits.Any(f => f.Name == fruit.Name && f.Id != fruit.Id))
            {
                ViewBag.NameExists = _localizer["Name '{0}' already exists.", fruit.Name];
                return View(fruit);
            }

            if (ModelState.IsValid)
            {
                // Try to upload file
                if (!(await TryUploadFile(fruit)))
                    return View(fruit);
                // Update DB
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

            var fruit = await _context.Fruits.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
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
            // Remove file at first
            if (fruit.ImagePath != null)
            {
                string filePath = Path.Combine(_environment.WebRootPath, fruit.ImagePath);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            // Then remove entity from DB
            _context.Fruits.Remove(fruit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FruitExists(int id)
        {
            return _context.Fruits.Any(e => e.Id == id);
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
        /// <param name="fruit"></param>
        /// <returns>true=No error; false=Has error</returns>
        private async Task<bool> TryUploadFile(Fruit fruit)
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
            if (fruit.ImagePath != null)
            {
                string filePath = Path.Combine(_environment.WebRootPath, fruit.ImagePath);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            // Upload new image
            fruit.ImagePath = await UploadFile(file);
            return true;
        }

        #endregion Helper

        // 检查名字是否已存在。更新时可能名字没有改，所以肯定存在它本身。所以不能用remote validation。
        // 暂时采用发送请求，服务器端判断，再返回的方式
        [AcceptVerbs("Get","Post")]
        public IActionResult NameExists(string name)
        {
            if (_context.Fruits.Any(f => f.Name.Equals(name)))
            {
                return Json(_localizer["Name '{0}' already exists.", name].Value);
            }
            return Json(true);
        }
    }
}
