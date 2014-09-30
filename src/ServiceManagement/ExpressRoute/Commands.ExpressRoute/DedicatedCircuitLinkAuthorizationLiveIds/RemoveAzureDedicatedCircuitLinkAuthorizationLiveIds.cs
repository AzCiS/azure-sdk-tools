﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Commands.ExpressRoute
{
    using Microsoft.WindowsAzure.Management.ExpressRoute.Models;
    using System.Management.Automation;
    
    [Cmdlet(VerbsCommon.Remove, "AzureDedicatedCircuitLinkAuthorizationLiveIds")]
    public class RemoveAzureDedicatedCircuitLinkAuthorizationLiveIdsCommand : ExpressRouteBaseCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true,
            HelpMessage = "Service Key representing Azure Circuit")]
        [ValidateGuid]
        [ValidateNotNullOrEmpty]
        public string ServiceKey { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Authorization Id")]
        [ValidateGuid]
        [ValidateNotNullOrEmpty]
        public string AuthId { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Live Ids to be added")]
        public string LiveIds { get; set; }

        public override void ExecuteCmdlet()
        {
            var mapping = ExpressRouteClient.NewAzureDedicatedCircuitLinkAuthorizationLiveIds(ServiceKey, AuthId, LiveIds);
            WriteObject(mapping);
        }
    }
}

