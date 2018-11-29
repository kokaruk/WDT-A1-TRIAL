using System.Collections.Generic;
 
 namespace wdt.controller
 {
     internal abstract class MenuController : Controller
     {
         string MenuHeadaer { get; }
         MenuController Parent { get; set; }
         List<MenuController> Children { get; set; }
     }
 }