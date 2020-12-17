using System;
using System.Collections.Generic;
using System.Text;

namespace Hasebni.Main.Dto.Member
{
    public class ChangeAdminDto
    {
        public int GroupId { get; set; }
        public int AdminId { get; set; }
        public string NewAdmin { get; set; }
    }
}
