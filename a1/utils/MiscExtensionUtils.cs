namespace wdt.utils
{
    public static class MiscExtensionUtils
    {
        public static bool IsWithinMaxValue(this int value, int max, int min = 1) => value >= min && value <= max;
        // pad menu greetings string with '=' char
        public static string MenuPad(this string value) => string.Empty.PadLeft(value.Length, '=');
    }
}