
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public string GetDeviceId(string deviceToUse)
        {
            if (deviceToUse == null) throw new ArgumentNullException("deviceToUse");
            var deviceInfos = GetAllDevices();
            return (from deviceInfo in deviceInfos where deviceInfo.FriendlyName.Equals(deviceToUse, StringComparison.InvariantCultureIgnoreCase) select deviceInfo.DeviceId).FirstOrDefault();
        }
    }
}