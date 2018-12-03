using System;

namespace wdt.utils
{
    public class StringValue : Attribute
    {
        public string Value { get; }

        public StringValue(string value)
        {
            Value = value;
        }
    }
}