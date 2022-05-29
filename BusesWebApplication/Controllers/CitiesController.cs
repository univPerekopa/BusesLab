#nullable disable
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
    public class CitiesController : Controller
    {
        private readonly DBBusesContext _context;

        public CitiesController(DBBusesContext context)
        {
            _context = context;
        }

        // GET: Cities
        public async Task<IActionResult> Index(int? countryId)
        {
            if(countryId == null) return RedirectToAction("Index", "Countries");

            ViewBag.CountryId = countryId;
            ViewBag.CountryName = _context.Countries.Where(c => c.Id == countryId).FirstOrDefault().Name;

            var citiesByCountry = _context.Cities.Where(c => c.CountryId == countryId).Include(c => c.Country);
            return View(await citiesByCountry.ToListAsync());
        }

        // GET: Cities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .Include(c => c.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Stations", new { cityId = city.Id });
        }

        // GET: Cities/Create
        public IActionResult Create(int countryId)
        {
            ViewBag.CountryId = countryId;
            ViewBag.CountryName = _context.Countries.Where(c => c.Id == countryId).FirstOrDefault().Name;
            return View();
        }

        // POST: Cities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int countryId, [Bind("Id,CountryId,Name")] City city)
        {
            city.CountryId = countryId;
            var res = await _context.Cities.FirstOrDefaultAsync(c => c.Name == city.Name);
            if (res != null)
            {
                ModelState.AddModelError("", "Таке місто вже існує");
            }

            if (ModelState.IsValid)
            {
                _context.Add(city);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Cities", new { countryId });
            }
            return RedirectToAction("Index", "Cities", new { countryId });
        }

        // GET: Cities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            ViewData["CountryIds"] = new SelectList(_context.Countries, "Id", "Name", city.CountryId);
            ViewBag.CountryId = city.CountryId;
            ViewBag.CountryName = _context.Countries.Where(c => c.Id == city.CountryId).FirstOrDefault().Name;
            return View(city);
        }

        // POST: Cities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CountryId,Name")] City city)
        {
            if (id != city.Id)
            {
                return NotFound();
            }
            var res = await _context.Cities.FirstOrDefaultAsync(c => c.Name == city.Name && c.CountryId == city.CountryId);
            if (res != null)
            {
                ModelState.AddModelError("", "Таке місто вже існує");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(city);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CityExists(city.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Cities", new { countryId = city.CountryId });
            }
            return RedirectToAction("Index", "Cities", new { countryId = city.CountryId });
        }

        // GET: Cities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .Include(c => c.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (city == null)
            {
                return NotFound();
            }
            ViewBag.CountryId = city.CountryId;
            ViewBag.CountryName = _context.Countries.Where(c => c.Id == city.CountryId).FirstOrDefault().Name;

            return View(city);
        }

        // POST: Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Cities", new { countryId = city.CountryId });
        }

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }
    }
}
