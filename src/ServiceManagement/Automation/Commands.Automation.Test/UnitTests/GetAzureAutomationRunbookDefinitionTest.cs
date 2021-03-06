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

namespace Microsoft.Azure.Commands.Automation.Test.UnitTests
{
    using Microsoft.Azure.Commands.Automation.Cmdlet;
    using Microsoft.Azure.Commands.Automation.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Commands.Test.Utilities.Common;
    using Moq;
    using System;

    [TestClass]
    public class GetAzureAutomationRunbookDefinitionTest : TestBase
    {
        private Mock<IAutomationClient> mockAutomationClient;

        private MockCommandRuntime mockCommandRuntime;

        private GetAzureAutomationRunbookDefinition cmdlet;

        [TestInitialize]
        public void SetupTest()
        {
            this.mockAutomationClient = new Mock<IAutomationClient>();
            this.mockCommandRuntime = new MockCommandRuntime();
            this.cmdlet = new GetAzureAutomationRunbookDefinition
                              {
                                  AutomationClient = this.mockAutomationClient.Object,
                                  CommandRuntime = this.mockCommandRuntime
                              };
        }

        [TestMethod]
        public void GetAzureAutomationRunbookDefinitionByRunbookIdWithoutSlotSuccessfull()
        {
            // Setup
            string accountName = "automation";
            var runbookId = new Guid();

            this.mockAutomationClient.Setup(f => f.ListRunbookDefinitionsByRunbookId(accountName, runbookId, null));

            // Test
            this.cmdlet.AutomationAccountName = accountName;
            this.cmdlet.Id = runbookId;
            this.cmdlet.ExecuteCmdlet();

            // Assert
            this.mockAutomationClient.Verify(f => f.ListRunbookDefinitionsByRunbookId(accountName, runbookId, null), Times.Once());
        }

        [TestMethod]
        public void GetAzureAutomationRunbookDefinitionByRunbookIdSlotDraftSuccessfull()
        {
            // Setup
            string accountName = "automation";
            var runbookId = new Guid();

            this.mockAutomationClient.Setup(f => f.ListRunbookDefinitionsByRunbookId(accountName, runbookId, true));

            // Test
            this.cmdlet.AutomationAccountName = accountName;
            this.cmdlet.Id = runbookId;
            this.cmdlet.Slot = "Draft";
            this.cmdlet.ExecuteCmdlet();

            // Assert
            this.mockAutomationClient.Verify(f => f.ListRunbookDefinitionsByRunbookId(accountName, runbookId, true), Times.Once());
        }

        [TestMethod]
        public void GetAzureAutomationRunbookDefinitionByRunbookNameWithoutSlotSuccessfull()
        {
            // Setup
            string accountName = "automation";
            string runbookName = "runbook";

            this.mockAutomationClient.Setup(f => f.ListRunbookDefinitionsByRunbookName(accountName, runbookName, null));

            // Test
            this.cmdlet.AutomationAccountName = accountName;
            this.cmdlet.Name = runbookName;
            this.cmdlet.ExecuteCmdlet();

            // Assert
            this.mockAutomationClient.Verify(f => f.ListRunbookDefinitionsByRunbookName(accountName, runbookName, null), Times.Once());
        }

        [TestMethod]
        public void GetAzureAutomationRunbookDefinitionByRunbookNameSlotPublishedSuccessfull()
        {
            // Setup
            string accountName = "automation";
            string runbookName = "runbook";

            this.mockAutomationClient.Setup(f => f.ListRunbookDefinitionsByRunbookName(accountName, runbookName, false));

            // Test
            this.cmdlet.AutomationAccountName = accountName;
            this.cmdlet.Name = runbookName;
            this.cmdlet.Slot = "Published";
            this.cmdlet.ExecuteCmdlet();

            // Assert
            this.mockAutomationClient.Verify(f => f.ListRunbookDefinitionsByRunbookName(accountName, runbookName, false), Times.Once());
        }

        [TestMethod]
        public void GetAzureAutomationRunbookDefinitionByRunbookVersionIdWithoutSlotSuccessfull()
        {
            // Setup
            string accountName = "automation";
            var runbookVersionId = new Guid();

            this.mockAutomationClient.Setup(
                f => f.ListRunbookDefinitionsByRunbookVersionId(accountName, runbookVersionId, null));

            // Test
            this.cmdlet.AutomationAccountName = accountName;
            this.cmdlet.VersionId = runbookVersionId;
            this.cmdlet.ExecuteCmdlet();

            // Assert
            this.mockAutomationClient.Verify(f => f.ListRunbookDefinitionsByRunbookVersionId(accountName, runbookVersionId, null), Times.Once());
        }
    }
}
