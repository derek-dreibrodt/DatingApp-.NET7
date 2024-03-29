using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using API.Extensions;
using Microsoft.AspNetCore.Identity;
using API.DTOs;

namespace API.Entities
{
    public class AppUser : IdentityUser<int>
    {

        public DateOnly DateOfBirth { get; set; }

        public string KnownAs { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        
        public string Gender { get; set; }

        public string Introduction { get; set; }

        public string LookingFor { get; set; }

        public string Interests { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public List<Photo> Photos { get; set; } = new(); // creates new list of photos on creation if null

        public List<UserLike> LikedByUsers { get; set; }
        public List<UserLike> LikedUsers { get; set; }

        public List<Message> MessagesSent { get; set; }
        public List<Message> MessagesReceived { get; set; }

        public ICollection<AppUserRole> UserRoles { get; set; } // Allows user to have multiple roles

        // public int GetAge()
        // {
        //     return DateOfBirth.CalculateAge();
        // }
    }
}