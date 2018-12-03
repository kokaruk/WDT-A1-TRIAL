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
        private Lazy<BaseController> _primaryController = new Lazy<BaseController>();
        private BaseController PrimaryBaseController => _primaryController.Value;

        // constructor
        internal LoginController(bool isTesting)
        {
            _isTesting = isTesting;
        }

        internal override void Start()
        {
            LoggedOnUser = _isTesting ? GetFakeUerFromInput() : Login();
            _primaryController = new Lazy<BaseController>(GetPrimaryController(this));
            PrimaryBaseController.Start();
        }

        private static User Login()
        {
            Console.Clear();
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
            while (true)
            {
                Console.Clear();
                var userLogon = SelectUserMenu();
                var maxInput = Enum.GetNames(typeof(UserType)).Length;
                userLogon.Append($"{Environment.NewLine}{++maxInput}. Quit{Environment.NewLine}");
                var option = GetInput(userLogon.ToString(), maxInput);
                if (option < 0) continue;
                if (option == maxInput) Environment.Exit(0);
                var user = UserFactory.MakeUserFromInt(--option);
                // if user type is franchisee need to select store
                if (user.GetType() == typeof(Franchisee))
                {
                    ((Franchisee) user).Location = GetUserLocation();
                }
                return user;
            }
        }


        // get location for user
        private static Franchises GetUserLocation()
        {
            while (true)
            {
                Console.Clear();
                var locationMenu = SelectLocationMenu();
                var maxInput = Enum.GetNames(typeof(Franchises)).Length;
                var option = GetInput(locationMenu.ToString(), maxInput, "Enter Store to use: ");
                if (option < 0) continue;
                return (Franchises) option;
            }
        }

        // build user type select menu
        private static StringBuilder SelectUserMenu()
        {
            const string greetingHeader = "Select User Type";

            var userLogon = new StringBuilder(greetingHeader);
            userLogon.Append($"{Environment.NewLine}{greetingHeader.MenuHeaderPad()}");
            Enum.GetNames(typeof(UserType))
                .ToList()
                .ForEach(userType =>
                {
                    var count = (int) Enum.Parse(typeof(UserType), userType);
                    userLogon.Append($"{Environment.NewLine}{++count}. {userType}");
                });
            return userLogon;
        }

        // select location for franchisee
        private static StringBuilder SelectLocationMenu()
        {
            const string greetingHeader = "Select Store";
            var franchiseSelect = new StringBuilder(greetingHeader);
            franchiseSelect.Append($"{Environment.NewLine}{greetingHeader.MenuHeaderPad()}");
            Enum.GetValues(typeof(Franchises)).Cast<Franchises>()
                .ToList()
                .ForEach(franchise =>
                {
                    var count = (int) franchise;
                    franchiseSelect.Append($"{Environment.NewLine}{++count}. {franchise.GetStringValue()}");
                });
            return franchiseSelect.Append($"{Environment.NewLine}");
        }
    }
}