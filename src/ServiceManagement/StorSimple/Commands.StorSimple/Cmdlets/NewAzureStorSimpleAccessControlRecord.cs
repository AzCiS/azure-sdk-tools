
using System;
using System.Management.Automation;
using Microsoft.WindowsAzure;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    /// <summary>
    /// Creates a new ACR
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureStorSimpleAccessControlRecord")]

    public class NewAzureStorSimpleAccessControlRecord : StorSimpleCmdletBase
    {
        [Alias("Name")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The access control record name.")]
        [ValidateNotNullOrEmpty]
        public string AccessControlRecordName { get; set; }

        [Alias("IQN")]
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The IQN.")]
        [ValidateNotNullOrEmpty]
        public string Iqn { get; set; }

        [Parameter(Position = 2, Mandatory = false, HelpMessage = "Wait for copy task complete")]
        public SwitchParameter WaitForComplete { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                var jobStatus = StorSimpleClient.CreateAccessControlRecord(AccessControlRecordName, Iqn, WaitForComplete);
                if (WaitForComplete.IsPresent)
                {
                    this.WriteObject(jobStatus);
                }
                else
                {
                    this.WriteObject(jobStatus);
                }
            }
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
            }
        }
    }
}
        
