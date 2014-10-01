using System;
using System.Management.Automation;
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    using Properties;

    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleDeviceVolume")]
    public class GetAzureStorSimpleDeviceVolume : StorSimpleCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDeviceName)]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceName { get; set; }

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyByParentObject, ValueFromPipeline = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDataContainerObject)]
        [ValidateNotNullOrEmptyAttribute]
        public DataContainer VolumeContainer { get; set; }

        [Alias("ID")]
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyById, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageVolumeId)]
        [ValidateNotNullOrEmptyAttribute]
        public string VolumeId { get; set; }

        [Alias("Name")]
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyByName, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageVolumeName)]
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
                    case StorSimpleCmdletParameterSet.IdentifyByParentObject:
                        var volumeInfoList = StorSimpleClient.GetAllVolumesFordataContainer(deviceId, VolumeContainer.InstanceId);
                        WriteObject(volumeInfoList.ListofVirtualDisks);
                        break;
                    case StorSimpleCmdletParameterSet.IdentifyById :
                        volumeInfo = StorSimpleClient.GetVolumeById(deviceId, VolumeId);
                        WriteObject(volumeInfo.VirtualDiskInfo);
                        break;
                    case StorSimpleCmdletParameterSet.IdentifyByName :
                        volumeInfo = StorSimpleClient.GetVolumeByName(deviceId, VolumeName);
                        WriteObject(volumeInfo.VirtualDiskInfo);
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