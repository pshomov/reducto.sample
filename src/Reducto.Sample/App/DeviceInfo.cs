using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Reducto.Sample
{
    public struct DeviceId {
        public string Id { get; private set;}
        public DeviceId (string id)
        {
            Id = id;
        }

    }
    public struct DeviceInfo {
        public string Name { get; set;}
        public string Location { get; set;}
        public Boolean Online { get; set;}
        public DeviceId Id { get; set;}
    }

}
