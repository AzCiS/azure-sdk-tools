using System;
using System.Management.Automation;
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;
using System.Linq;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    /// <summary>
    /// Get a list of Access Control Records present in the StorSimple Manager Service Configuration or retrieves a specific named ACR Object
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleAccessControlRecord"), OutputType(typeof(AcrChangeList))]
    public class GetAzureStorSimpleAccessControlRecord : StorSimpleCmdletBase
    {
        [Alias("Name")]
        [Parameter(Position = 0, Mandatory = false, HelpMessage = "The access control record name.")]
        public string ACRName { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                var allACRs = StorSimpleClient.GetAllAccessControlRecords();
                if (ACRName == null)
                {
                    WriteObject(allACRs);
                }
                else
                {
                    var acr = allACRs.Where(x => x.Name.Equals(ACRName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (acr == null)
                    {
                        WriteObject("Access control record with the given name doesn't exist");
                    }
                    else
                    {
                        WriteObject(acr);
                    }
                }
            }
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
            }
        }
    }
}