using System.Linq;
using System.Management.Automation;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Commands.Utilities.CloudService;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    using Properties;

    [Cmdlet(VerbsLifecycle.Start, "AzureStorSimpleDeviceBackupRestoreJob")]
    public class StartAzureStorSimpleDeviceBackupRestoreJob: StorSimpleCmdletBase
    {
        private const string PARAMETERSET_BACKUPID = "ByBackupId";
        private const string PARAMETERSET_BACKUPOBJECT = "ByBackupObject";
        private const string PARAMETERSET_BACKUPSNAPSHOTID = "ByBackupSnapshotId";
        private const string PARAMETERSET_BACKUPSNAPSHOTOBJECT = "ByBackupSnapshotObject";
        String finalBackupId = null;
        String finalSnapshotId = null;
        private string deviceId = null;

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_BACKUPID)]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_BACKUPOBJECT)]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_BACKUPSNAPSHOTID)]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_BACKUPSNAPSHOTOBJECT)]
        [ValidateNotNullOrEmpty]
        public string DeviceName { get; set; }

        [Parameter(Position = 1, Mandatory = true, HelpMessage = "InstanceId of the backupSet that needs to be restored", ParameterSetName = PARAMETERSET_BACKUPID)]
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "InstanceId of the backupSet that needs to be restored", ParameterSetName = PARAMETERSET_BACKUPSNAPSHOTID)]
        public Guid BackupId { get; set; }

        [Parameter(Position = 2, Mandatory = true, HelpMessage = "InstanceId of the Snapshot that needs to be restored", ParameterSetName = PARAMETERSET_BACKUPSNAPSHOTID)]
        public string SnapshotId { get; set; }

        [Parameter(Position = 1, Mandatory = true, HelpMessage = "BackupSet object which should be used for restore", ParameterSetName = PARAMETERSET_BACKUPOBJECT)]
        [Parameter(Position = 2, Mandatory = true, HelpMessage = "Snapshot object which should be used for restore", ParameterSetName = PARAMETERSET_BACKUPSNAPSHOTOBJECT)]
        public Backup Backup { get; set; }

        [Parameter(Position = 2, Mandatory = true, HelpMessage = "Snapshot object which should be used for restore", ParameterSetName = PARAMETERSET_BACKUPSNAPSHOTOBJECT)]
        public Snapshot Snapshot { get; set; }

        [Alias("WaitForCompletion")]
        [Parameter(Position = 2, Mandatory = false, HelpMessage = "Wait for remov task complete")]
        public SwitchParameter WaitForComplete { get; set; }

        [Parameter(HelpMessage = "Force to restore the backup without asking for confirmation")]
        public SwitchParameter Force
        {
            get { return force; }
            set { force = value; }
        }

        private bool force;

        public override void ExecuteCmdlet()
        {
            try
            {
                ProcessParameters();
                ConfirmAction(
                   Force.IsPresent,
                   string.Format(Resources.StartASSDBackupRestoreJobWarningMessage, finalBackupId),
                   string.Format(Resources.StartASSDBackupRestoreJobMessage, finalBackupId),
                  finalBackupId,
                  () =>
                  {
                      RestoreBackupRequest request = new RestoreBackupRequest();
                      request.BackupSetId = finalBackupId;
                      request.SnapshotId = finalSnapshotId;

                      if (WaitForComplete.IsPresent)
                      {
                          var restoreBackupResult = StorSimpleClient.RestoreBackup(deviceId, request);
                          WriteObject(restoreBackupResult);
                      }
                      else
                      {
                          //async scenario
                          var jobresult = StorSimpleClient.RestoreBackupAsync(deviceId, request);
                          WriteObject(jobresult);
                          WriteVerbose(String.Format("Your restore operation has been submitted for processing. Use commandlet \"Get-AzureStorSimpleJob -JobId {0}\" to track status.", jobresult.JobId));
                      }
                  });
            }
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
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
            
            finalBackupId = null;
            finalSnapshotId = null;
            switch (ParameterSetName)
            {
                case PARAMETERSET_BACKUPID:
                    finalBackupId = BackupId.ToString();
                    break;
                case PARAMETERSET_BACKUPSNAPSHOTID:
                    finalBackupId = BackupId.ToString();
                    finalSnapshotId = SnapshotId;
                    break;
                case PARAMETERSET_BACKUPOBJECT:
                    if (Backup == null)
                        throw new ArgumentException("Specify valid object for Backup parameter");
                    finalBackupId = Backup.InstanceId.ToString();
                    break;
                case PARAMETERSET_BACKUPSNAPSHOTOBJECT:
                    if (Backup == null)
                        throw new ArgumentException("Specify valid object for Backup parameter");
                    if (Snapshot == null)
                        throw new ArgumentException("Specify valid object for Snapshot parameter");
                    finalBackupId = Backup.InstanceId.ToString();
                    finalSnapshotId = SnapshotId;
                    break;
            }
        }
    }
}
