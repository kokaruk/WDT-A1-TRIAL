using System;

namespace wdt.utils
{
    public class TooManyLoginsException : Exception
    {
        public TooManyLoginsException()
        {
        }

        public TooManyLoginsException(string message) : base(message)
        {
        }

        public TooManyLoginsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}