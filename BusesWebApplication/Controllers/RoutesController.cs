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
    public class RoutesController : Controller
    {
        private readonly DBBusesContext _context;

        public RoutesController(DBBusesContext context)
        {
            _context = context;
        }

        // GET: Routes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Routes.Include(r => r.RoutesStations).ThenInclude(x => x.Station).ToListAsync());
        }

        // GET: Routes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var route = await _context.Routes.Include(r => r.RoutesStations).ThenInclude(rs => rs.Station)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (route == null)
            {
                return NotFound();
            }

            return View(route);
        }

        // GET: Routes/Create
        public IActionResult Create()
        {
            ViewData["Stations"] = new SelectList(_context.Stations, "Id", "Name");
            return View();
        }

        // POST: Routes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Route route, List<int> stationIds)
        {
            var res = await _context.Routes.FirstOrDefaultAsync(r => r.Name == route.Name);
            if (res != null)
            {
                ModelState.AddModelError("", "Маршрут з таким ім'ям вже існує");
            }
            if (stationIds.Distinct().Count() != stationIds.Count)
            {
                ModelState.AddModelError("", "Всі станції мають бути різними");
            }

            if (ModelState.IsValid)
            {
                _context.Add(route);
                await _context.SaveChangesAsync();
                int routeId = _context.Routes.OrderBy(r => r.Id).LastOrDefault(r => r.Name == route.Name).Id;

                List<RoutesStation> stations = new();
                int pos = 1;
                foreach (int stationId in stationIds)
                {
                    stations.Add(new RoutesStation { StationId = stationId, RouteId = routeId, PositionInRoute = pos });
                    pos += 1;
                }

                _context.AddRange(stations);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Stations"] = new SelectList(_context.Stations, "Id", "Name");
            return View(route);
        }

        // GET: Routes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var route = await _context.Routes.Include(r => r.RoutesStations).ThenInclude(rs => rs.Station).FirstOrDefaultAsync(r => r.Id == id);
            if (route == null)
            {
                return NotFound();
            }
            var list = new List<SelectList>();
            foreach(var rs in route.RoutesStations)
            {
                list.Add(new SelectList(_context.Stations, "Id", "Name", rs.StationId));
            }
            ViewData["Stations"] = list;
            return View(route);
        }

        // POST: Routes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Route route, List<int> stationIds)
        {
            if (id != route.Id)
            {
                return NotFound();
            }
            var res = await _context.Routes.FirstOrDefaultAsync(r => r.Name == route.Name);
            if (res != null)
            {
                ModelState.AddModelError("", "Маршрут з таким ім'ям вже існує");
            }
            if (stationIds.Distinct().Count() != stationIds.Count)
            {
                ModelState.AddModelError("", "Всі станції мають бути різними");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(route);
                    _context.RemoveRange(await _context.RoutesStations.Where(rs => rs.RouteId == id).ToListAsync());

                    List<RoutesStation> stations = new();
                    int pos = 1;
                    foreach (int stationId in stationIds)
                    {
                        stations.Add(new RoutesStation { StationId = stationId, RouteId = id, PositionInRoute = pos });
                        pos += 1;
                    }

                    _context.AddRange(stations);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RouteExists(route.Id))
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

            var oldRoute = await _context.Routes.Include(r => r.RoutesStations).ThenInclude(rs => rs.Station).FirstOrDefaultAsync(r => r.Id == id);
            var list = new List<SelectList>();
            foreach (var rs in oldRoute.RoutesStations)
            {
                list.Add(new SelectList(_context.Stations, "Id", "Name", rs.StationId));
            }
            ViewData["Stations"] = list;
            return View(route);
        }

        // GET: Routes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var route = await _context.Routes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (route == null)
            {
                return NotFound();
            }

            return View(route);
        }

        // POST: Routes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var route = await _context.Routes.FindAsync(id);
            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RouteExists(int id)
        {
            return _context.Routes.Any(e => e.Id == id);
        }
    }
}
