using Hasebni.Model.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hasebni.Model.Main
{
    public class Subscriber:EntityBase
    {
        public bool IsSubscriber { get; set; }
        public bool IsBuyer { get; set; }
        public int MemberFk { get; set; }

        [ForeignKey(nameof(MemberFk))]
        public Member Member { get; set; }
        public int PurchaseFk { get; set; }

        [ForeignKey(nameof(PurchaseFk))]
        public Purchase Purchase { get; set; }
    }
}
