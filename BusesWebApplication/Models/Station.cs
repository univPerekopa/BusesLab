using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusesWebApplication
{
    public partial class Station
    {
        public Station()
        {
            RoutesStations = new HashSet<RoutesStation>();
        }

        public int Id { get; set; }
        [Required]
        [Display(Name = "Місто")]
        public int CityId { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Назва станції")]
        public string Name { get; set; } = null!;

        [Display(Name = "Місто")]
        public virtual City City { get; set; } = null!;
        public virtual ICollection<RoutesStation> RoutesStations { get; set; }
    }
}
