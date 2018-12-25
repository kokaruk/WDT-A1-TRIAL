using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Wdt.DAL;
using Wdt.Model;
using Wdt.Utils;

namespace Wdt.Controller
{
    internal class CustomerDisplayProducts  : MenuControllerAdapter
    {

        public Franchises Location { private get; set; }
        private bool _paginationRequired;
        
        public CustomerDisplayProducts(BaseController parent) : base(parent)
        {
            MenuHeader = "Display Products";
        }

        protected override void GetInput()
        {
            while (true)
            {
                var maxInput = BuildMenu(out var menu);
                var option = GetInput(menu.ToString(), maxInput, "Enter product number to purchase or function: ",
                    allowTextInput: true);

                switch (option)
                {
                    // next page in pagination
                    case -2:
                        if (_paginationRequired)
                        {
                            DalFactory.Franchise.ResetStoreStocks();
                            DalFactory.Franchise.CurrentInvPage++;
                        }
                        // typed next page request when next page is not available
                        else
                        {
                            Message = "Invalid Input";
                        }
                        break;
                    // go back to previous    
                    case -3: // option r
                    case -1: // option 'empty input'
                        // reset Stock requests 
                        DalFactory.Franchise.ResetStoreStocks();
                        DalFactory.Franchise.CurrentInvPage = 1;
                        Parent.Start();
                        break;
                    default:
                        var availStock = DalFactory.Franchise.StoreStocks(Location.GetStringValue())[option-1].Level;
                        var purchQty = GetInput(prompt: "Enter quantity to purchase: ");
                        if (purchQty > availStock)
                        {
                            Message =
                                $"\"{DalFactory.Franchise.StoreStocks(Location.GetStringValue())[option-1].Name}\" doesn't have enough stock";
                            Console.Clear();
                        }
                        else
                        {
                            try
                            {
                                DalFactory.Franchise.PurchaseProduct(Location.GetStringValue(), 
                                    DalFactory.Franchise.StoreStocks(Location.GetStringValue())[option-1].Id,
                                    availStock - purchQty);
                                DalFactory.Franchise.ResetStoreStocks();
                                Message =
                                    $"Purchased {purchQty} of \"{DalFactory.Franchise.StoreStocks(Location.GetStringValue())[option-1].Name}\"";
                            }
                            catch (SqlException)
                            {
                                Message = "Error making a purchase, please try again";
                            }
                            Console.Clear();
                        }
                        break;
                }
            }
        }

        private new int BuildMenu(out StringBuilder menu)
        {

            var header = MenuHeader + $" ({Location.GetStringValue()})";
            
            menu = new StringBuilder(header);
            menu.Append($"{Environment.NewLine}{header.MenuHeaderPad()}");
            menu.Append(Environment.NewLine);

            if (DalFactory.Franchise.StoreStocks(Location.GetStringValue()).Count == 0)
            {
                menu.Append($"{Environment.NewLine}{Location.GetStringValue()}: No inventory found");
                menu.Append($"{Environment.NewLine}[Legend 'R' Return to Menu]");
                menu.Append($"{Environment.NewLine}{Message}");
            }
            else
            {
                const string format = "{0}{1, -4}{2, -25}{3}";
                menu.Append(string.Format(format, Environment.NewLine, "#", "Product", "Current Stock"));
                var storeStock = DalFactory.Franchise.StoreStocks(Location.GetStringValue());
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
                    (int) Math.Ceiling(DalFactory.Franchise.TotalInvItems / (decimal) DalFactory.Franchise.Fetch);
                menu.Append(
                    $"{Environment.NewLine}{Environment.NewLine}Page {DalFactory.Franchise.CurrentInvPage} of {totalPages}{Environment.NewLine}");

                _paginationRequired = DalFactory.Franchise.TotalInvItems -
                                      DalFactory.Franchise.Fetch * DalFactory.Franchise.CurrentInvPage > 0;

                var nextPage = _paginationRequired ? "'N' Next Page | " : string.Empty;
                var legend = $"[Legend {nextPage}'R' Return to Menu]";
                menu.Append($"{Environment.NewLine}{legend}{Environment.NewLine}{Message}");
            }

            return DalFactory.Franchise.TotalInvItems;
        }
        
    }
}