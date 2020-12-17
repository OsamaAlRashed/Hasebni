using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hasebni.SharedKernal.ExtensionMethod;

namespace Hasebni.Main.Data.Repository
{
    public class TestSignalR : Hub
    {
        public async Task Send()
        {
            int i = 0;
            while (i < 1000)
            {
                var timerManager = new TimerManager(() => Clients.All.SendAsync("send",i));
                i++;
            }
        }
    }
}
