using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            searchString = searchString ?? currentFilter;
            ViewData["CurrentFilter"] = searchString;

            var pirateGroups = from pg in _context.PirateGroups select pg;
            // Search
            if (!String.IsNullOrEmpty(searchString))
            {
                pirateGroups = pirateGroups.Where(group => group.Name.Contains(searchString));
            }
            // Order
            switch (sortOrder)
            {
                case "name_desc":
                    pirateGroups = pirateGroups.OrderByDescending(pg => pg.Name);
                    break;
                default:
                    pirateGroups = pirateGroups.OrderBy(pg => pg.Name);
                    break;
            }
            int pageSize = _context.Settings.FirstOrDefault().PirateGroupCountPerPage; ;
            return View(await PaginatedList<PirateGroup>.CreateAsync(pirateGroups.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: PirateGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pirateGroup = await _context.PirateGroups.AsNoTracking().Include(group => group.Persons)
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
            // 初始化一个空的group，且group中的list也要new。只是为了满足PopulateAssignedPerson函数的要求。
            PirateGroup group = new PirateGroup();
            group.Persons = new List<Person>();
            PopulateAssignedPerson(group);
            return View();
        }

        // POST: PirateGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,ImagePath")] PirateGroup pirateGroup, string[] selectedPersons)
        {
            if (_context.PirateGroups.Any(group => group.Name == pirateGroup.Name))
            {
                ViewBag.NameExists = _localizer["Name '{0}' already exists.", pirateGroup.Name];
                PopulateAssignedPerson(selectedPersons);
                return View(pirateGroup);
            }
            if (ModelState.IsValid)
            {
                // Try to upload file
                if (!(await TryUploadFile(pirateGroup)))
                {
                    PopulateAssignedPerson(selectedPersons);
                    return View(pirateGroup);
                }
                // Update persons
                pirateGroup.Persons = new List<Person>();
                foreach (var personIdStr in selectedPersons)
                {
                    int personId = int.Parse(personIdStr);
                    var person = _context.Persons.SingleOrDefault(p => p.Id == personId);
                    pirateGroup.Persons.Add(person);
                    person.PirateGroup = pirateGroup;
                }
                // Save DB
                _context.Add(pirateGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateAssignedPerson(selectedPersons);
            return View(pirateGroup);
        }

        // GET: PirateGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pirateGroup = await _context.PirateGroups.AsNoTracking().Include(group => group.Persons).SingleOrDefaultAsync(m => m.Id == id);
            if (pirateGroup == null)
            {
                return NotFound();
            }
            PopulateAssignedPerson(pirateGroup);
            return View(pirateGroup);
        }

        // POST: PirateGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImagePath")] PirateGroup pirateGroup, string[] selectedPersons)
        {
            if (id != pirateGroup.Id)
            {
                return NotFound();
            }
            var pirateGroupToUpdate = await _context.PirateGroups.Include(g => g.Persons).SingleOrDefaultAsync(g => g.Id == id);
            // Check if the name exists already
            if (_context.PirateGroups.Any(group => group.Name == pirateGroup.Name && group.Id != pirateGroup.Id))
            {
                ViewBag.NameExists = _localizer["Name '{0}' already exists.", pirateGroup.Name];
                PopulateAssignedPerson(selectedPersons);
                return View(pirateGroup);
            }

            if (ModelState.IsValid)
            {
                // Update persons
                // 把当前的object，用controller创建来的object更新
                await TryUpdateModelAsync<PirateGroup>(pirateGroupToUpdate, "", i => i.Name, i => i.Description, i => i.ImagePath);
                UpdatePirateGourpPersons(pirateGroupToUpdate, selectedPersons);
                // Try to upload file
                if (!(await TryUploadFile(pirateGroupToUpdate)))
                {
                    PopulateAssignedPerson(pirateGroupToUpdate);
                    return View(pirateGroupToUpdate);
                }
                // Update DB
                try
                {
                    _context.Update(pirateGroupToUpdate);
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

            var pirateGroup = await _context.PirateGroups.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
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
            var pirateGroup = await _context.PirateGroups.Include(g => g.Persons).SingleOrDefaultAsync(m => m.Id == id);
            // Remove file at first
            if (pirateGroup.ImagePath != null)
            {
                string filePath = Path.Combine(_environment.WebRootPath, pirateGroup.ImagePath);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            // Then remove entity from DB
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

        /// <summary>
        /// Try to upload file and update entity object
        /// </summary>
        /// <param name="group"></param>
        /// <returns>true=No error; false=Has error</returns>
        private async Task<bool> TryUploadFile(PirateGroup group)
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
            if (group.ImagePath != null)
            {
                string filePath = Path.Combine(_environment.WebRootPath, group.ImagePath);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            // Upload new image
            group.ImagePath = await UploadFile(file);
            return true;
        }

        private void PopulateAssignedPerson(PirateGroup group)
        {
            var allPersons = _context.Persons;
            var groupPersons = new HashSet<int>(group.Persons.Select(p => p.Id));
            var viewModel = new List<AssignedPerson>();
            foreach (var person in allPersons)
            {
                viewModel.Add(new AssignedPerson
                {
                    PersonID = person.Id,
                    Name = person.Name,
                    Assigned = groupPersons.Contains(person.Id)
                });
            }
            ViewData["Persons"] = viewModel;
        }

        private void PopulateAssignedPerson(string[] selectedPersons)
        {
            var allPersons = _context.Persons;
            var viewModel = new List<AssignedPerson>();
            foreach (var person in allPersons)
            {
                viewModel.Add(new AssignedPerson
                {
                    PersonID = person.Id,
                    Name = person.Name,
                    Assigned = selectedPersons.ToList().Contains(person.Id.ToString())
                });
            }
            ViewData["Persons"] = viewModel;
        }

        private void UpdatePirateGourpPersons(PirateGroup pirateGroupToUpdate, string[] selectedPersons)
        {
            var selectedPersonsHS = new HashSet<string>(selectedPersons);
            var groupPersons = new HashSet<int>(pirateGroupToUpdate.Persons.Select(p => p.Id));
            foreach (var person in _context.Persons)
            {
                if (selectedPersonsHS.Contains(person.Id.ToString())) // 本次选中这个Person
                {
                    if (!groupPersons.Contains(person.Id)) // 并且这个Person之前不在Group中
                    {
                        pirateGroupToUpdate.Persons.Add(person); // 则添加Person到Group
                        person.PirateGroup = pirateGroupToUpdate;
                    }
                }
                else // 本次没有选中这个Person
                {
                    if (groupPersons.Contains(person.Id)) // 并且这个Person之前属于Group
                    {
                        pirateGroupToUpdate.Persons.Remove(person); // 则从Group中删除Person
                        person.PirateGroup = null;
                    }
                }
            }
        }

        #endregion Helper
    }
}
