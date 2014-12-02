using System;
using System.Management.Automation;
using System.Net;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    using Properties;

    [Cmdlet(VerbsCommon.New, "AzureStorSimpleDeviceVolumeContainer"), OutputType(typeof(JobStatusInfo))]
    public class NewAzureStorSimpleDeviceVolumeContainer : StorSimpleCmdletBase
    {

        [Parameter(Position = 0, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDeviceName)]
        [ValidateNotNullOrEmpty]
        public string DeviceName { get; set; }

        [Alias("Name")]
        [Parameter(Position = 1, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDataContainerName)]
        [ValidateNotNullOrEmpty]
        public string VolumeContainerName { get; set; }

        [Alias("StorageAccount")]
        [Parameter(Position = 2, Mandatory = true, ValueFromPipeline = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageSACObject)]
        [ValidateNotNullOrEmpty]
        public StorageAccountCredential PrimaryStorageAccountCredential { get; set; }

        [Alias("CloudBandwidth")]
        [Parameter(Position = 3, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDataContainerBandwidth)]
        [ValidateNotNullOrEmpty]
        public int BandWidthRate { get; set; }

        [Parameter(Position = 4, Mandatory = false, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageWaitTillComplete)]
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
                    return;
                }

                var dc = new DataContainerRequest
                {
                    IsDefault = false,
                    Name = VolumeContainerName,
                    BandwidthRate = BandWidthRate,
                    IsEncryptionEnabled = false,
                    VolumeCount = 0,
                    PrimaryStorageAccountCredential = PrimaryStorageAccountCredential
                };

                if (WaitForComplete.IsPresent)
                {
                    var jobstatus = StorSimpleClient.CreateDataContainer(deviceid, dc);
                    WriteObject(jobstatus);
                }

                else
                {
                    var jobstatus = StorSimpleClient.CreateDataContainerAsync(deviceid, dc);
                    WriteObject(jobstatus);
                    WriteVerbose(ToAsyncJobMessage(jobstatus, "create"));

                }
            }
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
            }
        }
    }
}