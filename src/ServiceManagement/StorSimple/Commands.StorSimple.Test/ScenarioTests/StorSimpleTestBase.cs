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

using Microsoft.Azure.Management.StorSimple;
using Microsoft.WindowsAzure.Commands.ScenarioTest;
using Microsoft.WindowsAzure.Commands.Utilities.Common;
using Microsoft.WindowsAzure.Testing;
using System.Management;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.StorSimple.Test.ScenarioTests
{
    public class StorSimpleTestBase
    {
        private EnvironmentSetupHelper helper;
        protected const string TenantIdKey = "TenantId";

        protected StorSimpleTestBase()
        {
            helper = new EnvironmentSetupHelper();
        }

        //protected void SetupManagementClients()
        //{
        //    var storSimpleManagementClient = GetStorSimpleClient();
        //    var cloudServiceClient = GetCloudServiceClient();
        //    helper.SetupManagementClients(storSimpleManagementClient);
        //}

        //private StorSimpleManagementClient GetStorSimpleClient()
        //{
        //    TestBase.
        //}

        protected void RunPowerShellTest(params string[] scripts)
        {
            using (UndoContext context = UndoContext.Current)
            {
                context.Start(TestUtilities.GetCallingClass(2), TestUtilities.GetCurrentMethodName(2));
                //SetupManagementClients();
                //helper.SetupEnvironment(AzureModule.AzureServiceManagement);
                helper.SetupModules(AzureModule.AzureServiceManagement, "ScenarioTests\\" + this.GetType().Name + ".ps1");
                helper.RunPowerShellTest(scripts);
            }
        }
    }
}