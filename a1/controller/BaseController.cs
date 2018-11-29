using System;
using wdt.utils;

namespace wdt.Controller
{
    internal abstract class BaseController
    {
        // get primary controller based on logged on user
        internal static BaseController GetPrimaryController(LoginController loginController)
        {
            
            var controllerTypeName = $"wdt.Controller.{loginController.LoggedOnUser.Type.ToString()}PrimaryController";
            // use reflection to create instance 
            var controllerType = Type.GetType(controllerTypeName, true);
            var instance = Activator.CreateInstance(controllerType, loginController);
            return (BaseController)instance;
        }
        
        // get user inout from menu and max value
        internal static int GetInput(string menu, int maxInput)
        {
            while (true)
            {
                Console.WriteLine(menu);
                Console.Write("Enter an option: ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out var option) && option.IsWithinMaxValue(maxInput)) return option;
                Console.Clear();
                Console.Write("Invalid Input\n\n");
            }
        }

        internal abstract void Start();

    }
}