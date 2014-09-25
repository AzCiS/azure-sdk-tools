
using System;
using System.Management.Automation;
using Microsoft.WindowsAzure;
using Microsoft.Azure.Management.StorSimple.Models;
using System.Collections.Generic;
using System.Net;

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

        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The IQN.")]
        [ValidateNotNullOrEmpty]
        public string IQN { get; set; }

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
                                InitiatorName = IQN,
                                InstanceId = null,
                                Name = AccessControlRecordName,
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
                    var jobStatus = StorSimpleClient.CreateAccessControlRecord(serviceConfig);
                    WriteObject(jobStatus);
                }
                else
                {
                    var jobResponse = StorSimpleClient.CreateAccessControlRecordAsync(serviceConfig);
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
        
