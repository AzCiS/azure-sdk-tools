using System;
using System.Management.Automation;
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleDeviceVolume")]
    public class GetAzureStorSimpleDeviceVolume : StorSimpleCmdletBase
    {
        [Alias("DN")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.")]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceName { get; set; }

        [Alias("DCId")]
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "DataContainerId", HelpMessage = "The volume container id.")]
        [ValidateNotNullOrEmptyAttribute]
        public string VolumeContainerId { get; set; }

        [Alias("VId")]
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "VolumeId", HelpMessage = "The volume id.")]
        [ValidateNotNullOrEmptyAttribute]
        public string VolumeId { get; set; }

        [Alias("VName")]
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "VolumeName", HelpMessage = "The volume name.")]
        [ValidateNotNullOrEmptyAttribute]
        public string VolumeName { get; set; }
        public override void ExecuteCmdlet()
        {
            try
            {
                var deviceId = StorSimpleClient.GetDeviceId(DeviceName);
                if (deviceId == null) return;
                VirtualDiskGetResponse volumeInfo;
                switch (ParameterSetName)
                {
                    case "DataContainerId":
                        var volumeInfoList = StorSimpleClient.GetAllVolumesFordataContainer(deviceId, VolumeContainerId);
                        WriteObject(volumeInfoList);
                        break;
                    case "VolumeId" :
                        volumeInfo = StorSimpleClient.GetVolumeById(deviceId, VolumeId);
                        WriteObject(volumeInfo);
                        break;
                    case "VolumeName" :
                        volumeInfo = StorSimpleClient.GetVolumeByName(deviceId, VolumeName);
                        WriteObject(volumeInfo);
                        break;
                }
            }
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
            }
        }
    }
}