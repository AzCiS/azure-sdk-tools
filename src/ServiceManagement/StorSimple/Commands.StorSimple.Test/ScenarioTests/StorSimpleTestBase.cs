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

using System;
using System.Reflection;
using Microsoft.Azure.Utilities.HttpRecorder;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Commands.ScenarioTest;
using Microsoft.WindowsAzure.Commands.Utilities.Common;
using Microsoft.WindowsAzure.Management.Scheduler;
using Microsoft.WindowsAzure.Management.StorSimple;
using Microsoft.WindowsAzure.Testing;
using System.Management;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.StorSimple.Test.ScenarioTests
{
    public class StorSimpleTestBase
    {
        private EnvironmentSetupHelper helper;
        private RDFETestEnvironmentFactory rdfeTestFactory;

        protected StorSimpleTestBase()
        {
            this.helper = new EnvironmentSetupHelper();
            this.rdfeTestFactory = new RDFETestEnvironmentFactory();
        }

        protected void SetupManagementClients()
        {
            var storSimpleManagementClient = GetStorSimpleClient();
            var cloudServiceClient = GetCloudServiceClient();
            helper.SetupManagementClients(storSimpleManagementClient, cloudServiceClient);

            //helper.SetupSomeOfManagementClients();
        }

        private StorSimpleManagementClient GetStorSimpleClient()
        {
            try
            {
                var testEnvironment = this.rdfeTestFactory.GetTestEnvironment();
                //var storSimpleClient = TestBase.GetServiceClient<StorSimpleManagementClient>(this.rdfeTestFactory);
                var storSimpleClient = new StorSimpleManagementClient("", "", "", "", "",
                    testEnvironment.Credentials as SubscriptionCloudCredentials, testEnvironment.BaseUri).WithHandler(HttpMockServer.CreateInstance());
                return storSimpleClient;
            }
            catch (ReflectionTypeLoadException leException)
            {
                
                throw leException;
            }
            
        }

        private CloudServiceManagementClient GetCloudServiceClient()
        {
            return TestBase.GetServiceClient<CloudServiceManagementClient>(this.rdfeTestFactory);
        }

        protected void RunPowerShellTest(params string[] scripts)
        {
            try
            {
                using (UndoContext context = UndoContext.Current)
                {
                    context.Start(TestUtilities.GetCallingClass(2), TestUtilities.GetCurrentMethodName(2));

                    
                    helper.SetupEnvironment(AzureModule.AzureServiceManagement);
                    SetupManagementClients();

                    helper.SetupModules(AzureModule.AzureServiceManagement, "ScenarioTests\\" + this.GetType().Name + ".ps1");
                    helper.RunPowerShellTest(scripts);
                }
            }
            catch (TypeInitializationException ex)
            {

                
                throw ex;
            }
            
        }
    }
}