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
    public static class DalFactory
    {
        private static readonly Lazy<IUserDal> _user = new Lazy<IUserDal>(() => UserDal.Instance);
        public static IUserDal User => _user.Value;
        
        private static readonly ConsoleLoadingText _loader = new ConsoleLoadingText(); 
        
        /// <summary>
        /// async Scalar Method
        /// </summary>
        /// <param name="procedure">stored procedure name</param>
        /// <param name="connParams">dictionary of param name and values</param>
        /// <returns>Scalar Selection</returns>
        internal static async Task<dynamic> ExecuteScalarAsync(string procedure, Dictionary<string, string> connParams)
        {
            Display();
            using (var con = Program.ConnectionString.CreateConnection())
            {
                await con.OpenAsync();
                var command = con.CreateCommand();
                command.CommandText = procedure;
                command.CommandType = CommandType.StoredProcedure;
                command.FillParams(connParams);
                var data = await command.ExecuteScalarAsync();
                // stop loader before continue
                _loader.Stop();
                return data;
            }
        }

        /// <summary>
        /// proxy method for display loading method. If placed directly in async method compiler throws a warning
        /// and expects an 'await'
        /// </summary>
        private static void Display()
        {
            _loader.Display();
        }
        
    }
}