namespace Wdt.Controller
{
    // instance of class is created via static reflection in BaseController
    // ReSharper disable once UnusedMember.Global
    internal class CustomerPrimary : MenuControllerAdapter
    {
        public CustomerPrimary(BaseController parent) : base(parent)
        {
            MenuHeader = "Stores";   
        }
    }
}