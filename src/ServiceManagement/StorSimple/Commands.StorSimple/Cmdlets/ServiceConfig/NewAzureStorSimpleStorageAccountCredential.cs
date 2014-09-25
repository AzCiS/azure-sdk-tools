
using System;
using System.Management.Automation;
using Microsoft.WindowsAzure;
using Microsoft.Azure.Management.StorSimple.Models;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    /// <summary>
    /// Add Azure Storage account to the StorSimple Manager Service
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureStorSimpleStorageAccountCredential")]

    public class NewAzureStorSimpleStorageAccountCredential : StorSimpleCmdletBase
    {
        [Alias("Name")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The storage account name.")]
        [ValidateNotNullOrEmpty]
        public string StorageAccountName { get; set; }

        [Alias("Key")]
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The Key.")]
        [ValidateNotNullOrEmpty]
        public string StorageAccountKey { get; set; }

        [Parameter(Position = 2, Mandatory = true, HelpMessage = "Whether to use SSL")]
        [ValidateNotNullOrEmpty]
        public bool UseSSL { get; set; }

        [Alias("WaitForCompletion")]
        [Parameter(Position = 3, Mandatory = false, HelpMessage = "Wait for the task to complete")]
        public SwitchParameter WaitForComplete { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                var serviceConfig = new ServiceConfiguration()
                {
                    AcrChangeList = new AcrChangeList(),
                    CredentialChangeList = new SacChangeList()
                    {
                        Added = new[]
                        {
                            new StorageAccountCredential()
                            {
                                CloudType = CloudType.Azure,
                                Hostname = string.Empty,
                                Login = string.Empty,
                                Password = StorageAccountKey,
                                UseSSL = UseSSL,
                                VolumeCount = 0,
                                Name = StorageAccountName,
                                PasswordEncryptionCertThumbprint = string.Empty
                            },
                        },
                        Deleted = new List<string>(),
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
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
            }
        }
    }
}
        
