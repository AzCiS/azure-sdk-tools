using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Set, "AzureStorSimpleDeviceVolume")]
    public class SetAzureStorSimpleDeviceVolume : StorSimpleCmdletBase
    {
        [Alias("Device Name")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.")]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceName { get; set; }

        [Alias("VName")]
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "VolumeName", HelpMessage = "The name of volume.")]
        [ValidateNotNullOrEmptyAttribute]
        public string VolumeName { get; set; }

        [Alias("Id")]
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "VolumeId", HelpMessage = "The volume Id.")]
        [ValidateNotNullOrEmptyAttribute]
        public string VolumeId { get; set; }

        [Alias("VOnline")]
        [Parameter(Position = 2, Mandatory = false, HelpMessage = "Is volume online.")]
        [ValidateNotNullOrEmpty]
        public bool Online { get; set; }

        [Alias("Size")]
        [Parameter(Position = 3, Mandatory = false, HelpMessage = "The size of volume in bytes.")]
        [ValidateNotNullOrEmpty]
        public Int64 VolumeSize { get; set; }

        [Alias("Apptype")]
        [Parameter(Position = 4, Mandatory = false, HelpMessage = "The app type.")]
        [ValidateNotNullOrEmpty]
        public AppType VolumeAppType { get; set; }

        [Alias("AcrList")]
        [Parameter(Position = 5, Mandatory = false, HelpMessage = "List of access control records.")]
        [ValidateNotNullOrEmpty]
        public AccessControlRecord[] AccessControlRecords { get; set; }

        [Alias("Wait")]
        [Parameter(Position = 6, Mandatory = false, HelpMessage = "Wait for update task complete")]
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
                    case "VolumeId":
                        diskDetails = StorSimpleClient.GetVolumeById(deviceId, VolumeId).VirtualDiskInfo;
                        break;
                    case "VolumeName":
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
                    
                    var msg =
                        "Job submitted succesfully. Please use the command Get-AzureStorSimpleJob -InstanceId " +
                        jobresult.JobId + " for tracking the job status";
                    WriteObject(msg);
                }

            }
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
            }
        }

    }
}