using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hasebni.API.ViewModels
{
    public class ResetPasswordViewModel
    { 
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [StringLength(50,MinimumLength = 5)]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword))]
        [StringLength(50, MinimumLength = 5)]
        public string ConfirmPassword { get; set; }
    }
}
