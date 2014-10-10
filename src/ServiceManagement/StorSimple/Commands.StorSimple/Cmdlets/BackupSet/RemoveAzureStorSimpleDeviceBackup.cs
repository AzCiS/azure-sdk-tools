using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Commands.Utilities.CloudService;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    using Properties;

    [Cmdlet(VerbsCommon.Remove, "AzureStorSimpleDeviceBackup")]
    public class RemoveAzureStorSimpleDeviceBackup:StorSimpleCmdletBase
    {
        private const string PARAMETERSET_BYID = "ByID";

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_BYID)]
        [ValidateNotNullOrEmpty]
        public string DeviceName { get; set; }


        [Parameter(Position = 1, Mandatory = true, HelpMessage = "InstanceId of the Backup that needs to be deleted", ParameterSetName = PARAMETERSET_BYID)]
        public String BackupId { get; set; }

        [Parameter(HelpMessage = "Force to delete the backup without asking for confirmation")]
        public SwitchParameter Force
        {
            get { return force; }
            set { force = value; }
        }
        private bool force;

        [Alias("WaitForCompletion")]
        [Parameter(Position = 2, Mandatory = false, HelpMessage = "Wait for remov task complete")]
        public SwitchParameter WaitForComplete { get; set; }

        private string deviceId = null;


        public override void ExecuteCmdlet()
        {
            try
            {
                ProcessParameters();
                ConfirmAction(
                   Force.IsPresent,
                   string.Format(Resources.RemoveASSDBackupWarningMessage, BackupId),
                   string.Format(Resources.RemoveASSDBackupMessage, BackupId),
                  BackupId,
                  () =>
                  {

                      if (WaitForComplete.IsPresent)
                      {
                          var deleteJobStatusInfo = StorSimpleClient.DeleteBackup(deviceId, BackupId);
                          WriteObject(deleteJobStatusInfo);
                      }
                      else
                      {
                          var jobresult = StorSimpleClient.DeleteBackupAsync(deviceId, BackupId);
                          WriteObject(jobresult);
                          WriteVerbose(String.Format("Your remove operation has been submitted for processing. Use commandlet \"Get-AzureStorSimpleJob -JobId {0}\" to track status.", jobresult.JobId));
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
            if (String.IsNullOrEmpty(BackupId))
                throw new ArgumentException("Specify valid string for BackupId parameter");
        }
    }
}
