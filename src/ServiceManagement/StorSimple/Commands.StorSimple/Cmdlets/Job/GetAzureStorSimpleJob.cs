using System;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleJob")]
    public class GetAzureStorSimpleJob : StorSimpleCmdletBase
    {

        [Alias("InstanceId")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The job instance id.")]
        [ValidateNotNullOrEmptyAttribute]
        public string JobId { get; set; }

        public override void ExecuteCmdlet()
        {
            var jobStatus = StorSimpleClient.GetJobStatus(JobId);
            this.WriteObject(jobStatus);
        }
    }
}