using System;
using System.Management.Automation;
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    using Properties;

    [Cmdlet(VerbsCommon.Remove, "AzureStorSimpleDeviceVolume")]
    public class RemoveAzureStorSimpleDeviceVolume : StorSimpleCmdletBase
    {
        [Alias("DName")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.")]
        [ValidateNotNullOrEmpty]
        public string DeviceName { get; set; }

        [Alias("VName")]
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyByName, HelpMessage = "The name of volume.")]
        [ValidateNotNullOrEmpty]
        public string VolumeName { get; set; }

        [Alias("Id")]
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyById, HelpMessage = "The device name.")]
        [ValidateNotNullOrEmptyAttribute]
        public string VolumeId { get; set; }

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
                                      JobStatusInfo jobstatus;
                                      switch (ParameterSetName)
                                      {
                                          case StorSimpleCmdletParameterSet.IdentifyById:
                                              jobstatus = StorSimpleClient.RemoveVolume(deviceid, VolumeId);
                                              WriteObject(jobstatus);
                                              break;
                                          case StorSimpleCmdletParameterSet.IdentifyByName:
                                              var volumeInfo = StorSimpleClient.GetVolumeByName(deviceid, VolumeName);
                                              if (volumeInfo != null)
                                              {
                                                  jobstatus = StorSimpleClient.RemoveVolume(deviceid, volumeInfo.VirtualDiskInfo.InstanceId);
                                                  WriteObject(jobstatus);
                                              }
                                              break;
                                      }
                                  }
                                  else
                                  {
                                      GuidJobResponse jobresult = null;
                                      switch (ParameterSetName)
                                      {
                                          case StorSimpleCmdletParameterSet.IdentifyById:
                                              jobresult = StorSimpleClient.RemoveVolumeAsync(deviceid, VolumeId);
                                              //WriteObject(jobstatus);
                                              break;
                                          case StorSimpleCmdletParameterSet.IdentifyByName:
                                              var volumeInfo = StorSimpleClient.GetVolumeByName(deviceid, VolumeName);
                                              if (volumeInfo != null)
                                              {
                                                  jobresult = StorSimpleClient.RemoveVolumeAsync(deviceid, volumeInfo.VirtualDiskInfo.InstanceId);
                                                  //WriteObject(jobstatus);
                                              }
                                              break;
                                      }
                                      if (jobresult == null) return;
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