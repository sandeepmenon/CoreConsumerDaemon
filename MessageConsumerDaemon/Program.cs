using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MessageConsumerDaemon
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                 .ConfigureAppConfiguration((hostingContext, config) =>
                 {
                     config.AddEnvironmentVariables();
                     var env = hostingContext.HostingEnvironment;
                     config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                     if (args != null)
                     {
                         config.AddCommandLine(args);
                     }
                 })
                 .ConfigureServices((hostContext, services) =>
                 {
                     services.AddOptions();
                     services.Configure<DaemonConfig>(con => hostContext.Configuration.GetSection("AppSettings").Bind(con));
                     services.AddSingleton<IHostedService, MessageConsumerService>();
                 })
                 .ConfigureLogging((hostingContext, logging) =>
                 {
                     logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                     logging.AddConsole();
                 });
            await builder.RunConsoleAsync();
        }
    }
}
