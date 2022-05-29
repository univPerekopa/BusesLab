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
    public class BusModelsController : Controller
    {
        private readonly DBBusesContext _context;

        public BusModelsController(DBBusesContext context)
        {
            _context = context;
        }

        // GET: BusModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.BusModels.ToListAsync());
        }

        // GET: BusModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busModel = await _context.BusModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (busModel == null)
            {
                return NotFound();
            }

            return View(busModel);
        }

        // GET: BusModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BusModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] BusModel busModel)
        {
            var res = await _context.BusModels.FirstOrDefaultAsync(b => b.Name == busModel.Name);
            if (res != null)
            {
                ModelState.AddModelError("", "Така модель вже існує");
            }
            if (ModelState.IsValid)
            {
                _context.Add(busModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(busModel);
        }

        // GET: BusModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busModel = await _context.BusModels.FindAsync(id);
            if (busModel == null)
            {
                return NotFound();
            }
            return View(busModel);
        }

        // POST: BusModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] BusModel busModel)
        {
            if (id != busModel.Id)
            {
                return NotFound();
            }
            var res = await _context.BusModels.FirstOrDefaultAsync(b => b.Name == busModel.Name);
            if (res != null)
            {
                ModelState.AddModelError("", "Така модель вже існує");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(busModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusModelExists(busModel.Id))
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
            return View(busModel);
        }

        // GET: BusModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busModel = await _context.BusModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (busModel == null)
            {
                return NotFound();
            }

            return View(busModel);
        }

        // POST: BusModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var busModel = await _context.BusModels.FindAsync(id);
            _context.BusModels.Remove(busModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BusModelExists(int id)
        {
            return _context.BusModels.Any(e => e.Id == id);
        }
    }
}
