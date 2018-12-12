using System.Collections.Generic;
using Wdt.Model;

namespace Wdt.DAL
{
    public interface IStockReqDal
    {
        int Fetch { get; }
        int CurrentPage { get; set; }
        int TotalUserRequests { get; }
        List<StockRequest> Items { get; set; }
        void ActionStockRequest(int requestId);
    }
}