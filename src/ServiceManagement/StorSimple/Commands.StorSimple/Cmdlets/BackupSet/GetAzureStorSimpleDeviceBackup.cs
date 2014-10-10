using System.Linq;
using System.Management.Automation;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Commands.Utilities.CloudService;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleDeviceBackup"),OutputType(typeof(GetBackupResponse))]
    public class GetAzureStorSimpleDeviceBackup: StorSimpleCmdletBase
    {
        private const string PARAMETERSET_DEFAULT = "Default";
        private const string PARAMETERSET_BACKUPPOLICYIDFILTER = "BackupPolicyIdFilter";
        private const string PARAMETERSET_VOLUMEIDFILTER = "VolumeIdFilter";
        private const string PARAMETERSET_BACKUPPOLICYOBJECTFILTER = "BackupPolicyObjectFilter";
        private const string PARAMETERSET_VOLUMEOBJECTFILTER = "VolumeObjectFilter";

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_DEFAULT)]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_BACKUPPOLICYIDFILTER)]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_VOLUMEIDFILTER)]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_BACKUPPOLICYOBJECTFILTER)]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_VOLUMEOBJECTFILTER)]
        [ValidateNotNullOrEmpty]
        public string DeviceName { get; set; }

        [Parameter(Position = 1, Mandatory = true, HelpMessage = "InstanceId of the backupPolicy which created the backups.", ParameterSetName = PARAMETERSET_BACKUPPOLICYIDFILTER)]
        public Guid? BackupPolicyId { get; set; }

        [Parameter(Position = 1, Mandatory = true, HelpMessage = "InstanceId of the volume in which backups exist.", ParameterSetName = PARAMETERSET_VOLUMEIDFILTER)]
        public String VolumeId { get; set; }

        [Alias("BackupPolicyDetails")]
        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Provide the BackupPolicyDetails object. The InstanceId of this object will be used as a filter for backups.", ParameterSetName = PARAMETERSET_BACKUPPOLICYOBJECTFILTER)]
        public BackupPolicyDetails BackupPolicy { get; set; }

        [Alias("VirtualDiskInfo")]
        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Provide the VirtualDisk object. The InstanceId of this object will be used as a filter for backups.", ParameterSetName = PARAMETERSET_VOLUMEOBJECTFILTER)]
        public VirtualDisk Volume { get; set; }

        [Parameter(Position = 2, Mandatory = false, HelpMessage = "The start time for filtering backups.")]
        public DateTime? From { get; set; }

        [Parameter(Position = 3, Mandatory = false, HelpMessage = "The end time for filtering backups.")]
        public DateTime? To { get; set; }

        [Parameter(Position = 4, Mandatory = false, HelpMessage = "Gets only the specified number of objects. Enter the number of objects to get.")]
        public int? First { get; set; }

        [Parameter(Position = 5, Mandatory = false, HelpMessage = "Ignores the specified number of objects and then gets the remaining objects. Enter the number of objects to skip.")]
        public int? Skip { get; set; }

        private string deviceId = null;
 
        public override void ExecuteCmdlet()
        {
            try
            {
                ProcessParameters();
                GetBackupResponse backupList = null;
                switch (ParameterSetName)
                {
                    case PARAMETERSET_BACKUPPOLICYIDFILTER:
                        backupList = StorSimpleClient.GetAllBackups(
                            deviceId,
                            "BackupPolicy",
                            Boolean.FalseString,
                            BackupPolicyId.ToString(),
                            From.ToString(),
                            To.ToString(),
                            (Skip == null ? null : Skip.ToString()),
                            (First == null ? null : First.ToString()));
                        break;
                    case PARAMETERSET_VOLUMEIDFILTER:
                         backupList = StorSimpleClient.GetAllBackups(
                            deviceId,
                            "Volume",
                            Boolean.FalseString,
                            VolumeId,
                            From.ToString(),
                            To.ToString(),
                            (Skip == null ? null : Skip.ToString()),
                            (First == null ? null : First.ToString()));
                        break;
                    case PARAMETERSET_BACKUPPOLICYOBJECTFILTER:
                        backupList = StorSimpleClient.GetAllBackups(
                            deviceId,
                            "BackupPolicy",
                            Boolean.FalseString,
                            BackupPolicy.InstanceId.ToString(),
                            From.ToString(),
                            To.ToString(),
                            (Skip == null ? null : Skip.ToString()),
                            (First == null ? null : First.ToString()));
                        break;
                    case PARAMETERSET_VOLUMEOBJECTFILTER:
                        backupList = StorSimpleClient.GetAllBackups(
                            deviceId,
                            "Volume",
                            Boolean.FalseString,
                            Volume.InstanceId,
                            From.ToString(),
                            To.ToString(),
                            (Skip == null ? null : Skip.ToString()),
                            (First == null ? null : First.ToString()));
                        break;
                    default:
                            //case where only deviceName is passed
                        backupList = StorSimpleClient.GetAllBackups(
                            deviceId,
                            "BackupPolicy",
                            Boolean.TrueString,
                            null,
                            From.ToString(),
                            To.ToString(),
                            (Skip == null ? null : Skip.ToString()),
                            (First == null ? null : First.ToString()));
                        break;
                }
                WriteObject(backupList, true);
                WriteVerbose("# of backups returned : " + backupList.BackupSetsList.Count);
                if (backupList.NextPageUri != null 
                    && backupList.NextPageStartIdentifier!="1")
                {
                    if (First != null)
                    {
                        //user has provided First(Top) parameter while calling the commandlet
                        //so we need to provide it to him for calling the next page
                        WriteVerbose(String.Format("More backups are available for your query. To access the next page of your result use \"-First {0} -Skip {1}\" in your commandlet", First, backupList.NextPageStartIdentifier));
                    }
                    else
                    {
                        //user has NOT provided First(Top) parameter while calling the commandlet
                        //so we DONT need to provide it to him for calling the next page
                        WriteVerbose(String.Format("More backups are available in the subsequent pages for your query. To access the next page use \"-Skip {0}\"  in your commandlet", backupList.NextPageStartIdentifier));
                    }
                }
                else
                {
                    WriteVerbose("No more backup sets are present for your query!");
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
            if(First<0)
                throw new ArgumentException("Parameter First cannot be <0");
            if (Skip  < 0)
                throw new ArgumentException("Parameter Skip cannot be <0");
            if (Skip == null)
                Skip = 0;
            if (From == null)
                From = DateTime.MinValue;
            if (To == null)
                To = DateTime.MaxValue;
        }

    }
}
