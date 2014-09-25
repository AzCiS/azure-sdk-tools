
using System.Collections;
using System.Collections.Generic;
using Microsoft.Azure;
using Microsoft.Azure.Management.StorSimple.Models;
using System;
using System.Management.Automation;
using Microsoft.Azure.Management.StorSimple.Models;

namespace Microsoft.Azure.Commands.StorSimple
{
    public partial class PSStorSimpleClient
    {
        public JobStatusInfo CreateAccessControlRecord(ServiceConfiguration serviceConfig)
        {
            return GetStorSimpleClient().ServiceConfig.Create(serviceConfig, GetCustomeRequestHeaders());
        }

        public JobResponse CreateAccessControlRecordAsync(ServiceConfiguration serviceConfig)
        {
            return GetStorSimpleClient().ServiceConfig.BeginCreating(serviceConfig, GetCustomeRequestHeaders());
        }

        public IList<AccessControlRecord> GetAccessControlRecord()
        {
            var sc = GetStorSimpleClient().ServiceConfig.Get(GetCustomeRequestHeaders());
            //sc.AcrChangeList.Added.
            return sc.AcrChangeList.Updated;
        }
    }
}
