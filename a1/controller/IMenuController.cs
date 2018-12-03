using System.Text;

namespace wdt.Controller
 {
     internal interface IMenuController
     {
         string MenuHeader { get; set; }
         BaseController Parent { get; }
         int BuildMenu(out StringBuilder menu);
     }
 }