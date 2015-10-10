using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Reducto.Sample
{
    public class UserInfo {
        public string Username;
        public string HomeCity;

        public static UserInfo NotFound = new UserInfo();
    }
	public interface IServiceAPI
	{
        Task<List<DeviceInfo>> GetDevices ();
        Task<UserInfo> AuthUser (string username, string password);
	}

}

