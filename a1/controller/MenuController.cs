using System.Collections.Generic;

namespace wdt.controller
{
    public abstract class MenuController
    {
        MenuController Parent { get; set; }
        private List<MenuController> Children { get; set; }
    }
}