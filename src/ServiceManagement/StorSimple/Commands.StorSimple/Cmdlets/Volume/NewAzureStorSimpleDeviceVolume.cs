using System;
using System.Management.Automation;
using Microsoft.Azure.Management.StorSimple.Models;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets.Volume
{
    [Cmdlet(VerbsCommon.New, "AzureStorSimpleDeviceVolume")]
    public class NewAzureStorSimpleDeviceVolume : StorSimpleCmdletBase
    {
        [Alias("Device Name")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.")]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceName { get; set; }

        [Alias("Data Conatiner Name")]
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The name of data container to use.")]
        [ValidateNotNullOrEmpty]
        public DataContainer Container { get; set; }
        
        [Alias("Volume Name")]
        [Parameter(Position = 2, Mandatory = true, HelpMessage = "The name of volume.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Alias("Size of the volume")]
        [Parameter(Position = 3, Mandatory = true, HelpMessage = "The name of data container to use.")]
        [ValidateNotNullOrEmpty]
        public Int64 Size { get; set; }

        [Alias("Access control Records")]
        [Parameter(Position = 4, Mandatory = true, HelpMessage = "The name of data container to use.")]
        [ValidateNotNullOrEmpty]
        public AccessControlRecord[] AccessControlRecords { get; set; }

        [Alias("App type")]
        [Parameter(Position = 5, Mandatory = true, HelpMessage = "The name of data container to use.")]
        [ValidateNotNullOrEmpty]
        public AppType AppType { get; set; }

        [Alias("Online tyep")]
        [Parameter(Position = 6, Mandatory = true, HelpMessage = "The name of data container to use.")]
        [ValidateNotNullOrEmpty]
        public bool Online { get; set; }

        [Alias("Enable default backup")]
        [Parameter(Position = 7, Mandatory = true, HelpMessage = "The name of data container to use.")]
        [ValidateNotNullOrEmpty]
        public bool EnableDefaultBackup { get; set; }

        [Alias("Enable monitoring")]
        [Parameter(Position = 8, Mandatory = true, HelpMessage = "The name of data container to use.")]
        [ValidateNotNullOrEmpty]
        public bool EnableMonitoring { get; set; }
        public override void ExecuteCmdlet()
        {
            try
            {
                string deviceid = null;
                var deviceInfos = StorSimpleClient.GetAllDevices();
                foreach (var deviceInfo in deviceInfos)
                {
                    if (deviceInfo.FriendlyName.Equals(DeviceName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        deviceid = deviceInfo.DeviceId;
                    }
                }

                if (deviceid == null)
                {
                    WriteObject("device with name " + DeviceName + "not found");
                }

                //Virtual disk create request object
                var virtualDiskToCreate = new VirtualDiskRequest()
                {
                    Name = Name,
                    AccessType = AccessType.ReadWrite,
                    AcrList = AccessControlRecords,
                    AppType = AppType,
                    IsDefaultBackupEnabled = EnableDefaultBackup,
                    SizeInBytes = Size,
                    DataContainer = Container,
                    Online = Online,
                    IsMonitoringEnabled = EnableMonitoring
                };

                var jobStatusInfo = StorSimpleClient.CreateVolume(deviceid, virtualDiskToCreate);

                WriteObject(jobStatusInfo);
            }
            catch(Exception ex)
            {

            }
        }
    }
}