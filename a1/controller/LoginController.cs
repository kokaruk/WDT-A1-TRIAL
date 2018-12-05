using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Wdt.DAL;
using Wdt.Model;
using Wdt.Utils;

namespace Wdt.Controller
{
    internal class LoginController : BaseController
    {
        // max allowed login attempts
        private static int _logAttempts = 5;
        internal User LoggedOnUser { get; private set; }
        private Lazy<BaseController> _primaryController;
        private BaseController PrimaryBaseController => _primaryController.Value;


        internal override void Start()
        {
            while (true)
            {
                try
                {
                    LoggedOnUser = Login();
                    break;
                }
                catch (Exception ex)  when  (ex is NullReferenceException || ex is SqlException || ex is TypeLoadException)
                {
                    // is thrown when failed to create instance of user, trying again
                    Console.Clear();
                    Console.WriteLine($"Incorrect User name / Password{Environment.NewLine}");
                    Start();
                }
                catch (TooManyLoginsException ex)
                {
                    Console.WriteLine(ex.Message);
                    Environment.Exit(0);
                }
            }
            
            
            _primaryController = new Lazy<BaseController>(BuildPrimaryController(this));
            PrimaryBaseController.Start();
        }

        private User Login()
        {
            // if user exists, the caller is a child submenu and we need to log out
            if (LoggedOnUser != null && !Program.Testing) Environment.Exit(0);
            // if this is a test mode (set as string argument)
            if (Program.Testing) return DalFactory.User.GetUser("fake user name", "fake password");
            if (--_logAttempts < 0) throw new TooManyLoginsException("Exhausted max login attempts");

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
                        if (pass.Length <= 0) continue;
                        Console.Write("\b \b");
                        pass.Length--;
                    }
                }

                return pass.ToString();
            }))();

            return DalFactory.User.GetUser(userName, password);
        }
    }
}