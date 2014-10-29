
using System.Management.Automation;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleResourceContext"),OutputType(typeof(StorSimpleResourceContext))]
    public class GetAzureStorSimpleResourceContext : StorSimpleCmdletBase
    {
        public override void ExecuteCmdlet()
        {
            var currentContext = StorSimpleClient.GetResourceContext();
            this.WriteObject(currentContext);
        }
    }
}
