using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hasebni.Main.Dto
{
    public class CreateGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile GroupPhoto { get; set; }
        public int CreaterId { get; set; }
        public IEnumerable<string> Members { get; set; }
    }
}
