using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger.Classes.Logging
{
    public class DBLogger : ILogger
    {
        private const string FilePath = "RangerLog.txt";
        private const string Separator = ";";
        private OutputHelper helper = new OutputHelper(FilePath);

        public void Log(string message)
        {
            helper.WriteLine(message);
        }

        public void Log(IEnumerable<string> items)
        {
            string str = "";
            foreach(var i in items)
            {
                str += i + Separator;
            }
            helper.WriteLine(str);
        }
    }
}
