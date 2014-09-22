
using System.Collections;
using System.Collections.Generic;
using Microsoft.Azure;
using Microsoft.Azure.Management.StorSimple.Models;
using System;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.StorSimple
{
    public partial class PSStorSimpleClient
    {
        public JobStatusInfo CreateAccessControlRecord(string acrName, string iqn, SwitchParameter waitForComplete)
        {
            var client = GetStorSimpleClient();

            var serviceConfig = new ServiceConfiguration()
            {
                AcrChangeList = new AcrChangeList()
                {
                    Added = new[]
                        {
                            new AccessControlRecord()
                            {
                GlobalId = null,
                                InitiatorName = iqn,
                InstanceId = null,
                                Name = acrName,
                                VolumeCount = 0
                            },
                        },
                    Deleted = new List<string>(),
                    Updated = new List<AccessControlRecord>()
                },
                CredentialChangeList = new SacChangeList(),
            };
            
            CustomRequestHeaders hdrs = new CustomRequestHeaders();
            hdrs.ClientRequestId = Guid.NewGuid().ToString();
            hdrs.Language = "en-us";
            
            JobStatusInfo jobStatus = new JobStatusInfo();

            if (waitForComplete.IsPresent)
            {
                jobStatus = client.ServiceConfig.Create(serviceConfig, hdrs);
            }

            else
            {
                jobStatus = client.ServiceConfig.CreateAsync(serviceConfig, hdrs).Result;
            }

            return jobStatus;
        }

        public IList<AccessControlRecord> GetAccessControlRecord()
        {
            var sc = GetStorSimpleClient().ServiceConfig.Get(GetCustomeRequestHeaders());
            //sc.AcrChangeList.Added.
            return sc.AcrChangeList.Updated;
        }
    }
}
