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

    [Cmdlet(VerbsCommon.Remove, "AzureStorSimpleDeviceBackupPolicy")]
    public class RemoveAzureStorSimpleDeviceBackupPolicy : StorSimpleCmdletBase
    {
        private const string PARAMETERSET_DEFAULT = "Default";
        private const string PARAMETERSET_BYOBJECT = "ByObject";
        private string deviceId = null;

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_DEFAULT)]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_BYOBJECT)]
        [ValidateNotNullOrEmpty]
        public string DeviceName { get; set; }

        [Parameter(Position = 1, Mandatory = true, HelpMessage = "InstanceId of the backuppolicy that you want to remove", ParameterSetName = PARAMETERSET_DEFAULT)]
        public Guid? BackupPolicyId { get; set; }

        [Alias("BackupPolicyDetails")]
        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Provide the BackupPolicyDetails object that you want to remove.", ParameterSetName = PARAMETERSET_BYOBJECT)]
        public BackupPolicyDetails BackupPolicy { get; set; }

        [Parameter(HelpMessage = "Force to delete the backuppolicy without asking for confirmation")]
        public SwitchParameter Force
        {
            get { return force; }
            set { force = value; }
        }
        private bool force;

        [Alias("WaitForCompletion")]
        [Parameter(Position = 2, Mandatory = false, HelpMessage = "Wait for remov task complete")]
        public SwitchParameter WaitForComplete { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                ProcessParameters();
                string backupPolicyIdFinal = (ParameterSetName == PARAMETERSET_DEFAULT
                    ? BackupPolicyId.ToString()
                    : BackupPolicy.InstanceId);
                ConfirmAction(
                   Force.IsPresent,
                   string.Format(Resources.RemoveASSDBackupPolicyWarningMessage, backupPolicyIdFinal),
                   string.Format(Resources.RemoveASSDBackupPolicyMessage, backupPolicyIdFinal),
                  backupPolicyIdFinal,
                  () =>
                  {

                      if (WaitForComplete.IsPresent)
                      {
                          var deleteJobStatusInfo = StorSimpleClient.DeleteBackupPolicy(deviceId, backupPolicyIdFinal);
                          WriteObject(deleteJobStatusInfo);
                      }
                      else
                      {
                          var jobresult = StorSimpleClient.DeleteBackupPolicyAsync(deviceId, backupPolicyIdFinal);
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
            switch (ParameterSetName)
            {
                case PARAMETERSET_DEFAULT:
                    if (BackupPolicyId == null)
                        throw new ArgumentException("Specify valid value for BackupPolicyId parameter");
                    break;
                case PARAMETERSET_BYOBJECT:
                    if(BackupPolicy == null)
                        throw new ArgumentException("Specify a valid BackupPolicy to remove");
                    break;
            }
        }
    }
}
