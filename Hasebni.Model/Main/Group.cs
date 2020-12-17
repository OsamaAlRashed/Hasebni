using Hasebni.Model.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hasebni.Model.Main
{
    public class Group : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public ICollection<Member> Members { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}
