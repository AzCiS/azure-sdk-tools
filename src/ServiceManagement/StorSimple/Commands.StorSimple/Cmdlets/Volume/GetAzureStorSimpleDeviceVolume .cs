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
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyByDataContainerId, HelpMessage = "The volume container id.")]
        [ValidateNotNullOrEmptyAttribute]
        public string VolumeContainerId { get; set; }

        [Alias("VId")]
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyById, HelpMessage = "The volume id.")]
        [ValidateNotNullOrEmptyAttribute]
        public string VolumeId { get; set; }

        [Alias("VName")]
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyByName, HelpMessage = "The volume name.")]
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
                    case StorSimpleCmdletParameterSet.IdentifyByDataContainerId:
                        var volumeInfoList = StorSimpleClient.GetAllVolumesFordataContainer(deviceId, VolumeContainerId);
                        WriteObject(volumeInfoList);
                        break;
                    case StorSimpleCmdletParameterSet.IdentifyById :
                        volumeInfo = StorSimpleClient.GetVolumeById(deviceId, VolumeId);
                        WriteObject(volumeInfo);
                        break;
                    case StorSimpleCmdletParameterSet.IdentifyByName :
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