﻿using System.Management.Automation;
using Microsoft.WindowsAzure.Management.StorSimple.Models;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Commands.Utilities.CloudService;

namespace Microsoft.WindowsAzure.Commands.StorSimple.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "AzureStorSimpleDeviceBackupScheduleAddConfig"),OutputType(typeof(BackupScheduleBase))]
    public class NewAzureStorSimpleDeviceBackupScheduleAddConfig : StorSimpleCmdletBase 
    {
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "Enter LocalSnapshot or CloudSnapshot")]
        [ValidateNotNullOrEmpty]
        [ValidateSet("LocalSnapshot", "CloudSnapshot")]
        public String BackupType { get; set; }

        [Parameter(Position = 1, Mandatory = true, HelpMessage = "Enter \"Minutes or Hourly or Daily or Weekly\"")]
        [ValidateNotNullOrEmpty]
        [ValidateSet("Minutes", "Hourly", "Daily", "Weekly")]
        public String RecurrenceType { get; set; }

        [Parameter(Position = 2, Mandatory = true, HelpMessage = "How often do you want a backup to be taken? Enter a numerical value")]
        [ValidateNotNullOrEmpty]
        public int RecurrenceValue { get; set; }

        [Parameter(Position = 3, Mandatory = true, HelpMessage = "Number of days the backup should be retained before deleting")]
        [ValidateNotNullOrEmpty]
        public long RetentionCount { get; set; }

        [Parameter(Position = 4, Mandatory = false, HelpMessage = "Enter date from which you want to start taking backups. Default is now")]
        public DateTime? StartFrom { get; set; }

        [Parameter(Position = 5, Mandatory = false, HelpMessage = "Set this parameter to false if you want to this backupschedule to be disabled")]
        public bool? Enabled { get; set; }

        private DateTime startFromProcessed;
        private ScheduleStatus scheduleStatus;
        private void ProcessParameters()
        {
            if (StartFrom == null)
            {
                StartFrom = DateTime.Now;
                startFromProcessed = DateTime.Now;
            }
            else
            {
                bool dateTimeValid = false;
                bool.TryParse(StartFrom.ToString(), out dateTimeValid);
                if (!dateTimeValid)
                {
                    throw new ArgumentException("Provide valid dateTime for StartFrom parameter");
                }
                startFromProcessed = DateTime.Parse(StartFrom.ToString());
            }
            scheduleStatus = (Enabled == null || Enabled == true) ? ScheduleStatus.Enabled : ScheduleStatus.Disabled;
        }

        public override void ExecuteCmdlet()
        {
            try
            {
                ProcessParameters();

                BackupScheduleBase newScheduleObject = new BackupScheduleBase();
                newScheduleObject.BackupType = (BackupType)Enum.Parse(typeof(BackupType), BackupType);
                newScheduleObject.Status = scheduleStatus;
                newScheduleObject.RetentionCount = RetentionCount;
                newScheduleObject.StartTime = startFromProcessed.ToString("yyyy-MM-ddTHH:mm:sszzz");
                newScheduleObject.Recurrence = new ScheduleRecurrence();
                newScheduleObject.Recurrence.RecurrenceType = (RecurrenceType)Enum.Parse(typeof(RecurrenceType), RecurrenceType);
                newScheduleObject.Recurrence.RecurrenceValue = RecurrenceValue;

                WriteObject(newScheduleObject);
            }
            catch (CloudException cloudException)
            {
                StorSimpleClient.ThrowCloudExceptionDetails(cloudException);
            }
        }
    }
}
