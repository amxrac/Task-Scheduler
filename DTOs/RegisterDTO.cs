﻿using System.ComponentModel.DataAnnotations;

namespace TaskScheduler.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don't match.")]
        public string ConfirmPassword { get; set; }
    }
}
