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
    /// Retrieves Azure Site Recovery Virtual Machine.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureSiteRecoveryVM", DefaultParameterSetName = ASRParameterSets.ByObject)]
    [OutputType(typeof(IEnumerable<ASRVirtualMachine>))]
    public class GetAzureSiteRecoveryVM : RecoveryServicesCmdletBase
    {
        #region Parameters
        /// <summary>
        /// Virtual Machine ID.
        /// </summary>
        private string id;

        /// <summary>
        /// Name of the Virtual Machine.
        /// </summary>
        private string name;

        /// <summary>
        /// Protection Container ID.
        /// </summary>
        private string protectionContainerId;

        /// <summary>
        /// Protection Container object.
        /// </summary>
        private ASRProtectionContainer protectionContainer;

        /// <summary>
        /// Gets or sets ID of the Virtual Machine.
        /// </summary>
        [Parameter(ParameterSetName = ASRParameterSets.ByObjectWithId, Mandatory = true)]
        [Parameter(ParameterSetName = ASRParameterSets.ByIDsWithId, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>
        /// Gets or sets name of the Virtual Machine.
        /// </summary>
        [Parameter(ParameterSetName = ASRParameterSets.ByObjectWithName, Mandatory = true)]
        [Parameter(ParameterSetName = ASRParameterSets.ByIDsWithName, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Gets or sets ID of the ProtectionContainer containing the Virtual Machine.
        /// </summary>
        [Parameter(ParameterSetName = ASRParameterSets.ByIDs, Mandatory = true)]
        [Parameter(ParameterSetName = ASRParameterSets.ByIDsWithId, Mandatory = true)]
        [Parameter(ParameterSetName = ASRParameterSets.ByIDsWithName, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string ProtectionContainerId
        {
            get { return this.protectionContainerId; }
            set { this.protectionContainerId = value; }
        }

        /// <summary>
        /// Gets or sets Protection Container Object.
        /// </summary>
        [Parameter(ParameterSetName = ASRParameterSets.ByObject, Mandatory = true, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = ASRParameterSets.ByObjectWithId, Mandatory = true)]
        [Parameter(ParameterSetName = ASRParameterSets.ByObjectWithName, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public ASRProtectionContainer ProtectionContainer
        {
            get { return this.protectionContainer; }
            set { this.protectionContainer = value; }
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
                    case ASRParameterSets.ByObject:
                    case ASRParameterSets.ByObjectWithId:
                    case ASRParameterSets.ByObjectWithName:
                        this.protectionContainerId = this.protectionContainer.ID;
                        break;
                    case ASRParameterSets.ByIDs:
                    case ASRParameterSets.ByIDsWithId:
                    case ASRParameterSets.ByIDsWithName:
                        break;
                }

                if (this.id != null)
                {
                    this.GetById();
                }
                else if (this.name != null)
                {
                    this.GetByName();
                }
                else
                {
                    this.GetAll();
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
            VirtualMachineListResponse virtualMachineListResponse =
                RecoveryServicesClient.GetAzureSiteRecoveryVirtualMachine(
                this.protectionContainerId);

            bool found = false;
            foreach (VirtualMachine vm in virtualMachineListResponse.Vms)
            {
                if (0 == string.Compare(this.name, vm.Name, true))
                {
                    this.WriteVirtualMachine(vm);
                    found = true;
                }
            }

            if (!found)
            {
                throw new InvalidOperationException(
                    string.Format(
                    Properties.Resources.VirtualMachineNotFound,
                    this.name,
                    this.protectionContainerId));
            }
        }

        /// <summary>
        /// Queries by ID.
        /// </summary>
        private void GetById()
        {
            VirtualMachineResponse virtualMachineResponse =
                RecoveryServicesClient.GetAzureSiteRecoveryVirtualMachine(
                this.protectionContainerId,
                this.id);

            this.WriteVirtualMachine(virtualMachineResponse.Vm);
        }

        /// <summary>
        /// Queries all / by default.
        /// </summary>
        private void GetAll()
        {
            VirtualMachineListResponse virtualMachineListResponse =
                RecoveryServicesClient.GetAzureSiteRecoveryVirtualMachine(
                this.protectionContainerId);

            this.WriteVirtualMachines(virtualMachineListResponse.Vms);
        }

        /// <summary>
        /// Writes Virtual Machines.
        /// </summary>
        /// <param name="vms">List of Virtual Machines</param>
        private void WriteVirtualMachines(IList<VirtualMachine> vms)
        {
            foreach (VirtualMachine vm in vms)
            {
                this.WriteVirtualMachine(vm);
            }
        }

        /// <summary>
        /// Writes Virtual Machine.
        /// </summary>
        /// <param name="vm">Virtual Machine</param>
        private void WriteVirtualMachine(VirtualMachine vm)
        {
            this.WriteObject(
                new ASRVirtualMachine(
                    vm.ID,
                    vm.ServerId,
                    vm.ProtectionContainerId,
                    vm.Name,
                    vm.Type,
                    vm.FabricObjectId,
                    vm.Protected,
                    vm.CanCommit,
                    vm.CanFailover,
                    vm.CanReverseReplicate,
                    vm.ActiveLocation,
                    vm.ProtectionStateDescription,
                    vm.TestFailoverStateDescription,
                    vm.ReplicationHealth,
                    vm.ReplicationProvider),
                true);
        }
    }
}