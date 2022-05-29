using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusesWebApplication
{
    public partial class BusModel
    {
        public BusModel()
        {
            Buses = new HashSet<Bus>();
        }
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Назва моделі")]
        public string Name { get; set; } = null!;

        public virtual ICollection<Bus> Buses { get; set; }
    }
}
