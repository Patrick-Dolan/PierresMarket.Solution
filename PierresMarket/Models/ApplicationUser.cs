using Microsoft.AspNetCore.Identity;

namespace PierresMarket.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}