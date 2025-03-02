using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TaskScheduler.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public bool IsEmailVerified { get; set; }
        public string Phone {  get; set; }
        public bool IsPhoneVerified { get; set; }
    }
}
