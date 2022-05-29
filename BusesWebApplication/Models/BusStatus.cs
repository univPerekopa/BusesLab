using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusesWebApplication
{
    public partial class BusStatus
    {
        public BusStatus()
        {
            Buses = new HashSet<Bus>();
        }
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Стан")]
        public string Name { get; set; } = null!;

        public virtual ICollection<Bus> Buses { get; set; }
    }
}
