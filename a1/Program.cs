using wdt.model;

namespace wdt
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Controller login = new LoginController();
            login.Start();  
        }
    }
}