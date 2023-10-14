// Join table between appusers and roles
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    // Represents the linking table between roles and users
    public class AppUserRole : IdentityUserRole<int> // int specifies data type of identifier
    {
        public AppUser User { get; set; }

        public AppRole Role { get; set; }
    }
}