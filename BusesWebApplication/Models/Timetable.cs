using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusesWebApplication
{
    public partial class Timetable
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Маршрут")]
        public int RouteId { get; set; }
        [Required]
        [Display(Name = "Водій")]
        public int DriverId { get; set; }
        [Required]
        [Display(Name = "Автобус")]
        public string BusId { get; set; } = null!;
        [Required]
        [Display(Name = "Планове відбуття")]
        public DateTime ExpectedDeparture { get; set; }
        [Display(Name = "Відбуття")]
        public DateTime? ActualDeparture { get; set; }
        [Required]
        [Display(Name = "Планове прибуття")]
        public DateTime ExpectedArrival { get; set; }
        [Display(Name = "Прибуття")]
        public DateTime? ActualArrival { get; set; }
        [Required]
        [Display(Name = "Стан")]
        public int TripStatusId { get; set; }

        [Display(Name = "Автобус")]
        public virtual Bus Bus { get; set; } = null!;
        [Display(Name = "Водій")]
        public virtual Driver Driver { get; set; } = null!;
        [Display(Name = "Маршрут")]
        public virtual Route Route { get; set; } = null!;
        [Display(Name = "Стан")]
        public virtual TripStatus TripStatus { get; set; } = null!;
    }
}
