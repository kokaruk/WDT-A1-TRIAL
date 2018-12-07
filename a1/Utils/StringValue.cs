using System;
using System.Diagnostics.CodeAnalysis;

namespace Wdt.Utils
{
    /// <summary>
    /// Sets string attribute for enum members
    /// </summary>
    [SuppressMessage("ReSharper", "InheritdocConsiderUsage")]
    public class StringValue : Attribute
    {
        public string Value { get; }

        public StringValue(string value)
        {
            Value = value;
        }
    }
}