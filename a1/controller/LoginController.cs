using System;
using System.Linq;
using System.Text;
using wdt.DAL;
using wdt.Model;
using wdt.utils;

namespace wdt.controller
{
    internal class LoginController : Controller
    {
        private static int _logAttempts = 3;
        internal User LoggedOnUser { get; private set; }
        private readonly bool _testing;

        internal LoginController(bool testing)
        {
            _testing = testing;
        }

        internal override void Start()
        {
            // base class start clear the screen
            base.Start();
            LoggedOnUser = _testing ? GetFakeUerFromInput() : Login();
            Console.WriteLine($"\nUsername: {LoggedOnUser.Name}, Type: {LoggedOnUser.Type} ");
        }

        private static User Login()
        {
            if (_logAttempts <= 0)
            {
                throw new TooManyLoginsException("Exhausted max login attempts");
            }

            _logAttempts--;
            Console.Write("Username: ");
            var userName = Console.ReadLine();
            Console.Write("Password: ");
            // in-line lambda without delegate
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
            return DalFactory.UserDal.GetUser(userName, password);
        }

        // test mode, no login required
        private static User GetFakeUerFromInput()
        {
            var userLogon = new StringBuilder(
                "Select User Type\n================");
            // menu options counter
            var count = 0;
            Enum.GetNames(typeof(UserType))
                .ToList()
                .ForEach(
                    type =>
                    {
                        count = (int) Enum.Parse(typeof(UserType), type) + 1;
                        userLogon.Append($"\n{count}. {type}");
                    }
                );
            userLogon.Append($"\n{++count}. Quit\n");

            while (true)
            {
                var maxInput = Enum.GetNames(typeof(UserType)).Length + 1;

                Console.WriteLine(userLogon);
                Console.Write("Enter an option: ");
                var input = Console.ReadLine();
                if (!int.TryParse(input, out var option)
                    || !option.IsWithinMaxValue(maxInput))
                {
                    Console.Clear();
                    Console.Write("Invalid Input\n\n");
                    continue;
                }

                if (option == 4) Environment.Exit(0);

                return UserFactory.MakeUserFromInt(--option);
            }
        }
    }
}