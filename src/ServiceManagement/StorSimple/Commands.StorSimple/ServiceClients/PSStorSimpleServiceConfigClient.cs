
using System.Collections;
using System.Collections.Generic;
using Microsoft.Azure;
using Microsoft.Azure.Management.StorSimple.Models;

namespace Microsoft.Azure.Commands.StorSimple
{
    public partial class PSStorSimpleClient
    {
        public void CreateAccessControlRecord(string acrName, string iqn)
        {
            var serviceConfiguration = new ServiceConfiguration();
            //var acr1 = new AccessControlRecord {InitiatorName = "ACR101IntiatorName"};
            serviceConfiguration.CredentialChangeList = new SacChangeList();
            var acr1 = new AccessControlRecord
            {
                Name = acrName,
                InitiatorName = iqn,
                GlobalId = null,
                InstanceId = null,
                VolumeCount = 0,
            };
            var acrChangeList = new AcrChangeList();
            acrChangeList.Added.Add(acr1);
            serviceConfiguration.AcrChangeList = acrChangeList;
            
            

            //JobResponse x = GetStorSimpleClient().ServiceConfig.BeginCreatingAsync(serviceConfiguration).Result;

            JobStatusInfo x = GetStorSimpleClient().ServiceConfig.CreateAsync(serviceConfiguration,GetCustomeRequestHeaders()).Result;

        }
    }
}
