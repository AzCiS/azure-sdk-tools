
using System.Management.Automation;
using Microsoft.WindowsAzure;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "AzureHCSDevice")]
    public class GetAzureHCSDevice : StorSimpleCmdletBase
    {
        public override void ExecuteCmdlet()
        {
            try
            {
                var deviceInfos = StorSimpleClient.GetAllDevices();
                foreach (var deviceInfo in deviceInfos)
                {
                    this.WriteObject(deviceInfo);
                }
            }
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
            }
        }
    }
}