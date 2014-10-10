using System.Linq;
using System.Management.Automation;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Commands.Utilities.CloudService;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "AzureStorSimpleDeviceBackupPolicy")]
    public class NewAzureStorSimpleDeviceBackupPolicy:StorSimpleCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.")]
        [ValidateNotNullOrEmpty]
        public string DeviceName { get; set; }

        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The name of the backup policy.")]
        [ValidateNotNullOrEmpty]
        public String BackupPolicyName { get; set; }

        [Parameter(Position = 2, Mandatory = false, HelpMessage = "List of BackupScheduleBase objects to be added to the policy")]
        public PSObject[] BackupSchedulesToAdd { get; set; }

        [Parameter(Position = 3, Mandatory = false, HelpMessage = "List of VolumeIds to be added")]
        public PSObject[] VolumeIdsToUpdate { get; set; }

        [Alias("WaitForCompletion")]
        [Parameter(Position = 4, Mandatory = false, HelpMessage = "Wait for add task to complete")]
        public SwitchParameter WaitForComplete { get; set; }

        private string deviceId = null;
        private List<BackupScheduleBase> schedulesToAdd = null;
        private List<String> volumeIdsToUpdate = null;

        private NewBackupPolicyConfig newConfig = null;

        public override void ExecuteCmdlet()
        {
            try
            {
                ProcessParameters();
                newConfig = new NewBackupPolicyConfig();
                newConfig.Name = BackupPolicyName;
                ProcessAddSchedules();
                ProcessUpdateVolumeIds();
                if (WaitForComplete.IsPresent)
                {
                    var JobStatusInfo = StorSimpleClient.CreateBackupPolicy(deviceId, newConfig);
                    WriteObject(JobStatusInfo);
                }
                else
                {
                    var jobresult = StorSimpleClient.CreateBackupPolicyAsync(deviceId, newConfig);
                    WriteObject(jobresult);
                    WriteVerbose(String.Format("Your remove operation has been submitted for processing. Use commandlet \"Get-AzureStorSimpleJob -JobId {0}\" to track status.", jobresult.JobId));
                }
            }
            catch (CloudException exception)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(exception);
            }
        }
        private void ProcessParameters()
        {
            var deviceInfos = StorSimpleClient.GetAllDevices(); //get list of all devices
            foreach (var deviceInfo in deviceInfos)
            {
                if (deviceInfo.FriendlyName.Equals(DeviceName, StringComparison.InvariantCultureIgnoreCase))
                {
                    deviceId = deviceInfo.DeviceId;
                }
            }
            if (deviceId == null)
                throw new ArgumentException("Device with name " + DeviceName + "not found");
        }

        private void ProcessAddSchedules()
        {
            newConfig.BackupSchedules = new List<BackupScheduleBase>();
            if (BackupSchedulesToAdd.Length > 0)
            {
                foreach (var addSchedule in BackupSchedulesToAdd)
                {
                    BackupScheduleBase backupSchedule = (BackupScheduleBase)addSchedule.BaseObject;
                    newConfig.BackupSchedules.Add(backupSchedule);
                }
            }
        }

        private void ProcessUpdateVolumeIds()
        {
            volumeIdsToUpdate = new List<string>();
            if (VolumeIdsToUpdate.Length > 0)
            {
                foreach (var volume in VolumeIdsToUpdate)
                {
                    String volumeId = (String)volume.BaseObject;
                    volumeIdsToUpdate.Add(volumeId);
                }
            }
            newConfig.VolumeIds = volumeIdsToUpdate;
        }
    }
}
