using System;
using System.Management.Automation;
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;
using System.Linq;
using System.Collections.Generic;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    using Properties;

    /// <summary>
    /// Lists all the connected ISCSI initiators
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleDeviceConnectedInitiator")]
    public class GetAzureStorSimpleDeviceConnectedInitiator : StorSimpleCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyById, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDeviceId)]
        [ValidateNotNullOrEmpty]
        public string DeviceId { get; set; }

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = StorSimpleCmdletParameterSet.IdentifyByName, HelpMessage = StorSimpleCmdletHelpMessage.HelpMessageDeviceName)]
        [ValidateNotNullOrEmpty]
        public string DeviceName { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                List<IscsiConnection> iscsiConnections = null;
                switch(ParameterSetName)
                {
                    case StorSimpleCmdletParameterSet.IdentifyByName:
                        var deviceToUse = StorSimpleClient.GetAllDevices().Where(x => x.FriendlyName.Equals(DeviceName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        if (deviceToUse == null)
                        {
                            WriteObject(Resources.NotFoundMessageDevice);
                        }
                        else
                        {
                            iscsiConnections = StorSimpleClient.GetAllIscsiConnections(deviceToUse.DeviceId);
                        }
                        break;
                    case StorSimpleCmdletParameterSet.IdentifyById:
                        iscsiConnections = StorSimpleClient.GetAllIscsiConnections(DeviceId);
                        break;
                }
                WriteObject(iscsiConnections);
            }
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
            }
        }
    }
}