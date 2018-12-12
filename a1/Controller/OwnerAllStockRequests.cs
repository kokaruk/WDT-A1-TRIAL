using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Wdt.DAL;
using static Wdt.DAL.DalFactory;
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
            var maxInput = BuildMenu(out var menu);
            var option = GetInput(menu.ToString(), maxInput, "Enter request number to process or function: ", true);

            // process next page
            if (option == -2)
            {
                if (_paginationRequired)
                {
                    StockRequests.Items = new List<StockRequest>();
                    StockRequests.CurrentPage++;
                }
                // typed next page request when next page is not available
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid Input");
                    Console.WriteLine();
                }

                GetInput();
            }


            if (option == -3 || option == -1)
            {
                // reset Stock requests 
                {
                    StockRequests.Items = new List<StockRequest>();
                    StockRequests.CurrentPage = 1;
                }

                Parent.Start();
            }
            else
            {
                if (StockRequests.Items[option - 1].StockAvail)
                {
                    try
                    {
                        var requestId = StockRequests.Items[option - 1].Id;
                        StockRequests.ActionStockRequest(requestId);
                        StockRequests.Items = new List<StockRequest>();
                        if (StockRequests.TotalUserRequests % StockRequests.Fetch == 0) StockRequests.CurrentPage--;  
                    }
                    catch (SqlException e)
                    {
                        Console.Clear();
                        Console.WriteLine(e);
                    }
                      
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Insufficient Stock");
                    Console.WriteLine();
                }
                
                // ReSharper disable once TailRecursiveCall
                GetInput();
            }
        }

        private new int BuildMenu(out StringBuilder menu)
        {
            menu = new StringBuilder(MenuHeader);
            menu.Append($"{Environment.NewLine}{MenuHeader.MenuHeaderPad()}");
            menu.Append(Environment.NewLine);
            const string format = "{0}{1, -4}{2, -25}{3, -25}{4, -10}{5, -15}{6, -5}";
            menu.Append(string.Format(format,
                Environment.NewLine, "#", "Store", "Product", "Quantity", "Current Stock", "Stock Availability"));
            var requests = StockRequests.Items;
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

            _paginationRequired = StockRequests.TotalUserRequests -
                                  StockRequests.Fetch * StockRequests.CurrentPage > 0;

            var legend = _paginationRequired ? "[Legend 'N' Next Page | 'R' Return to Menu]" : "'R' Return to menu";
            menu.Append($"{Environment.NewLine}{Environment.NewLine}{legend}{Environment.NewLine}");
            return requests.Count;
        }
    }
}