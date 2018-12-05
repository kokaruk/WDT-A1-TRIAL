using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Wdt.Utils
{
    public static class MiscExtensionUtils
    {
        public static bool IsWithinMaxValue(this int value, int max, int min = 1) => value >= min && value <= max;
        
        // pad menu greetings string with '=' char
        public static string MenuHeaderPad(this string value) => string.Empty.PadLeft(value.Length, '=');
        
        // get string value from enum
        // partially based on https://automationrhapsody.com/efficiently-use-of-enumerations-with-string-values-in-c/
        public static string GetStringValue(this Enum value) 
        {
            var stringValue = value.ToString();
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            if (fieldInfo.
                    GetCustomAttributes(typeof(StringValue), false) is StringValue[] attrs && attrs.Length > 0)
            {
                stringValue = attrs[0].Value;
            }
            return stringValue;
        }
        
        public static SqlConnection CreateConnection(this string connectionString) =>
            new SqlConnection(connectionString);

        public static void fillParams(this SqlCommand command, Dictionary<string, string> connParams)
        {
            foreach (var keyValue in connParams)
            {
                command.Parameters.AddWithValue(keyValue.Key, keyValue.Value);
            }
        }
        
    }
}