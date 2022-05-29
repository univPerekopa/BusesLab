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
    public class StationsController : Controller
    {
        private readonly DBBusesContext _context;

        public StationsController(DBBusesContext context)
        {
            _context = context;
        }

        // GET: Stations
        public async Task<IActionResult> Index(int? cityId)
        {
            if (cityId == null) return RedirectToAction("Index", "Countries");

            City city = _context.Cities.Where(c => c.Id == cityId).FirstOrDefault();
            ViewBag.cityId = city.Id;
            ViewBag.CityName = city.Name;
            ViewBag.CountryId = city.CountryId;

            var stationsByCity = _context.Stations.Where(s => s.CityId == cityId).Include(s => s.City);
            return View(await stationsByCity.ToListAsync());
        }

        // GET: Stations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var station = await _context.Stations
                .Include(s => s.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (station == null)
            {
                return NotFound();
            }

            ViewBag.CityId = station.CityId;
            ViewBag.CityName = _context.Cities.Where(c => c.Id == station.CityId).FirstOrDefault().Name;

            return View(station);
        }

        // GET: Stations/Create
        public IActionResult Create(int cityId)
        {
            ViewBag.CityId = cityId;
            ViewBag.CityName = _context.Cities.Where(c => c.Id == cityId).FirstOrDefault().Name;
            return View();
        }

        // POST: Stations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int cityId, [Bind("Id,CityId,Name")] Station station)
        {
            if (ModelState.IsValid)
            {
                _context.Add(station);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Stations", new { cityId });
            }
            return RedirectToAction("Index", "Stations", new { cityId });
        }

        // GET: Stations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var station = await _context.Stations.FindAsync(id);
            if (station == null)
            {
                return NotFound();
            }
            ViewData["CityIds"] = new SelectList(_context.Cities, "Id", "Name", station.CityId);
            ViewBag.CityId = station.CityId;
            ViewBag.CityName = _context.Cities.Where(c => c.Id == station.CityId).FirstOrDefault().Name;
            return View(station);
        }

        // POST: Stations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CityId,Name")] Station station)
        {
            if (id != station.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(station);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StationExists(station.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Stations", new { cityId = station.CityId });
            }
            return RedirectToAction("Index", "Stations", new { cityId = station.CityId });
        }

        // GET: Stations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var station = await _context.Stations
                .Include(s => s.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (station == null)
            {
                return NotFound();
            }
            ViewBag.CityId = station.CityId;
            ViewBag.CityName = _context.Cities.Where(c => c.Id == station.CityId).FirstOrDefault().Name;

            return View(station);
        }

        // POST: Stations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var station = await _context.Stations.FindAsync(id);
            _context.Stations.Remove(station);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Stations", new { cityId = station.CityId });
        }

        private bool StationExists(int id)
        {
            return _context.Stations.Any(e => e.Id == id);
        }
    }
}
