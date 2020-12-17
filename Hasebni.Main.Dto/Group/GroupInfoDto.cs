using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Hasebni.Main.Dto
{
    public class GroupInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile GroupPhoto { get; set; }
    }
}
