using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusesWebApplication
{
    public partial class Route
    {
        public Route()
        {
            RoutesStations = new HashSet<RoutesStation>();
            Timetables = new HashSet<Timetable>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Назва маршруту")]
        public string Name { get; set; } = null!;

        public virtual ICollection<RoutesStation> RoutesStations { get; set; }
        public virtual ICollection<Timetable> Timetables { get; set; }
    }
}
