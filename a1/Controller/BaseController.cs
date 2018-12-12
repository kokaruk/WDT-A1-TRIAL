using System;
using Wdt.Utils;

namespace Wdt.Controller
{
    /// <summary>
    /// Base abstract controller, inherited by all Controllers
    /// </summary>
    internal abstract class BaseController
    {
        /// <summary>
        /// build primary controller factory based on logged on user with reflection instance call
        /// </summary>
        /// <param name="loginController">contains instances of logged on user and namespace string</param>
        /// <returns>instance of Primary Controller or login controller if error is thrown</returns>
        internal static BaseController BuildPrimaryController(LoginController loginController)
        {
            try
            {
                var baseNamespaceName = loginController.GetType().Namespace;
                var controllerTypeName =
                    $"{baseNamespaceName}.{loginController.LoggedOnUser.UserType.ToString()}Primary";
                // use reflection to create instance 
                var controllerType = Type.GetType(controllerTypeName, true);
                var instance = (BaseController) Activator.CreateInstance(controllerType, loginController);
                return instance;
            }
            catch (TypeLoadException)
            {
                return loginController;
            }
        }

        /// <summary>
        /// get user input from menu and max value
        /// </summary>
        /// <param name="menu">menu prompt string</param>
        /// <param name="maxInput">max allowed value</param>
        /// <param name="prompt">Optional param for input prompt, can be overriden from calling function</param>
        /// <param name="allowTextInput">Optional flag param allowing text input (for next / previous pagination)</param>
        /// <returns></returns>
        internal static int GetInput(string menu, int maxInput, string prompt = "Enter an option: ",
            bool allowTextInput = false)
        {
            while (true)
            {
                Console.WriteLine(menu);
                Console.Write(prompt);
                var input = Console.ReadLine();
                // if allowing text input, return negative values 
                // positive values are for selection of numeric options
                if (allowTextInput)
                {
                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (input)
                    {
                        case "n":
                        case "N":
                            return -2;
                        case "r":
                        case "R":
                            return -3;
                    }
                }

                // return negative one, requesting function to handle as go up one level
                if (string.Empty.Equals(input)) return -1;
                if (int.TryParse(input, out var option) && option.IsWithinMaxValue(maxInput)) return option;
                Console.Clear();
                Console.WriteLine("Invalid Input");
            }
        }

        /// <summary>
        /// abstract Start method
        /// </summary>
        internal abstract void Start();
    }
}