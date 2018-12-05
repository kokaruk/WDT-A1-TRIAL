namespace Wdt.Controller
{
    // instance of class is created via static reflection in BaseController
    // ReSharper disable once UnusedMember.Global
    internal class OwnerPrimary : MenuControllerAdapter
    {
        public OwnerPrimary(BaseController parent) : base(parent)
        {
            MenuHeader = $"Welcome to Marvelous magic ({((LoginController) Parent).LoggedOnUser.UserType})";
            Children.Add(new OwnerAllStockRequests(this));
            Children.Add(new OwnerInventory(this));
            Children.Add(new OwnerResetInventoryItem(this));
        }
    }
}