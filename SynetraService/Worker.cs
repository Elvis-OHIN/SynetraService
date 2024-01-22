using SynetraService.Models;
using SynetraService.Remote;
using System.Net.Http.Json;

namespace SynetraService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            HttpClient client = new HttpClient();

            var pc = new Computer
            {
                CarteMere = SystemInfo.GetMotherboardInfo(),
                GPU = SystemInfo.GetGPUName(),
                Os = SystemInfo.GetOperatingSystemInfo(),
                Name = SystemInfo.GetComputerName(),
                IDProduct = SystemInfo.GetWindowsProductId(),
                Statut = true,
                IsActive = true,
                OperatingSystem = SystemInfo.GetSystemArchitecture()
            };
            await client.PostAsJsonAsync("https://localhost:7082/api/Computers", pc);
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    //await RemoteDesktop.SendImageAsync();
                }
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
