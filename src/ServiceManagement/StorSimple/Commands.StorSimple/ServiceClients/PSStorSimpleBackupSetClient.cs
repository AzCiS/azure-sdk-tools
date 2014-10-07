using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure.Commands.CloudService.Development;

namespace Microsoft.Azure.Commands.StorSimple
{
    public partial class PSStorSimpleClient
    {
        public GetBackupResponse GetAllBackups(string deviceId, string filterType, string isAllSelected,string filterValue, string startDateTime, string endDateTime,
            string skip, string top)
        {
            return this.GetStorSimpleClient()
                .Backup.Get(deviceId, filterType, isAllSelected, filterValue, startDateTime, endDateTime, skip, top, GetCustomRequestHeaders());
        }

        public JobStatusInfo DeleteBackup(string deviceid, string backupSetId)
        {
            return GetStorSimpleClient().Backup.Delete(deviceid, backupSetId, GetCustomRequestHeaders());
        }

        public JobResponse DeleteBackupAsync(string deviceid, string backupSetId)
        {
            return GetStorSimpleClient().Backup.BeginDeleting(deviceid, backupSetId, GetCustomRequestHeaders());
        }

        public JobStatusInfo RestoreBackup(string deviceid, RestoreBackupRequest backupRequest)
        {
            return GetStorSimpleClient().Backup.Restore(deviceid, backupRequest, GetCustomeRequestHeaders());
        }

        public JobResponse RestoreBackupAsync(string deviceid, RestoreBackupRequest backupRequest)
        {
            return GetStorSimpleClient().Backup.BeginRestoring(deviceid, backupRequest, GetCustomeRequestHeaders());
        }

        public JobStatusInfo DoBackup(string deviceid,String backupPolicyId, BackupNowRequest request)
        {
            return GetStorSimpleClient().Backup.Create(deviceid, backupPolicyId,request, GetCustomeRequestHeaders());
        }

        public JobResponse DoBackupAsync(string deviceid, String backupPolicyId, BackupNowRequest request)
        {
            return GetStorSimpleClient().Backup.BeginCreatingBackup(deviceid, backupPolicyId, request, GetCustomeRequestHeaders());
        }
    }
}
