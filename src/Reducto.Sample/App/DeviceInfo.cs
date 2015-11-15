using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Reducto.Sample
{
    public struct DeviceId
    {
        public string Id { get; private set; }

        public DeviceId(string id)
        {
            Id = id;
        }

    }

    public struct DeviceInfo
    {
        public string Name;
        public string Location;
        public Boolean Online;
        public DeviceId Id;
    }

}
