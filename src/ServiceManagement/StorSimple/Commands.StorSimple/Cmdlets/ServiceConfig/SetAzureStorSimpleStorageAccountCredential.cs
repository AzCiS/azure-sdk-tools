
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
    /// Edit the Storage Account Cred
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "AzureStorSimpleStorageAccountCredential")]

    public class SetAzureStorSimpleStorageAccountCredential : StorSimpleCmdletBase
    {
        [Alias("Name")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The storage account name.")]
        [ValidateNotNullOrEmpty]
        public string StorageAccountName { get; set; }

        [Alias("Key")]
        [Parameter(Position = 1, Mandatory = false, HelpMessage = "The Key.")]
        [ValidateNotNullOrEmpty]
        public string StorageAccountKey { get; set; }

        [Parameter(Position = 2, Mandatory = false, HelpMessage = "Whether to use SSL")]
        [ValidateNotNullOrEmpty]
        public bool? UseSSL { get; set; }

        [Alias("WaitForCompletion")]
        [Parameter(Position = 3, Mandatory = false, HelpMessage = "Wait for the task to complete")]
        public SwitchParameter WaitForComplete { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {

                var allSACs = StorSimpleClient.GetAllStorageAccountCredentials();
                var existingSac = allSACs.Where(x => x.Name.Equals(StorageAccountName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (existingSac == null)
                {
                    WriteObject("Storage account with the given Name doesn't exist.");
                }
                else
                {
                    var serviceConfig = new ServiceConfiguration()
                    {
                        AcrChangeList = new AcrChangeList(),
                        CredentialChangeList = new SacChangeList()
                        {
                            Added = new List<StorageAccountCredential>(),
                            Deleted = new List<string>(),
                            Updated = new[]
                            {
                                new StorageAccountCredential()
                                {
                                    CloudType = existingSac.CloudType,
                                    Hostname = existingSac.Hostname,
                                    Login = existingSac.Login,
                                    Password = StorageAccountKey ?? existingSac.Password,
                                    UseSSL = UseSSL ?? existingSac.UseSSL,
                                    VolumeCount = existingSac.VolumeCount,
                                    Name = existingSac.Name,
                                    PasswordEncryptionCertThumbprint = existingSac.PasswordEncryptionCertThumbprint
                                },
                            }
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

