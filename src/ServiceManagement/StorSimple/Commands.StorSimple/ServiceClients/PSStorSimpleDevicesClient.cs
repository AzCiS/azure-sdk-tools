
using System.Collections;
using System.Collections.Generic;
using Microsoft.Azure;
using Microsoft.Azure.Commands.StorSimple;
using Microsoft.Azure.Management.StorSimple.Models;

namespace Microsoft.Azure.Commands.StorSimple
{
    public partial class PSStorSimpleClient
    {
        public IEnumerable<DeviceInfo> GetAllDevices()
        {
            return this.GetStorSimpleClient().Devices.List(this.GetCustomeRequestHeaders());
        }
    }
}