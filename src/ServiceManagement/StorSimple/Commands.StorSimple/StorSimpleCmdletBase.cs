
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure.Commands.Utilities.Common;
using System.Net;

namespace Microsoft.Azure.Commands.StorSimple
{
    public class StorSimpleCmdletBase : AzurePSCmdlet
    {
        private PSStorSimpleClient storSimpleClient;

        internal PSStorSimpleClient StorSimpleClient
        {
            get
            {
                if (this.storSimpleClient == null)
                {
                    this.storSimpleClient = new PSStorSimpleClient(CurrentContext.Subscription);
                }

                return this.storSimpleClient;
            }
        }

        internal virtual string ToAsyncJobMessage(JobResponse jobResponse)
        {
            string msg = string.Empty;
            if (jobResponse.StatusCode != HttpStatusCode.Accepted && jobResponse.StatusCode != HttpStatusCode.OK)
            {
                msg = "Job failed to submit.";
            }
            msg = string.Format(
                "Job submitted succesfully. Please use the command Get-AzureStorSimpleJob -InstanceId {0} for tracking the job status",
                jobResponse.JobId);
            return msg;
        }
    }
}