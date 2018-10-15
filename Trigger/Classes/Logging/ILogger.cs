using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger.Classes.Logging
{
    public interface ILogger
    {
        void Log(string message);
        void Log(IEnumerable<string> items);
    }
}
