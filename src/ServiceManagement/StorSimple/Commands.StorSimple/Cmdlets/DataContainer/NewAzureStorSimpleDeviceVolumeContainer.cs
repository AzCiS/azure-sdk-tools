using System;
using System.Management.Automation;
using System.Net;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;
using Microsoft.Azure.Commands.StorSimple.Properties;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    using Properties;

    [Cmdlet(VerbsCommon.New, "AzureStorSimpleDeviceVolumeContainer")]
    public class NewAzureStorSimpleDeviceVolumeContainer : StorSimpleCmdletBase
    {

        [Parameter(Position = 0, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDeviceName)]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceName { get; set; }

        [Alias("Name")]
        [Parameter(Position = 1, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDataContainerName)]
        [ValidateNotNullOrEmptyAttribute]
        public string DataContainerName { get; set; }

        [Alias("StorageAccount")]
        [Parameter(Position = 2, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageSACObject)]
        [ValidateNotNullOrEmptyAttribute]
        public StorageAccountCredential PrimaryStorageAccountCredential { get; set; }

        [Alias("CloudBandwidth")]
        [Parameter(Position = 3, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDataContainerBandwidth)]
        [ValidateNotNullOrEmptyAttribute]
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
                }

                var dc = new DataContainerRequest
                {
                    IsDefault = false,
                    Name = DataContainerName,
                    BandwidthRate = BandWidthRate,
                    IsEncryptionEnabled = false,
                    PrimaryStorageAccountCredential = PrimaryStorageAccountCredential
                };

                if (WaitForComplete.IsPresent)
                {
                    var jobstatus = StorSimpleClient.CreateDataContainer(deviceid, dc);
                    WriteObject(jobstatus);
                }

                else
                {
                    var jobstatus = StorSimpleClient.CreateDataContainerAsync(deviceid,dc);
                    WriteObject(ToAsyncJobMessage(jobstatus, "create"));
                    
                }
                
            }
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
            }
        }
    }
}