﻿

// TODO :- Revisit this File again. THe person who starts work on PSScripts needs to review and change

using System.Net;
using System.Net.Security;
using System.Runtime.Caching;
using Microsoft.WindowsAzure.Commands.Common.Models;

namespace Microsoft.Azure.Commands.StorSimple
{
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Microsoft.Azure.Management.StorSimple;
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Commands.Utilities.Common;
using Microsoft.WindowsAzure.Management.Scheduler;
using Microsoft.WindowsAzure.Management.Scheduler.Models;
    using Microsoft.WindowsAzure.Commands.Common;

    public partial class PSStorSimpleClient
    {
        private CloudServiceManagementClient cloudServicesClient;
        
        private ObjectCache Resourcecache = MemoryCache.Default;

        private CacheItemPolicy ResourceCachetimeoutPolicy = new CacheItemPolicy();

        public PSStorSimpleClient(AzureSubscription currentSubscription)
        {
            // Temp code to be able to test internal env.
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };//IgnoreCertificateErrorHandler;//delegate { return true; };
            
            this.cloudServicesClient = AzureSession.ClientFactory.CreateClient<CloudServiceManagementClient>(currentSubscription, AzureEnvironment.Endpoint.ServiceManagement);
            
            ResourceCachetimeoutPolicy.AbsoluteExpiration = DateTimeOffset.Now.AddHours(1.0d);
        }

        public CloudServiceListResponse GetAzureCloudServicesSyncInt()
        {
            return this.cloudServicesClient.CloudServices.List();
        }

        private StorSimpleManagementClient GetStorSimpleClient()
        {
            var storSimpleClient =
                AzureSession.ClientFactory.CreateCustomClient<StorSimpleManagementClient>(
                    StorSimpleContext.CloudServiceName,
                    StorSimpleContext.ResourceName, StorSimpleContext.ResourceId,
                    StorSimpleContext.ResourceProviderNameSpace, StorSimpleContext.StampId,
                    this.cloudServicesClient.Credentials,
                    AzureSession.CurrentContext.Environment.GetEndpointAsUri(AzureEnvironment.Endpoint.ServiceManagement));
            
            if (storSimpleClient == null)
            {
                throw new InvalidOperationException();
            }

            return storSimpleClient;

        }

        public void ThrowCloudExceptionDetails(CloudException cloudException)
        {
            Error error = null;
            try
            {
                using (Stream stream = new MemoryStream())
                {
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(cloudException.ErrorMessage);
                    stream.Write(data, 0, data.Length);
                    stream.Position = 0;

                    var deserializer = new DataContractSerializer(typeof(Error));
                    error = (Error)deserializer.ReadObject(stream);
                }
            }
            catch (XmlException)
            {
                throw new XmlException(cloudException.ErrorMessage);
            }
            catch (SerializationException)
            {
                throw new SerializationException(cloudException.ErrorMessage);
            }

            throw new InvalidOperationException(
                string.Format(error.Message,"\n",error.HttpCode,"\n",error.ExtendedCode));
        }
        
        private CustomRequestHeaders GetCustomRequestHeaders()
        {
            return new CustomRequestHeaders()
            {
                // ClientRequestId is a unique ID for every request to StorSimple .
                // It is useful when diagnosing failures in API calls.
                ClientRequestId = Guid.NewGuid().ToString("D") + "_PS",
                Language = "en-US"
            };
        }

        private static bool IgnoreCertificateErrorHandler
           (object sender,
           System.Security.Cryptography.X509Certificates.X509Certificate certificate,
           System.Security.Cryptography.X509Certificates.X509Chain chain,
           SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        
    }
}
