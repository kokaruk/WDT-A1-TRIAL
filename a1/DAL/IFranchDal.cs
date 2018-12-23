using System.Collections.Generic;
using Wdt.Model;

namespace Wdt.DAL
{
    public interface IFranchDal
    {
        int Fetch { get; }
        int CurrentInvPage { get; set; }
        int TotalInvItems { get; }
        List<StoreStock> StoreStocks(string storeName);
        List<StoreStock> StoreStocksThreshold(string storeName, int threshold);
        List<StoreStock> NonStoreStocks(string storeName);
        void ResetStoreStocks();
        void CreateStockRequest(string location, int prodId, int qty);
    }
}