using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Exceptions;
using StupidFirewallManager.Common;
using System;

namespace StupidFirewallManager.Core
{
    public static class Bootstrapper
    {
        static Bootstrapper()
        {
            Log.Logger = new LoggerConfiguration()
               .Enrich.WithExceptionDetails()
               .MinimumLevel.Debug()
               .WriteTo.File(
                   "logs\\logs.txt",
                    rollingInterval: RollingInterval.Day
               )
               .WriteTo.File(
                   "logs\\errors.txt",
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error
               )
               .WriteTo.Console()
               .CreateLogger();
        }

        public static Configuration Configuration { get; private set; }

        public static void Initialize()
        {
            Configuration = new Configuration();

            IConfiguration configBuilder = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile("config.json")
               .Build();

            Configuration.Bind(configBuilder);
        }
    }
}
