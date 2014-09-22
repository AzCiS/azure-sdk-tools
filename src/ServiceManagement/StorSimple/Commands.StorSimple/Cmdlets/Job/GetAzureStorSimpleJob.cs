using System;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleJob")]
    public class GetAzureStorSimpleJob : StorSimpleCmdletBase
    {

        [Alias("JobId")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The job instance id.")]
        [ValidateNotNullOrEmptyAttribute]
        public string InstanceId { get; set; }

        public override void ExecuteCmdlet()
        {
            var jobStatus = StorSimpleClient.GetJobStatus(InstanceId);
            this.WriteObject(jobStatus);
        }
    }
}