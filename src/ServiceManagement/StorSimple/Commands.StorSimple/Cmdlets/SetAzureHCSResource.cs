using System;
using System.Management.Automation;
using Micro.Azure.Commands.StorSimple;

namespace Micro.Azure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Set, "AzureHCSResource")]
    public class SetAzureHCSResource : StorSimpleCmdletBase
    {
        private string resourceName;

        /// <summary>
        /// Gets or sets Protection to set, either enable or disable.
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string ResourceName
        {
            get { return this.resourceName; }
            set { this.resourceName = value; }
        }

        /// <summary>
        /// ProcessRecord of the command.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            try
            {
                var status = StorSimpleClient.SetResourceContext(resourceName);
                this.WriteObject(status);

                var currentContext = StorSimpleClient.GetResourceContext();
                this.WriteObject(currentContext);

            }
            catch (Exception exception)
            {
                throw exception;
            }
        }         
    }
}
