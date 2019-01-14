using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Interfaces;

namespace Trigger.Rangers
{
    public class RangerReport
    {
        protected const string ColumnSplit = "$";

        private ICollection<ReportItem> _items = new List<ReportItem>();
        private ReportItem current;
        StringBuilder sb = new StringBuilder();
        protected string _headers = string.Empty;

        public void BeginNewItem()
        {
            current = new ReportItem();
            _items.Add(current);
        }

        public ReportItem Current
        {
            get
            {
                if (_items.Count == 0)
                    BeginNewItem();

                return current;
            }
        }

        private readonly IEnumerable<string> headers = new List<string>
        { "Time", "FL_aver_rssi", "SL_aver_rssi", "Position", "Event" };

        public override string ToString()
        {
            foreach(var header in headers)
            {
                sb.Append($"{header}{ColumnSplit}");
            }
            sb.AppendLine();

            foreach(var item in _items)
            {
                sb.Append($"{item.Time}{ColumnSplit}");
                sb.Append($"{item.FL_Aver_Rssi}{ColumnSplit}");
                sb.Append($"{item.SL_Aver_Rssi}{ColumnSplit}");
                sb.Append($"{item.Position}{ColumnSplit}");
                sb.Append($"{item.Event}{ColumnSplit}");

                sb.AppendLine();
            }


            return sb.ToString();
        }

    }
}
