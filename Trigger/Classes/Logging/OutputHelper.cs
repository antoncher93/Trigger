using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Trigger.Classes.Logging
{
    public class OutputHelper
    {
        private string _path;
        public OutputHelper(string path)
        {
            _path = path;
            Clear();
        }

        public void WriteLine(string message)
        {
            File.AppendAllLines(_path, new string[]{ message});
        }

        public void Clear()
        {
            File.Delete(_path);
        }
    }
}
