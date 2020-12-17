using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Hasebni.Main.Dto.Member
{
    public class MemberInfoDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public long Balance { get; set; }
        public bool IsAdmin { get; set; }
    }
}
