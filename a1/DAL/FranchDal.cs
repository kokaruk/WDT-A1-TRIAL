using System;
using System.Collections.Generic;
using System.Linq;
using Wdt.Model;

namespace Wdt.DAL
{
    public class FranchDal : IFranchDal
    {
        public int Fetch { get; } = Program.FetchLines;
        public int CurrentInvPage { get; set; } = 1;
        public int TotalInvItems { get; private set; }


        private static readonly Lazy<IFranchDal> _instance = new Lazy<IFranchDal>(() => new FranchDal());

        private readonly IDalDbProxy _dbProxy = DalDbProxy.Instance;

        private FranchDal()
        {
        }

        public static IFranchDal Instance => _instance.Value;

        private List<StoreStock> _storeStocks = new List<StoreStock>();

        public List<StoreStock> StoreStocks(string storeName)
        {
            if (_storeStocks.Any()) return _storeStocks;
            UpdateCountStoreStocks(storeName);
            _storeStocks = TotalInvItems > 0 ? ((Func<List<StoreStock>>) (() =>
            {
                var connParams = new Dictionary<string, dynamic>
                {
                    {"storename", storeName},
                    {"offset", Fetch * CurrentInvPage - Fetch},
                    {"fetch", Fetch}
                };
                var table = _dbProxy.GetDataTable("get stock for store", connParams);
                var items = table.Select().Select(x =>
                    new StoreStock(
                        (int) x["ProductID"],
                        (string) x["product"],
                        (int) x["StockLevel"])).ToList();
                return items;
            }))() : new List<StoreStock>();
            return _storeStocks;
        }
        
        public List<StoreStock> NonStoreStocks(string storeName)
        {
            if (_storeStocks.Any()) return _storeStocks;
            UpdateCountNonStoreStocks(storeName);
            _storeStocks = TotalInvItems > 0 ? ((Func<List<StoreStock>>) (() =>
            {
                var connParams = new Dictionary<string, dynamic>
                {
                    {"storename", storeName},
                    {"offset", Fetch * CurrentInvPage - Fetch},
                    {"fetch", Fetch}
                };
                var table = _dbProxy.GetDataTable("get non stock for store", connParams);
                var items = table.Select().Select(x =>
                    new StoreStock(
                        (int) x["ProductID"],
                        (string) x["Name"],
                        (int) x["StockLevel"])).ToList();
                return items;
            }))() : new List<StoreStock>();
            return _storeStocks;
        }

        public List<StoreStock> StoreStocksThreshold(string storeName, int threshold)
        {
            UpdateCountStoreStocksWithThreshold(storeName, threshold);
            if (_storeStocks.Any()) return _storeStocks;
            _storeStocks = TotalInvItems > 0 ? ((Func<List<StoreStock>>) (() =>
            {
                var connParams = new Dictionary<string, dynamic>
                {
                    {"storename", storeName},
                    {"threshold", threshold},
                    {"offset", Fetch * CurrentInvPage - Fetch},
                    {"fetch", Fetch}
                };
                var table = _dbProxy.GetDataTable("get stock for store with threshold", connParams);
                var items = table.Select().Select(x =>
                    new StoreStock(
                        (int) x["ProductID"],
                        (string) x["product"],
                        (int) x["StockLevel"])).ToList();
                return items;
            }))() : new List<StoreStock>();
            return _storeStocks;
            
        }

        public void ResetStoreStocks()
        {
            _storeStocks = new List<StoreStock>();
        }

        private void UpdateCountStoreStocks(string storeName)
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"storename", storeName}
            };

            TotalInvItems = _dbProxy.ExecuteScalarAsync("count stocks for store", connParams).Result;
        }
        
        private void UpdateCountNonStoreStocks(string storeName)
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"storename", storeName}
            };

            TotalInvItems = _dbProxy.ExecuteScalarAsync("count non stocks for store", connParams).Result;
        }
        
        

        private void UpdateCountStoreStocksWithThreshold(string storeName, int threshold)
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"storename", storeName},
                {"threshold", threshold}
            };

            TotalInvItems = _dbProxy.ExecuteScalarAsync("count stocks for store with threshold", connParams).Result;
        }

        public void CreateStockRequest(string location, int prodId, int qty)
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"storeName", location},
                {"prodId", prodId},
                {"qty", qty}
            };
            _dbProxy.ExecuteNonQuery("create stock request", connParams);
        }

        public void PurchaseProduct(string location, int prodId, int qty)
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"storeName", location},
                {"prodId", prodId},
                {"qty", qty}
            };
            _dbProxy.ExecuteNonQuery("purchase an item in store", connParams);
        }
    }
}