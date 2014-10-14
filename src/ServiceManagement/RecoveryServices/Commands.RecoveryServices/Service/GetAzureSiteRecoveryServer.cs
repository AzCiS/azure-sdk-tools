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

namespace Microsoft.Azure.Commands.RecoveryServices
{
    #region Using directives
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using Microsoft.Azure.Commands.RecoveryServices.SiteRecovery;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Management.SiteRecovery.Models;
    #endregion

    /// <summary>
    /// Retrieves Azure Site Recovery Server.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureSiteRecoveryServer", DefaultParameterSetName = ASRParameterSets.Default)]
    [OutputType(typeof(IEnumerable<ASRServer>))]
    public class GetAzureSiteRecoveryServer : RecoveryServicesCmdletBase
    {
        #region Parameters
        /// <summary>
        /// Server ID.
        /// </summary>
        private string id;

        /// <summary>
        /// Name of the Server.
        /// </summary>
        private string name;

        /// <summary>
        /// Gets or sets ID of the Server.
        /// </summary>
        [Parameter(ParameterSetName = ASRParameterSets.ById, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>
        /// Gets or sets name of the Server.
        /// </summary>
        [Parameter(ParameterSetName = ASRParameterSets.ByName, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        #endregion Parameters

        /// <summary>
        /// ProcessRecord of the command.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case ASRParameterSets.ByName:
                        this.GetByName();
                        break;
                    case ASRParameterSets.ById:
                        this.GetById();
                        break;
                    case ASRParameterSets.Default:
                        this.GetAll();
                        break;
                }
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }

        /// <summary>
        /// Queries by name.
        /// </summary>
        private void GetByName()
        {
            ServerListResponse serverListResponse =
                RecoveryServicesClient.GetAzureSiteRecoveryServer();

            bool found = false;
            foreach (Server server in serverListResponse.Servers)
            {
                if (0 == string.Compare(this.name, server.Name, true))
                {
                    this.WriteServer(server);
                    found = true;
                }
            }

            if (!found)
            {
                throw new InvalidOperationException(
                    string.Format(
                    Properties.Resources.ServerNotFound,
                    this.name,
                    PSRecoveryServicesClient.asrVaultCreds.ResourceName));
            }
        }

        /// <summary>
        /// Queries by ID.
        /// </summary>
        private void GetById()
        {
            ServerResponse serverResponse =
                RecoveryServicesClient.GetAzureSiteRecoveryServer(this.id);

            this.WriteServer(serverResponse.Server);
        }

        /// <summary>
        /// Queries all / by default.
        /// </summary>
        private void GetAll()
        {
            ServerListResponse serverListResponse =
                RecoveryServicesClient.GetAzureSiteRecoveryServer();

            this.WriteServers(serverListResponse.Servers);
        }

        /// <summary>
        /// Write Servers.
        /// </summary>
        /// <param name="servers">List of Servers</param>
        private void WriteServers(IList<Server> servers)
        {
            foreach (Server server in servers)
            {
                this.WriteServer(server);
            }
        }

        /// <summary>
        /// Write Server.
        /// </summary>
        /// <param name="server">Server object</param>
        private void WriteServer(Server server)
        {
            this.WriteObject(new ASRServer(server), true);
        }
    }
}