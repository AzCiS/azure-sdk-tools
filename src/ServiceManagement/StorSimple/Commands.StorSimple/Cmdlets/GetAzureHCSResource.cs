﻿using System;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "AzureHCSResource")]
    public class GetAzureHCSResource : StorSimpleCmdletBase
    {
        public override void ExecuteCmdlet()
        {
            try
            {
                var serviceList = StorSimpleClient.GetAllResources();
                foreach (var resourceCredentialse in serviceList)
                {
                    this.WriteObject(resourceCredentialse);
                }
            }
            catch (Exception exception)
            {
                
                throw exception;
            }
        }
    }
}