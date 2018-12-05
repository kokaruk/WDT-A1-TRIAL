using System.Diagnostics.CodeAnalysis;
using Wdt.Model;
using Wdt.Utils;

namespace Wdt.Controller
{
    // instance of class is created via static reflection in BaseController
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal class FranchiseePrimary : MenuControllerAdapter
    {
        public FranchiseePrimary(BaseController parent) : base(parent)
        {
            // build menu header string
            var userType = ((LoginController) Parent).LoggedOnUser.UserType;
            var location = ((Franchisee) ((LoginController) Parent).LoggedOnUser).Location.GetStringValue();
            MenuHeader = $"Welcome to Marvelous magic ({userType} - {location})";
            Children.Add(new FranchiseeDisplayInventory(this));
            Children.Add(new FranchiseeStockRequest(this));
            Children.Add(new FranchiseeAddInventory(this));
        }
    }
}