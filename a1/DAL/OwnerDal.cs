using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wdt.Model;
using Wdt.Utils;

namespace Wdt.DAL
{
    /// <summary>
    /// Stock Request Layer, implements singleton interface
    /// </summary>
    public class OwnerDal : IOwnerDal
    {
        /// <summary>
        /// ints for paginated output 
        /// </summary>
        public int Fetch { get; } = 6;

        public int CurrentRequestsPage { get; set; } = 1;
        public int CurrentInvPage { get; set; } = 1;
        public int TotalStockRequests { get; private set; }
        public int TotalInvItems { get; private set; }

        /// <summary>
        /// static instance holder
        /// </summary>
        private static readonly Lazy<IOwnerDal> _instance = new Lazy<IOwnerDal>(() => new OwnerDal());

        /// <summary>
        /// private constructor
        /// </summary>
        private OwnerDal()
        {
        }

        /// <summary>
        /// accessor for instance
        /// </summary>
        public static IOwnerDal Instance => _instance.Value;

        private readonly IDalDbProxy _dbProxy = DalDbProxy.Instance;

        private List<StockRequest> _stockRequests = new List<StockRequest>();

        public List<StockRequest> StockRequests
        {
            get
            {
                UpdateCountRequests();
                if (_stockRequests.Any()) return _stockRequests;
                _stockRequests = TotalStockRequests > 0 ?  ((Func<List<StockRequest>>) (() =>
                {
                    var connParams = new Dictionary<string, dynamic>
                    {
                        {"offset", Fetch * CurrentRequestsPage - Fetch},
                        {"fetch", Fetch}
                    };
                    var table = _dbProxy.GetDataTable("get all stock requests", connParams);
                    var items = table.Select().Select(x =>
                        new StockRequest(
                            (int) x["ID"],
                            (string) x["Store"],
                            (string) x["Product"],
                            (int) x["Quantity"],
                            (int) x["Current Stock"],
                            (bool) x["Stock Availability"]
                        )
                    ).ToList();
                    return items;
                }))() : new List<StockRequest>() ;
                return _stockRequests;
            }
            set => _stockRequests = value;
        }

        private List<OwnerInventory> _ownerInventories = new List<OwnerInventory>();

        public List<OwnerInventory> OwnerInventories
        {
            get
            {
                if (_ownerInventories.Any()) return _ownerInventories;
                UpdateCountInventory();
                _ownerInventories = ((Func<List<OwnerInventory>>) (() =>
                {
                    var connParams = new Dictionary<string, dynamic>
                    {
                        {"offset", Fetch * CurrentInvPage - Fetch},
                        {"fetch", Fetch}
                    };

                    var table = _dbProxy.GetDataTable("get all owner inventory", connParams);
                    var items = table.Select().Select(x =>
                        new OwnerInventory(
                            (int) x["ProductID"],
                            (string) x["Name"],
                            (int) x["StockLevel"]
                        )
                    ).ToList();
                    return items;
                }))();

                return _ownerInventories;
            }
            set => _ownerInventories = value;
        }

        public void ActionStockRequest(int requestId)
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"RequestID", requestId},
            };
            _dbProxy.ExecuteNonQuery("action stock request", connParams);
            UpdateCountRequests();
        }

        public void ResetStockLevel(int productId, int level)
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"ProductID", productId},
                {"level", level}
            };
            _dbProxy.ExecuteNonQuery("reset stock level", connParams);
        }


        /// <summary>
        /// Update total number of requests
        /// </summary>
        private void UpdateCountRequests()
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"tablename", "StockRequestsView"}
            };

            TotalStockRequests = _dbProxy.ExecuteScalarAsync("count total rows", connParams).Result;
        }

        /// <summary>
        /// Update total number of Inventory
        /// </summary>
        private void UpdateCountInventory()
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"tablename", "OwnerInventoryView"}
            };

            TotalInvItems = _dbProxy.ExecuteScalarAsync("count total rows", connParams).Result;
        }
    }
}