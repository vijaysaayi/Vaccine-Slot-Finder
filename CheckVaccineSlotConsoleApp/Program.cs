using CommandDotNet;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace CheckVaccineSlotConsoleApp
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            var serilogLogger = new LoggerConfiguration()
                                .WriteTo.Console()
                                .WriteTo.RollingFile("vaccine-slot-finder.log")
                                .CreateLogger();

            services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Information);
                logging.AddSerilog(logger: serilogLogger, dispose: true);
            });

            services.AddSingleton<CheckSlotController>();
            services.AddSingleton<ICheckVaccineSlotService,CheckVaccineSlotService>();

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            AppRunner<CheckSlotController> appRunner = new AppRunner<CheckSlotController>();
            appRunner.UseMicrosoftDependencyInjection(serviceProvider);

            string version = "v1.0.0.0";
            Console.WriteLine($"Starting Vaccine Slot Checker {version}");

            return appRunner.Run(args);
        }
    }
}