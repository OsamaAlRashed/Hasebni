using System;
using System.Collections.Generic;
using System.Text;

namespace Hasebni.Security.Dto.User
{
    public class ConfirmEmailDto
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}
