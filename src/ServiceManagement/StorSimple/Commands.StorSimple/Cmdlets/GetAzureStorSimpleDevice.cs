
using System.Management.Automation;
using Microsoft.WindowsAzure;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleDevice")]
    public class GetAzureStorSimpleDevice : StorSimpleCmdletBase
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