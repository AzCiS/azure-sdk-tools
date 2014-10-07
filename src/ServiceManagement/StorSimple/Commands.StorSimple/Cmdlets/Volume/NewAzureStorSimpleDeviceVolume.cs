using System;
using System.Management.Automation;
using System.Net;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets.Volume
{
    using Properties;
    using System.Collections.Generic;

    [Cmdlet(VerbsCommon.New, "AzureStorSimpleDeviceVolume")]
    public class NewAzureStorSimpleDeviceVolume : StorSimpleCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDeviceName)]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceName { get; set; }

        [Parameter(Position = 1, Mandatory = true, ValueFromPipeline = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDataContainerName)]
        [ValidateNotNullOrEmpty]
        public DataContainer Container { get; set; }
        
        [Alias("Name")]
        [Parameter(Position = 2, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageVolumeName)]
        [ValidateNotNullOrEmpty]
        public string VolumeName { get; set; }

        [Alias("Size")]
        [Parameter(Position = 3, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageVolumeSize)]
        [ValidateNotNullOrEmpty]
        public Int64 VolumeSize { get; set; }

        [Parameter(Position = 4, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageVolumeAcrList)]
        [ValidateNotNullOrEmpty]
        public List<AccessControlRecord> AccessControlRecords { get; set; }

        [Alias("AppType")]
        [Parameter(Position = 5, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageVolumeAppType)]
        [ValidateNotNullOrEmpty]
        public AppType VolumeAppType { get; set; }

        [Parameter(Position = 6, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageVolumeOnline)]
        [ValidateNotNullOrEmpty]
        public bool Online { get; set; }

        [Parameter(Position = 7, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageVolumeDefaultBackup)]
        [ValidateNotNullOrEmpty]
        public bool EnableDefaultBackup { get; set; }

        [Parameter(Position = 8, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageVolumeMonitoring)]
        [ValidateNotNullOrEmpty]
        public bool EnableMonitoring { get; set; }

        [Parameter(Position = 9, Mandatory = false, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageWaitTillComplete)]
        public SwitchParameter WaitForComplete { get; set; }
        public override void ExecuteCmdlet()
        {
            try
            {
                string deviceid = null;
                deviceid = StorSimpleClient.GetDeviceId(DeviceName);

                if (deviceid == null)
                {
                    WriteObject(Resources.NotFoundMessageDevice);
                }

                else
                {
                    //TODO: fix logic 

                    var sacToUse = new StorageAccountCredential
                    {
                        InstanceId = Container.PrimaryStorageAccountCredential.InstanceId,
                        Name = Container.PrimaryStorageAccountCredential.Name,
                        CloudType = Container.PrimaryStorageAccountCredential.CloudType,
                        Hostname = Container.PrimaryStorageAccountCredential.Hostname,
                        Login = Container.PrimaryStorageAccountCredential.Login,
                        Password = Container.PrimaryStorageAccountCredential.Password,
                        UseSSL = Container.PrimaryStorageAccountCredential.UseSSL,
                        VolumeCount = Container.PrimaryStorageAccountCredential.VolumeCount,
                        IsDefault = Container.PrimaryStorageAccountCredential.IsDefault,
                        Location = Container.PrimaryStorageAccountCredential.Location,
                        OperationInProgress = Container.PrimaryStorageAccountCredential.OperationInProgress,
                        PasswordEncryptionCertThumbprint = "dummy"
                    };

                    var dcToUse = new DataContainer
                    {
                        IsDefault = Container.IsDefault,
                        InstanceId = Container.InstanceId,
                        Name = Container.Name,
                        BandwidthRate = Container.BandwidthRate,
                        IsEncryptionEnabled = Container.IsEncryptionEnabled,
                        VolumeCount = Container.VolumeCount,
                        EncryptionKey = Container.EncryptionKey,
                        OperationInProgress = Container.OperationInProgress,
                        Owned = Container.Owned,
                        SecretsEncryptionThumbprint = Container.SecretsEncryptionThumbprint,
                        PrimaryStorageAccountCredential = sacToUse
                    };

                    //Virtual disk create request object
                    var virtualDiskToCreate = new VirtualDiskRequest()
                    {
                        Name = VolumeName,
                        AccessType = AccessType.ReadWrite,
                        AcrList = AccessControlRecords,
                        AppType = VolumeAppType,
                        IsDefaultBackupEnabled = EnableDefaultBackup,
                        SizeInBytes = VolumeSize,
                        DataContainer = dcToUse,
                        Online = Online,
                        IsMonitoringEnabled = EnableMonitoring
                    };

                    //var jobStatusInfo = StorSimpleClient.CreateVolume(deviceid, virtualDiskToCreate);
                    //WriteObject(jobStatusInfo);

                    if (WaitForComplete.IsPresent)
                    {
                        var jobstatus = StorSimpleClient.CreateVolume(deviceid, virtualDiskToCreate); ;
                        WriteObject(jobstatus);
                    }

                    else
                    {
                        var jobstatus = StorSimpleClient.CreateVolumeAsync(deviceid, virtualDiskToCreate); ;

                        WriteObject(ToAsyncJobMessage(jobstatus, "create"));

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