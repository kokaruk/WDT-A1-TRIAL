using System;
using wdt.Model;

namespace wdt.Controller
{
    internal class OwnerPrimaryController : MenuControllerAdapter
    {
        
        public OwnerPrimaryController(BaseController parent) : base(parent)
        {
            MenuHeader = "All Stock Requests";
            Children.Add(new OwnerAllStockRequestsController(this));
            Children.Add(new OwnerInventoryController(this));
            Children.Add(new OwnerResetInventoryItemController(this));
        }

        internal override void Start()
        {
            Console.Clear();
            Console.WriteLine("Owner Primary Takes Control");
            Console.WriteLine($"\nUsername: {  ((LoginController)Parent).LoggedOnUser.Name}, Type: {((LoginController)Parent).LoggedOnUser.Type} ");
        }
    }
}