﻿using System.Linq;
using System.Management.Automation;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Commands.Utilities.CloudService;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    /// <summary>
    /// commandlet that returns one or more BackupPolicy objects for a given DeviceName and BackupPolicyName
    /// </summary>
     [Cmdlet(VerbsCommon.Get, "AzureStorSimpleDeviceBackupPolicy")]
    public class GetAzureStorSimpleDeviceBackupPolicy:StorSimpleCmdletBase
    {
        private const string PARAMETERSET_BACKUPPOLICYNAME = "ByBackupPolicyName";
        private const string PARAMETERSET_DEFAULT = "Default";
        private string deviceId = null;

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_DEFAULT)]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.", ParameterSetName = PARAMETERSET_BACKUPPOLICYNAME)]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceName { get; set; }

        [Parameter(Position = 1, Mandatory = true, HelpMessage = "Give the name of the backuppolicy if available.", ParameterSetName = PARAMETERSET_BACKUPPOLICYNAME)]
        public String BackupPolicyName { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                ProcessParameters();
                switch (ParameterSetName)
                {
                    case PARAMETERSET_BACKUPPOLICYNAME:
                        GetBackupPolicyDetailsResponse backupPolicyDetail = null;
                        backupPolicyDetail = StorSimpleClient.GetBackupPolicyByName(deviceId, BackupPolicyName);
                        WriteVerbose(String.Format("BackupPolicy with id {0} found!",backupPolicyDetail.BackupPolicyDetails.InstanceId));
                        WriteObject(backupPolicyDetail);
                        break;

                    default:
                        BackupPolicyListResponse backupPolicyList = null;
                        backupPolicyList = StorSimpleClient.GetAllBackupPolicies(deviceId);
                        WriteVerbose("# of backups policies returned : " + backupPolicyList.BackupPolicies.Count);
                        WriteObject(backupPolicyList);
                        break;
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
                 throw new ArgumentException("Device with name " + DeviceName + " not found");
            if (ParameterSetName == PARAMETERSET_BACKUPPOLICYNAME
                && String.IsNullOrEmpty(BackupPolicyName))
                throw new ArgumentException("BackupPolicyName parameter is specified but value is empty. Please skip this parameter to get all backuppolicies on the device");
        }
    }

    }

