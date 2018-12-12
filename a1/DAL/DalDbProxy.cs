using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Wdt.Utils;

namespace Wdt.DAL
{
    /// <summary>
    /// database proxy class
    /// implements singleton and facade pattert
    /// </summary>
    public class DalDbProxy : IDalDbProxy
    {
        /// <summary>
        /// static instance holder
        /// </summary>
        private static readonly Lazy<IDalDbProxy> _instance = new Lazy<IDalDbProxy>(() => new DalDbProxy());

        /// <summary>
        /// accessor for instance
        /// </summary>
        public static IDalDbProxy Instance => _instance.Value;

        /// <summary>
        /// private constructor
        /// </summary>
        private DalDbProxy()
        {
        }

        // Loading status 
        private static readonly ConsoleLoadingText _loader = new ConsoleLoadingText();

        /// <summary>
        /// async Scalar Method
        /// </summary>
        /// <param name="procedure">stored procedure name</param>
        /// <param name="connParams">dictionary of param name and values</param>
        /// <returns>Scalar Selection</returns>
        public async Task<dynamic> ExecuteScalarAsync(string procedure,
            Dictionary<string, dynamic> connParams = null)
        {
            _loader.Display();
            using (var con = Program.ConnectionString.CreateConnection())
            using (var command = con.CreateProcedureCommand(procedure, connParams))
            {
                await con.OpenAsync();
                var data = await command.ExecuteScalarAsync();
                // stop loader before continue
                _loader.Stop();
                return data;
            }
        }

        /// <summary>
        /// gets data table from procedure
        /// </summary>
        /// <param name="procedure">stored procedure name</param>
        /// <param name="connParams">dictionary of param name and values</param>
        /// <returns>Data table</returns>
        public DataTable GetDataTable(string procedure,
            Dictionary<string, dynamic> connParams)
        {
            _loader.Display();
            using (var con = Program.ConnectionString.CreateConnection())
            using (var command = con.CreateProcedureCommand(procedure, connParams))
            using (var da = new SqlDataAdapter(command))
            {
                var dt = new DataTable();
                da.Fill(dt);
                _loader.Stop();
                return dt;
            }
        }


        public async void ExecuteNonQuery(string procedure,
            Dictionary<string, dynamic> connParams)
        {
            _loader.Display();
            using (var con = Program.ConnectionString.CreateConnection())
            using (var command = con.CreateProcedureCommand(procedure, connParams))
            {
                await con.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}