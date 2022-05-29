using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusesWebApplication
{
    public partial class Bus
    {
        public Bus()
        {
            Timetables = new HashSet<Timetable>();
        }

        [Required]
        [StringLength(20)]
        public string Id { get; set; } = null!;
        [Required]
        [Display(Name = "Модель")]
        public int ModelId { get; set; }
        [Required]
        [Display(Name = "Кількість сидінь")]
        public int Capacity { get; set; }
        [Required]
        [Display(Name = "Категорія для керування")]
        public int CategoryNeeded { get; set; }
        [Required]
        [Display(Name = "Стан")]
        public int StatusId { get; set; }
        [Display(Name = "Категорія для керування")]
        public virtual Category CategoryNeededNavigation { get; set; } = null!;
        [Display(Name = "Модель")]
        public virtual BusModel Model { get; set; } = null!;
        [Display(Name = "Стан")]
        public virtual BusStatus Status { get; set; } = null!;
        public virtual ICollection<Timetable> Timetables { get; set; }
    }
}
