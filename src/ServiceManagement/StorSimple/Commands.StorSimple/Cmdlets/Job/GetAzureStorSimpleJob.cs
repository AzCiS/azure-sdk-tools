﻿using System;
using System.Management.Automation;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    using Microsoft.WindowsAzure.Management.StorSimple.Models;
    using Properties;

    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleJob"), OutputType(typeof(JobStatusInfo))]
    public class GetAzureStorSimpleJob : StorSimpleCmdletBase
    {

        [Alias("JobId")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageJobId)]
        [ValidateNotNullOrEmpty]
        public string InstanceId { get; set; }

        public override void ExecuteCmdlet()
        {
            var jobStatus = StorSimpleClient.GetJobStatus(InstanceId);
            this.WriteObject(jobStatus);
        }
    }
}