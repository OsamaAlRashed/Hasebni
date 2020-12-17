using System;
using System.Collections.Generic;
using System.Text;

namespace Hasebni.Security.Dto.User
{
    public class EmailConfirmationDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
