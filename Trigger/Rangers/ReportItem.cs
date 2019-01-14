using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Enums;

namespace Trigger.Rangers
{
    public class ReportItem
    {
        public string Time { get; set; }
        public AppearStatus Position { get; set; }
        public string FL_Aver_Rssi { get; set; }
        public string SL_Aver_Rssi { get; set; }
        public string Event { get; set; } = "";

    }
}
