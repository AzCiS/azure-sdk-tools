using System;
using System.Management.Automation;
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    using Properties;

    [Cmdlet(VerbsCommon.Remove, "AzureStorSimpleDeviceVolumeContainer")]
    public class RemoveAzureStorSimpleDeviceVolumeContainer : StorSimpleCmdletBase
    {
        [Alias("DN")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.")]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceName { get; set; }

        [Alias("DC")]
        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The volume container name which needs to be removed.")]
        [ValidateNotNullOrEmptyAttribute]
        public DataContainer VolumeContainer { get; set; }

        [Alias("Wait")]
        [Parameter(Position = 2, Mandatory = false, HelpMessage = "Wait for remov task complete")]
        public SwitchParameter WaitForComplete { get; set; }

        [Parameter(Position = 3, Mandatory = false, HelpMessage = "Do not confirm deletion")]
        public SwitchParameter Force { get; set; }

        public override void ExecuteCmdlet()
        {
            ConfirmAction(Force.IsPresent,
                          Resources.RemoveStorSimpleVolumeWarning,
                          Resources.RemoveStorSimpleVolumeConfirmation,
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
                                      var msg =
                                              "Job submitted succesfully. Please use the command Get-AzureStorSimpleJob -InstanceId " +
                                              jobresult.JobId + " for tracking the job status";
                                      WriteObject(msg);
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