using System.Collections.Generic;

namespace wdt.Controller
{
    internal abstract class MenuControllerAdapter: BaseController, IMenuController
    {
        public string MenuHeader { get; set; }
        public BaseController Parent { get; }
        public List<IMenuController> Children { get; set; } = new List<IMenuController>();

        protected MenuControllerAdapter(BaseController parent)
        {
            Parent = parent;
        }
    }
}