using System;
using System.Management.Automation;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;
using Microsoft.Azure.Commands.StorSimple.Properties;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    using Properties;

    [Cmdlet(VerbsCommon.Remove, "AzureStorSimpleDeviceVolumeContainer")]
    public class RemoveAzureStorSimpleDeviceVolumeContainer : StorSimpleCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDeviceName)]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceName { get; set; }

        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDataContainerName)]
        [ValidateNotNullOrEmptyAttribute]
        public DataContainer VolumeContainer { get; set; }

        [Parameter(Position = 2, Mandatory = false, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageWaitTillComplete)]
        public SwitchParameter WaitForComplete { get; set; }

        [Parameter(Position = 3, Mandatory = false, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageForce)]
        public SwitchParameter Force { get; set; }

        public override void ExecuteCmdlet()
        {
            ConfirmAction(Force.IsPresent,
                          Resources.RemoveWarningVolume,
                          Resources.RemoveConfirmationVolume,
                          string.Empty,
                          () =>
                          {
                              try
                              {
                                  var deviceid = StorSimpleClient.GetDeviceId(DeviceName);

                                  if (deviceid == null) return;
                                  if (WaitForComplete.IsPresent)
                                  {
                                      var jobstatusInfo = StorSimpleClient.DeleteDataContainer(deviceid, VolumeContainer.InstanceId);
                                      WriteObject(jobstatusInfo);
                                  }
                                  else
                                  {
                                      var jobresult = StorSimpleClient.DeleteDataContainerAsync(deviceid, VolumeContainer.InstanceId);
                                      WriteObject(ToAsyncJobMessage(jobresult, "delete"));
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