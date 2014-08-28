
using System.Collections;
using System.Collections.Generic;
using Microsoft.Azure;
using Microsoft.Azure.Management.StorSimple.Models;

namespace Micro.Azure.Commands.StorSimple
{
    public partial class PSStorSimpleClient
    {
        public IEnumerable<DeviceInfo> GetAllDevices()
        {
            return this.GetStorSimpleClient().Devices.List(resourceId,null);
        }
    }
}