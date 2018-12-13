using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wdt.Controller;
using Wdt.Utils;

namespace Wdt
{
    public static class Program
    {
        private static readonly Lazy<IConfiguration> _configuration;
        private static IConfiguration Configuration => _configuration.Value;
        private static readonly Lazy<string> _connectionString;
        internal static string ConnectionString => _connectionString.Value;
        // 
        private static readonly Lazy<int> _invResetThreshold;
        internal static int InvResetThreshold => _invResetThreshold.Value;
        
        public static bool Testing { get; private set; }
        
        /// <summary>
        /// static constructor
        /// </summary>
        static Program()
        {
            _configuration = new Lazy<IConfiguration>(() =>
            {
                var devEnvironmentVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                //Determines the working environment as IHostingEnvironment is unavailable in a console app
                var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) ||
                                    devEnvironmentVariable.ToLower() == "development";
                var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json");
                //only add secrets in development
                if (isDevelopment) builder.AddUserSecrets<MagicDb>();
                return builder.Build();
            });
            _connectionString = new Lazy<string>(() =>
            {
                var secrets = Configuration.GetSection(nameof(MagicDb)).Get<MagicDb>();
                var sqlString = new SqlConnectionStringBuilder(Configuration.GetConnectionString("Magic"))
                {
                    UserID = secrets.Uid,
                    Password = secrets.Password
                };
                return sqlString.ConnectionString;
            });
            _invResetThreshold = new Lazy<int>(() =>
                    Configuration.GetSection("OwnerManageInventory").GetValue<int>("UpdateThreshold"));
        }

        private static void Main(string[] args)
        {
            Testing = args.Length > 0 && args[0] == "test";
            Console.Clear();
            BaseController login = new LoginController();
            login.Start();
        }
    }
}