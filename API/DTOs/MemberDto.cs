using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class MemberDto
    {
        // Don't include the password in the Dto
        // Add age and main profile photo url
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PhotoUrl { get; set; }
        public int Age { get; set; }
        
        public int AgeOlder {
            get{
                return Age + 1;
            }
        }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public List<PhotoDto> Photos { get; set; } = new(); // creates new list of photos on creation if null
    }
}