using System;
using System.Collections;
using System.Collections.Generic;
using CommandsService.Interfaces;
using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
                
            var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

            var platforms = grpcClient.ReturnAllPlatforms();
            
            SeedData(serviceScope.ServiceProvider.GetService<ICommandRepository>(), platforms);
        }

        private static void SeedData(ICommandRepository repository, IEnumerable<Platform> platforms)
        {
            Console.WriteLine($"--> Seeding new platforms...");

            foreach (var platform in platforms)
            {
                if (!repository.ExternalPlatformExists(platform.ExternalId))
                {
                    repository.CreatePlatform(platform);
                }
            }

            repository.SaveChanges();
        }
    }
}