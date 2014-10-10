using System.Linq;
using System.Management.Automation;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Commands.Utilities.CloudService;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsLifecycle.Start, "AzureStorSimpleDeviceBackupJob")]
    public class StartAzureStorSimpleDeviceBackupJob : StorSimpleCmdletBase
    {
        private const string PARAMETERSET_DEFAULT = "Default";
        private const string PARAMETERSET_BACKUPTYPE = "BackupTypeGiven";

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_DEFAULT)]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_BACKUPTYPE)]
        [ValidateNotNullOrEmpty]
        public string DeviceName { get; set; }


        [Parameter(Position = 1, Mandatory = true, HelpMessage = "Id of the backupPolicy which will be used to create backup", ParameterSetName = PARAMETERSET_DEFAULT)]
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "Id of the backupPolicy which will be used to create backup", ParameterSetName = PARAMETERSET_BACKUPTYPE)]
        public String BackupPolicyId { get; set; }


        [Parameter(Position = 2, Mandatory = true, HelpMessage = "Provide one of the backupTypes : LocalSnapshot/CloudSnapshot", ParameterSetName = PARAMETERSET_BACKUPTYPE)]
        [ValidateSet("LocalSnapshot", "CloudSnapshot")]
        public String BackupType { get; set; }

        [Alias("WaitForCompletion")]
        [Parameter(Position = 3, Mandatory = false, HelpMessage = "Wait for remov task complete")]
        public SwitchParameter WaitForComplete { get; set; }

        private string deviceId = null;
        private BackupNowRequest backupNowRequest = null;

         public override void ExecuteCmdlet()
        {
             try
             {
                 ProcessParameters();
                 if (WaitForComplete.IsPresent)
                 {
                     var JobStatusInfo = StorSimpleClient.DoBackup(deviceId, BackupPolicyId, backupNowRequest);
                     WriteObject(JobStatusInfo);
                 }
                 else
                 {
                     var jobresult = StorSimpleClient.DoBackupAsync(deviceId, BackupPolicyId, backupNowRequest);
                     WriteObject(jobresult);
                     WriteVerbose(String.Format("Your backup operation has been submitted for processing. Use commandlet \"Get-AzureStorSimpleJob -JobId {0}\" to track status.", jobresult.JobId));
                 }
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

             BackupType backupTypeSelected = Management.StorSimple.Models.BackupType.Invalid;
             switch (ParameterSetName)
             {
                 case PARAMETERSET_DEFAULT:
                     backupTypeSelected = Microsoft.WindowsAzure.Management.StorSimple.Models.BackupType.LocalSnapshot;
                     break;
                 case PARAMETERSET_BACKUPTYPE:
                     backupTypeSelected = (BackupType)Enum.Parse(typeof(BackupType), BackupType);
                     break;
             }
             backupNowRequest = new BackupNowRequest();
             backupNowRequest.Type = backupTypeSelected;
        }
    }
}
