using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusesWebApplication
{
    public partial class Category
    {
        public Category()
        {
            DriversCategories = new HashSet<DriversCategory>();
            Buses = new HashSet<Bus>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Категорія")]
        public string Name { get; set; } = null!;

        public virtual ICollection<DriversCategory> DriversCategories { get; set; }
        public virtual ICollection<Bus> Buses { get; set; }
    }
}
