
using Microsoft.WindowsAzure.Commands.Utilities.Common;

namespace Micro.Azure.Commands.StorSimple
{
    public class StorSimpleCmdletBase : CmdletWithSubscriptionBase
    {
        private PSStorSimpleClient storSimpleClient;

        internal PSStorSimpleClient StorSimpleClient
        {
            get
            {
                if (this.storSimpleClient == null)
                {
                    this.storSimpleClient = new PSStorSimpleClient(CurrentSubscription);
                }

                return this.storSimpleClient;
            }
        }
    }
}