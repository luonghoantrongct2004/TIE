using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TIE_Decor.Models
{
    public class ProfileViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Fullname not null")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email not null"), EmailAddress(ErrorMessage = "Email not valid")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be 8 characters")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password is not match")]
        public string ConfirmPassword { get; set; }
        public string Phone { get; set; }

        [Display(Name = "Profile Image")]
        public string? ImageUrl { get; set; }
    }
}
