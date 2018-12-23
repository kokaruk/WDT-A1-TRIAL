using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Wdt.Utils;

namespace Wdt.Controller
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressMessage("ReSharper", "InheritdocConsiderUsage")]
    internal abstract class MenuControllerAdapter : BaseController, IMenuController
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

        protected virtual void GetInput()
        {
            var maxInput = BuildMenu(out var menu);
            var option = GetInput(menu.ToString(), maxInput);
            if (option == maxInput || option == -1)
            {
                if (Parent.GetType() == typeof(LoginController) && !Program.Testing)
                {
                    if (option == -1)
                    {
                        Console.Clear();
                        // ReSharper disable once TailRecursiveCall
                        GetInput();
                    }
                    else
                    {
                        Environment.Exit(0);    
                    }
                    
                }
                else
                {
                    Parent.Start();
                }
            }
            else
            {
                Children[--option].Start();
            }
        }

        /// <summary>
        /// menu string builder
        /// </summary>
        /// <param name="menu"> string builder for menu contents</param>
        /// <returns></returns>
        public int BuildMenu(out StringBuilder menu)
        {
            menu = new StringBuilder(MenuHeader);
            menu.Append($"{Environment.NewLine}{MenuHeader.MenuHeaderPad()}");
            var maxValue = Children.Count + 1;
            if (Children.Count > 0)
            {
                for (var i = 0; i < Children.Count; i++)
                {
                    menu.Append($"{Environment.NewLine}{i + 1}. {Children[i].MenuHeader}");
                }
            }

            menu.Append(Parent.GetType() == typeof(LoginController) && !Program.Testing
                ? $"{Environment.NewLine}{maxValue}. Quit{Environment.NewLine}"
                : $"{Environment.NewLine}{maxValue}. Return to Main Menu{Environment.NewLine}");
            return maxValue;
        }
    }
}