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
    [Route("api/[controller]")]
    [ApiController]
    public class SimpleQueryController : ControllerBase
    {
        private readonly DBBusesContext _context;

        public SimpleQueryController(DBBusesContext context)
        {
            _context = context;
        }

        [HttpGet("1/{minStations}")]
        public ActionResult<List<string>> Query1(int minStations)
        {
            var countries = new List<string>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    SELECT Countries.Name
                    FROM Stations
                    INNER JOIN Cities ON Cities.Id = Stations.CityId
                    INNER JOIN Countries ON Countries.Id = Cities.CountryId
                    GROUP BY Countries.Name
                    HAVING COUNT(*) >= @MinStations;
                ";
                var param = command.CreateParameter();
                param.ParameterName = "@MinStations";
                param.Value = minStations;
                command.Parameters.Add(param);

                _context.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        countries.Add(result.GetFieldValue<string>(0));
                    }
                }
            }
            return countries;
        }

        [HttpGet("2/{stationId}")]
        public ActionResult<List<string>> Query2(int stationId)
        {
            var routes = new List<string>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    SELECT Routes.Name
                    FROM RoutesStations
                    INNER JOIN Routes ON Routes.Id = RoutesStations.RouteId
                    INNER JOIN Stations ON Stations.Id = RoutesStations.StationId
                    WHERE Stations.Id = @StationId
                ";
                var param = command.CreateParameter();
                param.ParameterName = "@StationId";
                param.Value = stationId;
                command.Parameters.Add(param);

                _context.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        routes.Add(result.GetFieldValue<string>(0));
                    }
                }
            }
            return routes;
        }

        [HttpGet("3/{busId}")]
        public ActionResult<List<string>> Query3(string busId)
        {
            var drivers = new List<string>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    SELECT Drivers.FullName
                    FROM DriversCategories
                    INNER JOIN Drivers ON Drivers.Id = DriversCategories.DriverId
                    WHERE DriversCategories.CategoryId = (SELECT categoryNeeded FROM Buses Where id = @BusId)
                ";
                var param = command.CreateParameter();
                param.ParameterName = "@BusId";
                param.Value = busId;
                command.Parameters.Add(param);

                _context.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        drivers.Add(result.GetFieldValue<string>(0));
                    }
                }
            }
            return drivers;
        }

        [HttpGet("4/{statusId}")]
        public ActionResult<List<string>> Query4(int statusId)
        {
            var drivers = new List<string>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    SELECT DISTINCT Drivers.FullName
                    FROM Timetable
                    INNER JOIN Drivers ON Drivers.Id = Timetable.DriverId
                    WHERE Timetable.TripStatusId = @StatusId
                ";
                var param = command.CreateParameter();
                param.ParameterName = "@StatusId";
                param.Value = statusId;
                command.Parameters.Add(param);

                _context.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        drivers.Add(result.GetFieldValue<string>(0));
                    }
                }
            }
            return drivers;
        }

        [HttpGet("5/{numberOfCities}")]
        public ActionResult<List<string>> Query5(int numberOfCities)
        {
            var countries = new List<string>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    SELECT Countries.Name
                    FROM Cities
                    INNER JOIN Countries ON Countries.Id = Cities.CountryId
                    GROUP BY Countries.Name
                    HAVING COUNT(*) = @NumberOfCities;
                ";
                var param = command.CreateParameter();
                param.ParameterName = "@NumberOfCities";
                param.Value = numberOfCities;
                command.Parameters.Add(param);

                _context.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        countries.Add(result.GetFieldValue<string>(0));
                    }
                }
            }
            return countries;
        }
    }
}
