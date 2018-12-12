using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Wdt.Model;
using Wdt.Utils;

namespace Wdt.DAL
{
    /// <summary>
    /// Stock Request Layer, implements singleton interface
    /// </summary>
    public class StockReqDal : IStockReqDal
    {
        /// <summary>
        /// ints for paginated output 
        /// </summary>
        public int Fetch { get;} = 6;
        public int CurrentPage { get; set; } = 1;
        public int TotalUserRequests { get; private set;  }

        /// <summary>
        /// static instance holder
        /// </summary>
        private static readonly Lazy<IStockReqDal> _instance = new Lazy<IStockReqDal>(() => new StockReqDal());

        /// <summary>
        /// private constructor
        /// </summary>
        private StockReqDal(){}

        /// <summary>
        /// accessor for instance
        /// </summary>
        public static IStockReqDal Instance => _instance.Value;

        private readonly IDalDbProxy _dbProxy = DalDbProxy.Instance;

        private List<StockRequest> _items = new List<StockRequest>();

        public List<StockRequest> Items
        {
            get
            {
                if (_items.Any()) return _items;
                UpdateCountRequests();
                _items = ((Func<List<StockRequest>>) (() =>
                {
                    var connParams = new Dictionary<string, dynamic>
                    {
                        {"offset", Fetch * CurrentPage - Fetch},
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
                }))();
                return _items;
            }
            set => _items = value;
        }


        public void ActionStockRequest(int requestId)
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"RequestID", requestId}
            };
            _dbProxy.ExecuteNonQuery("action stock request", connParams);
            UpdateCountRequests();
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

            TotalUserRequests = _dbProxy.ExecuteScalarAsync("count total rows", connParams).Result;
        }
    }
}