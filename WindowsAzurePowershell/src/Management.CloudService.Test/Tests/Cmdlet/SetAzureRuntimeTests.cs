﻿// ----------------------------------------------------------------------------------
//
// Copyright 2011 Microsoft Corporation
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

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Management.CloudService.Cmdlet;
using Microsoft.WindowsAzure.Management.CloudService.Model;
using Microsoft.WindowsAzure.Management.CloudService.Properties;
using Microsoft.WindowsAzure.Management.CloudService.Test.Utilities;
using System.Management.Automation;

namespace Microsoft.WindowsAzure.Management.CloudService.Test.Tests.Cmdlet
{
    [TestClass]
    public class SetAzureRuntimeTests : TestBase
    {
        private FakeWriter writer;

        private SetAzureServiceProjectRoleCommand cmdlet;

        private const string serviceName = "AzureService";

        public static void VerifyPackageJsonVersion(string servicePath, string roleName, string runtime, string version)
        {
            string packagePath = Path.Combine(servicePath, roleName);
            string actualVersion;
            Assert.IsTrue(JavaScriptPackageHelpers.TryGetEngineVersion(packagePath, runtime, out actualVersion));
            Assert.AreEqual(version, actualVersion, true);
        }

        public static void VerifyInvalidPackageJsonVersion(string servicePath, string roleName, string runtime, string version)
        {
            string packagePath = Path.Combine(servicePath, roleName);
            string actualVersion;
            Assert.IsFalse(JavaScriptPackageHelpers.TryGetEngineVersion(packagePath, runtime, out actualVersion));
        }

        [TestInitialize]
        public void TestSetup()
        {
            writer = new FakeWriter();
            cmdlet = new SetAzureServiceProjectRoleCommand();
            cmdlet.Writer = writer;
        }

        /// <summary>
        /// Verify that adding valid role runtimes results in valid changes in the commandlet scaffolding 
        /// (in this case, valid package.json changes).  Test for both a valid node runtiem version and 
        /// valid iisnode runtiem version
        /// </summary>
        [TestMethod]
        public void TestSetAzureRuntimeValidRuntimeVersions()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                AzureService service = new AzureService(files.RootPath, serviceName, null);
                service.AddWebRole(Resources.NodeScaffolding);
                string roleName = "WebRole1";
                
                cmdlet.SetAzureRuntimesProcess(roleName, "node", "0.8.2", service.Paths.RootPath, RuntimePackageHelper.GetTestManifest(files));
                cmdlet.SetAzureRuntimesProcess(roleName, "iisnode", "0.1.21", service.Paths.RootPath, RuntimePackageHelper.GetTestManifest(files));
                VerifyPackageJsonVersion(service.Paths.RootPath, roleName, "node", "0.8.2");
                VerifyPackageJsonVersion(service.Paths.RootPath, roleName, "iisnode", "0.1.21");
                Assert.AreEqual<string>(roleName, ((PSObject)writer.OutputChannel[0]).Members[Parameters.RoleName].Value.ToString());
                Assert.AreEqual<string>(roleName, ((PSObject)writer.OutputChannel[1]).Members[Parameters.RoleName].Value.ToString());
            }
        }

        /// <summary>
        /// Test that attempting to set an invlaid runtime version (one that is not listed in the runtime manifest) 
        /// results in no changes to package scaffolding (no changes in package.json)
        /// </summary>
        [TestMethod]
        public void TestSetAzureRuntimeInvalidRuntimeVersion()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                AzureService service = new AzureService(files.RootPath, serviceName, null);
                service.AddWebRole(Resources.NodeScaffolding);
                string roleName = "WebRole1";
                cmdlet.SetAzureRuntimesProcess(roleName, "node", "0.8.99", service.Paths.RootPath, RuntimePackageHelper.GetTestManifest(files));
                cmdlet.SetAzureRuntimesProcess(roleName, "iisnode", "0.9.99", service.Paths.RootPath, RuntimePackageHelper.GetTestManifest(files));
                VerifyInvalidPackageJsonVersion(service.Paths.RootPath, roleName, "node", "*");
                VerifyInvalidPackageJsonVersion(service.Paths.RootPath, roleName, "iisnode", "*");
                Assert.AreEqual<string>(roleName, ((PSObject)writer.OutputChannel[0]).Members[Parameters.RoleName].Value.ToString());
                Assert.AreEqual<string>(roleName, ((PSObject)writer.OutputChannel[1]).Members[Parameters.RoleName].Value.ToString());
            }
        }

        /// <summary>
        /// Test that attempting to add a runtime with an invlid runtime type (a runtime type that has no entries in the 
        /// master package.json).  Results in no scaffolding changes - no changes to package.json.
        /// </summary>
        [TestMethod]
        public void TestSetAzureRuntimeInvalidRuntimeType()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                AzureService service = new AzureService(files.RootPath, serviceName, null);
                string roleName = "WebRole1";
                service.AddWebRole(Resources.NodeScaffolding);
                cmdlet.SetAzureRuntimesProcess(roleName, "noide", "0.8.99", service.Paths.RootPath, RuntimePackageHelper.GetTestManifest(files));
                cmdlet.SetAzureRuntimesProcess(roleName, "iisnoide", "0.9.99", service.Paths.RootPath, RuntimePackageHelper.GetTestManifest(files));
                VerifyInvalidPackageJsonVersion(service.Paths.RootPath, roleName, "node", "*");
                VerifyInvalidPackageJsonVersion(service.Paths.RootPath, roleName, "iisnode", "*");
                Assert.AreEqual<string>(roleName, ((PSObject)writer.OutputChannel[0]).Members[Parameters.RoleName].Value.ToString());
                Assert.AreEqual<string>(roleName, ((PSObject)writer.OutputChannel[1]).Members[Parameters.RoleName].Value.ToString());
            }
        }
    }
}