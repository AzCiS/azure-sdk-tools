﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure.Commands.CloudService.Development;

namespace Microsoft.Azure.Commands.StorSimple
{
    public partial class PSStorSimpleClient
    {
        public BackupPolicyListResponse GetAllBackupPolicies(string deviceId)
        {
            return this.GetStorSimpleClient().BackupPolicy.List(deviceId, GetCustomeRequestHeaders());
        }

        public GetBackupPolicyDetailsResponse GetBackupPolicyByName(string deviceId, string backupPolicyName)
        {
            return this.GetStorSimpleClient().BackupPolicy.GetBackupPolicyDetailsByName(deviceId, backupPolicyName, GetCustomeRequestHeaders());
        }

        public JobStatusInfo DeleteBackupPolicy(string deviceid, string backupPolicyId)
        {
            return GetStorSimpleClient().BackupPolicy.Delete(deviceid, backupPolicyId, GetCustomeRequestHeaders());
        }

        public JobResponse DeleteBackupPolicyAsync(string deviceid, string backupPolicyId)
        {
            return GetStorSimpleClient().BackupPolicy.BeginDeleting(deviceid, backupPolicyId, GetCustomeRequestHeaders());
        }

        public JobStatusInfo CreateBackupPolicy(string deviceId, NewBackupPolicyConfig config)
        {
            return GetStorSimpleClient().BackupPolicy.Create(deviceId, config, GetCustomeRequestHeaders());
        }

        public JobResponse CreateBackupPolicyAsync(string deviceId, NewBackupPolicyConfig config)
        {
            return GetStorSimpleClient().BackupPolicy.BeginCreatingBackupPolicy(deviceId, config, GetCustomeRequestHeaders());
        }
    }
}
