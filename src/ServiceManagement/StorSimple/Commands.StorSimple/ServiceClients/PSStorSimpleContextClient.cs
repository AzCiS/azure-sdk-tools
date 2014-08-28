using System;
using Microsoft.WindowsAzure;
using System.Collections.Generic;
using Microsoft.Azure.Commands.StorSimple;

namespace Micro.Azure.Commands.StorSimple
{
    public partial class PSStorSimpleClient
    {
        
        public IEnumerable<ResourceCredentials> GetAllResources()
        {
            var services = cloudServicesClient.CloudServices.List();
            var toReturn = new List<ResourceCredentials>();

            
            foreach (var service in services)
            {

                if (service.Resources.Count == 0)
                {
                    continue;
                }
                foreach (var resource in service.Resources)
                {
                    if (!(resource.Type.Equals("CiSVault", StringComparison.CurrentCultureIgnoreCase)) || !(resource.Namespace.Equals("WACiS", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        continue;
                    }
                    var resCredentials = new ResourceCredentials();
                    resCredentials.CloudServiceName = service.Name;
                    resCredentials.ResourceType = resource.Type;
                    resCredentials.BackendStampId = resource.OutputItems["BackendStampId"];
                    resCredentials.ResourceId = resource.OutputItems["ResourceId"];
                    resCredentials.ResourceName = resource.Name;
                    resCredentials.ResourceNameSpace = resource.Namespace;
                    resCredentials.StampId = resource.OutputItems["StampId"];
                    resCredentials.ResourceState = resource.State;

                    toReturn.Add(resCredentials);
                }
                
            }
            Resourcecache.Add("resourceObject",toReturn, ResourceCachetimeoutPolicy);
            return toReturn;
        }

        public ResourceCredentials GetResourceDetails(string resourceName)
        {
            IEnumerable<ResourceCredentials> resCredList = ((IEnumerable<ResourceCredentials>)Resourcecache.Get("resourceObject"));
            if (resCredList == null)
            {
                resCredList = GetAllResources();
                
            }

            foreach(ResourceCredentials resCred in resCredList)
            {
                if (!resCred.ResourceName.Equals(resourceName,StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }
                return resCred;
            }
            return null;
        }

        public string SetResourceContext(string resourceName)
        {
            var resCred = GetResourceDetails(resourceName);
            if (resCred == null)
            {
                return "No resource found for the given resource name. Please check the name again.";
            }

            this.resourceId = resCred.ResourceId;
            this.stampId = resCred.StampId;
            this.cloudServiceName = resCred.CloudServiceName;
            this.resourceType = resCred.ResourceType;
            this.resourceName = resCred.ResourceName;
            this.resourceProviderNameSpace = resCred.ResourceNameSpace;

            return "Context set successfully for the given resource name.";
        }

        public string GetResourceContext()
        {
            return this.resourceName + " :: " + this.resourceId + " :: " + this.cloudServiceName + " :: " + this.resourceType + " :: " + this.resourceProviderNameSpace + " :: " + this.stampId; 
        }
    }
}
