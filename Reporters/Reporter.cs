using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reporters
{
    public class Reporter
    {
        private IDictionary<string, IList<object>> _items = new Dictionary<string, IList<object>>();

        public class Builder
        {
            private Reporter _reporter;
            public Builder()
            {
                _reporter = new Reporter();
            }

            private Builder Modify(Action act)
            {
                act.Invoke();
                return this;
            }

            public Builder AddColumn(string head)
                => Modify(() => _reporter._items.Add(head, new List<object>()));


        }
    }
}
