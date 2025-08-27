using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BL.Dtos.BaseDto;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BL.Dtos
{
    public class UserDto : BasDto 
    {
        [Required(ErrorMessage = "📧 Email is required")]
        [EmailAddress(ErrorMessage = "❌ Invalid email address format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "👤 First name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "First name can only contain letters")] // عربي + إنجليزي
        public string FirstName { get; set; }

        [Required(ErrorMessage = "👤 Last name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "Last name can only contain letters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "📞 Phone number is required")]
        [Phone(ErrorMessage = "❌ Invalid phone number")]
        [StringLength(11, MinimumLength = 6, ErrorMessage = "Phone number must be between 6 and 15 digits")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "🔑 Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [MaxLength(50, ErrorMessage = "Password cannot exceed 50 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "🔑 Confirm Password is required")]
        [Compare("Password", ErrorMessage = "❌ Passwords do not match")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

     //   [StringLength(20, ErrorMessage = "Role cannot exceed 20 characters")]
        public string? Role { get; set; }

        public string? ReturnUrl { get; set; }




    }

}
