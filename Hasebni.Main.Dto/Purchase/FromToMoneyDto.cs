using System;
using System.Collections.Generic;
using System.Text;

namespace Hasebni.Main.Dto.Purchase
{
    public class FromToMoneyDto
    {
        public string FromUser { get; set; }
        public string ToUser { get; set; }
        public long Quantity { get; set; }
    }
}
