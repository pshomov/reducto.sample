using System;
using System.Threading.Tasks;

namespace Reducto.Sample.Services
{
    public class ServiceAPI : IServiceAPI
    {
        public System.Threading.Tasks.Task<System.Collections.Generic.List<DeviceInfo>> GetDevices ()
        {
            throw new NotImplementedException ();
        }
        public async System.Threading.Tasks.Task<UserInfo> AuthUser (string username, string password)
        {
            await Task.Delay(5000);
            return new UserInfo{Username = username, HomeCity = "Reykjavik"};
        }
    }
}

