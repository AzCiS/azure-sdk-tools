using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Micro.Azure.Commands.StorSimple;

namespace Micro.Azure.Commands.StorSimple.Cmdlets
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
