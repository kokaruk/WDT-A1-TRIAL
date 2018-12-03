using System;
using wdt.utils;

namespace wdt.Controller
{
    internal abstract class BaseController
    {
        // get primary controller factory based on logged on user with reflection instance call
        internal static BaseController GetPrimaryController(LoginController loginController)
        {
            
            var controllerTypeName = $"wdt.Controller.{loginController.LoggedOnUser.UserType.ToString()}PrimaryController";
            // use reflection to create instance 
            var controllerType = Type.GetType(controllerTypeName, true);
            var instance = Activator.CreateInstance(controllerType, loginController);
            return (BaseController)instance;
        }
        
        // get user input from menu and max value
        internal static int GetInput(string menu, int maxInput, string prompt = "Enter an option: ")
        {
            while (true)
            {
                Console.WriteLine(menu);
                Console.Write(prompt);
                var input = Console.ReadLine();
                // return negative one, requesting function to handle as go up one level
                if (string.Empty.Equals(input)) return -1;
                if (int.TryParse(input, out var option) && option.IsWithinMaxValue(maxInput)) return option;
                Console.Clear();
                Console.Write($"Invalid Input{Environment.NewLine}");
            }
        }

        internal abstract void Start();

    }
}