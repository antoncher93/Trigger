using System;
using System.Collections.Generic;
using System.Text;

namespace RangerTest.Infrastructure
{
    public interface IRepository<T> : IDisposable
    {
        IEnumerable<T> GetItems();
    }
}
