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

namespace Microsoft.WindowsAzure.Commands.Scheduler
{
    using Microsoft.WindowsAzure.Commands.Utilities.Scheduler;
    using Microsoft.WindowsAzure.Commands.Utilities.Scheduler.Model;
    using System;
    using System.Collections;
    using System.Management.Automation;

    /// <summary>
    /// Cmdlet to patch HttpJob
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "AzureSchedulerStorageQueueJob"), OutputType(typeof(string))]
    public class SetSchedulerStorageQueueJobCommand : SchedulerBaseCmdlet
    {
        const string RequiredParamSet = "Required";
        const string RecurringParamSet = "Recurring";

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = RequiredParamSet, HelpMessage = "The location name.")]
        [ValidateNotNullOrEmpty]
        public string Location { get; set; }

        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = RequiredParamSet, HelpMessage = "The job collection name.")]
        [ValidateNotNullOrEmpty]
        public string JobCollectionName { get; set; }

        [Parameter(Position = 2, Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = RequiredParamSet, HelpMessage = "The job name.")]
        [ValidateNotNullOrEmpty]
        public string JobName { get; set; }

        [Parameter(Position = 3, Mandatory = false, ValueFromPipelineByPropertyName = true, ParameterSetName = RequiredParamSet, HelpMessage = "The Storage account name.")]
        [ValidateNotNullOrEmpty]
        public string StorageQueueAccount { get; set; }

        [Parameter(Position = 4, Mandatory = false, ValueFromPipelineByPropertyName = true, ParameterSetName = RequiredParamSet, HelpMessage = "The Storage Queue name.")]
        [ValidateNotNullOrEmpty]
        public string StorageQueueName { get; set; }

        [Parameter(Position = 5, Mandatory = false, ValueFromPipelineByPropertyName = true, ParameterSetName = RequiredParamSet, HelpMessage = "The SAS token for storage queue.")]
        [ValidateNotNullOrEmpty]
        public string SASToken { get; set; }

        [Parameter(Position = 6, Mandatory = false, ValueFromPipelineByPropertyName = false, ParameterSetName = RequiredParamSet, HelpMessage = "The Body for PUT, POST and Storage job actions.")]
        [ValidateNotNullOrEmpty]
        public string StorageQueueMessage { get; set; }

        [Parameter(Position = 8, Mandatory = false, ValueFromPipelineByPropertyName = false, ParameterSetName = RequiredParamSet, HelpMessage = "The Start Time")]
        [ValidateNotNullOrEmpty]
        public DateTime? StartTime { get; set; }

        [Parameter(Position = 9, Mandatory = false, ValueFromPipelineByPropertyName = false, ParameterSetName = RequiredParamSet, HelpMessage = "The frequency count for recurring schedule")]
        [Parameter(Position = 9, Mandatory = false, ValueFromPipelineByPropertyName = false, ParameterSetName = RecurringParamSet, HelpMessage = "The frequency count for recurring schedule")]
        [ValidateNotNullOrEmpty]
        public int? Interval { get; set; }

        [Parameter(Position = 10, Mandatory = false, ValueFromPipelineByPropertyName = false, ParameterSetName = RequiredParamSet, HelpMessage = "The interval for recurring schedule")]
        [Parameter(Position = 10, Mandatory = false, ValueFromPipelineByPropertyName = false, ParameterSetName = RecurringParamSet, HelpMessage = "The interval for recurring schedule")]
        [ValidateSet("Minute", "Hour", "Day", "Week", "Month", "Year", IgnoreCase = true)]
        public string Frequency { get; set; }

        [Parameter(Position = 11, Mandatory = false, ValueFromPipelineByPropertyName = false, ParameterSetName = RequiredParamSet, HelpMessage = "The End Time")]
        [Parameter(Position = 11, Mandatory = false, ValueFromPipelineByPropertyName = false, ParameterSetName = RecurringParamSet, HelpMessage = "The End Time")]
        [ValidateNotNullOrEmpty]
        public DateTime? EndTime { get; set; }

        [Parameter(Position = 12, Mandatory = false, ValueFromPipelineByPropertyName = false, ParameterSetName = RecurringParamSet, HelpMessage = "Count of occurrences that will execute. Optional. Default will recur infinitely")]
        [Parameter(Position = 12, Mandatory = false, ValueFromPipelineByPropertyName = false, ParameterSetName = RequiredParamSet, HelpMessage = "Count of occurrences that will execute. Optional. Default will recur infinitely")]
        public int? ExecutionCount { get; set; }

        [Parameter(Position = 13, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The job state.")]
        [ValidateSet("Enabled", "Disabled", IgnoreCase = true)]
        public string JobState { get; set; }

        [Parameter(Position = 14, Mandatory = false, ValueFromPipelineByPropertyName = false, ParameterSetName = RequiredParamSet, HelpMessage = "The Method for Http and Https Action types (GET, PUT, POST, HEAD or DELETE).")]
        [ValidateSet("GET", "PUT", "POST", "HEAD", "DELETE", IgnoreCase = true)]
        public string ErrorActionMethod { get; set; }

        [Parameter(Position = 15, Mandatory = false, ValueFromPipelineByPropertyName = false, ParameterSetName = RequiredParamSet, HelpMessage = "The Uri for error job action.")]
        public Uri ErrorActionURI { get; set; }

        [Parameter(Position = 16, Mandatory = false, ValueFromPipelineByPropertyName = false, ParameterSetName = RequiredParamSet, HelpMessage = "The Body for PUT and POST job actions.")]
        [ValidateNotNullOrEmpty]
        public string ErrorActionRequestBody { get; set; }

        [Parameter(Position = 17, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The header collection.")]
        public Hashtable ErrorActionHeaders { get; set; }

        [Parameter(Position = 18, Mandatory = false, ValueFromPipelineByPropertyName = true, ParameterSetName = RequiredParamSet, HelpMessage = "The Storage account name.")]
        [ValidateNotNullOrEmpty]
        public string ErrorActionStorageAccount { get; set; }

        [Parameter(Position = 19, Mandatory = false, ValueFromPipelineByPropertyName = true, ParameterSetName = RequiredParamSet, HelpMessage = "The Storage Queue name.")]
        [ValidateNotNullOrEmpty]
        public string ErrorActionStorageQueue { get; set; }

        [Parameter(Position = 20, Mandatory = false, ValueFromPipelineByPropertyName = true, ParameterSetName = RequiredParamSet, HelpMessage = "The SAS token for storage queue.")]
        [ValidateNotNullOrEmpty]
        public string ErrorActionSASToken { get; set; }

        [Parameter(Position = 21, Mandatory = false, ValueFromPipelineByPropertyName = false, ParameterSetName = RequiredParamSet, HelpMessage = "The Body for Storage job actions.")]
        [ValidateNotNullOrEmpty]
        public string ErrorActionQueueMessageBody { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter PassThru { get; set; }

        public override void ExecuteCmdlet()
        {
            string status = string.Empty;           
            if (PassThru.IsPresent)
            {
                WriteObject(SMClient.PatchStorageJob(new PSCreateJobParams
                {
                    Region = Location,
                    JobCollectionName = JobCollectionName,
                    JobName = JobName,
                    StorageAccount = StorageQueueAccount,
                    QueueName = StorageQueueName,
                    SasToken = SASToken,
                    StorageQueueMessage = StorageQueueMessage,
                    StartTime = StartTime,
                    Interval = Interval,
                    Frequency = Frequency,
                    EndTime = EndTime,
                    ExecutionCount = ExecutionCount,
                    JobState = JobState,
                    ErrorActionMethod = ErrorActionMethod,
                    ErrorActionBody = ErrorActionRequestBody,
                    ErrorActionHeaders = ErrorActionHeaders,
                    ErrorActionUri = ErrorActionURI,
                    ErrorActionStorageAccount = ErrorActionStorageAccount,
                    ErrorActionQueueName = ErrorActionStorageQueue,
                    ErrorActionQueueBody = ErrorActionQueueMessageBody,
                    ErrorActionSasToken = ErrorActionSASToken
                }, out status), true);
                WriteObject(status);
            }
            else
            {
                SMClient.PatchStorageJob(new PSCreateJobParams
                {
                    Region = Location,
                    JobCollectionName = JobCollectionName,
                    JobName = JobName,
                    StorageAccount = StorageQueueAccount,
                    QueueName = StorageQueueName,
                    SasToken = SASToken,
                    StorageQueueMessage = StorageQueueMessage,
                    StartTime = StartTime,
                    Interval = Interval,
                    Frequency = Frequency,
                    EndTime = EndTime,
                    ExecutionCount = ExecutionCount,
                    JobState = JobState,
                    ErrorActionMethod = ErrorActionMethod,
                    ErrorActionBody = ErrorActionRequestBody,
                    ErrorActionHeaders = ErrorActionHeaders,
                    ErrorActionUri = ErrorActionURI,
                    ErrorActionStorageAccount = ErrorActionStorageAccount,
                    ErrorActionQueueName = ErrorActionStorageQueue,
                    ErrorActionQueueBody = ErrorActionQueueMessageBody,
                    ErrorActionSasToken = ErrorActionSASToken
                }, out status);
                WriteDebug(status);
            }
        }
    }
}
