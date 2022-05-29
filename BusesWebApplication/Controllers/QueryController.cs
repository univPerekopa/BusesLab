using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusesWebApplication.Controllers
{
    public class QueryController : Controller
    {
        private readonly DBBusesContext _context;

        public QueryController(DBBusesContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Stations"] = new SelectList(_context.Stations, "Id", "Name");
            ViewData["Buses"] = new SelectList(_context.Buses, "Id", "Id");
            ViewData["Statuses"] = new SelectList(_context.TripStatuses, "Id", "Name");
            ViewData["Drivers"] = new SelectList(_context.Drivers, "Id", "FullName");
            ViewData["BusStatuses"] = new SelectList(_context.BusStatuses, "Id", "Name");
            return View();
        }
    }
}
