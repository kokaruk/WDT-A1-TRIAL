using System.Collections.Generic;
using System.Threading.Tasks;
using Wdt.Model;

namespace Wdt.DAL
{
    public interface IOwnerDal
    {
        int Fetch { get; }
        int CurrentRequestsPage { get; set; }
        int CurrentInvPage { get; set;  }
        int TotalStockRequests { get; }
        int TotalInvItems { get; }
        List<StockRequest> StockRequests { get; set; }
        void ActionStockRequest(int requestId);
        void ResetStockLevel(int productId, int level);
        List<OwnerInventory> OwnerInventories { get; set; }
    }
}