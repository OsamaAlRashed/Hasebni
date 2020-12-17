using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Globalization;
using System.Text;

namespace Hasebni.Main.Dto.Notification
{
    public class NotificationInfoDto
    {
        public int NotificationId { get; set; }
        public string BuyerName { get; set; }
        public string ItemName { get; set; }
        public int AmountAdded { get; set; }
        public string GroupName { get; set; }
        //public string DeviceToken { get; set; }
    }
}
