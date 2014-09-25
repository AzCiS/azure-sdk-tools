using System;
using System.Management.Automation;
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;
using System.Linq;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    /// <summary>
    /// Get a list of Storage accounts from the StorSimple Service config or retrieves a specified Storage Account Cred
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleStorageAccountCredential"), OutputType(typeof(SacChangeList))]
    public class GetAzureStorSimpleStorageAccountCredential : StorSimpleCmdletBase
    {
        [Alias("Name")]
        [Parameter(Position = 0, Mandatory = false, HelpMessage = "The storage account name.")]
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
                    var sac = allSACs.Where(x => x.Name.Equals(StorageAccountName)).FirstOrDefault();
                    if (sac == null)
                    {
                        WriteObject("Storage Account with the given name doesn't exist");
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