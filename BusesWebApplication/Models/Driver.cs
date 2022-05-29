using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusesWebApplication
{
    public partial class Driver
    {
        public Driver()
        {
            DriversCategories = new HashSet<DriversCategory>();
            Timetables = new HashSet<Timetable>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Повне ім'я")]
        public string FullName { get; set; } = null!;
        [Required]
        [Display(Name = "Дата народження")]
        public DateTime BirthDate { get; set; }
        [Required]
        [Display(Name = "Зарплата")]
        public int Salary { get; set; }

        public virtual ICollection<DriversCategory> DriversCategories { get; set; }
        public virtual ICollection<Timetable> Timetables { get; set; }
    }
}
