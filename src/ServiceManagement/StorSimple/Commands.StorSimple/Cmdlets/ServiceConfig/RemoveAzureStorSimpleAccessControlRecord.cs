﻿
using System;
using System.Management.Automation;
using Microsoft.WindowsAzure;
using Microsoft.Azure.Management.StorSimple.Models;
using System.Collections.Generic;
using System.Net;
using System.Linq;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    using Properties;

    /// <summary>
    /// Removes a ACR from the StorSimple Manager Service Configuration
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "AzureStorSimpleAccessControlRecord")]

    public class RemoveAzureStorSimpleAccessControlRecord : StorSimpleCmdletBase
    {
        [Alias("Name")]
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyByName, HelpMessage = "The access control record name.")]
        [ValidateNotNullOrEmpty]
        public string ACRName { get; set; }

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyByObject, ValueFromPipeline = true, HelpMessage = "The ACR object.")]
        [ValidateNotNullOrEmpty]
        public AccessControlRecord ACR { get; set; }

        [Alias("WaitForCompletion")]
        [Parameter(Position = 1, Mandatory = false, HelpMessage = "Wait for the task to complete")]
        public SwitchParameter WaitForComplete { get; set; }

        [Parameter(Position = 2, Mandatory = false, HelpMessage = "Do not confirm deletion")]
        public SwitchParameter Force { get; set; }

        public override void ExecuteCmdlet()
        {
            ConfirmAction(Force.IsPresent,
                          Resources.RemoveStorSimpleAcrWarning,
                          Resources.RemoveStorSimpleAcrConfirmation,
                          string.Empty,
                          () =>
                          {
                              try
                              {
                                  AccessControlRecord existingAcr = null;
                                  switch (ParameterSetName)
                                  {
                                      case StorSimpleCmdletParameterSet.IdentifyByName:
                                          var allACRs = StorSimpleClient.GetAllAccessControlRecords();
                                          existingAcr = allACRs.Where(x => x.Name.Equals(ACRName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                          break;
                                      case StorSimpleCmdletParameterSet.IdentifyByObject:
                                          existingAcr = ACR;
                                          break;
                                  }
                                  if (existingAcr == null)
                                  {
                                      WriteObject("Specified Access Control Record doesn't exist.");
                                  }
                                  else
                                  {
                                      var serviceConfig = new ServiceConfiguration()
                                      {
                                          AcrChangeList = new AcrChangeList()
                                          {
                                              Added = new List<AccessControlRecord>(),
                                              Deleted = new[] { existingAcr.InstanceId },
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
                              }
                              catch (CloudException cloudException)
                              {
                                  StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
                              }
                          });
        }
    }
}

