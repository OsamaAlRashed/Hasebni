using System;
using System.Collections.Generic;
using System.Text;

namespace Hasebni.Security.Dto.User
{
    public class ChangePasswordDto
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }
}
