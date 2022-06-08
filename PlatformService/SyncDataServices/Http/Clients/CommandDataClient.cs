using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using PlatformService.SyncDataServices.Http.Interfaces;

namespace PlatformService.SyncDataServices.Http.Clients
{
    public class CommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CommandDataClient(HttpClient httpClient,IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        
        public async Task SendPlatformToCommand(PlatformReadDto platformReadDto)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(platformReadDto),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"--> Sync POST to CommandService {_configuration["CommandService"]} was OK");
                return;
            }

            Console.WriteLine($"--> Sync POST to CommandService {_configuration["CommandService"]} was not OK");
        }
    }
}