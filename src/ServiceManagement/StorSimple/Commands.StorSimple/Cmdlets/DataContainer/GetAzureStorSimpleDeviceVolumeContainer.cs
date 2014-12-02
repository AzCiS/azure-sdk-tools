using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    using Properties;
    using System.Collections.Generic;

    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleDeviceVolumeContainer"),OutputType(typeof(DataContainer), typeof(IList<DataContainer>))]
    public class GetAzureStorSimpleDeviceVolumeContainer : StorSimpleCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDeviceName)]
        [ValidateNotNullOrEmpty]
        public string DeviceName { get; set; }

        [Alias("Name")]
        [Parameter(Position = 1, Mandatory = false, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDataContainerName)]
        [ValidateNotNullOrEmpty]
        public string VolumeContainerName { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                var deviceid = StorSimpleClient.GetDeviceId(DeviceName);

                if (deviceid == null)
                {
                    WriteObject(Resources.NotFoundMessageDevice);
                    return;
                }

                if (VolumeContainerName == null)
                {
                    var dataContainerList = StorSimpleClient.GetAllDataContainers(deviceid);
                    WriteObject(dataContainerList.DataContainers);
                }
                else
                {
                    var dataContainer = StorSimpleClient.GetDataContainer(deviceid, VolumeContainerName);
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