using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusesWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly DBBusesContext _context;
        public ChartController(DBBusesContext context)
        {
            _context = context;
        }

        [HttpGet("StationsByCities")]
        public JsonResult StationsByCities(int countryId)
        {
            var cities = _context.Cities.Where(c => c.CountryId == countryId).Include(c => c.Stations).ToList();
            List<object> result = new List<object>();
            result.Add(new[] { "Місто", "Кількість станцій"});
            foreach(var c in cities)
            {
                result.Add(new object[] { c.Name, c.Stations.Count() });
            }
            return new JsonResult(result);
        }

        [HttpGet("DriversAge")]
        public JsonResult DriversAge()
        {
            var drivers = _context.Drivers.ToList();
            List<object> result = new List<object>();
            result.Add(new[] { "Вік" });

            foreach (var d in drivers)
            {
                var age = (DateTime.Now - d.BirthDate).TotalDays / 365;
                result.Add(new object[] { (int)age });
            }
            return new JsonResult(result);
        }
    }
}
