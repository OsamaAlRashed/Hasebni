using System;
using System.Collections.Generic;
using System.Text;

namespace Hasebni.Main.Dto.Notification
{
    public class SendNotificationDto
    {
        public string BuyerName { get; set; }
        public string ItemName { get; set; }
        //public int AmountAdded { get; set; }
        public string GroupName { get; set; }
        public List<string> DeviceTokens { get; set; } = new List<string>();
    }
}
