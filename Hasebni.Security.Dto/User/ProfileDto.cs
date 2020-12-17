using System;
using System.Collections.Generic;
using System.Text;

namespace Hasebni.Security.Dto.User
{
    public class ProfileDto
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int Gender { get; set; }
        public int Avatar { get; set; }
    }
}
