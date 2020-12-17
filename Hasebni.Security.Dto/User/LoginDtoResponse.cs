using System;
using System.Collections.Generic;
using System.Text;

namespace Hasebni.Security.Dto.User
{
    public class LoginDtoResponse
    {
        public UserInfoDtoResponse UserData { get; set; }
        public string Token { get; set; }
    }
}
