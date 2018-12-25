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

        public OwnerAllStockRequests(BaseController parent) : base(parent)
        {
            MenuHeader = "All Stock Requests";
        }

        protected override void GetInput()
        {
            while (true)
            {
                var maxInput = BuildMenu(out var menu);

                if (DalFactory.Owner.StockRequests.Count == 0)
                {
                    var option = GetInput(menu.ToString(), maxInput, allowTextInput: true);
                    if (option == -1 || option == -3)
                    {
                        Parent.Start();
                    }
                    else
                    {
                        Message = "Invalid Input";
                    }
                }
                else
                {
                    var option = GetInput(menu.ToString(), maxInput, "Enter request number to process or function: ",
                        allowTextInput: true);

                    switch (option)
                    {
                        // next page in pagination
                        case -2:
                            if (_paginationRequired)
                            {
                                DalFactory.Owner.StockRequests = new List<StockRequest>();
                                DalFactory.Owner.CurrentRequestsPage++;
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
                            DalFactory.Owner.StockRequests = new List<StockRequest>();
                            DalFactory.Owner.CurrentRequestsPage = 1;
                            Parent.Start();
                            break;
                        // action a stock request
                        default:
                            if (DalFactory.Owner.StockRequests[option - 1].StockAvail)
                            {
                                try
                                {
                                    var requestId = DalFactory.Owner.StockRequests[option - 1].Id;
                                    DalFactory.Owner.ActionStockRequest(requestId);
                                    DalFactory.Owner.StockRequests = new List<StockRequest>();
                                    if (DalFactory.Owner.TotalStockRequests % DalFactory.Owner.Fetch == 0 &&
                                        DalFactory.Owner.CurrentRequestsPage > 1)
                                        DalFactory.Owner.CurrentRequestsPage--;

                                    Message = "Request complete";
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
                                Message = ">>>>Insufficient Stock<<<<<";
                            }

                            break;
                    }
                }
            }
        }

        private new int BuildMenu(out StringBuilder menu)
        {
            menu = new StringBuilder(MenuHeader);
            menu.Append($"{Environment.NewLine}{MenuHeader.MenuHeaderPad()}");
            menu.Append(Environment.NewLine);

            if (DalFactory.Owner.StockRequests.Count == 0)
            {
                menu.Append($"{Environment.NewLine}No stock requests found");
                menu.Append($"{Environment.NewLine}[Legend 'R' Return to Menu]");
                menu.Append($"{Environment.NewLine}{Message}");
            }
            else
            {
                const string format = "{0}{1, -4}{2, -25}{3, -25}{4, -10}{5, -15}{6, -5}";
                menu.Append(string.Format(format,
                    Environment.NewLine, "#", "Store", "Product", "Quantity", "Current Stock", "Stock Availability"));

                var rowNum = 0;
                foreach (var stockRequest in DalFactory.Owner.StockRequests)
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

                var totalPages =
                    (int) Math.Ceiling(DalFactory.Owner.TotalStockRequests / (decimal) DalFactory.Owner.Fetch);
                menu.Append(
                    $"{Environment.NewLine}{Environment.NewLine}Page {DalFactory.Owner.CurrentRequestsPage} of {totalPages}{Environment.NewLine}");

                _paginationRequired = DalFactory.Owner.TotalStockRequests -
                                      DalFactory.Owner.Fetch * DalFactory.Owner.CurrentRequestsPage > 0;

                var nextPage = _paginationRequired ? "'N' Next Page | " : string.Empty;
                var legend = $"[Legend {nextPage}'R' Return to Menu]";
                menu.Append($"{Environment.NewLine}{legend}{Environment.NewLine}{Message}");
                return DalFactory.Owner.StockRequests.Count;
            }

            return -1;
        }
    }
}