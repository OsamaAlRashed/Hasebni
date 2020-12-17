using System;
using System.Collections.Generic;
using System.Text;

namespace Hasebni.Security.Dto.User
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string DeviceToken { get; set; }

    }
}
