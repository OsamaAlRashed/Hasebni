using System;
using System.Collections.Generic;
using System.Text;

namespace Hasebni.Security.Dto.User
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        //public string Username { get; set; }
        public string LastName { get; set; }
        public int Gender { get; set; }
        public int Avatar { get; set; }
        public string BirthDate { get; set; }
    }
}
