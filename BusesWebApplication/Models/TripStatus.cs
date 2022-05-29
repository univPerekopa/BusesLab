using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusesWebApplication
{
    public partial class TripStatus
    {
        public TripStatus()
        {
            Timetables = new HashSet<Timetable>();
        }

        public int Id { get; set; }
        [Required]
        [Display(Name = "Статус")]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Timetable> Timetables { get; set; }
    }
}
