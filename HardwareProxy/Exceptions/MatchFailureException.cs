using System;

namespace HardwareProxy.Exceptions
{
    public class MatchFailureException : Exception
    {
        public MatchFailureException(string message) : base(message)
        {
        }

        public MatchFailureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
