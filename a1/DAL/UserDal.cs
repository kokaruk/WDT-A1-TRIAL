using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Wdt.Controller;
using Wdt.Model;
using Wdt.Utils;

namespace Wdt.DAL
{
    /// <summary>
    /// user database layer, implements lazy singleton
    /// </summary>
    public class UserDal : IUserDal
    {
        
       //  static instance holder
        private static readonly Lazy<IUserDal> _instance = new Lazy<IUserDal>(() => new UserDal());
        
        // private constructor
        private UserDal()
        {
        }

        // accessor for instance 
        public static IUserDal Instance => Program.Testing ? new FakeUserDal() : _instance.Value;
        
        public User GetUser(string userName, string password)
        {
            var connParams = new Dictionary<string, string>
            {
                {"userName", userName}, 
                {"password", password}
            };
            var userType = DalFactory.ExecuteScalar("get user type", connParams);
            var user = UserFactory.MakeUser(userName, userType);
            return user;
        }
        /// <summary>
        /// fake userClass for testing mode only, when no user logon is required
        /// </summary>
        private class FakeUserDal : IUserDal
        {
            public User GetUser(string userName, string password)
            {
                while (true)
                {
                    Console.Clear();
                    var userLogon = SelectUserMenu();
                    var maxInput = Enum.GetNames(typeof(UserType)).Length;
                    userLogon.Append($"{Environment.NewLine}{++maxInput}. Quit{Environment.NewLine}");
                    var option = BaseController.GetInput(userLogon.ToString(), maxInput);
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

            /// <summary>
            /// build user type select menu
            /// </summary>
            /// <returns>user logon requests</returns>
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

            /// <summary>
            /// get instance franchise for user
            /// </summary>
            /// <returns>franchise enum member</returns>
            private static Franchises GetUserLocation()
            {
                while (true)
                {
                    Console.Clear();
                    var locationMenu = SelectLocationMenu();
                    var maxInput = Enum.GetNames(typeof(Franchises)).Length;
                    var option = BaseController.GetInput(locationMenu.ToString(), maxInput, "Enter Store to use: ");
                    if (option < 0) continue;
                    return (Franchises) (--option);
                }
            }

            /// <summary>
            /// select location for franchisee
            /// </summary>
            /// <returns></returns>
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
    
}