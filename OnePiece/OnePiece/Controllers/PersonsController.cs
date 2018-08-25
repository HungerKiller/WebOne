using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using OnePiece.Data;
using OnePiece.Models;
using OnePiece.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnePiece.Controllers
{
    public class PersonsController : Controller
    {
        private readonly OnePieceContext _context;
        private readonly IHostingEnvironment _environment;
        private readonly IStringLocalizer _localizer;

        public PersonsController(OnePieceContext context, IHostingEnvironment IHostingEnvironment, IStringLocalizer<PersonsController> localizer)
        {
            _context = context;
            _environment = IHostingEnvironment;
            _localizer = localizer;
        }

        // GET: Person
        public async Task<IActionResult> Index()
        {
            return View(await _context.Persons.AsNoTracking().ToListAsync());
        }

        // GET: Person/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // TODO include WeaponPossessions, GroupPossessions
            var person = await _context.Persons.AsNoTracking()
                .Include(p => p.FruitPossessions).ThenInclude(fp => fp.Fruit)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: Person/Create
        public IActionResult Create()
        {
            Person person = new Person();
            person.FruitPossessions = new List<FruitPossession>();
            // TODO populate also Weapon PirateGroup
            PopulateAssignedFruit(person);
            return View();
        }

        // POST: Person/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Nickname,Description,RewardMoney,Race,Sex,Birthday,FeatureType,Title,ImagePath")] Person person, string[] selectedFruits)
        {
            if (_context.Persons.Any(p => p.Name == person.Name))
            {
                ViewBag.NameExists = _localizer["Name '{0}' already exists.", person.Name];
                PopulateAssignedFruit(selectedFruits);
                return View(person);
            }
            if (ModelState.IsValid)
            {
                // Try to upload file
                if (!(await TryUploadFile(person)))
                {
                    PopulateAssignedFruit(selectedFruits);
                    return View(person);
                }
                // Update fruits
                person.FruitPossessions = new List<FruitPossession>();
                foreach (var fruitIdStr in selectedFruits)
                {
                    int fruitId = int.Parse(fruitIdStr);
                    // 多对多关系，指定了Id，会自动绑定object的值。此处new FruitPossession时，只设置了PersonID和FruitID，而Person和Fruit对象没有指定，但是会自动绑定。
                    person.FruitPossessions.Add(new FruitPossession { PersonID = person.Id, FruitID = fruitId });
                }
                // Reward money
                if (person.RewardMoney == null)
                    person.RewardMoney = 0;
                // Save DB
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateAssignedFruit(selectedFruits);
            return View(person);
        }

        // GET: Person/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.AsNoTracking()
                .Include(p => p.FruitPossessions).ThenInclude(fp => fp.Fruit)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }
            PopulateAssignedFruit(person);
            return View(person);
        }

        // POST: Person/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Nickname,Description,RewardMoney,Race,Sex,Birthday,FeatureType,Title,ImagePath")] Person person, string[] selectedFruits)
        {
            if (id != person.Id)
            {
                return NotFound();
            }

            var personToUpdate = await _context.Persons
                .Include(p => p.FruitPossessions).ThenInclude(fp => fp.Fruit)
                .SingleOrDefaultAsync(p => p.Id == id);
            // Check if the name exists already
            if (_context.Persons.Any(p => p.Name == person.Name && p.Id != person.Id))
            {
                ViewBag.NameExists = _localizer["Name '{0}' already exists.", person.Name];
                PopulateAssignedFruit(selectedFruits);
                return View(person);
            }

            if (ModelState.IsValid)
            {
                // Update fruits
                // 把当前的object，用controller创建来的object更新
                await TryUpdateModelAsync<Person>(personToUpdate, "", i => i.Name, i => i.Nickname , i => i.Description, i => i.RewardMoney, i => i.Race, i => i.Sex, 
                    i => i.Birthday, i => i.FeatureType, i => i.Title, i => i.ImagePath);
                UpdatePersonFruits(personToUpdate, selectedFruits);
                // Try to upload file
                if (!(await TryUploadFile(personToUpdate)))
                {
                    PopulateAssignedFruit(personToUpdate);
                    return View(personToUpdate);
                }
                // Reward money
                if (person.RewardMoney == null)
                    person.RewardMoney = 0;
                // Update DB
                try
                {
                    _context.Update(personToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.Id))
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
            return View(person);
        }

        // GET: Person/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: Person/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.Persons.SingleOrDefaultAsync(m => m.Id == id);
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
            return _context.Persons.Any(e => e.Id == id);
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
        /// <param name="person"></param>
        /// <returns>true=No error; false=Has error</returns>
        private async Task<bool> TryUploadFile(Person person)
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
            if (person.ImagePath != null)
            {
                string filePath = Path.Combine(_environment.WebRootPath, person.ImagePath);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            // Upload new image
            person.ImagePath = await UploadFile(file);
            return true;
        }

        #endregion Helper

        #region Polulate and update fruit of person

        private void PopulateAssignedFruit(Person person)
        {
            var allFruits = _context.Fruits;
            var personFruits = new HashSet<int>(person.FruitPossessions.Select(fp => fp.FruitID));
            var viewModel = new List<AssignedFruit>();
            foreach (var fruit in allFruits)
            {
                viewModel.Add(new AssignedFruit
                {
                    FruitID = fruit.Id,
                    Name = fruit.Name,
                    Assigned = personFruits.Contains(fruit.Id)
                });
            }
            ViewData["Fruits"] = viewModel;
        }

        private void PopulateAssignedFruit(string[] selectedFruits)
        {
            var allFruits = _context.Fruits;
            var viewModel = new List<AssignedFruit>();
            foreach (var friut in allFruits)
            {
                viewModel.Add(new AssignedFruit
                {
                    FruitID = friut.Id,
                    Name = friut.Name,
                    Assigned = selectedFruits.ToList().Contains(friut.Id.ToString())
                });
            }
            ViewData["Fruits"] = viewModel;
        }

        private void UpdatePersonFruits(Person personToUpdate, string[] selectedFruits)
        {
            var selectedFruitsHS = new HashSet<string>(selectedFruits);
            var personFruits = new HashSet<int>(personToUpdate.FruitPossessions.Select(fp => fp.FruitID));
            foreach (var fruit in _context.Fruits)
            {
                if (selectedFruitsHS.Contains(fruit.Id.ToString())) // 本次选中这个Fruit
                {
                    if (!personFruits.Contains(fruit.Id)) // 并且这个Fruit之前不在Person中
                    {
                        personToUpdate.FruitPossessions.Add(new FruitPossession { PersonID = personToUpdate.Id, FruitID = fruit.Id }); // 则添加Fruit到Person
                    }
                }
                else // 本次没有选中这个Fruit
                {
                    if (personFruits.Contains(fruit.Id)) // 并且这个Fruit之前属于Person
                    {
                        var fPossession = personToUpdate.FruitPossessions.SingleOrDefault(fp => fp.PersonID == personToUpdate.Id && fp.FruitID == fruit.Id);
                        personToUpdate.FruitPossessions.Remove(fPossession); // 则从Person中删除Fruit
                    }
                }
            }
        }

        #endregion Polulate and update fruit of person
    }
}
