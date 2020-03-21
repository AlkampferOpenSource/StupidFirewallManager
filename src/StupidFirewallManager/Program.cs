using Serilog;
using StupidFirewallManager.Core;
using StupidFirewallManager.Support;
using System;
using Topshelf;

namespace StupidFirewallManager
{
    public static class Program
    {
        private const string ServiceDescriptiveName = "Stupid firewall manager";
        private const string ServiceName = "StupidFirewallManager";

        private static void Main(string[] args)
        {
            Bootstrapper.Initialize();

            if (args.Length == 1 && (args[0] == "install" || args[0] == "uninstall"))
            {
                StartForInstallOrUninstall(ServiceDescriptiveName, ServiceName);
            }
            else
            {
                var exitCode = StandardStart();
                if (exitCode != TopshelfExitCode.Ok && Environment.UserInteractive)
                {
                    Console.WriteLine("Service exited with error code {0} press a key to close", exitCode);
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// Method called to install/uninstall the service
        /// </summary>
        /// <param name="runAsSystem"></param>
        /// <param name="dependOnServiceList"></param>
        /// <param name="serviceDescriptiveName"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static TopshelfExitCode StartForInstallOrUninstall(
            String serviceDescriptiveName,
            String serviceName)
        {
            var exitCode = HostFactory.Run(host =>
            {
                host.Service<Object>(service =>
                {
                    service.ConstructUsing(() => new Object());
                    service.WhenStarted(_ => Console.WriteLine("Start fake for install"));
                    service.WhenStopped(_ => Console.WriteLine("Stop fake for install"));
                });

                host.RunAsLocalSystem();

                host.SetDescription(serviceDescriptiveName);
                host.SetDisplayName(serviceDescriptiveName);
                host.SetServiceName(serviceName);
            });
            return exitCode;
        }

        private static TopshelfExitCode StandardStart()
        {
            if (Environment.UserInteractive)
            {
                Console.Title = ServiceDescriptiveName;
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.Clear();
                Console.SetWindowPosition(0, 0);
                Console.WindowWidth = Console.LargestWindowWidth - 80;
            }

            return HostFactory.Run(x =>
            {
                Log.Information("Windows stupid firewall started");
                x.UseSerilog(Log.Logger);
                x.Service<ServiceBootstrapper>(s =>
                {
                    s.ConstructUsing(_ => new ServiceBootstrapper());
                    s.WhenStarted((tc, hc) => tc.Start(hc));
                    s.WhenStopped((tc, hc) => tc.Stop(hc));
                });

                x.SetDescription(ServiceDescriptiveName);
                x.SetDisplayName(ServiceDescriptiveName);
                x.SetServiceName(ServiceName);
            });
        }
    }
}
