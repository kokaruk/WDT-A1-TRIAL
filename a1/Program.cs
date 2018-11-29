using wdt.Controller;

namespace wdt
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            var runMode = args.Length > 0 && args[0] == "test";
            BaseController login = new LoginController(runMode);
            login.Start();  
        }
    }
}