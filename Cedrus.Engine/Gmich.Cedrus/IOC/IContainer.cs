﻿using System;
using System.Collections.Generic;

namespace Gmich.Cedrus.IOC
{
    public interface IContainer : IDisposable
    {
        object Resolve(Type serviceType);
        TService Resolve<TService>();
        IContainer Scope { get; }
    }
}