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
    public class BusesController : Controller
    {
        private readonly DBBusesContext _context;

        public BusesController(DBBusesContext context)
        {
            _context = context;
        }

        // GET: Buses
        public async Task<IActionResult> Index()
        {
            var dBBusesContext = _context.Buses.Include(b => b.CategoryNeededNavigation).Include(b => b.Model).Include(b => b.Status);
            return View(await dBBusesContext.ToListAsync());
        }

        // GET: Buses/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bus = await _context.Buses
                .Include(b => b.CategoryNeededNavigation)
                .Include(b => b.Model)
                .Include(b => b.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bus == null)
            {
                return NotFound();
            }

            return View(bus);
        }

        // GET: Buses/Create
        public IActionResult Create()
        {
            ViewData["CategoryNeeded"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["ModelId"] = new SelectList(_context.BusModels, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_context.BusStatuses, "Id", "Name");
            return View();
        }

        // POST: Buses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ModelId,Capacity,CategoryNeeded,StatusId")] Bus bus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryNeeded"] = new SelectList(_context.Categories, "Id", "Name", bus.CategoryNeeded);
            ViewData["ModelId"] = new SelectList(_context.BusModels, "Id", "Name", bus.ModelId);
            ViewData["StatusId"] = new SelectList(_context.BusStatuses, "Id", "Name", bus.StatusId);
            return View(bus);
        }

        // GET: Buses/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bus = await _context.Buses.FindAsync(id);
            if (bus == null)
            {
                return NotFound();
            }
            ViewData["CategoryNeeded"] = new SelectList(_context.Categories, "Id", "Name", bus.CategoryNeeded);
            ViewData["ModelId"] = new SelectList(_context.BusModels, "Id", "Name", bus.ModelId);
            ViewData["StatusId"] = new SelectList(_context.BusStatuses, "Id", "Name", bus.StatusId);
            return View(bus);
        }

        // POST: Buses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,ModelId,Capacity,CategoryNeeded,StatusId")] Bus bus)
        {
            if (id != bus.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusExists(bus.Id))
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
            ViewData["CategoryNeeded"] = new SelectList(_context.Categories, "Id", "Name", bus.CategoryNeeded);
            ViewData["ModelId"] = new SelectList(_context.BusModels, "Id", "Name", bus.ModelId);
            ViewData["StatusId"] = new SelectList(_context.BusStatuses, "Id", "Name", bus.StatusId);
            return View(bus);
        }

        // GET: Buses/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bus = await _context.Buses
                .Include(b => b.CategoryNeededNavigation)
                .Include(b => b.Model)
                .Include(b => b.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bus == null)
            {
                return NotFound();
            }

            return View(bus);
        }

        // POST: Buses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var bus = await _context.Buses.FindAsync(id);
            _context.Buses.Remove(bus);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BusExists(string id)
        {
            return _context.Buses.Any(e => e.Id == id);
        }
    }
}
