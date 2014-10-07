using System;
using System.Management.Automation;
using System.Net;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    using Properties;

    [Cmdlet(VerbsCommon.Set, "AzureStorSimpleDeviceVolume")]
    public class SetAzureStorSimpleDeviceVolume : StorSimpleCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDeviceName)]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceName { get; set; }

        [Alias("Name")]
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyByName, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageVolumeName)]
        [ValidateNotNullOrEmptyAttribute]
        public string VolumeName { get; set; }

        [Alias("ID")]
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyById, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageVolumeId)]
        [ValidateNotNullOrEmptyAttribute]
        public string VolumeId { get; set; }

        [Alias("Online")]
        [Parameter(Position = 2, Mandatory = false, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageVolumeOnline)]
        [ValidateNotNullOrEmpty]
        public bool Online { get; set; }

        [Alias("Size")]
        [Parameter(Position = 3, Mandatory = false, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageVolumeSize)]
        [ValidateNotNullOrEmpty]
        public Int64 VolumeSize { get; set; }

        [Alias("Apptype")]
        [Parameter(Position = 4, Mandatory = false, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageVolumeAppType)]
        [ValidateNotNullOrEmpty]
        public AppType VolumeAppType { get; set; }

        [Parameter(Position = 5, Mandatory = false, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageVolumeAcrList)]
        [ValidateNotNullOrEmpty]
        public AccessControlRecord[] AccessControlRecords { get; set; }

        [Parameter(Position = 6, Mandatory = false, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageWaitTillComplete)]
        public SwitchParameter WaitForComplete { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                var deviceId = StorSimpleClient.GetDeviceId(DeviceName);
                if (deviceId == null)
                {
                    return;
                }

                VirtualDisk diskDetails = null;

                switch (ParameterSetName)
                {
                    case StorSimpleCmdletParameterSet.IdentifyById:
                        diskDetails = StorSimpleClient.GetVolumeById(deviceId, VolumeId).VirtualDiskInfo;
                        break;
                    case StorSimpleCmdletParameterSet.IdentifyByName:
                        diskDetails = StorSimpleClient.GetVolumeByName(deviceId, VolumeName).VirtualDiskInfo;
                        break;
                }

                if (diskDetails == null)
                {
                    return;
                }
                
                    diskDetails.Online = Online;
                    diskDetails.SizeInBytes = VolumeSize;
                    diskDetails.AppType = VolumeAppType;
                    diskDetails.AcrList = AccessControlRecords;

                if (WaitForComplete.IsPresent)
                {
                    var jobstatus = StorSimpleClient.UpdateVolume(deviceId, diskDetails.InstanceId,diskDetails);
                    WriteObject(jobstatus);
                }
                else
                {
                    var jobresult = StorSimpleClient.UpdateVolumeAsync(deviceId, diskDetails.InstanceId, diskDetails);
                    
                    WriteObject(ToAsyncJobMessage(jobresult, "update"));
                }

            }
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
            }
        }

    }
}