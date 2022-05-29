using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusesWebApplication;

namespace BusesWebApplication.Controllers
{
    public class DriversController : Controller
    {
        private readonly DBBusesContext _context;

        public DriversController(DBBusesContext context)
        {
            _context = context;
        }

        // GET: Drivers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Drivers.ToListAsync());
        }

        // GET: Drivers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers
                .Include(d => d.DriversCategories)
                .ThenInclude(dc => dc.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // GET: Drivers/Create
        public IActionResult Create()
        {
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Drivers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,BirthDate,Salary")] Driver driver, List<int> categoryIds)
        {
            if(categoryIds.Distinct().Count() != categoryIds.Count)
            {
                ModelState.AddModelError("", "Всі категорії мають бути різними");
            }
            if(driver.BirthDate.Year < 1900)
            {
                ModelState.AddModelError("", "Рік народження не може бути менше 1900");
            }
            if (driver.BirthDate.Year > DateTime.Now.Year - 18)
            {
                ModelState.AddModelError("", "Водію має бути прийнамні 18 років");
            }
            if (ModelState.IsValid)
            {
                _context.Add(driver);
                await _context.SaveChangesAsync();

                int driverId = _context.Drivers.OrderBy(d => d.Id).LastOrDefault(d => d.FullName == driver.FullName).Id;
                List<DriversCategory> categories = new();
                foreach (int categoryId in categoryIds)
                {
                    categories.Add(new DriversCategory{ DriverId = driverId, CategoryId = categoryId });
                }

                _context.AddRange(categories);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name");
            return View(driver);
        }

        // GET: Drivers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers.Include(d => d.DriversCategories).ThenInclude(dc => dc.Category).FirstOrDefaultAsync(d => d.Id == id);
            if (driver == null)
            {
                return NotFound();
            }
            var list = new List<SelectList>();
            foreach (var dc in driver.DriversCategories)
            {
                list.Add(new SelectList(_context.Categories, "Id", "Name", dc.CategoryId));
            }
            ViewData["Categories"] = list;
            return View(driver);
        }

        // POST: Drivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,BirthDate,Salary")] Driver driver, List<int> categoryIds)
        {
            if (id != driver.Id)
            {
                return NotFound();
            }
            if (categoryIds.Distinct().Count() != categoryIds.Count)
            {
                ModelState.AddModelError("", "Всі категорії мають бути різними");
            }
            if (driver.BirthDate.Year < 2000)
            {
                ModelState.AddModelError("", "Рік народження не може бути менше 1900");
            }
            if (driver.BirthDate.Year > DateTime.Now.Year - 18)
            {
                ModelState.AddModelError("", "Водію має бути прийнамни 18 років");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(driver);
                    _context.RemoveRange(await _context.DriversCategories.Where(dc => dc.DriverId == id).ToListAsync());

                    List<DriversCategory> categories = new();
                    foreach (int categoryId in categoryIds)
                    {
                        categories.Add(new DriversCategory { DriverId = id, CategoryId = categoryId });
                    }
                    _context.AddRange(categories);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverExists(driver.Id))
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

            var oldDriver = await _context.Drivers.Include(d => d.DriversCategories).ThenInclude(dc => dc.Category).FirstOrDefaultAsync(d => d.Id == id);
            var list = new List<SelectList>();
            foreach (var dc in oldDriver.DriversCategories)
            {
                list.Add(new SelectList(_context.Categories, "Id", "Name", dc.CategoryId));
            }
            ViewData["Categories"] = list;
            return View(driver);
        }

        // GET: Drivers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // POST: Drivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DriverExists(int id)
        {
            return _context.Drivers.Any(e => e.Id == id);
        }
    }
}
