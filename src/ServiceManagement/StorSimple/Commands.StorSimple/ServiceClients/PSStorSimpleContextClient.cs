using System;
using System.Linq;
using Microsoft.WindowsAzure;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Commands.StorSimple;

namespace Microsoft.WindowsAzure.Commands.StorSimple
{
    using Properties;

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
                    if (!(resource.Type.Equals("CiSVault", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        continue;
                    }
                    try
                    {
                        var resCredentials = new ResourceCredentials
                        {
                            CloudServiceName = service.Name,
                            ResourceType = resource.Type,
                            BackendStampId = resource.OutputItems["BackendStampId"],
                            ResourceId = resource.OutputItems["ResourceId"],
                            ResourceName = resource.Name,
                            ResourceNameSpace = resource.Namespace,
                            StampId = resource.OutputItems["StampId"],
                            ResourceState = resource.State
                        };

                        toReturn.Add(resCredentials);
                    }
                    catch (Exception)
                    {
                    }
                    
                }
                
            }
            Resourcecache.Add("resourceObject",toReturn, ResourceCachetimeoutPolicy);
            return toReturn;
        }

        public ResourceCredentials GetResourceDetails(string resourceName)
        {
            var resCredList = GetAllResources();
            return resCredList.FirstOrDefault(resCred => resCred.ResourceName.Equals(resourceName, StringComparison.CurrentCultureIgnoreCase));
        }

        public string SetResourceContext(string resourceName)
        {
            var resCred = GetResourceDetails(resourceName);
            if (resCred == null)
            {
                return Resources.NotFoundMessageResource;
            }


            StorSimpleContext.ResourceId = resCred.ResourceId;
            StorSimpleContext.StampId = resCred.BackendStampId;
            StorSimpleContext.CloudServiceName = resCred.CloudServiceName;
            StorSimpleContext.ResourceType = resCred.ResourceType;
            StorSimpleContext.ResourceName = resCred.ResourceName;
            StorSimpleContext.ResourceProviderNameSpace = resCred.ResourceNameSpace;

            return Resources.SuccessMessageSetResourceContext;
        }

        public string GetResourceContext()
        {
            return StorSimpleContext.ResourceName + " :: " + StorSimpleContext.ResourceId + " :: " + StorSimpleContext.CloudServiceName + " :: " + StorSimpleContext.ResourceType + " :: " + StorSimpleContext.ResourceProviderNameSpace + " :: " + StorSimpleContext.StampId; 
        }
    }
}
