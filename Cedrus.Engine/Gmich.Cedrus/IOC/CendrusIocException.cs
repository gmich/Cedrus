﻿using System;

namespace Gmich.Cedrus.IOC
{
    public class CendrusIocException : Exception
    {
        public CendrusIocException(string message) : base(message)
        {
        }

        public CendrusIocException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
