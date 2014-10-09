
using System.Management.Automation;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleResourceContext")]
    public class GetAzureStorSimpleResourceContext : StorSimpleCmdletBase
    {
        public override void ExecuteCmdlet()
        {
            var currentContext = StorSimpleClient.GetResourceContext();
            this.WriteVerbose(currentContext);
        }
    }
}
