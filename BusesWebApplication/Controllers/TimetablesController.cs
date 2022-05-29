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
    public class TimetablesController : Controller
    {
        private readonly DBBusesContext _context;

        public TimetablesController(DBBusesContext context)
        {
            _context = context;
        }

        // GET: Timetables
        public async Task<IActionResult> Index()
        {
            var dBBusesContext = _context.Timetables.Include(t => t.Bus).Include(t => t.Driver).Include(t => t.Route).Include(t => t.TripStatus);
            return View(await dBBusesContext.ToListAsync());
        }

        // GET: Timetables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timetable = await _context.Timetables
                .Include(t => t.Bus)
                .Include(t => t.Driver)
                .Include(t => t.Route)
                .Include(t => t.TripStatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timetable == null)
            {
                return NotFound();
            }

            return View(timetable);
        }

        // GET: Timetables/Create
        public async Task<IActionResult> Create()
        {
            ViewData["BusId"] = new SelectList(_context.Buses, "Id", "Id");
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FullName");
            ViewData["RouteId"] = new SelectList(_context.Routes, "Id", "Name");
            var planned = await _context.TripStatuses.FirstOrDefaultAsync(ts => ts.Name == "Заплановано");
            ViewData["StatusId"] = planned.Id;
            return View();
        }

        // POST: Timetables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RouteId,DriverId,BusId,ExpectedDeparture,ActualDeparture,ExpectedArrival,ActualArrival,TripStatusId")] Timetable timetable)
        {
            timetable.Bus = await _context.Buses.Include(b => b.Status).FirstOrDefaultAsync(x => x.Id == timetable.BusId);
            timetable.Driver = await _context.Drivers.Include(d => d.DriversCategories).FirstOrDefaultAsync(x => x.Id == timetable.DriverId);
            timetable.TripStatus = await _context.TripStatuses.FirstOrDefaultAsync(x => x.Id == timetable.TripStatusId);
            if (timetable.ExpectedArrival <= timetable.ExpectedDeparture)
            {
                ModelState.AddModelError("", "Прибуття має бути пізніше ніж відправлення");
            }
            if (timetable.Bus.Status.Name == "Потребує ремонту")
            {
                ModelState.AddModelError("", "Автобус потребує ремонту");
            }
            var categories = timetable.Driver.DriversCategories.Select(dc => dc.CategoryId);
            if (!categories.Contains(timetable.Bus.CategoryNeeded))
            {
                ModelState.AddModelError("", "Водій не має необхідної категорії");
            }

            if (ModelState.IsValid)
            {
                _context.Add(timetable);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BusId"] = new SelectList(_context.Buses, "Id", "Id", timetable.BusId);
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FullName", timetable.DriverId);
            ViewData["RouteId"] = new SelectList(_context.Routes, "Id", "Name", timetable.RouteId);
            ViewData["StatusId"] = _context.TripStatuses.FirstOrDefault(ts => ts.Name == "Заплановано").Id;
            return View(timetable);
        }

        // GET: Timetables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timetable = await _context.Timetables.FindAsync(id);
            if (timetable == null)
            {
                return NotFound();
            }
            ViewData["BusId"] = new SelectList(_context.Buses, "Id", "Id", timetable.BusId);
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FullName", timetable.DriverId);
            ViewData["RouteId"] = new SelectList(_context.Routes, "Id", "Name", timetable.RouteId);
            ViewData["TripStatusId"] = new SelectList(_context.TripStatuses, "Id", "Name", timetable.TripStatusId);
            return View(timetable);
        }

        // POST: Timetables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RouteId,DriverId,BusId,ExpectedDeparture,ActualDeparture,ExpectedArrival,ActualArrival,TripStatusId")] Timetable timetable)
        {
            if (id != timetable.Id)
            {
                return NotFound();
            }


            timetable.Bus = await _context.Buses.Include(b => b.Status).FirstOrDefaultAsync(x => x.Id == timetable.BusId);
            timetable.Driver = await _context.Drivers.Include(d => d.DriversCategories).FirstOrDefaultAsync(x => x.Id == timetable.DriverId);
            if (timetable.ExpectedArrival <= timetable.ExpectedDeparture)
            {
                ModelState.AddModelError("", "Прибуття має бути пізніше ніж відправлення");
            }
            if (timetable.ActualArrival != null && timetable.ActualDeparture != null && timetable.ActualArrival <= timetable.ActualDeparture)
            {
                ModelState.AddModelError("", "Прибуття має бути пізніше ніж відправлення");
            }
            if (timetable.Bus.Status.Name == "Потребує ремонту")
            {
                ModelState.AddModelError("", "Автобус потребує ремонту");
            }
            var categories = timetable.Driver.DriversCategories.Select(dc => dc.CategoryId);
            if (!categories.Contains(timetable.Bus.CategoryNeeded))
            {
                ModelState.AddModelError("", "Водій не має необхідної категорії");
            }

            if(timetable.ActualArrival != null && timetable.ActualDeparture != null)
            {
                timetable.TripStatusId = (await _context.TripStatuses.FirstOrDefaultAsync(ts => ts.Name == "Виконано")).Id;
            } 
            else if(timetable.ActualArrival == null && timetable.ActualDeparture != null)
            {
                timetable.TripStatusId = (await _context.TripStatuses.FirstOrDefaultAsync(ts => ts.Name == "В процесі")).Id;
            }
            else if (timetable.ActualArrival != null && timetable.ActualDeparture == null)
            {
                ModelState.AddModelError("", "Не можна встановити час прибуття без часу відбуття");
            }
            else
            {
                timetable.TripStatusId = (await _context.TripStatuses.FirstOrDefaultAsync(ts => ts.Name == "Заплановано")).Id;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timetable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimetableExists(timetable.Id))
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
            ViewData["BusId"] = new SelectList(_context.Buses, "Id", "Id", timetable.BusId);
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FullName", timetable.DriverId);
            ViewData["RouteId"] = new SelectList(_context.Routes, "Id", "Name", timetable.RouteId);
            ViewData["TripStatusId"] = new SelectList(_context.TripStatuses, "Id", "Name", timetable.TripStatusId);
            return View(timetable);
        }

        // GET: Timetables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timetable = await _context.Timetables
                .Include(t => t.Bus)
                .Include(t => t.Driver)
                .Include(t => t.Route)
                .Include(t => t.TripStatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timetable == null)
            {
                return NotFound();
            }

            return View(timetable);
        }

        // POST: Timetables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var timetable = await _context.Timetables.FindAsync(id);
            _context.Timetables.Remove(timetable);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TimetableExists(int id)
        {
            return _context.Timetables.Any(e => e.Id == id);
        }
    }
}
