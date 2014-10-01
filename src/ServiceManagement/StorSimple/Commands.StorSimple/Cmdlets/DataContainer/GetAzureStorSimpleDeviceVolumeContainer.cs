using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    using Properties;

    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleDeviceVolumeContainer"),OutputType(typeof(DataContainerGetResponse))]
    public class GetAzureStorSimpleDeviceVolumeContainer : StorSimpleCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDeviceName)]
        [ValidateNotNullOrEmptyAttribute]
        public string DeviceName { get; set; }

        [Alias("Name")]
        [Parameter(Position = 1, Mandatory = false, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDataContainerName)]
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
                    WriteObject(Resources.NotFoundMessageDevice);
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