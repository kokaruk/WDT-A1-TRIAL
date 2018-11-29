using System.Collections.Generic;
 
 namespace wdt.Controller
 {
     internal interface IMenuController
     {
         string MenuHeader { get; set;  }
         BaseController Parent { get; }
         List<IMenuController> Children { get; set; }
     }
 }