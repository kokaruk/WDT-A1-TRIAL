using System;
using wdt.Model;
using wdt.utils;

namespace wdt.Controller
{
    internal abstract class Controller
    {
        // get primary controller based on logged on user
        internal static Controller GetPrimaryController(User loggedOnUser)
        {
            
            var controllerTypeName = $"wdt.controller.{loggedOnUser.Type.ToString()}PrimaryController";
            // use reflection to create instance 
            var controllerType = Type.GetType(controllerTypeName, true);
            var instance = Activator.CreateInstance(controllerType);
            return (Controller)instance;
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