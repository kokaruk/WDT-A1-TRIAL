using System.Collections.Generic;
using System.Text;
using wdt.utils;
using System;

namespace wdt.Controller
{
    internal abstract class MenuControllerAdapter: BaseController, IMenuController
    {
        public string MenuHeader { get; set; }
        public BaseController Parent { get; }
        protected List<MenuControllerAdapter> Children { get; } = new List<MenuControllerAdapter>();

        protected MenuControllerAdapter(BaseController parent)
        {
            Parent = parent;
        }
        
        internal override void Start()
        {
            Console.Clear();
            GetInput();
        }
        
        private void GetInput()
        {
            var maxInput = BuildMenu(out var menu);
            var option = GetInput(menu.ToString(), maxInput);
            if (option == maxInput || option == -1)
            {
                Parent.Start();
            }
            else
            {
                Children[--option].Start();
            }
            
        }
        
        // build menu string builder
        public int BuildMenu(out StringBuilder menu)
        {
            menu = new StringBuilder(MenuHeader);
            menu.Append($"{Environment.NewLine}{MenuHeader.MenuHeaderPad()}");
            var maxValue = Children.Count + 1;
            // ReSharper waring results in double return statement
            // ReSharper disable once InvertIf
            if (Children.Count > 0)
            {
                for (var i = 0; i < Children.Count; i++)
                {
                    menu.Append($"{Environment.NewLine}{i + 1}. {Children[i].MenuHeader}");
                }
            }
            menu.Append($"{Environment.NewLine}{maxValue}. Return to Main Menu{Environment.NewLine}");
            return maxValue;
        }
        
        
    }
}