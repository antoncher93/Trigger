using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger.Interfaces
{
    public interface IObjectPool<TI, T>
    {
        T this[TI @key] { get; }

        void Flush();
    }
}
