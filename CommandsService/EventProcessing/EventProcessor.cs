using System;
using System.Text.Json;
using AutoMapper;
using CommandsService.Dtos;
using CommandsService.EventProcessing.Interfaces;
using CommandsService.Interfaces;
using CommandsService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }
        
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private void AddPlatform(string publishedPublishedMessage)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(publishedPublishedMessage);

            try
            {
                var platform = _mapper.Map<Platform>(platformPublishedDto);
                if (!repo.ExternalPlatformExists(platform.ExternalId))
                {
                    repo.CreatePlatform(platform);
                    repo.SaveChanges();

                    Console.WriteLine("--> Platform was added");
                }
                else
                {
                    Console.WriteLine("--> Platform already exists");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not add Platform to database {e.Message}");
                throw;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining event");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform published event detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could not determine the event type");
                    return EventType.Undetermined;
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}