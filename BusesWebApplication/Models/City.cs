using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusesWebApplication
{
    public partial class City
    {
        public City()
        {
            Stations = new HashSet<Station>();
        }
        public int Id { get; set; }
        [Required]
        [Display(Name = "Країна")]
        public int CountryId { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Назва міста")]
        public string Name { get; set; } = null!;
        [Display(Name = "Країна")]
        public virtual Country Country { get; set; } = null!;
        public virtual ICollection<Station> Stations { get; set; }
    }
}
