using Hasebni.Model.Base;
using Hasebni.Model.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hasebni.Model.Main
{
    public class Purchase:EntityBase
    {
        public DateTimeOffset DatePurchase { get; set; }
        public int Price { get; set; }
        public ICollection<Subscriber> Subscribers { get; set; }
        public ICollection<Notification> Notifications { get; set; }

        [ForeignKey(nameof(ItemFk))]
        public Item Item { get; set; }
        public int ItemFk { get; set; }
    }
}
