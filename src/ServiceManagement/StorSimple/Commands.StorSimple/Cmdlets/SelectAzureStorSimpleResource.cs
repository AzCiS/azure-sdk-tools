﻿
using System.Management.Automation;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Select, "AzureStorSimpleResource")]
    public class SelectAzureStorSimpleResource : StorSimpleCmdletBase
    {
        private string resourceName;
        /// <summary>
        /// Gets or sets Protection to set, either enable or disable.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
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
            var status = StorSimpleClient.SetResourceContext(resourceName);
            this.WriteVerbose(status);
            var currentContext = StorSimpleClient.GetResourceContext();
            this.WriteVerbose(currentContext);
        }         
    }
}
