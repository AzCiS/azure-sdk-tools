﻿// ----------------------------------------------------------------------------------
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

namespace Microsoft.WindowsAzure.Management.Utilities.Common
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class WindowsAzureEnvironment
    {
        /// <summary>
        /// The Windows Azure environment name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The management portal endpoint.
        /// </summary>
        public string PortalEndpoint { get; set; }

        /// <summary>
        /// The service management RDFE endpoint.
        /// </summary>
        public string ServiceEndpoint { get; set; }

        /// <summary>
        /// Predefined Windows Azure environments
        /// </summary>
        public static Dictionary<string, WindowsAzureEnvironment> PublicEnvironments
        {
            get { return environments; }
            private set { environments = value; }
        }

        private static Dictionary<string, WindowsAzureEnvironment> environments = 
            new Dictionary<string, WindowsAzureEnvironment>(StringComparer.InvariantCultureIgnoreCase)
        {
            {
                EnvironmentName.Azure,
                new WindowsAzureEnvironment()
                {
                    Name = EnvironmentName.Azure,
                    PortalEndpoint = WindowsAzureEnvironmentConstants.AzurePortalUrl,
                    ServiceEndpoint = WindowsAzureEnvironmentConstants.AzureServiceEndpoint
                }
            },
            {
                EnvironmentName.China,
                new WindowsAzureEnvironment()
                {
                    Name = EnvironmentName.China,
                    PortalEndpoint = WindowsAzureEnvironmentConstants.ChinaPortalUrl,
                    ServiceEndpoint = WindowsAzureEnvironmentConstants.ChinaServiceEndpoint
                }
            }
        };
    }
}
