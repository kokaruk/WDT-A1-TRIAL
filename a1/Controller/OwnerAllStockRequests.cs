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
    internal class OwnerAllStockRequests : MenuControllerAdapter
    {
        private bool _paginationRequired;
        private string _message = string.Empty;

        public OwnerAllStockRequests(BaseController parent) : base(parent)
        {
            MenuHeader = "All Stock Requests";
        }

        protected override void GetInput()
        {
            var maxInput = BuildMenu(out var menu);
            if (DalFactory.Owner.StockRequests.Count > 0)
            {
                var option = GetInput(menu.ToString(), maxInput,
                    "Enter request number to process or function: ",
                    allowTextInput: true);

                // process next page
                if (option == -2)
                {
                    if (_paginationRequired)
                    {
                        DalFactory.Owner.StockRequests = new List<StockRequest>();
                        DalFactory.Owner.CurrentRequestsPage++;
                    }
                    // typed next page request when next page is not available
                    else
                    {
                        _message = $"Invalid Input{Environment.NewLine}";
                    }

                    GetInput();
                }

                // go back to previous
                if (option == -3 || option == -1)
                {
                    // reset Stock requests 
                    {
                        DalFactory.Owner.StockRequests = new List<StockRequest>();
                        DalFactory.Owner.CurrentRequestsPage = 1;
                    }
                    Parent.Start();
                }
                else
                {
                    if (DalFactory.Owner.StockRequests[option - 1].StockAvail)
                    {
                        try
                        {
                            var requestId = DalFactory.Owner.StockRequests[option - 1].Id;
                            DalFactory.Owner.ActionStockRequest(requestId);
                            DalFactory.Owner.StockRequests = new List<StockRequest>();
                            if (DalFactory.Owner.TotalStockRequests % DalFactory.Owner.Fetch == 0 
                                && DalFactory.Owner.CurrentRequestsPage > 1) DalFactory.Owner.CurrentRequestsPage--;
                            
                            _message = $"Request complete{Environment.NewLine}";
                            
                        }
                        catch (SqlException e)
                        {
                            Console.Clear();
                            Console.WriteLine(e.Message);
                        }
                    }
                    else
                    {
                        Console.Clear();
                        _message = $"Insufficient Stock{Environment.NewLine}";
                        Console.WriteLine();
                    }

                    // ReSharper disable once TailRecursiveCall
                    GetInput();
                }
            }
            else
            {
                var option = GetInput(menu.ToString(), maxInput);
                if (option == maxInput || option == -1)
                {
                    Parent.Start();
                }
            }
        }

        private new int BuildMenu(out StringBuilder menu)
        {
            menu = new StringBuilder(MenuHeader);
            menu.Append($"{Environment.NewLine}{MenuHeader.MenuHeaderPad()}");
            menu.Append(Environment.NewLine);
            menu.Append(_message);
            _message = string.Empty;
            
            var requests = DalFactory.Owner.StockRequests;
            if (requests.Count > 0)
            {
                const string format = "{0}{1, -4}{2, -25}{3, -25}{4, -10}{5, -15}{6, -5}";
                menu.Append(string.Format(format,
                    Environment.NewLine, "#", "Store", "Product", "Quantity", "Current Stock", "Stock Availability"));

                var rowNum = 0;
                foreach (var stockRequest in requests)
                {
                    menu.Append(string.Format(format, Environment.NewLine,
                        ++rowNum,
                        stockRequest.Store,
                        stockRequest.Product,
                        stockRequest.Quantity,
                        stockRequest.StockLevel,
                        stockRequest.StockAvail
                    ));
                }

                _paginationRequired = DalFactory.Owner.TotalStockRequests -
                                      DalFactory.Owner.Fetch * DalFactory.Owner.CurrentRequestsPage > 0;

                var legend = _paginationRequired ? "[Legend 'N' Next Page | 'R' Return to Menu]" : "'R' Return to menu";
                menu.Append($"{Environment.NewLine}{Environment.NewLine}{legend}{Environment.NewLine}");
                return requests.Count;
            }
            else
            {
                menu.Append($"{Environment.NewLine}No stock requests found");
                menu.Append($"{Environment.NewLine}1. Return to previous menu{Environment.NewLine}");
                return 1;
            }
        }
    }
}