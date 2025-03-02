using System.ComponentModel.DataAnnotations;

namespace TaskScheduler.DTOs
{
    public class VerifyEmailDTO
    {
        [Required]
        [EmailAddress]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string Email { get; set; }
    }
}
