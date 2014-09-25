using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Commands.Utilities.CloudService;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Remove, "AzureStorSimpleBackup")]
    public class RemoveAzureStorSimpleBackup:StorSimpleCmdletBase
    {
        private const string PARAMETERSET_BYID = "ByID";
        private const string PARAMETERSET_BYOBJECT = "ByObject";

        [Alias("DeviceName")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_BYID)]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_BYOBJECT)]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceNameToUse { get; set; }


        [Parameter(Position = 1, Mandatory = true, HelpMessage = "InstanceId of the backupSet that needs to be deleted", ParameterSetName = PARAMETERSET_BYID)]
        public String BackupSetInstanceId { get; set; }

        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Provide the BackupSet object here.", ParameterSetName = PARAMETERSET_BYOBJECT)]
        public BackupSetInfo BackupSet { get; set; }

        [Alias("WaitForCompletion")]
        [Parameter(Position = 2, Mandatory = false, HelpMessage = "Wait for remov task complete")]
        public SwitchParameter WaitForComplete { get; set; }

        private string deviceId = null;


        public override void ExecuteCmdlet()
        {
            try
            {
                ProcessParameters();
                string instanceIdToPass = (ParameterSetName == PARAMETERSET_BYID
                    ? BackupSetInstanceId
                    : BackupSet.InstanceId);
                if (WaitForComplete.IsPresent)
                {
                    var deleteJobStatusInfo = StorSimpleClient.DeleteBackup(deviceId, instanceIdToPass);
                    WriteObject(deleteJobStatusInfo);
                }
                else
                {
                    var jobresult = StorSimpleClient.DeleteBackupAsync(deviceId, instanceIdToPass);
                    WriteObject(jobresult);
                    WriteVerbose(String.Format("Your async operation has been submitted for processing. Use commandlet \"Get-AzureStorSimpleJob -JobId {0}\" to track status.", jobresult.JobId));
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
                if (deviceInfo.FriendlyName.Equals(DeviceNameToUse, StringComparison.InvariantCultureIgnoreCase))
                {
                    deviceId = deviceInfo.DeviceId;
                }
            }
            if (deviceId == null)
                throw new ArgumentException("Device with name " + DeviceNameToUse + "not found");
            switch (ParameterSetName)
            {
                case PARAMETERSET_BYID:
                    if(String.IsNullOrEmpty(BackupSetInstanceId))
                        throw new ArgumentException("Specify valid string for BackupSetInstanceId parameter");
                    break;
                case PARAMETERSET_BYOBJECT:
                    if (BackupSet == null)
                        throw new ArgumentException("Specify valid object for BackupPolicyDetails parameter");
                    break;
            }
        }
    }
}
