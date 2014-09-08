
using System.Management.Automation;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleResourceContext")]
    public class GetAzureStorSimpleResourceContext : StorSimpleCmdletBase
    {
        public override void ExecuteCmdlet()
        {
            var currentContext = StorSimpleClient.GetResourceContext();
            this.WriteObject(currentContext);
        }
    }
}
