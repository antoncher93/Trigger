﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger.Interfaces
{
    public interface IRangerBuilder<T> where T : IRanger
    {
        T Build();
    }
}
