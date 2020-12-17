using System;
using System.Collections.Generic;
using System.Text;

namespace Hasebni.Main.Dto.Purchase
{
    public class PurchaseInfoDto
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public int ItemId { get; set; }
        public string NewItemName { get; set; }
        public int GroupId { get; set; }
        public IEnumerable<int> MembersIds { get; set; }
        public int ByerId { get; set; }
    }
}
