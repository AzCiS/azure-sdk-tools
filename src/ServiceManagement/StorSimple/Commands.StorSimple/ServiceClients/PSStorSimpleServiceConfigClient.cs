﻿
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
        public JobStatusInfo ConfigureService(ServiceConfiguration serviceConfig)
        {
            return GetStorSimpleClient().ServiceConfig.Create(serviceConfig, GetCustomeRequestHeaders());
        }

        public JobResponse ConfigureServiceAsync(ServiceConfiguration serviceConfig)
        {
            return GetStorSimpleClient().ServiceConfig.BeginCreating(serviceConfig, GetCustomeRequestHeaders());
        }

        public IList<AccessControlRecord> GetAllAccessControlRecords()
        {
            var sc = GetStorSimpleClient().ServiceConfig.Get(GetCustomeRequestHeaders());
            if (sc == null || sc.AcrChangeList == null)
            {
                return null;
            }
            return sc.AcrChangeList.Updated;
        }

        public IList<StorageAccountCredential> GetAllStorageAccountCredentials()
        {
            var sc = GetStorSimpleClient().ServiceConfig.Get(GetCustomeRequestHeaders());
            if (sc == null || sc.CredentialChangeList == null)
            {
                return null;
            }
            return sc.CredentialChangeList.Updated;
        }
    }
}
