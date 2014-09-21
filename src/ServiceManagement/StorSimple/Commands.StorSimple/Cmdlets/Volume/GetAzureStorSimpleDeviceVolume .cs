using System;
using System.Management.Automation;
using Microsoft.Azure.Management.StorSimple.Models;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleDeviceVolume")]
    public class GetAzureStorSimpleDeviceVolume : StorSimpleCmdletBase
    {
        [Alias("Device Name")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.")]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceName { get; set; }

        [Alias("volume container id")]
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "DataContainerId", HelpMessage = "The device name.")]
        [ValidateNotNullOrEmptyAttribute]
        public string VolumeContainerId { get; set; }

        [Alias("volume id")]
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "VolumeId", HelpMessage = "The device name.")]
        [ValidateNotNullOrEmptyAttribute]
        public string Id { get; set; }

        [Alias("volume name")]
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "VolumeName", HelpMessage = "The device name.")]
        [ValidateNotNullOrEmptyAttribute]
        public string Name { get; set; }
        public override void ExecuteCmdlet()
        {
            try
            {
                var deviceId = StorSimpleClient.GetDeviceId(DeviceName);
                if (deviceId != null)
                {
                    VirtualDiskGetResponse volumeInfo;
                    switch (ParameterSetName)
                    {
                        case "DataContainerId":
                            var volumeInfoList = StorSimpleClient.GetAllVolumesFordataContainer(deviceId, VolumeContainerId);
                            WriteObject(volumeInfoList);
                            break;
                        case "VolumeId" :
                            volumeInfo = StorSimpleClient.GetVolumeById(deviceId, Id);
                            WriteObject(volumeInfo);
                            break;
                        case "VolumeName" :
                            volumeInfo = StorSimpleClient.GetVolumeByName(deviceId, Name);
                            WriteObject(volumeInfo);
                            break;
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}