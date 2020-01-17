using System;

namespace HardwareProxy.Exceptions
{
    public class NoBoardAttachedException :Exception
    {
        public NoBoardAttachedException(string message) : base(message)
        {
        }

        public NoBoardAttachedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
