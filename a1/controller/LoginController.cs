using System;
using System.Linq;
using System.Text;
using wdt.DAL;
using wdt.Model;
using wdt.utils;

namespace wdt.Controller
{
    internal class LoginController : BaseController
    {
        // max allowed login attempts
        private static int _logAttempts = 3;
        internal User LoggedOnUser { get; private set; }
        private readonly bool _isTesting;
        private Lazy<BaseController> _primaryController;
        internal BaseController PrimaryBaseController => _primaryController.Value;


        //{ get; } = new Lazy<BaseController>( () => BaseController.GetPrimaryController(LoggedOnUser) )  ;
        private BaseController _childBaseController;

        // constructor
        internal LoginController(bool isTesting)
        {
            _isTesting = isTesting;
        }

        internal override void Start()
        {
            // base class start clears the screen only
            Console.Clear();
            LoggedOnUser = _isTesting ? GetFakeUerFromInput() : Login();
            _primaryController = new Lazy<BaseController>(GetPrimaryController(this));
            PrimaryBaseController.Start();
        }

        private static User Login()
        {
            if (--_logAttempts < 0)
            {
                throw new TooManyLoginsException("Exhausted max login attempts");
            }

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
            const string greetingHeader = "Select User Type";

            var userLogon = new StringBuilder(greetingHeader);
            userLogon.Append($"\n{greetingHeader.MenuPad()}");
            // menu options counter
            Enum.GetNames(typeof(UserType))
                .ToList()
                .ForEach(type =>
                {
                    var count = (int) Enum.Parse(typeof(UserType), type) + 1;
                    userLogon.Append($"\n{count}. {type}");
                });

            var maxInput = Enum.GetNames(typeof(UserType)).Length + 1;
            userLogon.Append($"\n{maxInput}. Quit\n");
            var option = GetInput(userLogon.ToString(), maxInput);
            if (option == 4) Environment.Exit(0);
            return UserFactory.MakeUserFromInt(--option);
        }
    }
}