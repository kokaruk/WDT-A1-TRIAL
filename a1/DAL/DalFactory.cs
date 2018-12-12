using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wdt.Controller;
using Wdt.Model;
using Wdt.Utils;

namespace Wdt.DAL
{
    /// <summary>
    /// DAL. Returns instances of data Accessors
    /// And performs proxy functions for various sql operations
    /// </summary>
    public static class DalFactory
    {
        private static readonly Lazy<IUserDal> _user = new Lazy<IUserDal>(() => UserDal.Instance);
        public static IUserDal User => _user.Value;
        private static readonly Lazy<IStockReqDal> _stockRequests = new Lazy<IStockReqDal>(() => StockReqDal.Instance);
        public static IStockReqDal StockRequests => _stockRequests.Value;
    }
}