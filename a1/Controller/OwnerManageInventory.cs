using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
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
            while (true)
            {
                var maxInput = BuildMenu(out var menu);
                var option = GetInput(menu.ToString(), maxInput, "Enter product number to reset stock or function: ",
                    allowTextInput: true);


                switch (option)
                {
                    // next page in pagination
                    case -2:
                        if (_paginationRequired)
                        {
                            DalFactory.Owner.OwnerInventories = new List<OwnerInventory>();
                            DalFactory.Owner.CurrentInvPage++;
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
                        DalFactory.Owner.OwnerInventories = new List<OwnerInventory>();
                        DalFactory.Owner.CurrentInvPage = 1;
                        Parent.Start();
                        break;
                    default:
                        // read threshold from app settings
                        var updateThreshold = Program.InvResetThreshold;
                        if (DalFactory.Owner.OwnerInventories[option - 1].StockLevel >= updateThreshold)
                        {
                            Message = $"\"{DalFactory.Owner.OwnerInventories[option - 1].Name}\" has enough stock";
                        }
                        else
                        {
                            try
                            {
                                var productId = DalFactory.Owner.OwnerInventories[option - 1].ProductId;
                                DalFactory.Owner.ResetStockLevel(productId, updateThreshold);
                                DalFactory.Owner.OwnerInventories = new List<OwnerInventory>();
                                Message = $"\"{DalFactory.Owner.OwnerInventories[option - 1].Name}\" has been reset to {updateThreshold}";
                            }
                            catch (SqlException e)
                            {
                                Message = e.Message;
                            }
                        }

                        break;
                }
            }
        }

        private new int BuildMenu(out StringBuilder menu)
        {
            Console.Clear();
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
            
            
            var totalPages =
                (int) Math.Ceiling(DalFactory.Owner.TotalInvItems / (decimal) DalFactory.Owner.Fetch);
            menu.Append(
                $"{Environment.NewLine}{Environment.NewLine}Page {DalFactory.Owner.CurrentInvPage} of {totalPages}{Environment.NewLine}");

            _paginationRequired = DalFactory.Owner.TotalInvItems -
                                  DalFactory.Owner.Fetch * DalFactory.Owner.CurrentInvPage > 0;
            var nextPage = _paginationRequired ? "'N' Next Page | " : string.Empty;
            var legend = $"[Legend {nextPage}'R' Return to Menu]";
            menu.Append($"{Environment.NewLine}{legend}{Environment.NewLine}{Message}");
            return inventories.Count;
        }
    }
}