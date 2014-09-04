
using System.Collections;
using System.Collections.Generic;
using Microsoft.Azure;
using Microsoft.Azure.Management.StorSimple.Models;

namespace Micro.Azure.Commands.StorSimple
{
    public partial class PSStorSimpleClient
    {
        public void CreateAccessControlRecord(string acrName, string iqn)
        {
            var serviceConfiguration = new ServiceConfiguration();
            //var acr1 = new AccessControlRecord {InitiatorName = "ACR101IntiatorName"};

            var acr1 = new AccessControlRecord
            {
                Name = acrName,
                InitiatorName = iqn
            };
            var acrChangeList = new AcrChangeList();
            acrChangeList.Added.Add(acr1);
            serviceConfiguration.AcrChangeList = acrChangeList;
            //serviceConfiguration.AcrChangeList.Added.Add(acr1);
            //var emptySacList = new List<StorageAccountCredential>();
            //serviceConfiguration.CredentialChangeList.Added = emptySacList;
            //serviceConfiguration.CredentialChangeList.Added.Add(new StorageAccountCredential());

            //JobResponse x = GetStorSimpleClient().ServiceConfig.BeginCreatingAsync(serviceConfiguration).Result;

            JobStatusInfo x = GetStorSimpleClient().ServiceConfig.CreateAsync(serviceConfiguration).Result;

        }
    }
}
