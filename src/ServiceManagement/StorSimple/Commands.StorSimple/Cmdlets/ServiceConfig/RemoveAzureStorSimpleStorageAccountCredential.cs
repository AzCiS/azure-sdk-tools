
using System;
using System.Management.Automation;
using Microsoft.WindowsAzure;
using Microsoft.Azure.Management.StorSimple.Models;
using System.Collections.Generic;
using System.Net;
using System.Linq;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    /// <summary>
    /// Removes the Storage Account Cred specified from the StorSimple Service Config
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "AzureStorSimpleStorageAccountCredential")]

    public class RemoveAzureStorSimpleStorageAccountCredential : StorSimpleCmdletBase
    {
        [Alias("Name")]
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyByName, HelpMessage = "The storage account name.")]
        [ValidateNotNullOrEmpty]
        public string StorageAccountName { get; set; }

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyByObject, ValueFromPipeline = true, HelpMessage = "The SAC object.")]
        [ValidateNotNullOrEmpty]
        public StorageAccountCredential SAC { get; set; }

        [Alias("WaitForCompletion")]
        [Parameter(Position = 1, Mandatory = false, HelpMessage = "Wait for the task to complete")]
        public SwitchParameter WaitForComplete { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                StorageAccountCredential existingSac = null;
                switch(ParameterSetName)
                {
                    case StorSimpleCmdletParameterSet.IdentifyByName:
                        var allSACs = StorSimpleClient.GetAllStorageAccountCredentials();
                        existingSac = allSACs.Where(x => x.Name.Equals(StorageAccountName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        break;
                    case StorSimpleCmdletParameterSet.IdentifyByObject:
                        existingSac = SAC;
                        break;
                }
                if (existingSac == null)
                {
                    WriteObject("Specified Storage Account doesn't exist.");
                }
                else
                {
                    var serviceConfig = new ServiceConfiguration()
                    {
                        AcrChangeList = new AcrChangeList(),
                        CredentialChangeList = new SacChangeList()
                        {
                            Added = new List<StorageAccountCredential>(),
                            Deleted = new[] { existingSac.InstanceId },
                            Updated = new List<StorageAccountCredential>()
                        }
                    };

                    if (WaitForComplete.IsPresent)
                    {
                        var jobStatus = StorSimpleClient.ConfigureService(serviceConfig);
                        WriteObject(jobStatus);
                    }
                    else
                    {
                        var jobResponse = StorSimpleClient.ConfigureServiceAsync(serviceConfig);
                        WriteObject(ToAsyncJobMessage(jobResponse));
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

