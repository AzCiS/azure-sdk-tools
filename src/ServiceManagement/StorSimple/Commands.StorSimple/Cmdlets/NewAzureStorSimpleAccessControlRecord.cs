
using System;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    /// <summary>
    /// Creates a new resource group.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureStorSimpleAccessControlRecord")]

    public class NewAzureStorSimpleAccessControlRecord : StorSimpleCmdletBase
    {
        [Alias("AccessControlRecordName")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The access control record name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Alias("IQNName")]
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The IQN.")]
        [ValidateNotNullOrEmpty]
        public string Iqn { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                StorSimpleClient.CreateAccessControlRecord(Name,Iqn);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
        
