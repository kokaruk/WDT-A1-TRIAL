using System;
using System.Text;
using wdt.utils;

namespace wdt.model
{
    internal class LoginController : Controller
    {
        private static int _logAttempts = 3;
        
        internal override void Start()
        {
            base.Start();
            Login();
        }

        private void Login()
        {
            if (_logAttempts > 0)
            {
                _logAttempts--;
                Console.Write("Username: ");
                var userName = Console.ReadLine();
                Console.Write("Password: ");
                var password = ((Func<string>) (() =>
                {
                    var pass = new StringBuilder();

                    ConsoleKeyInfo key;
                    while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
                    {
                        if (key.Key != ConsoleKey.Backspace)
                        {
                            pass.Append(key.KeyChar);
                            Console.Write("*");
                        }
                        else
                        {
                            Console.Write("\b \b");
                            pass.Length--;
                        }
                    }
                    return pass.ToString();
                }))();
                Console.WriteLine();
                Console.WriteLine($"username: {userName}, password: {password}");
            }
            else
            {
                throw new TooManyLoginsException("Exhausted max login attempts");
            }
        }
    }
}