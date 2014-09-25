
using System;
using System.Management.Automation;
using Microsoft.WindowsAzure;
using Microsoft.Azure.Management.StorSimple.Models;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    /// <summary>
    /// Add New Access Control Record to the StorSimple Manager Service Configuration
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureStorSimpleAccessControlRecord")]

    public class NewAzureStorSimpleAccessControlRecord : StorSimpleCmdletBase
    {
        [Alias("Name")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The access control record name.")]
        [ValidateNotNullOrEmpty]
        public string ACRName { get; set; }

        [Alias("IQN")]
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The IQN.")]
        [ValidateNotNullOrEmpty]
        public string IQNInitiatorName { get; set; }

        [Alias("WaitForCompletion")]
        [Parameter(Position = 2, Mandatory = false, HelpMessage = "Wait for the task to complete")]
        public SwitchParameter WaitForComplete { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                var serviceConfig = new ServiceConfiguration()
                {
                    AcrChangeList = new AcrChangeList()
                    {
                        Added = new[]
                        {
                            new AccessControlRecord()
                            {
                                GlobalId = null,
                                InitiatorName = IQNInitiatorName,
                                InstanceId = null,
                                Name = ACRName,
                                VolumeCount = 0
                            },
                        },
                        Deleted = new List<string>(),
                        Updated = new List<AccessControlRecord>()
                    },
                    CredentialChangeList = new SacChangeList(),
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
        
