namespace Wdt.Controller
{
    internal class FranchiseeStockRequest : MenuControllerAdapter
    {
        public FranchiseeStockRequest(BaseController parent) : base(parent)
        {
            MenuHeader = "Stock Requests (Threshold)";
        }
    }
}