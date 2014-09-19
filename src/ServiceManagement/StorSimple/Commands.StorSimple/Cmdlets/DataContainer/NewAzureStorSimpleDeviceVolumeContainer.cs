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

        [Alias("DeviceName")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.")]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceToUse { get; set; }

        [Alias("Name")]
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The name of data container.")]
        [ValidateNotNullOrEmptyAttribute]
        public string DcName { get; set; }

        [Alias("StorageAccount")]
        [Parameter(Position = 2, Mandatory = true, HelpMessage = "The sac.")]
        [ValidateNotNullOrEmptyAttribute]
        public StorageAccountCredential sacToUse { get; set; }

        [Alias("CloudBandwidth")]
        [Parameter(Position = 3, Mandatory = true, HelpMessage = "The cloud bandwidth setting.")]
        [ValidateNotNullOrEmptyAttribute]
        public int BandWidthRate { get; set; }

        [Alias("WaitForCompletion")]
        [Parameter(Position = 4, Mandatory = false, HelpMessage = "Wait for copy task complete")]
        public SwitchParameter WaitForComplete { get; set; }
        public override void ExecuteCmdlet()
        {
            try
            {
                string deviceid = null;
                var deviceInfos = StorSimpleClient.GetAllDevices();
                foreach (var deviceInfo in deviceInfos)
                {
                    if (deviceInfo.FriendlyName.Equals(DeviceToUse, StringComparison.InvariantCultureIgnoreCase))
                    {
                        deviceid = deviceInfo.DeviceId;
                    }
                }

                if (deviceid == null)
                {
                    WriteObject("device with name " + DeviceToUse + "not found");
                }

                var dc = new DataContainerRequest
                {
                    IsDefault = false,
                    Name = DcName,
                    BandwidthRate = BandWidthRate,
                    IsEncryptionEnabled = false,
                    PrimaryStorageAccountCredential = sacToUse
                };

                if (WaitForComplete.IsPresent)
                {
                    var jobstatus = StorSimpleClient.CreateDataContainer(deviceid, dc);
                    WriteObject(jobstatus);
                }

                else
                {
                    var jobstatus = StorSimpleClient.CreateDataContainerAsync(deviceid,dc);
                    if (jobstatus.StatusCode != HttpStatusCode.Accepted)
                    {
                        
                    }
                    var msg =
                        "Job submitted succesfully. Please use the command Get-AzureStorSimpleJob -InstanceId " +
                        jobstatus.JobId + "for tracking the job status";
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