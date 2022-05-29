using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusesWebApplication
{
    public partial class RoutesStation
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Станція")]
        public int StationId { get; set; }
        [Required]
        [Display(Name = "Маршрут")]
        public int RouteId { get; set; }
        [Required]
        [Display(Name = "Позиція в маршруті")]
        public int PositionInRoute { get; set; }

        [Display(Name = "Маршрут")]
        public virtual Route Route { get; set; } = null!;
        [Display(Name = "Станція")]
        public virtual Station Station { get; set; } = null!;
    }
}
