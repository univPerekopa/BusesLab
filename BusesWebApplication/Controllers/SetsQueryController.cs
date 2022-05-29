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
    public class SetsQueryController : ControllerBase
    {
        private readonly DBBusesContext _context;

        public SetsQueryController(DBBusesContext context)
        {
            _context = context;
        }

        [HttpGet("1/{excludeStationId}")]
        public ActionResult<List<string>> Query1(int excludeStationId)
        {
            var routes = new List<string>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    SELECT Routes.Name
                    FROM Routes
                    WHERE NOT EXISTS (
                        SELECT Stations.Id FROM Stations
                        WHERE Stations.Id <> @ExcludeStationId
                        AND NOT EXISTS (
                            SELECT RoutesStations.Id
                            FROM RoutesStations
                            WHERE RoutesStations.RouteId = Routes.Id AND RoutesStations.StationId = Stations.Id
                        )
                    )
                ";
                var param = command.CreateParameter();
                param.ParameterName = "@ExcludeStationId";
                param.Value = excludeStationId;
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

        [HttpGet("2/{driverId}")]
        public ActionResult<List<string>> Query2(int driverId)
        {
            var drivers = new List<string>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    SELECT Drivers.FullName
                    FROM Drivers
                    WHERE NOT EXISTS (
                        SELECT Categories.Id FROM Categories
                        WHERE 
                            EXISTS (
                                SELECT DriversCategories.Id
                                FROM DriversCategories
                                WHERE DriversCategories.DriverId = @DriverId AND DriversCategories.CategoryId = Categories.Id
                            )
                        AND
                            NOT EXISTS (
                                SELECT DriversCategories.Id
                                FROM DriversCategories
                                WHERE DriversCategories.DriverId = Drivers.Id AND DriversCategories.CategoryId = Categories.Id
                            )
                    )
                ";
                var param = command.CreateParameter();
                param.ParameterName = "@DriverId";
                param.Value = driverId;
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

        [HttpGet("3/{statusId}")]
        public ActionResult<List<string>> Query3(int statusId)
        {
            var drivers = new List<string>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    SELECT Drivers.FullName
                    FROM Drivers
                    WHERE NOT EXISTS (
                        SELECT Buses.Id FROM Buses
                        WHERE Buses.StatusId = @StatusId
                        AND NOT EXISTS (
                            SELECT DriversCategories.Id FROM DriversCategories
                            WHERE DriversCategories.DriverId = Drivers.Id AND DriversCategories.CategoryId = Buses.CategoryNeeded
                        )
                    )
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
    }
}
