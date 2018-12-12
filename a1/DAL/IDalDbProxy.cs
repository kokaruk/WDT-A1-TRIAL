using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Wdt.DAL
{
    public interface IDalDbProxy
    {
        Task<dynamic> ExecuteScalarAsync(string procedure,
            Dictionary<string, dynamic> connParams = null);

        DataTable GetDataTable(string procedure,
            Dictionary<string, dynamic> connParams);
        
        void ExecuteNonQuery(string procedure,
            Dictionary<string, dynamic> connParams);
        
    }
}