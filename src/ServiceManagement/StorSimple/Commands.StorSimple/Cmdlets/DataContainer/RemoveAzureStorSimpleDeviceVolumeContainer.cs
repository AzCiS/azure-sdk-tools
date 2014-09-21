using System;
using System.Management.Automation;
using Microsoft.Azure.Management.StorSimple.Models;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Remove, "AzureStorSimpleDeviceVolumeContainer")]
    public class RemoveAzureStorSimpleDeviceVolumeContainer : StorSimpleCmdletBase
    {
        [Alias("DeviceToUse")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.")]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceName { get; set; }

        [Alias("DataContainer")]
        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The volume container name which needs to be removed.")]
        [ValidateNotNullOrEmptyAttribute]
        public DataContainer VolumeContainer { get; set; }

        [Alias("WaitForCompletion")]
        [Parameter(Position = 2, Mandatory = false, HelpMessage = "Wait for remov task complete")]
        public SwitchParameter WaitForComplete { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                string deviceid = null;
                var deviceInfos = StorSimpleClient.GetAllDevices();
                foreach (var deviceInfo in deviceInfos)
                {
                    if (deviceInfo.FriendlyName.Equals(DeviceName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        deviceid = deviceInfo.DeviceId;
                    }
                }

                if (deviceid == null) return;
                if (WaitForComplete.IsPresent)
                {
                    var jobstatusInfo = StorSimpleClient.DeleteDataContainer(deviceid, VolumeContainer.InstanceId);
                    WriteObject(jobstatusInfo);
                }
                else
                {
                    var jobresult = StorSimpleClient.DeleteDataContainerAsync(deviceid, VolumeContainer.InstanceId);
                    WriteObject(jobresult);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}