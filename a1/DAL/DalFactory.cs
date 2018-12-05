using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Wdt.Controller;
using Wdt.Model;
using Wdt.Utils;

namespace Wdt.DAL
{
    public static class DalFactory
    {
        public static IUserDal User { get; }
        
        static DalFactory()
        {
            User = UserDal.Instance;
        }

        internal static dynamic ExecuteScalar(string procedure, Dictionary<string, string> connParams)
        {
            using (var connection = Program.ConnectionString.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = procedure;
                    command.CommandType = CommandType.StoredProcedure;
                    command.fillParams(connParams);
                    return command.ExecuteScalar();
                }
            }
        }

//        private static void fillParams(SqlCommand command, Dictionary<string, string> connParams)
//        {
//            foreach (var keyValue in connParams)
//            {
//                command.Parameters.AddWithValue(keyValue.Key, keyValue.Value);
//            }
//        }
        
    }
}