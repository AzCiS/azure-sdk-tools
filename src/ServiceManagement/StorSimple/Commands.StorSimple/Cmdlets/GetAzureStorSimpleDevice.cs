using Microsoft.Azure.Management.StorSimple.Models;
using System.Management.Automation;
using Microsoft.WindowsAzure;
using System.Linq;
using System;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleDevice", DefaultParameterSetName = StorSimpleCmdletParameterSet.Empty)]
    public class GetAzureStorSimpleDevice : StorSimpleCmdletBase
    {
        [Alias("ID")]
        [Parameter(Position = 0, Mandatory = false, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyById, HelpMessage = "The device identifier.")]
        [ValidateNotNullOrEmpty]
        public string DeviceId { get; set; }

        [Alias("Name")]
        [Parameter(Position = 0, Mandatory = false, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyByName, HelpMessage = "The device name.")]
        [ValidateNotNullOrEmpty]
        public string DeviceName { get; set; }

        [Parameter(Position = 1, Mandatory = false, HelpMessage = "StorSimple device type.")]
        [ValidateNotNullOrEmpty]
        public string Type { get; set; }

        [Parameter(Position = 2, Mandatory = false, HelpMessage = "StorSimple device Model Id.")]
        [ValidateNotNullOrEmpty]
        public string ModelID { get; set; }

        [Parameter(Position = 3, Mandatory = false, HelpMessage = "Configuration details about the device")]
        public SwitchParameter Detailed { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                var deviceInfos = StorSimpleClient.GetAllDevices();
                switch(ParameterSetName)
                {
                    case StorSimpleCmdletParameterSet.IdentifyByName:
                        deviceInfos = deviceInfos.Where(x => x.FriendlyName.Equals(DeviceName, System.StringComparison.InvariantCultureIgnoreCase));
                        break;
                    case StorSimpleCmdletParameterSet.IdentifyById:
                        deviceInfos = deviceInfos.Where(x => x.DeviceId.Equals(DeviceId, System.StringComparison.InvariantCultureIgnoreCase));
                        break;
                    default:
                        break;
                }
                
                if (Type != null)
                {
                    DeviceType deviceType;
                    bool parseSuccess = Enum.TryParse(Type, true, out deviceType);
                    if (parseSuccess)
                    {
                        deviceInfos = deviceInfos.Where(x => x.Type.Equals(deviceType));
                    }
                }

                if (ModelID != null)
                {
                    deviceInfos = deviceInfos.Where(x => x.ModelDescription.Equals(ModelID, System.StringComparison.InvariantCultureIgnoreCase));
                }

                if (Detailed.IsPresent)
                {
                    foreach (var deviceInfo in deviceInfos)
                    {
                        var deviceDetails = StorSimpleClient.GetDeviceDetails(deviceInfo.DeviceId);
                        WriteObject(deviceDetails);
                    }
                }

                else
                {
                    WriteObject(deviceInfos);
                }
            }
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
            }
        }
    }
}