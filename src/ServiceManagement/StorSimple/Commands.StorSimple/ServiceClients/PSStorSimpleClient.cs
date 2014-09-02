
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Microsoft.Azure.Management.StorSimple;
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Commands.Utilities.Common;
using Microsoft.WindowsAzure.Management.Scheduler;
using Microsoft.WindowsAzure.Management.Scheduler.Models;

// TODO :- Revisit this File again. THe person who starts work on PSScripts needs to review and change

using System.Runtime.Caching;

namespace Micro.Azure.Commands.StorSimple
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Cryptography.X509Certificates;
    using System.Xml;
    using Microsoft.Azure.Management.StorSimple;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Commands.Utilities.Common;
    using Microsoft.WindowsAzure.Management.Scheduler;
    using Microsoft.WindowsAzure.Management.Scheduler.Models;

    public partial class PSStorSimpleClient
    {
        private CloudServiceManagementClient cloudServicesClient;
        private string subscriptionId;
        private X509Certificate2 certificate;
        private Uri serviceEndPoint;
        private string resourceId;
        private string stampId;
        private string cloudServiceName;
        private string resourceProviderNameSpace;
        private string resourceType;
        private string resourceName;

        private ObjectCache Resourcecache = MemoryCache.Default;

        private CacheItemPolicy ResourceCachetimeoutPolicy = new CacheItemPolicy();
        
        public PSStorSimpleClient(WindowsAzureSubscription currentSubscription)
        {
            this.cloudServicesClient = currentSubscription.CreateClient<CloudServiceManagementClient>();
            this.subscriptionId = currentSubscription.SubscriptionId;
            this.serviceEndPoint = currentSubscription.ServiceEndpoint;
            this.certificate = currentSubscription.Certificate;
            ResourceCachetimeoutPolicy.AbsoluteExpiration = DateTimeOffset.Now.AddHours(1.0d);
        }

        public CloudServiceListResponse GetAzureCloudServicesSyncInt()
        {
            return this.cloudServicesClient.CloudServices.List();
        }

        private StorSimpleManagementClient GetStorSimpleClient()
        {
            var services = this.cloudServicesClient.CloudServices.List();

            var selectedService = services.First(s => s.Name.Equals("CisProdCSEA01"));

            var selectedResource = selectedService.Resources.First(
                                                        r => r.Namespace.Equals("WACiS", StringComparison.InvariantCultureIgnoreCase)
                                                             && r.Name.Equals("CisProdResEA01"));

            var stampId = string.Empty;

            foreach (var item in selectedResource.OutputItems)
            {
                if (item.Key.Equals("StampId"))
                {
                    stampId = item.Value;
                }
                else if (item.Key.Equals("ResourceId"))
                {
                    resourceId = item.Value;
                }
            }

            var storSimpleClient = new StorSimpleManagementClient("CisProdResSEA01", "CisProdResSEA01", stampId,
                new CertificateCloudCredentials(this.subscriptionId, this.certificate), this.serviceEndPoint);
            
            if (storSimpleClient == null)
            {
                throw  new InvalidOperationException();
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

        private CustomRequestHeaders GetCustomeRequestHeaders()
        {
            return new CustomRequestHeaders()
            {
                // ClientRequestId is a unique ID for every request to StorSimple .
                // It is useful when diagnosing failures in API calls.
                ClientRequestId = Guid.NewGuid().ToString("D") + "_PS"
            };
        }
        
    }
}
