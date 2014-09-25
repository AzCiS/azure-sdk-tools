using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleDeviceVolumeContainer"),OutputType(typeof(DataContainerGetResponse))]
    public class GetAzureStorSimpleDeviceVolumeContainer : StorSimpleCmdletBase
    {
        [Alias("DN")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The device name.")]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceName { get; set; }

        [Alias("Name")]
        [Parameter(Position = 1, Mandatory = false, HelpMessage = "The name of data container.")]
        [ValidateNotNullOrEmptyAttribute]
        public string DataContainerName { get; set; }

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

                if (DataContainerName == null)
                {
                    var dataContainerList = StorSimpleClient.GetAllDataContainers(deviceid);
                    if (dataContainerList.Any())
                    {
                        foreach (var datacontainer in dataContainerList)
                        {
                            WriteObject(datacontainer);
                        }
                    }
                }
                else
                {
                    var dataContainer = StorSimpleClient.GetDataContainer(deviceid, DataContainerName);

                    WriteObject(dataContainer.DataContainerInfo);
                }
            }
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
            }
        }
    }
}