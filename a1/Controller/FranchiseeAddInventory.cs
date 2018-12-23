using System;
using System.Text;
using Wdt.DAL;
using Wdt.Model;
using Wdt.Utils;

namespace Wdt.Controller
{
    internal class FranchiseeAddInventory : MenuControllerAdapter
    {
        private bool _paginationRequired;
        private readonly string _location;

        public FranchiseeAddInventory(BaseController parent) : base(parent)
        {
            _location = ((Franchisee) ((LoginController) ((MenuControllerAdapter) Parent).Parent).LoggedOnUser)
                .Location.GetStringValue();
            MenuHeader = "Add New Inventory Item";
        }

        protected override void GetInput()
        {
            while (true)
            {
                var maxInput = BuildMenu(out var menu);
                var option = GetInput(menu.ToString(), maxInput, allowTextInput: true);
                switch (option)
                {
                    case -2:
                        if (_paginationRequired)
                        {
                            DalFactory.Franchisee.ResetStoreStocks();
                            DalFactory.Franchisee.CurrentInvPage++;
                        }
                        // typed next page request when next page is not available
                        else
                        {
                            Message = "Invalid Input";
                        }

                        break;
                    case -3: // option r
                    case -1: // option 'empty input' 
                        // reset inventory requests
                        DalFactory.Franchisee.ResetStoreStocks();
                        DalFactory.Franchisee.CurrentInvPage = 1;
                        Parent.Start();
                        break;
                    default:
                        var productId = DalFactory.Franchisee.NonStoreStocks(_location)[option - 1].Id;
                        DalFactory.Franchisee.CreateStockRequest(_location, productId, qty: 1);
                        DalFactory.Franchisee.ResetStoreStocks();
                        DalFactory.Franchisee.CurrentInvPage = 1;
                        Parent.Start();
                        break;
                }
            }
        }
        
        private int BuildMenu(out StringBuilder menu)
        {
            menu = new StringBuilder(MenuHeader);
            menu.Append($"{Environment.NewLine}{MenuHeader.MenuHeaderPad()}");
            menu.Append(Environment.NewLine);

            if (DalFactory.Franchisee.NonStoreStocks(_location).Count == 0)
            {
                menu.Append($"{Environment.NewLine}{_location}: No inventory found");
                menu.Append($"{Environment.NewLine}[Legend 'R' Return to Menu]");
                menu.Append($"{Environment.NewLine}{Message}");
            }
            else
            {
                const string format = "{0}{1, -4}{2, -25}{3}";
                menu.Append(string.Format(format, Environment.NewLine, "#", "Product", "Current Stock"));
                var storeStock = DalFactory.Franchisee.NonStoreStocks(_location);
                var rowNum = 0;
                foreach (var stock in storeStock)
                {
                    menu.Append(string.Format(format, Environment.NewLine,
                        ++rowNum,
                        stock.Name,
                        stock.Level
                    ));
                }

                var totalPages =
                    (int) Math.Ceiling(DalFactory.Franchisee.TotalInvItems / (decimal) DalFactory.Franchisee.Fetch);
                menu.Append(
                    $"{Environment.NewLine}{Environment.NewLine}Page {DalFactory.Franchisee.CurrentInvPage} of {totalPages}{Environment.NewLine}");

                _paginationRequired = DalFactory.Franchisee.TotalInvItems -
                                      DalFactory.Franchisee.Fetch * DalFactory.Franchisee.CurrentInvPage > 0;

                var nextPage = _paginationRequired ? "'N' Next Page | " : string.Empty;
                var legend = $"[Legend {nextPage}'R' Return to Menu]";
                menu.Append($"{Environment.NewLine}{legend}{Environment.NewLine}{Message}");

                return storeStock.Count;
            }

            return -1;
        }
        
    }
}