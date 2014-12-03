
using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using System.Collections.Generic;
using System.Net;
using Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets.Library;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    using Properties;

    /// <summary>
    /// Add Azure Storage account to the StorSimple Manager Service
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureStorSimpleStorageAccountCredential"), OutputType(typeof(JobStatusInfo))]

    public class NewAzureStorSimpleStorageAccountCredential : StorSimpleCmdletBase
    {
        [Alias("Name")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageStorageAccountName)]
        [ValidateNotNullOrEmpty]
        public string StorageAccountName { get; set; }

        [Alias("Key")]
        [Parameter(Position = 1, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageStorageAccountKey)]
        [ValidateNotNullOrEmpty]
        public string StorageAccountKey { get; set; }

        [Parameter(Position = 2, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageUseSSL)]
        [ValidateNotNullOrEmpty]
        public bool UseSSL { get; set; }

        [Parameter(Position = 3, Mandatory = false, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageWaitTillComplete)]
        public SwitchParameter WaitForComplete { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                //TODO: Remove
                IKeyManager keyManager = new LocalKeyStoreManager();
                keyManager.PersistKey("JovQDqP1KyWdh4m3mYkdzQ==", "onesdk.dat");
                //end TODO

                String encryptedSecret = null;
                StorSimpleCryptoManager storSimpleCryptoManager = new StorSimpleCryptoManager(this);
                storSimpleCryptoManager.EncryptSecretWithRakPub(StorageAccountKey, out encryptedSecret);

                var serviceConfig = new ServiceConfiguration()
                {
                    AcrChangeList = new AcrChangeList(),
                    CredentialChangeList = new SacChangeList()
                    {
                        Added = new[]
                        {
                            new StorageAccountCredential()
                            {
                                CloudType = CloudType.Azure,        //TODO:
                                Hostname = "blob.core.windows.net", //TODO:
                                Login = StorageAccountName,
                                Password = encryptedSecret,
                                UseSSL = UseSSL,
                                Name = StorageAccountName
                            },
                        },
                        Deleted = new List<string>(),
                        Updated = new List<StorageAccountCredential>()
                    }
                };

                if (WaitForComplete.IsPresent)
                {
                    var jobStatus = StorSimpleClient.ConfigureService(serviceConfig);
                    HandleSyncJobResponse(jobStatus, "create");
                    if (jobStatus.TaskResult == TaskResult.Succeeded)
                    {
                        var createdSac = StorSimpleClient.GetAllStorageAccountCredentials()
                                            .Where(x => x.Name.Equals(StorageAccountName, StringComparison.InvariantCultureIgnoreCase));
                        WriteObject(createdSac);
                    }
                }
                else
                {
                    var jobResponse = StorSimpleClient.ConfigureServiceAsync(serviceConfig);
                    HandleAsyncJobResponse(jobResponse, "create");
                }
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }
    }
}
        
