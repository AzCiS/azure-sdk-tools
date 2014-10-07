using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Commands.StorSimple;
using Microsoft.Azure.Management.StorSimple.Models;

namespace Microsoft.Azure.Commands.StorSimple
{
    public partial class PSStorSimpleClient
    {
        public VirtualDiskListResponse GetAllVolumesFordataContainer(string deviceid,string datacontainerid)
        {
            return GetStorSimpleClient().VirtualDisk.List(deviceid, datacontainerid, GetCustomeRequestHeaders());
        }

        public VirtualDiskGetResponse GetVolumeByName(string deviceid, string diskName)
        {
            return GetStorSimpleClient().VirtualDisk.GetByName(deviceid, diskName, GetCustomeRequestHeaders());
            return null;
        }

        public VirtualDiskGetResponse GetVolumeById(string deviceid, string diskId)
        {
            return GetStorSimpleClient().VirtualDisk.Get(deviceid, diskId, GetCustomeRequestHeaders());
        }

        public JobStatusInfo CreateVolume(string deviceid, VirtualDiskRequest diskDetails)
        {
            return GetStorSimpleClient().VirtualDisk.Create(deviceid, diskDetails, GetCustomeRequestHeaders());
        }

        public GuidJobResponse CreateVolumeAsync(string deviceid, VirtualDiskRequest diskDetails)
        {
            return GetStorSimpleClient().VirtualDisk.BeginCreating(deviceid, diskDetails, GetCustomeRequestHeaders());
        }

        public JobStatusInfo RemoveVolume(string deviceid, string diskid)
        {
            return GetStorSimpleClient().VirtualDisk.Delete(deviceid, diskid, GetCustomeRequestHeaders());
        }
        public GuidJobResponse RemoveVolumeAsync(string deviceid, string diskid)
        {
            return GetStorSimpleClient().VirtualDisk.BeginDeleting(deviceid, diskid, GetCustomeRequestHeaders());
        }

        public JobStatusInfo UpdateVolume(string deviceid, string diskid, VirtualDisk diskDetails)
        {
            return GetStorSimpleClient().VirtualDisk.Update(deviceid, diskid, diskDetails, GetCustomeRequestHeaders());
        }
        public GuidJobResponse UpdateVolumeAsync(string deviceid, string diskid, VirtualDisk diskDetails)
        {
            return GetStorSimpleClient().VirtualDisk.BeginUpdating(deviceid, diskid, diskDetails,GetCustomeRequestHeaders());
        }
    }
}