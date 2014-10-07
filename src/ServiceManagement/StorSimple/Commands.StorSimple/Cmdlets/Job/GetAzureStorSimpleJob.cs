using System;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    using Properties;

    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleJob")]
    public class GetAzureStorSimpleJob : StorSimpleCmdletBase
    {

        [Alias("JobId")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageJobId)]
        [ValidateNotNullOrEmptyAttribute]
        public string InstanceId { get; set; }

        public override void ExecuteCmdlet()
        {
            var jobStatus = StorSimpleClient.GetJobStatus(InstanceId);
            this.WriteObject(jobStatus);
        }
    }
}