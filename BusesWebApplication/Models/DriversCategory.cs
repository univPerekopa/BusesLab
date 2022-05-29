using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusesWebApplication
{
    public partial class DriversCategory
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Водій")]
        public int DriverId { get; set; }
        [Required]
        [Display(Name = "Категорія")]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual Driver Driver { get; set; } = null!;
    }
}
