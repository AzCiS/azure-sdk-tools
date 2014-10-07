using System;
using System.Management.Automation;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;
using System.Linq;
using Microsoft.Azure.Commands.StorSimple.Properties;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    using Properties;

    /// <summary>
    /// Get a list of Storage accounts from the StorSimple Service config or retrieves a specified Storage Account Cred
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleStorageAccountCredential"), OutputType(typeof(SacChangeList))]
    public class GetAzureStorSimpleStorageAccountCredential : StorSimpleCmdletBase
    {
        [Alias("Name")]
        [Parameter(Position = 0, Mandatory = false, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageStorageAccountName)]
        public string StorageAccountName { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                var allSACs = StorSimpleClient.GetAllStorageAccountCredentials();
                if (StorageAccountName == null)
                {
                    WriteObject(allSACs);
                }
                else
                {
                    var sac = allSACs.Where(x => x.Name.Equals(StorageAccountName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (sac == null)
                    {
                        WriteObject(Resources.NotFoundMessageStorageAccount);
                    }
                    else
                    {
                        WriteObject(sac);
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