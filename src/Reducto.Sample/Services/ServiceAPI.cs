using System;

namespace Reducto.Sample.Services
{
    public class ServiceAPI : IServiceAPI
    {
        public System.Threading.Tasks.Task<System.Collections.Generic.List<DeviceInfo>> GetDevices ()
        {
            throw new NotImplementedException ();
        }
        public System.Threading.Tasks.Task<UserInfo> AuthUser (string username, string password)
        {
            throw new NotImplementedException ();
        }
    }
}

