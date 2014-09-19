using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.WindowsAzure;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleDeviceVolumeContainer")]
    public class GetAzureStorSimpleDeviceVolumeContainer : StorSimpleCmdletBase
    {
        [Alias("DeviceName")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.")]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceToUse { get; set; }

        [Alias("Name")]
        [Parameter(Position = 1, Mandatory = false, HelpMessage = "The name of data container.")]
        [ValidateNotNullOrEmptyAttribute]
        public string DcName { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                string deviceid = null;
                var deviceInfos = StorSimpleClient.GetAllDevices();
                foreach (var deviceInfo in deviceInfos)
                {
                    if (deviceInfo.FriendlyName.Equals(DeviceToUse, StringComparison.InvariantCultureIgnoreCase))
                    {
                        deviceid = deviceInfo.DeviceId;
                    }
                }

                if (deviceid == null)
                {
                    WriteObject("device with name " + DeviceToUse + "not found");
                }

                if (DcName == null)
                {
                    var dataContainerList = StorSimpleClient.GetAllDataContainers(deviceid);
                    if (dataContainerList.Any())
                    {
                        foreach (var datacontainer in dataContainerList)
                        {
                            WriteObject(datacontainer);
                        }
                    }
                }
                var dataContainer = StorSimpleClient.GetDataContainer(deviceid, DcName);

                WriteObject(dataContainer.DataContainerInfo);
            }
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
            }
        }
    }
}