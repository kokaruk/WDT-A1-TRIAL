namespace Wdt.Utils
{
    public class Utils
    {
        public static ConsoleLoadingText CreateLoading (string productName = ConsoleLoadingText.DefaultProductName, string loadingText = ConsoleLoadingText.DefaultLoadingText, int millisecondsDelay = ConsoleLoadingText.DefaultMillisecondsDelay)
        {
            return new ConsoleLoadingText (productName, loadingText, millisecondsDelay);
        }
    }
}