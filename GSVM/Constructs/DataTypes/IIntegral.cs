﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSVM.Constructs.DataTypes
{
    public interface IIntegral : IDataType
    {
        object Value { get; }
    }

    public interface IIntegral<T> : IIntegral
    {
        new T Value { get; set; }
    }
}
