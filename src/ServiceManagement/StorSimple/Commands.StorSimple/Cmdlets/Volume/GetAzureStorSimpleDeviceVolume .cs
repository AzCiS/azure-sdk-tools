﻿using System;
using System.Management.Automation;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    using Properties;

    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleDeviceVolume")]
    public class GetAzureStorSimpleDeviceVolume : StorSimpleCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDeviceName)]
        [ValidateNotNullOrEmpty]
        public string DeviceName { get; set; }

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyByParentObject, ValueFromPipeline = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDataContainerObject)]
        [ValidateNotNullOrEmpty]
        public DataContainer VolumeContainer { get; set; }

        [Alias("Name")]
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyByName, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageVolumeName)]
        [ValidateNotNullOrEmpty]
        public string VolumeName { get; set; }
        
        public override void ExecuteCmdlet()
        {
            try
            {
                var deviceId = StorSimpleClient.GetDeviceId(DeviceName);
                if (deviceId == null)
                {
                    WriteObject(Resources.NotFoundMessageDevice);
                    return;
                }
                
                switch (ParameterSetName)
                {
                    case StorSimpleCmdletParameterSet.IdentifyByParentObject:
                        var volumeInfoList = StorSimpleClient.GetAllVolumesFordataContainer(deviceId, VolumeContainer.InstanceId);
                        WriteObject(volumeInfoList.ListofVirtualDisks);
                        break;
                    case StorSimpleCmdletParameterSet.IdentifyByName:
                        var volumeInfo = StorSimpleClient.GetVolumeByName(deviceId, VolumeName);
                        WriteObject(volumeInfo.VirtualDiskInfo);
                        break;
                }
            }
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
            }
        }
    }
}