
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Azure;
using Microsoft.Azure.Commands.StorSimple;
using Microsoft.Azure.Management.StorSimple.Models;

namespace Microsoft.Azure.Commands.StorSimple
{
    public partial class PSStorSimpleClient
    {
        public DataContainerListResponse GetAllDataContainers(string deviceId)
        {
            return this.GetStorSimpleClient().DataContainer.List(deviceId, this.GetCustomeRequestHeaders());
        }

        public JobStatusInfo GetJobStatus(string jobId)
        {
            return GetStorSimpleClient().GetOperationStatus(jobId);
        }
        public JobStatusInfo CreateDataContainer(string deviceId,DataContainerRequest dc)
        {
                return GetStorSimpleClient().DataContainer.Create(deviceId, dc, GetCustomeRequestHeaders());
           
        }

        public JobResponse CreateDataContainerAsync(string deviceId, DataContainerRequest dc)
        {
            return GetStorSimpleClient().DataContainer.BeginCreating(deviceId, dc, GetCustomeRequestHeaders());
        }


        public DataContainerGetResponse GetDataContainer(string deviceId, string Name)
        {
            return GetStorSimpleClient().DataContainer.Get(deviceId, Name, GetCustomeRequestHeaders());
        }
    }
}