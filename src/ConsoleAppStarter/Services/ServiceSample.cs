using ConsoleAppStarter.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ConsoleAppStarter
{
    public class ServiceSample
    {
        ILogger<ServiceSample> _logger;
        ConfigurationRoot _config;

        public ServiceSample(ILogger<ServiceSample> logger, ConfigurationRoot configuration)
        {
            _logger = logger;
            _config = configuration;
        }

        public void Launch()
        {
            _logger.LogInformation($"Hello from {nameof(ServiceSample)}");
            _logger.LogInformation($"You started with the following configuration:\n " +
                $"{JsonSerializer.Serialize(_config, new JsonSerializerOptions() { WriteIndented = true })}");
        }
    }
}
