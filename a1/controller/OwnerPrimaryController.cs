namespace wdt.Controller
{
    // instance of class is created via static reflection in BaseController
    // ReSharper disable once UnusedMember.Global
    internal class OwnerPrimaryController : MenuControllerAdapter
    {
        public OwnerPrimaryController(BaseController parent) : base(parent)
        {
            MenuHeader = $"Welcome to Marvelous magic ({((LoginController) Parent).LoggedOnUser.UserType})";
            Children.Add(new OwnerAllStockRequestsController(this));
            Children.Add(new OwnerInventoryController(this));
            Children.Add(new OwnerResetInventoryItemController(this));
        }
    }
}