using FirebaseAdmin.Messaging;
using Hasebni.Base;
using Hasebni.Main.Dto.Notification;
using Hasebni.Main.Idata.Interfaces;
using Hasebni.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hasebni.Main.Data.Repository
{
    public class NotificationRepository:HasebniRepository , INotificationRepository
    {
       // HttpClient tRequest;
        public NotificationRepository(HasebniDbContext context):base(context)
        {

        }

        public async Task<bool> SendMulticastAsync(List<string> registrationTokens, string BuyerName, string ItemName, string GroupName, int quantity)
        {
            var message = new MulticastMessage()
            {
                Tokens = registrationTokens,
                Notification = new Notification()
                {
                    Title = "شراء مادة",
                    Body = @$"قام {BuyerName} بشراء {ItemName} في المجموعة {GroupName} وبذلك يترتب عليك مبلغ {quantity}ل.س"
                },

            };
            
            var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
            // See the BatchResponse reference documentation
            // for the contents of response.
            //Console.WriteLine($"{response.SuccessCount} messages were sent successfully");
            // [END send_multicast]
            if(response.SuccessCount == registrationTokens.Count)
            {
                return true;
            }
            return false;
        }



        //public async Task<bool> SendNavigation(NotificationInfoDto notificationDto)
        //{

        //    string applicationID = String.Empty;
        //    string senderId = String.Empty;
        //    string deviceId = "";

        //        var user = await Context.Users.Where(user => user.Id == notificationDto.Type).
        //            Select(user => new TypeUserDeviceDto
        //            {
        //                TypeAccount = user.TypeAccount,
        //                DeviceToken = user.Devices.FirstOrDefault(device => device.EUserId == notificationDto.Type)
        //            }).SingleOrDefaultAsync();


        //        if (user is null || user.DeviceToken is null)
        //        {
        //            return false;
        //        }


        //        switch (user.TypeAccount) // captain
        //        {
        //            case 1:
        //                applicationID = "AAAAehQzHhc:APA91bF4T3sCK_LcG-gK8mLnhU2a50I9VHwpdXiHIwh4B_fTKvfr26CF2EucYirF7ZP_AyApHjDklZN0VYjBf_NYFmtQJDknKvQ6twCant0IAhh0do7tc0kFWx1zsZwCzGyyOiFCVznZ";
        //                senderId = "524324904471";
        //                deviceId = user.DeviceToken.DeviceToken;
        //                break;
        //            default:
        //                applicationID = "AAAAOYURRw4:APA91bEyRkoMzrCL-AvrUlduOkE0E1FQ1e0o8gNSDaB9xgmUeMgVqQ0YGsaKp7I5H43SfgQUwDJlVv9YLVsncop8qrvR0TxSBlS76vNZfrmTVo_zThnxy5AC7SGGOx3-Zvq32zTe4Cep";
        //                senderId = "247045637902";
        //                deviceId = user.DeviceToken.DeviceToken;
        //                break;
        //        }


        //        // deviceId = Context.Devices.FirstOrDefault(device => device.EUserId == notificationDto.Type)?.DeviceToken;

        //    }
        //    else if (notificationDto.Type == (int)NotificationTypes.AllUser)
        //    {
        //        applicationID = "AAAAOYURRw4:APA91bEyRkoMzrCL-AvrUlduOkE0E1FQ1e0o8gNSDaB9xgmUeMgVqQ0YGsaKp7I5H43SfgQUwDJlVv9YLVsncop8qrvR0TxSBlS76vNZfrmTVo_zThnxy5AC7SGGOx3-Zvq32zTe4Cep";
        //        senderId = "247045637902";
        //        deviceId = "/topics/all";
        //    }
        //    else if (notificationDto.Type == (int)NotificationTypes.AllCaptain)
        //    {
        //        applicationID = "AAAAehQzHhc:APA91bF4T3sCK_LcG-gK8mLnhU2a50I9VHwpdXiHIwh4B_fTKvfr26CF2EucYirF7ZP_AyApHjDklZN0VYjBf_NYFmtQJDknKvQ6twCant0IAhh0do7tc0kFWx1zsZwCzGyyOiFCVznZ";
        //        senderId = "524324904471";
        //        deviceId = "/topics/all";
        //    }
        //    else  // for All
        //    {

        //        // send to user
        //        applicationID = "AAAAOYURRw4:APA91bEyRkoMzrCL-AvrUlduOkE0E1FQ1e0o8gNSDaB9xgmUeMgVqQ0YGsaKp7I5H43SfgQUwDJlVv9YLVsncop8qrvR0TxSBlS76vNZfrmTVo_zThnxy5AC7SGGOx3-Zvq32zTe4Cep";
        //        senderId = "247045637902";
        //        deviceId = "/topics/all";
        //        var response1 = await HttpFCMSender(applicationID, senderId, deviceId, notificationDto.Text, notificationDto.Title);


        //        // send to captain
        //        applicationID = "AAAAehQzHhc:APA91bF4T3sCK_LcG-gK8mLnhU2a50I9VHwpdXiHIwh4B_fTKvfr26CF2EucYirF7ZP_AyApHjDklZN0VYjBf_NYFmtQJDknKvQ6twCant0IAhh0do7tc0kFWx1zsZwCzGyyOiFCVznZ";
        //        senderId = "524324904471";
        //        deviceId = "/topics/all";
        //        var response2 = await HttpFCMSender(applicationID, senderId, deviceId, notificationDto.Text, notificationDto.Title);

        //        switch (response1.StatusCode)
        //        {
        //            case HttpStatusCode.OK when response2.StatusCode == HttpStatusCode.OK:
        //                //string body1 = await response1.Content.ReadAsStringAsync();
        //                //string body2 = await response2.Content.ReadAsStringAsync();
        //                return true;
        //            default:
        //                return false;
        //        }

        //    }


        //    var response = await HttpFCMSender(applicationID, senderId, deviceId, notificationDto.Text, notificationDto.Title);

        //    switch (response.StatusCode)
        //    {
        //        case HttpStatusCode.OK:
        //            {
        //                // string body = await response.Content.ReadAsStringAsync();
        //                return true;
        //            }

        //        default:
        //            return false;
        //    }


        //}

        //public async Task<HttpResponseMessage> HttpFCMSender(string applicationID, string senderId, string deviceId, string text, string title)
        //{
        //    tRequest = new HttpClient();

        //    tRequest.BaseAddress = new Uri("https://fcm.googleapis.com");

        //    var Note = new
        //    {
        //        to = deviceId,
        //        priority = "high",
        //        notification = new
        //        {
        //            title = title,
        //            body = text,
        //        }
        //    };

        //    var json = JsonSerializer.Serialize(Note);
        //    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        //    tRequest.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={applicationID}");

        //    tRequest.DefaultRequestHeaders.TryAddWithoutValidation("Sender", $"id={applicationID}");

        //    return await tRequest.PostAsync("fcm/send", httpContent);
        //}



    }
}
