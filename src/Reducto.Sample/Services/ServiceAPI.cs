using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Reducto.Sample.Services
{
    public class ServiceAPI : IServiceAPI
    {
        public async System.Threading.Tasks.Task<System.Collections.Generic.List<DeviceInfo>> GetDevices ()
        {
            await Task.Delay (1000); // simulate latency
            return new List<DeviceInfo> (){{new DeviceInfo{Id = new DeviceId("1"), Name= "Device1", Location = "Reykjavik", Online = true}}};
        }
        public async System.Threading.Tasks.Task<UserInfo> AuthUser (string username, string password)
        {
            return new UserInfo{Username = username, HomeCity = "Reykjavik"};
        }
    }
}

