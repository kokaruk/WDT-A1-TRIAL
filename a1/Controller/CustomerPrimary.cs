using System;
using System.Linq;
using System.Text;
using Wdt.Model;
using Wdt.Utils;

namespace Wdt.Controller
{
    // instance of class is created via static reflection in BaseController
    // ReSharper disable once UnusedMember.Global
    internal class CustomerPrimary : MenuControllerAdapter
    {
        public CustomerPrimary(BaseController parent) : base(parent)
        {
            MenuHeader = "Stores";
            Children.Add(new CustomerDisplayProducts(this));
        }

        protected override void GetInput()
        {
            while (true)
            {
                var maxInput = BuildMenu(out var menu);
                var option = GetInput(menu.ToString(), maxInput, "Enter Store to use: ");
                if (option == -1)
                {
                    Parent.Start();
                }
                else
                {
                    ((CustomerDisplayProducts) Children[0]).Location = (Franchises) (--option);
                    Children[0].Start();
                }
            }
        }

        private new int BuildMenu(out StringBuilder menu)
        {
            menu = new StringBuilder(MenuHeader);
            menu.Append($"{Environment.NewLine}{MenuHeader.MenuHeaderPad()}");
            menu.Append(Environment.NewLine);
            var stores = new StringBuilder();
            Enum.GetValues(typeof(Franchises)).Cast<Franchises>()
                .ToList()
                .ForEach(franchise =>
                {
                    var count = (int) franchise;
                    stores.Append($"{Environment.NewLine}{++count}. {franchise.GetStringValue()}");
                });
            menu.Append(stores);
            menu.Append(Environment.NewLine);
            return Enum.GetNames(typeof(Franchises)).Length;
        }
    }
}