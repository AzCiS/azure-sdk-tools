using System;
using System.Management.Automation;
using Microsoft.Azure.Management.StorSimple.Models;

namespace Microsoft.Azure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "AzureStorSimpleAccessControlRecord"), OutputType(typeof(AcrChangeList))]
    public class GetAzureStorSimpleAccessControlRecord : StorSimpleCmdletBase
    {
        public override void ExecuteCmdlet()
        {
            try
            {
                //var deviceid = StorSimpleClient.GetDeviceId(DeviceToUse);

                var sc = StorSimpleClient.GetAccessControlRecord();
                WriteObject(sc);
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}