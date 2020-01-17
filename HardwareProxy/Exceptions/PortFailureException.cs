using System;

namespace HardwareProxy.Exceptions
{
    public class PortFailureException : Exception
    {
        public PortFailureException(string message) : base(message)
        {
        }

        public PortFailureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
