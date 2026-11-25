using System;
using System.Collections.Generic;
using System.Text;

namespace TunnelWatch.Domain.Alarms
{
    /// <summary>
    /// Razina ozbiljnosti alarma: informacija, upozorenje ili kritično.
    /// </summary>
    public enum AlarmSeverity
    {
        Info,
        Warning,
        Critical,
    }
}
