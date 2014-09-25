
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
    /// Sets the Host IQN of the ACR in the StorSimple Manager Service Configuration
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "AzureStorSimpleAccessControlRecord")]

    public class SetAzureStorSimpleAccessControlRecord : StorSimpleCmdletBase
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

                var allACRs = StorSimpleClient.GetAllAccessControlRecords();
                var existingAcr = allACRs.Where(x => x.Name.Equals(ACRName)).FirstOrDefault();
                if (existingAcr == null)
                {
                    WriteObject("Access Control Record with the given Name doesn't exist.");
                }
                else
                {
                    var serviceConfig = new ServiceConfiguration()
                    {
                        AcrChangeList = new AcrChangeList()
                        {
                            Added = new List<AccessControlRecord>(),
                            Deleted = new List<string>(),
                            Updated = new []
                            {
                                new AccessControlRecord()
                                {
                                    GlobalId = existingAcr.GlobalId,
                                    InitiatorName = IQNInitiatorName,
                                    InstanceId = existingAcr.InstanceId,
                                    Name = existingAcr.Name,
                                    VolumeCount = existingAcr.VolumeCount
                                },
                            }
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
            }
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
            }
        }
    }
}

