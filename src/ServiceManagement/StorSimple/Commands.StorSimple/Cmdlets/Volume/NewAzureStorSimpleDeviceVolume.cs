using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets.Volume
{
    [Cmdlet(VerbsCommon.New, "AzureStorSimpleDeviceVolume")]
    public class NewAzureStorSimpleDeviceVolume : StorSimpleCmdletBase
    {
        [Alias("DName")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.")]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceName { get; set; }

        [Alias("DC")]
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The name of data container to use.")]
        [ValidateNotNullOrEmpty]
        public DataContainer Container { get; set; }
        
        [Alias("VName")]
        [Parameter(Position = 2, Mandatory = true, HelpMessage = "The name of volume.")]
        [ValidateNotNullOrEmpty]
        public string VolumeName { get; set; }

        [Alias("Size")]
        [Parameter(Position = 3, Mandatory = true, HelpMessage = "The size of volume in bytes.")]
        [ValidateNotNullOrEmpty]
        public Int64 VolumeSize { get; set; }

        [Alias("ACRList")]
        [Parameter(Position = 4, Mandatory = true, HelpMessage = "List of access control records.")]
        [ValidateNotNullOrEmpty]
        public AccessControlRecord[] AccessControlRecords { get; set; }

        [Alias("AppType")]
        [Parameter(Position = 5, Mandatory = true, HelpMessage = "The app type.")]
        [ValidateNotNullOrEmpty]
        public AppType VolumeAppType { get; set; }

        [Alias("VOnline")]
        [Parameter(Position = 6, Mandatory = true, HelpMessage = "Is volume online.")]
        [ValidateNotNullOrEmpty]
        public bool Online { get; set; }

        [Alias("DefaultBackup")]
        [Parameter(Position = 7, Mandatory = true, HelpMessage = "Flag for enabling default backup.")]
        [ValidateNotNullOrEmpty]
        public bool EnableDefaultBackup { get; set; }

        [Alias("Monitoring")]
        [Parameter(Position = 8, Mandatory = true, HelpMessage = "Flag for enabling monitoring.")]
        [ValidateNotNullOrEmpty]
        public bool EnableMonitoring { get; set; }

        [Alias("Wait")]
        [Parameter(Position = 9, Mandatory = false, HelpMessage = "Wait for remov task complete")]
        public SwitchParameter WaitForComplete { get; set; }
        public override void ExecuteCmdlet()
        {
            try
            {
                string deviceid = null;
                deviceid = StorSimpleClient.GetDeviceId(DeviceName);

                if (deviceid == null)
                {
                    WriteObject("device with name " + DeviceName + "not found");
                }

                //Virtual disk create request object
                var virtualDiskToCreate = new VirtualDiskRequest()
                {
                    Name = VolumeName,
                    AccessType = AccessType.ReadWrite,
                    AcrList = AccessControlRecords,
                    AppType = VolumeAppType,
                    IsDefaultBackupEnabled = EnableDefaultBackup,
                    SizeInBytes = VolumeSize,
                    DataContainer = Container,
                    Online = Online,
                    IsMonitoringEnabled = EnableMonitoring
                };

                //var jobStatusInfo = StorSimpleClient.CreateVolume(deviceid, virtualDiskToCreate);
                //WriteObject(jobStatusInfo);

                if (WaitForComplete.IsPresent)
                {
                    var jobstatus = StorSimpleClient.CreateVolume(deviceid, virtualDiskToCreate); ;
                    WriteObject(jobstatus);
                }

                else
                {
                    var jobstatus = StorSimpleClient.CreateVolumeAsync(deviceid, virtualDiskToCreate); ;
                    
                    var msg =
                        "Job submitted succesfully. Please use the command Get-AzureStorSimpleJob -InstanceId " +
                        jobstatus.JobId + "for tracking the job status";
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