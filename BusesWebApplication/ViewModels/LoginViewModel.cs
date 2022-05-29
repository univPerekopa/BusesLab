using System.ComponentModel.DataAnnotations;

namespace BusesWebApplication.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Запам'ятати?")]
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}
