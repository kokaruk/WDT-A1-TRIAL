using System;

namespace Wdt.Utils
{
    /// <summary>
    /// Exception thrown when maximum login attempts is exhausted 
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
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