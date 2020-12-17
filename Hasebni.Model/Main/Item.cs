using Hasebni.Model.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hasebni.Model.Main
{
    public class Item : EntityBase
    {
        public string Name { get; set; }
        public ICollection<Purchase> Purchases { get; set; }

        [ForeignKey(nameof(GroupFk))]
        public Group Group { get; set; }
        public int GroupFk { get; set; }
    }
}
