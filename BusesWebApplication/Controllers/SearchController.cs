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
    public class SearchController : Controller
    {
        private readonly DBBusesContext _context;

        public SearchController(DBBusesContext context)
        {
            _context = context;
        }

        // GET: Search
        public async Task<IActionResult> Index()
        {
            ViewData["Stations"] = new SelectList(await _context.Stations.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int stationFrom, int stationTo)
        {
            if(stationFrom == stationTo)
            {
                ModelState.AddModelError("", "Оберіть різні станції");
            }
            if(ModelState.IsValid)
            {
                var allRoutes = await _context.Routes.Include(r => r.RoutesStations).ToListAsync();
                var filteredRoutes = new List<int>();
                foreach (var route in allRoutes)
                {
                    bool wasFrom = false;
                    foreach (var rs in route.RoutesStations)
                    {
                        if (rs.StationId == stationFrom)
                        {
                            wasFrom = true;
                        }
                        else if (rs.StationId == stationTo && wasFrom)
                        {
                            filteredRoutes.Add(rs.RouteId);
                            break;
                        }
                    }
                }
                var timetable = _context.Timetables.Include(t => t.Bus).Include(t => t.Driver).Include(t => t.Route).Include(t => t.TripStatus)
                    .Where(t => filteredRoutes.Contains(t.RouteId));
                return View("../Timetables/Index", timetable);
            }
            ViewData["Stations"] = new SelectList(await _context.Stations.ToListAsync(), "Id", "Name");
            return View();
        }
    }
}
