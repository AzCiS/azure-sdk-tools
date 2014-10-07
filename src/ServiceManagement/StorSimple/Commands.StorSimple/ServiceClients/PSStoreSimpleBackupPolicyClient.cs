﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using Microsoft.WindowsAzure.Commands.CloudService.Development;

namespace Microsoft.WindowsAzure.Commands.StorSimple
{
    public partial class PSStorSimpleClient
    {
        public BackupPolicyListResponse GetAllBackupPolicies(string deviceId)
        {
            return this.GetStorSimpleClient().BackupPolicy.List(deviceId, GetCustomRequestHeaders());
        }

        public GetBackupPolicyDetailsResponse GetBackupPolicyByName(string deviceId, string backupPolicyName)
        {
            return this.GetStorSimpleClient().BackupPolicy.GetBackupPolicyDetailsByName(deviceId, backupPolicyName, GetCustomRequestHeaders());
        }

        public JobStatusInfo DeleteBackupPolicy(string deviceid, string backupPolicyId)
        {
            return GetStorSimpleClient().BackupPolicy.Delete(deviceid, backupPolicyId, GetCustomRequestHeaders());
        }

        public JobResponse DeleteBackupPolicyAsync(string deviceid, string backupPolicyId)
        {
            return GetStorSimpleClient().BackupPolicy.BeginDeleting(deviceid, backupPolicyId, GetCustomRequestHeaders());
        }

        public JobStatusInfo CreateBackupPolicy(string deviceId, NewBackupPolicyConfig config)
        {
            return GetStorSimpleClient().BackupPolicy.Create(deviceId, config, GetCustomRequestHeaders());
        }

        public JobResponse CreateBackupPolicyAsync(string deviceId, NewBackupPolicyConfig config)
        {
            return GetStorSimpleClient().BackupPolicy.BeginCreatingBackupPolicy(deviceId, config, GetCustomRequestHeaders());
        }
    }
}
