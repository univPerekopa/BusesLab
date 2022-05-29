using Microsoft.AspNetCore.Identity;

namespace BusesWebApplication
{
    public class User : IdentityUser
    {
        public int Year { get; set; }
    }
}