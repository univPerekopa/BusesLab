#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusesWebApplication;
using ClosedXML.Excel;
using System.Data.SqlClient;
using System.Data;

namespace BusesWebApplication.Controllers
{
    public class CountriesController : Controller
    {
        private readonly DBBusesContext _context;

        public CountriesController(DBBusesContext context)
        {
            _context = context;
        }

        // GET: Countries
        public async Task<IActionResult> Index()
        {
            return View(await _context.Countries.ToListAsync());
        }

        // GET: Countries/Query1
        public IActionResult Query1()
        {
            return View();
        }

        // GET: Countries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Cities", new { countryId = country.Id });
        }

        // GET: Countries/Create`
        public IActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Country country)
        {
            var res = await _context.Countries.FirstOrDefaultAsync(c => c.Name == country.Name);
            if (res != null)
            {
                ModelState.AddModelError("", "Така країна вже існує");
            }
            if (ModelState.IsValid)
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: Countries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Country country)
        {
            if (id != country.Id)
            {
                return NotFound();
            }
            var res = await _context.Countries.FirstOrDefaultAsync(c => c.Name == country.Name);
            if (res != null)
            {
                ModelState.AddModelError("", "Така країна вже існує");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.Id))
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
            return View(country);
        }

        // GET: Countries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            try
            {
                if (fileExcel != null)
                {
                    using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                    {
                        await fileExcel.CopyToAsync(stream);
                        using (XLWorkbook workBook = new XLWorkbook(stream, XLEventTracking.Disabled))
                        {
                            foreach (IXLWorksheet worksheet in workBook.Worksheets)
                            {
                                var country = await CreateOrGetCountry(worksheet.Name);
                                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                {
                                    var city = await CreateOrGetCity(country, row.Cell(1).Value.ToString());
                                    int cellPos = 2;
                                    while (row.Cell(cellPos).Value.ToString() != "")
                                    {
                                        await CreateStationIfNotExists(country, city, row.Cell(cellPos).Value.ToString());
                                        cellPos += 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Неправильний формат файлу");
            }
            if (ModelState.IsValid)
            {
                await _context.SaveChangesAsync();
            }
            return View("./Index", await _context.Countries.ToListAsync());
        }

        public ActionResult Export()
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var countries = _context.Countries.Include(c => c.Cities).ThenInclude(c => c.Stations).ToList();
                foreach (var country in countries)
                {
                    var worksheet = workbook.Worksheets.Add(country.Name);
                    worksheet.Cell("A1").Value = "Назва міста";
                    worksheet.Row(1).Style.Font.Bold = true;
                    var cities = country.Cities.ToList();
                    int maxNumberOfStations = 0;
                    for (int i = 0; i < cities.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = cities[i].Name;
                        var stations = cities[i].Stations.ToList();
                        for (int j = 0; j < stations.Count; j++)
                        {
                            worksheet.Cell(i + 2, j + 2).Value = stations[j].Name;
                        }
                        if(maxNumberOfStations < stations.Count)
                        {
                            maxNumberOfStations = stations.Count;
                        }
                    }
                    for(int i = 0; i < maxNumberOfStations; i++)
                    {
                        worksheet.Cell(1, i + 2).Value = "Станція" + (i + 1).ToString();
                    }
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();
                    return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") 
                    {
                        FileDownloadName = $"stations_{ DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }

        private async Task<Country> CreateOrGetCountry(string countryName)
        {
            Country country = await _context.Countries.FirstOrDefaultAsync(c => c.Name == countryName);
            if (country == null)
            {
                country = new Country { Name = countryName };
                _context.Countries.Add(country);
            }
            return country;
        }

        private async Task<City> CreateOrGetCity(Country country, string cityName)
        {
            City city = await _context.Cities.Include(c => c.Country).FirstOrDefaultAsync(c => c.Name == cityName && c.Country.Name == country.Name);
            if (city == null)
            {
                city = new City
                {
                    Name = cityName,
                    Country = country
                };
                _context.Cities.Add(city);
            }
            return city;
        }

        private async Task CreateStationIfNotExists(Country country, City city, string stationName)
        {
            Station station = await _context.Stations.Include(s => s.City).ThenInclude(c => c.Country)
                .FirstOrDefaultAsync(s => s.Name == stationName && s.City.Name == city.Name && s.City.Country.Name == country.Name);
            if (station == null)
            {
                station = new Station
                {
                    Name = stationName,
                    City = city
                };
                _context.Stations.Add(station);
            }
        }
    }
}
