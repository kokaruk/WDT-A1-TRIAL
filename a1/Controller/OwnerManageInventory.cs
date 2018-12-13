using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Wdt.DAL;
using Wdt.Model;
using Wdt.Utils;

namespace Wdt.Controller
{
    internal class OwnerManageInventory : MenuControllerAdapter
    {
        private bool _paginationRequired;

        public OwnerManageInventory(BaseController parent) : base(parent)
        {
            MenuHeader = "Manage Owner Inventory";
        }


        protected override void GetInput()
        {
            var maxInput = BuildMenu(out var menu);
            var option = GetInput(menu.ToString(), maxInput,
                "Enter product number to process or function: ",
                allowTextInput: true);

            // process next page
            if (option == -2)
            {
                if (_paginationRequired)
                {
                    DalFactory.Owner.OwnerInventories = new List<OwnerInventory>();
                    DalFactory.Owner.CurrentInvPage++;
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

            // go back to previous
            if (option == -3 || option == -1)
            {
                // reset Inv  
                {
                    DalFactory.Owner.OwnerInventories = new List<OwnerInventory>();
                    DalFactory.Owner.CurrentInvPage = 1;
                }

                Parent.Start();
            }
            else
            {
                var updateThreshold = Program.InvResetThreshold;
                if (DalFactory.Owner.OwnerInventories[option - 1].StockLevel >= updateThreshold)
                {
                    Console.Clear();
                    Console.WriteLine($"{DalFactory.Owner.OwnerInventories[option - 1].Name} has enough stock");
                    Console.WriteLine();
                }
                else
                {
                    try
                    {
                        var productId = DalFactory.Owner.OwnerInventories[option - 1].ProductId;
                        DalFactory.Owner.ResetStockLevel(productId, updateThreshold);
                        DalFactory.Owner.OwnerInventories = new List<OwnerInventory>();
                        Console.Clear();
                        Console.WriteLine(
                            $"{DalFactory.Owner.OwnerInventories[option - 1].Name} has been reset to {updateThreshold}");
                        Console.WriteLine();
                    }
                    catch (SqlException e)
                    {
                        Console.WriteLine(e.Message);
                    }
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
            const string format = "{0}{1, -4}{2, -25}{3}";
            menu.Append(string.Format(format, Environment.NewLine, "#", "Product", "Current Stock"));
            var inventories = DalFactory.Owner.OwnerInventories;
            var rowNum = 0;
            foreach (var ownerInventory in inventories)
            {
                menu.Append(string.Format(format, Environment.NewLine,
                    ++rowNum,
                    ownerInventory.Name,
                    ownerInventory.StockLevel
                ));
            }

            _paginationRequired = DalFactory.Owner.TotalInvItems -
                                  DalFactory.Owner.Fetch * DalFactory.Owner.CurrentInvPage > 0;

            var legend = _paginationRequired ? "[Legend 'N' Next Page | 'R' Return to Menu]" : "'R' Return to menu";
            menu.Append($"{Environment.NewLine}{Environment.NewLine}{legend}{Environment.NewLine}");
            return inventories.Count;
        }
    }
}