﻿﻿// ----------------------------------------------------------------------------------
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

namespace Microsoft.WindowsAzure.Management.Storage.Common.Cmdlet
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Shared.Protocol;
    using System;
    using System.Globalization;
    using System.Management.Automation;
    using System.Security.Permissions;
    using StorageClient = WindowsAzure.Storage.Shared.Protocol;

    /// <summary>
    /// Show azure storage service properties
    /// </summary>
    [Cmdlet(VerbsCommon.Set, StorageNouns.StorageServiceProperties),
        OutputType(typeof(ServiceProperties))]
    public class SetAzureStorageServiceProperties : StorageCloudBlobCmdletBase
    {
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "Azure storage type")]
        [ValidateSet(StorageNouns.BlobService, StorageNouns.TableService, StorageNouns.QueueService,
            IgnoreCase = true)]
        public string Type { get; set; }

        [Parameter(HelpMessage = "Logging version")]
        public double? LoggingVersion { get; set; }

        [Parameter(HelpMessage = "Logging retention days. 0 means disable Logging, otherwise enable.")]
        [ValidateRange(0, 365)]
        public int? LoggingRetentionDays { get; set; }

        public const string LoggingOperationHelpMessage =
            "Logging operations.(All, None, combinations of Read,write,delete which is seperated by semicolon.)";
        [Parameter(HelpMessage = LoggingOperationHelpMessage)]
        public string LoggingOperations { get; set; }

        [Parameter(HelpMessage = "Metrics version")]
        public double? MetricsVersion { get; set; }

        [Parameter(HelpMessage = "Metrics retention days.  0 means disable Metrics, otherwise enable.")]
        [ValidateRange(0, 365)]
        public int? MetricsRetentionDays { get; set; }

        [Parameter(HelpMessage = "Metrics level.(None/Service/ServiceAndApi)")]
        [ValidateSet(StorageNouns.OffMetrics, StorageNouns.MinimalMetrics, StorageNouns.VerboseMetrics,
            IgnoreCase = true)]
        public string MetricsLevel { get; set; }

        [Parameter(HelpMessage = "Service default version")]
        public string DefaultServiceVersion { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Display ServiceProperties")]
        public SwitchParameter PassThru { get; set; }

        /// <summary>
        /// Logging/Metrics default retention days
        /// </summary>
        internal const int DefaultRetentionDays = 1;

        /// <summary>
        /// Minimal logging/metrics retention days.
        /// </summary>
        internal const int MinRetentionDays = 1;

        /// <summary>
        /// Maximal logging/metrics retention days
        /// </summary>
        internal const int MaxRetentionDays = 365;

        /// <summary>
        /// Update the specified service properties according to the input
        /// </summary>
        /// <param name="serviceProperties">Service properties</param>
        internal void UpdateServiceProperties(ServiceProperties serviceProperties)
        {
            if (LoggingVersion != null)
            {
                serviceProperties.Logging.Version = LoggingVersion.ToString();
            }

            if (LoggingRetentionDays != null)
            {
                if (serviceProperties.Logging.RetentionDays == 0)
                {
                    //Disable logging
                    serviceProperties.Logging.RetentionDays = null;
                }
                else
                {
                    serviceProperties.Logging.RetentionDays = LoggingRetentionDays;
                }
            }

            if (LoggingOperations != null)
            {
                LoggingOperations logOperations = GetLoggingOperations(LoggingOperations);
                serviceProperties.Logging.LoggingOperations = logOperations;
            }

            if (MetricsVersion != null)
            {
                serviceProperties.Metrics.Version = MetricsVersion.ToString();
            }

            if (LoggingRetentionDays != null)
            {
                if (serviceProperties.Logging.RetentionDays == 0)
                {
                    //Disable metrics
                    serviceProperties.Metrics.RetentionDays = null;
                }
                else
                {
                    serviceProperties.Metrics.RetentionDays = LoggingRetentionDays;
                }
            }

            if (MetricsLevel != null)
            {
                MetricsLevel metricsLevel = GetMetricsLevel(MetricsLevel);
                serviceProperties.Metrics.MetricsLevel = metricsLevel;
            }

            if (!string.IsNullOrEmpty(DefaultServiceVersion))
            {
                serviceProperties.DefaultServiceVersion = DefaultServiceVersion;
            }
        }

        /// <summary>
        /// Get metrics level
        /// </summary>
        /// <param name="MetricsLevel">The string type of Metrics level</param>
        /// <example>GetMetricsLevel("None"), GetMetricsLevel("Service")</example>
        /// <returns>MetricsLevel object</returns>
        internal MetricsLevel GetMetricsLevel(string MetricsLevel)
        {
            try
            {
                return (MetricsLevel)Enum.Parse(typeof(MetricsLevel), MetricsLevel, true);
            }
            catch 
            {
                throw new ArgumentException(String.Format(Resources.InvalidEnumName, MetricsLevel));
            }
        }

        /// <summary>
        /// Get logging operations
        /// </summary>
        /// <param name="LoggingOperations">The string type of Logging operations</param>
        /// <example>GetLoggingOperations("all"), GetLoggingOperations("read, write")</example>
        /// <returns>LoggingOperations object</returns>
        internal LoggingOperations GetLoggingOperations(string LoggingOperations)
        {
            LoggingOperations = LoggingOperations.ToLower();
            if (LoggingOperations.IndexOf("all") != -1)
            {
                if (LoggingOperations == "all")
                {
                    return StorageClient.LoggingOperations.All;
                }
                else
                {
                    throw new ArgumentException(LoggingOperationHelpMessage);
                }
            }
            else if (LoggingOperations.IndexOf("none") != -1)
            {
                if (LoggingOperations == "none")
                {
                    return StorageClient.LoggingOperations.None;
                }
                else
                {
                    throw new ArgumentException(LoggingOperationHelpMessage);
                }
            }
            else 
            {
                try
                {
                    return (StorageClient.LoggingOperations)Enum.Parse(typeof(StorageClient.LoggingOperations),
                        LoggingOperations, true);
                }
                catch
                {
                    throw new ArgumentException(String.Format(Resources.InvalidEnumName, LoggingOperations));
                }
            }
        }

        /// <summary>
        /// Set storage service properties
        /// </summary>
        /// <param name="account">Cloud storage account</param>
        /// <param name="type">Service type</param>
        /// <param name="serviceProperties">Service properties</param>
        private void SetStorageServiceProperties(CloudStorageAccount account, string type,
            ServiceProperties serviceProperties)
        {
            switch (CultureInfo.CurrentCulture.TextInfo.ToTitleCase(type))
            {
                case StorageNouns.BlobService:
                    account.CreateCloudBlobClient().SetServiceProperties(serviceProperties);
                    break;
                case StorageNouns.QueueService:
                    account.CreateCloudQueueClient().SetServiceProperties(serviceProperties);
                    break;
                case StorageNouns.TableService:
                    account.CreateCloudTableClient().SetServiceProperties(serviceProperties);
                    break;
                default:
                    throw new ArgumentException(Resources.InvalidStorageServiceType);
            }
        }

        /// <summary>
        /// Execute command
        /// </summary>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public override void ExecuteCmdlet()
        {
            CloudStorageAccount account = GetCloudStorageAccount();
            ServiceProperties serviceProperties = 
                GetAzureStorageServiceProperties.GetStorageServiceProperties(account, Type);
            UpdateServiceProperties(serviceProperties);
            SetStorageServiceProperties(account, Type, serviceProperties);

            if (PassThru)
            {
                WriteObject(serviceProperties);
            }
        }
    }
}
