using CommandLine;
using ConsoleAppStarter;
using ConsoleAppStarter.Configuration;
using ConsoleAppStarter.Parameters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.LayoutRenderers;
using System;
using System.IO;
using System.Reflection;

namespace RunTest
{
    public class Program
    {
        private static ConfigurationRoot _configurationRoot;

        private static ServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Directory.SetCurrentDirectory(path);

            var param = ParseParameters(args);
            _configurationRoot = _serviceProvider.GetRequiredService<ConfigurationParser>()
                                                    .Parse(Path.Combine(param.ConfigFolderPath, "app-config.xml"));
            ConfigureLogging(param.ConfigFolderPath, _configurationRoot);

            try
            {
                _serviceProvider.GetRequiredService<ServiceSample>().Launch();
                _serviceProvider.GetRequiredService<ILogger<Program>>().LogInformation("Finished");
            }
            catch (Exception ex)
            {
                try
                {
                    _serviceProvider.GetRequiredService<ILogger<Program>>().LogCritical(ex, "Program terminated");
                }
                catch { }
                throw;
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // configure logging
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                builder.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
            });

            services.AddTransient<ConfigurationParser>();
            services.AddTransient<ConfigurationRoot>((_) => _configurationRoot);
            services.AddTransient<ServiceSample>();
        }

        private static void ConfigureLogging(string configPath, ConfigurationRoot configurationRoot)
        {
            var assembly = Assembly.GetExecutingAssembly();
            GlobalDiagnosticsContext.Set("Application", assembly.GetName().Name);
            GlobalDiagnosticsContext.Set("Version", assembly.GetName().Version);
            GlobalDiagnosticsContext.Set("LogPath", configurationRoot.Logging.LogPath);

            LayoutRenderer.Register("context", _ => 1);
            LogManager.Configuration = new XmlLoggingConfiguration(Path.Combine(configPath, "nlog.config"));
        }

        private static ParameterSet ParseParameters(string[] args)
        {
            ParameterSet parameters = null;
            var result = Parser.Default.ParseArguments<ParameterSet>(args);
            if (result.Tag == ParserResultType.Parsed)
            {
                result.WithParsed(p => parameters = p);
                return parameters;
            }
            else
            {
                throw new Exception($"unable to parse command parameters : {string.Join(",", args)} \n {CommandLine.Text.HelpText.AutoBuild(result)} ");
            }
        }
    }
}
