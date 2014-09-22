using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "AzureStorSimpleDeviceVolumeContainer")]
    public class NewAzureStorSimpleDeviceVolumeContainer : StorSimpleCmdletBase
    {

        [Alias("DN")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.")]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceName { get; set; }

        [Alias("DCName")]
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The name of data container.")]
        [ValidateNotNullOrEmptyAttribute]
        public string DataContainerName { get; set; }

        [Alias("StorageAccount")]
        [Parameter(Position = 2, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The sac.")]
        [ValidateNotNullOrEmptyAttribute]
        public StorageAccountCredential PrimaryStorageAccountCredential { get; set; }

        [Alias("BW")]
        [Parameter(Position = 3, Mandatory = true, HelpMessage = "The cloud bandwidth setting.")]
        [ValidateNotNullOrEmptyAttribute]
        public int BandWidthRate { get; set; }

        [Alias("Wait")]
        [Parameter(Position = 4, Mandatory = false, HelpMessage = "Wait for create task to complete")]
        public SwitchParameter WaitForComplete { get; set; }
        public override void ExecuteCmdlet()
        {
            try
            {
                string deviceid = null;
                deviceid = StorSimpleClient.GetDeviceId(DeviceName);
                if (deviceid == null)
                {
                    WriteObject("device with name " + DeviceName + "not found");
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
                    
                    var msg =
                        "Job submitted succesfully. Please use the command Get-AzureStorSimpleJob -InstanceId " +
                        jobstatus.JobId + " for tracking the job status";
                    WriteObject(msg);
                    
                }
                
            }
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
            }
        }
    }
}