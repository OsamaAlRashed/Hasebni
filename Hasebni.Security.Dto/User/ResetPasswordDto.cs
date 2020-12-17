using System;
using System.Collections.Generic;
using System.Text;

namespace Hasebni.Security.Dto.User
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
