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

namespace Microsoft.WindowsAzure.Commands.Websites.WebHostingPlan
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using Utilities.Websites.Common;
    using Utilities.Websites.Services.WebEntities;

    /// <summary>
    /// Gets an azure website.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureWebHostingPlanMetric"), OutputType(typeof(IList<MetricResponse>))]
    public class GetAzureWebHostingPlanMetricCommand : WebHostingPlanContextBaseCmdlet
    {
        [Parameter(Position = 2, Mandatory = false, ValueFromPipelineByPropertyName = true,
            HelpMessage = "List of metrics names to retrieve.")]
        [ValidateNotNullOrEmpty]
        public string[] MetricNames { get; set; }

        [Parameter(Position = 3, Mandatory = false, ValueFromPipelineByPropertyName = true,
            HelpMessage = "The start time.")]
        [ValidateNotNullOrEmpty]
        public DateTime? StartDate { get; set; }

        [Parameter(Position = 4, Mandatory = false, ValueFromPipelineByPropertyName = true,
            HelpMessage = "The end time.")]
        public DateTime? EndDate { get; set; }

        [Parameter(Position = 5, Mandatory = false, ValueFromPipelineByPropertyName = true,
            HelpMessage = "Time grain for the metrics. Supported values are PT1M (per minute), PT1H (per hour), P1D (per day).")]
        public string TimeGrain { get; set; }

        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();

            var response = WebsitesClient.GetPlanHistoricalUsageMetrics(WebSpaceName, Name, MetricNames, StartDate, EndDate, TimeGrain);
            foreach (var metricResponse in response)
            {
                WriteObject(metricResponse, true);
            }
        }
    }
}