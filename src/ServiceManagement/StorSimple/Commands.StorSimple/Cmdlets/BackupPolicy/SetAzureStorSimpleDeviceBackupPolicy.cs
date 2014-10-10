using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Commands.Utilities.CloudService;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Set, "AzureStorSimpleDeviceBackupPolicy"), OutputType(typeof(NewBackupPolicyConfig))]
    public class SetAzureStorSimpleDeviceBackupPolicy: StorSimpleCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.")]
        [ValidateNotNullOrEmpty]
        public string DeviceName { get; set; }

        [Parameter(Position = 1, Mandatory = true, HelpMessage = "InstanceId of the backupPolicy which you are trying to update")]
        [ValidateNotNullOrEmpty]
        public Guid BackupPolicyId { get; set; }

        [Parameter(Position = 2, Mandatory = true, HelpMessage = "Name of the backup policy. If you are changing the name, set -IsPolicyRenamed to 1")]
        [ValidateNotNullOrEmpty]
        public string BackupPolicyName { get; set; }

        [Parameter(Position = 3, Mandatory = true, HelpMessage = "If you are renaming the policy set this value to 1")]
        [ValidateNotNullOrEmpty]
        public bool IsPolicyRenamed { get; set; }

        [Parameter(Position = 4, Mandatory = false, HelpMessage = "List of BackupScheduleBase objects to be added to the policy")]
        public PSObject[] BackupSchedulesToAdd { get; set; }

        [Parameter(Position = 5, Mandatory = false, HelpMessage = "List of BackupScheduleUpdateRequest objects to be updated")]
        public PSObject[] BackupSchedulesToUpdate { get; set; }

        [Parameter(Position = 6, Mandatory = false, HelpMessage = "List of Instance Id of BackupSchedule objects to be deleted")]
        public PSObject[] BackupScheduleIdsToDelete { get; set; }

        [Parameter(Position = 7, Mandatory = false, HelpMessage = "List of VolumeIds to be updated")]
        public PSObject[] VolumeIdsToUpdate { get; set; }

        private List<BackupScheduleBase> schedulesToAdd = null;
        private List<BackupScheduleUpdateRequest> schedulesToUpdate = null;
        private List<String> scheduleIdsTodelete = null;
        private List<String> volumeIdsToUpdate = null;

        private UpdateBackupPolicyConfig updateConfig = null;
        public override void ExecuteCmdlet()
        {
            updateConfig = new UpdateBackupPolicyConfig();
            ProcessAddSchedules();
            ProcessUpdateSchedules();
            ProcessDeleteScheduleIds();
            ProcessUpdateVolumeIds();
        }

        private void ProcessAddSchedules()
        {
            schedulesToAdd = new List<BackupScheduleBase>();
            if (BackupSchedulesToAdd.Length > 0)
            {
                foreach (var addSchedule in BackupSchedulesToAdd)
                {
                    BackupScheduleBase backupSchedule = (BackupScheduleBase)addSchedule.BaseObject;
                    schedulesToAdd.Add(backupSchedule);
                }
            }
            updateConfig.BackupSchedulesToBeAdded = schedulesToAdd;
        }

        private void ProcessUpdateSchedules()
        {
            schedulesToUpdate = new List<BackupScheduleUpdateRequest>();
            if (BackupSchedulesToUpdate.Length > 0)
            {
                foreach (var updateSchedule in BackupSchedulesToUpdate)
                {
                    BackupScheduleUpdateRequest updateschedule = (BackupScheduleUpdateRequest) updateSchedule.BaseObject;
                    schedulesToUpdate.Add(updateschedule);
                }
            }
            updateConfig.BackupSchedulesToBeUpdated = schedulesToUpdate;
        }

        private void ProcessDeleteScheduleIds()
        {
            scheduleIdsTodelete = new List<string>();
            if (BackupScheduleIdsToDelete.Length > 0)
            {
                foreach (var deleteSchedule in BackupScheduleIdsToDelete)
                {
                    String scheduleIdToDelete = (String)deleteSchedule.BaseObject;
                    scheduleIdsTodelete.Add(scheduleIdToDelete);
                }
            }
            updateConfig.BackupSchedulesToBeDeleted = scheduleIdsTodelete;
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
            updateConfig.VolumeIds = volumeIdsToUpdate;
        }
    }
}
