using Hasebni.Model.Base;
using Hasebni.Model.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hasebni.Model.Setting
{
    public class Notification:EntityBase
    {

        public int FromMemberId { get; set; }
        public int ToMemberId { get; set; }
        
        [ForeignKey(nameof(FromMemberId))]
        public Member FromMember  { get; set; }

        [ForeignKey(nameof(ToMemberId))]
        public Member ToMember { get; set; }

        public int PurchaseFK { get; set; }
        
        [ForeignKey(nameof(PurchaseFK))]
        public Purchase Purchase { get; set; }

        public int AmountAdded { get; set; }
        public int State { get; set; } //0 غير مقروء
                                       //1 مقيول
                                       //2 مرفوض

    }
}
