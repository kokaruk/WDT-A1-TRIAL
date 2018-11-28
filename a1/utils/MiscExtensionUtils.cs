namespace wdt.utils
{
    public static class MiscExtensionUtils
    {
        public static bool IsWithinMaxValue(this int value, int max, int min = 1) => value >= min && value <= max;
    }
}