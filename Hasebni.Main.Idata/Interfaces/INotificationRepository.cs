using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hasebni.Main.Idata.Interfaces
{
    public interface INotificationRepository
    {
        Task<bool> SendMulticastAsync(List<string> registrationTokens, string BuyerName, string ItemName, string GroupName, int quantity);
      //  Task<> GetParam(IEnumerable<int> userSubscriberIds, int itemId, int BuyerId, int GroupId , int purchaseId);
        
    }
}
