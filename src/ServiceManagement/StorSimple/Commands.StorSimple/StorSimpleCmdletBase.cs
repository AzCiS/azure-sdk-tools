
using Microsoft.Azure.Management.StorSimple.Models;
using Microsoft.WindowsAzure.Commands.Utilities.Common;
using System.Net;
using Microsoft.WindowsAzure;

namespace Microsoft.Azure.Commands.StorSimple
{
    using Properties;

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

        internal virtual string ToAsyncJobMessage(OperationResponse opResponse, string operationName)
        {
            string msg = string.Empty;
            if (opResponse.StatusCode != HttpStatusCode.Accepted && opResponse.StatusCode != HttpStatusCode.OK)
            {
                msg = string.Format(Resources.FailureMessageSubmitJob, operationName);
            }

            else
            {
                if (opResponse.GetType().Equals(typeof(JobResponse)))
                {
                    var jobResponse = opResponse as JobResponse;
                    msg = string.Format(Resources.SuccessMessageSubmitJob, operationName, jobResponse.JobId);
                }

                else if (opResponse.GetType().Equals(typeof(GuidJobResponse)))
                {
                    var guidJobResponse = opResponse as GuidJobResponse;
                    msg = string.Format(Resources.SuccessMessageSubmitJob, operationName, guidJobResponse.JobId);
                }
            }
            return msg;
        }
    }
}